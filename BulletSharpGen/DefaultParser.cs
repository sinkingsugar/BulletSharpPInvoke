﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletSharpGen
{
    class DefaultParser
    {
        public WrapperProject Project { get; private set; }

        public DefaultParser(WrapperProject project)
        {
            Project = project;
        }

        public virtual void Parse()
        {
            ResolveReferences();
            MapSymbols();
            ParseEnums();
            SetClassProperties();
            RemoveRedundantMethods();
            CreateDefaultConstructors();
            CreateFieldAccessors();
            CreateProperties();
            ResolveIncludes();
        }

        // n = 2 -> "\t\t"
        protected static string GetTabs(int n)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                builder.Append('\t');
            }
            return builder.ToString();
        }

        // one_two_three -> oneTwoThree
        // one_twoThree -> oneTwoThree
        // ONE_TWO_THREE -> oneTwoThree
        // one_two_THREE -> oneTwoThree
        protected static string ToCamelCase(string text, bool upper)
        {
            if (text.Length == 0)
            {
                return text;
            }

            StringBuilder outText = new StringBuilder();
            int left = 0;

            while (left < text.Length)
            {
                int right = text.IndexOf('_', left);
                if (right == -1)
                {
                    right = text.Length;
                }
                else if (right == left)
                {
                    left++;
                    continue;
                }

                char first = text[left];
                if (outText.Length == 0)
                {
                    first = upper ? char.ToUpper(first) : char.ToLower(first);
                }
                else
                {
                    first = char.ToUpper(first);
                }
                outText.Append(first);
                left++;

                string rest = text.Substring(left, right - left);
                if (rest.All(c => char.IsDigit(c) || char.IsUpper(c)))
                {
                    // Two-letter acronyms are preserved as capitalized
                    // https://msdn.microsoft.com/en-us/library/ms229043%28v=vs.110%29.aspx
                    if (rest.Length > 1 ||
                        (first + rest).Equals("NO") ||
                        (first + rest).Equals("OF") ||
                        (first + rest).Equals("IS"))
                    {
                        rest = rest.ToLower();
                    }
                }
                outText.Append(rest);

                left = right + 1;
            }

            return outText.ToString();
        }

        protected virtual bool IsExcludedClass(ClassDefinition cl)
        {
            return false;
        }

        void SetClassProperties()
        {
            foreach (var @class in Project.ClassDefinitions.Values)
            {
                @class.IsAbstract = @class.AbstractMethods.Any();
                if (IsExcludedClass(@class))
                {
                    @class.IsExcluded = true;
                }
            }
        }

        void ParseEnums()
        {
            // Remove any common prefix and check for flags
            foreach (var @enum in Project.ClassDefinitions.Values.
                Where(c => c is EnumDefinition).Cast<EnumDefinition>())
            {
                int prefixLength = @enum.GetCommonPrefix().Length;
                @enum.GetCommonSuffix();
                for (int i = 0; i < @enum.EnumConstants.Count; i++)
                {
                    string enumConstant = @enum.EnumConstants[i];
                    enumConstant = enumConstant.Substring(prefixLength);
                    @enum.EnumConstants[i] = ToCamelCase(enumConstant, true);
                }

                if (@enum.Name.EndsWith("Flags"))
                {
                    @enum.IsFlags = true;
                }
                else
                {
                    // If all values are powers of 2, then it is considered a Flags enum.
                    @enum.IsFlags = @enum.EnumConstantValues.All(value =>
                    {
                        int x;
                        if (int.TryParse(value, out x))
                        {
                            return (x != 0) && ((x & (~x + 1)) == x);
                        }
                        return false;
                    });
                }

                if (@enum.IsFlags)
                {
                    if (!@enum.EnumConstantValues.Any(value => value.Equals("0")))
                    {
                        @enum.EnumConstants.Insert(0, "None");
                        @enum.EnumConstantValues.Insert(0, "0");
                    }
                }
            }
        }

        void ResolveReferences()
        {
            // Resolve references (match TypeRefDefinitions to ClassDefinitions)
            // List might be modified with template specialization classes, so make a copy
            var classDefinitionsList = new List<ClassDefinition>(Project.ClassDefinitions.Values);
            foreach (var @class in classDefinitionsList)
            {
                // Resolve typedef
                if (@class.TypedefUnderlyingType != null)
                {
                    ResolveTypeRef(@class.TypedefUnderlyingType);
                }

                // Resolve method return type and parameter types
                foreach (var method in @class.Methods)
                {
                    ResolveTypeRef(method.ReturnType);
                    foreach (ParameterDefinition param in method.Parameters)
                    {
                        ResolveTypeRef(param.Type);
                    }
                }

                // Resolve field types
                foreach (var field in @class.Fields)
                {
                    ResolveTypeRef(field.Type);
                }
            }
        }

        void ResolveTypeRef(TypeRefDefinition typeRef)
        {
            if (typeRef.IsBasic || typeRef.HasTemplateTypeParameter)
            {
                return;
            }
            if (typeRef.IsPointer || typeRef.IsReference || typeRef.IsConstantArray)
            {
                ResolveTypeRef(typeRef.Referenced);
            }
            else if (Project.ClassDefinitions.ContainsKey(typeRef.Name))
            {
                typeRef.Target = Project.ClassDefinitions[typeRef.Name];
            }
            else
            {
                // Search for unscoped enums
                bool resolvedEnum = false;
                foreach (var @class in Project.ClassDefinitions.Values.Where(c => c is EnumDefinition))
                {
                    if (typeRef.Name.Equals(@class.FullyQualifiedName + "::" + @class.Name))
                    {
                        typeRef.Target = @class;
                        resolvedEnum = true;
                    }
                }
                if (!resolvedEnum)
                {
                    Console.WriteLine("Class " + typeRef.Name + " not found!");
                }
            }

            if (typeRef.SpecializedTemplateType != null)
            {
                ResolveTypeRef(typeRef.SpecializedTemplateType);

                // Create template specialization class
                string name = string.Format("{0}<{1}>", typeRef.Name, typeRef.SpecializedTemplateType.Name);
                if (!Project.ClassDefinitions.ContainsKey(name))
                {
                    var templateClass = typeRef.Target;
                    if (templateClass != null && !templateClass.IsExcluded)
                    {
                        var header = templateClass.Header;
                        var specializedClass = new ClassDefinition(name, header);
                        specializedClass.BaseClass = templateClass;
                        header.Classes.Add(specializedClass);
                        Project.ClassDefinitions.Add(name, specializedClass);
                    }
                }
            }
        }

        // Remove overridden methods, methods that differ by const/non-const return values
        // and abstract class constructors
        void RemoveRedundantMethods()
        {
            // Remove by index, not by reference, otherwise the wrong method could be removed.
            // MethodDefinition.Equals compares methods from the POV of C#, not C++,
            // so const/non-const methods will be equal.
            var removedMethodsIndices = new SortedSet<int>();

            foreach (var @class in Project.ClassDefinitions.Values)
            {
                for (int i = 0; i < @class.Methods.Count; i++)
                {
                    var method = @class.Methods[i];

                    if (method.IsConstructor)
                    {
                        if (@class.IsAbstract) removedMethodsIndices.Add(i);
                        continue;
                    }

                    // Check if the method already exists in a base class
                    var baseClass = @class.BaseClass;
                    while (baseClass != null)
                    {
                        var baseMethod = baseClass.Methods.FirstOrDefault(m => m.Equals(method));
                        if (baseMethod != null)
                        {
                            if (baseMethod.IsExcluded)
                            {
                                method.IsExcluded = true;
                            }
                            else
                            {
                                removedMethodsIndices.Add(i);
                            }
                            break;
                        }
                        baseClass = baseClass.BaseClass;
                    }

                    for (int j = i + 1; j < @class.Methods.Count; j++)
                    {
                        var method2 = @class.Methods[j];
                        if (!method.Equals(method2)) continue;

                        var type1 = method.ReturnType;
                        var type2 = method2.ReturnType;
                        bool const1 = type1.IsConst || (type1.Referenced != null && type1.Referenced.IsConst);
                        bool const2 = type2.IsConst || (type2.Referenced != null && type2.Referenced.IsConst);

                        // Prefer non-const return value
                        if (const1)
                        {
                            if (!const2)
                            {
                                removedMethodsIndices.Add(i);
                                break;
                            }
                        }
                        else if (const2)
                        {
                            removedMethodsIndices.Add(j);
                            break;
                        }

                        // Couldn't see the difference
                        //throw new NotImplementedException();
                    }
                }

                foreach (int i in removedMethodsIndices.Reverse())
                {
                    @class.Methods.RemoveAt(i);
                }
                removedMethodsIndices.Clear();
            }
        }

        // Give managed names to headers, classes and methods
        void MapSymbols()
        {
            // Get managed header and enum names
            var headerNameMapping = Project.HeaderNameMapping as ScriptedMapping;
            foreach (var header in Project.HeaderDefinitions.Values)
            {
                headerNameMapping.Globals.Header = header;
                header.ManagedName = headerNameMapping.Map(header.Name);
            }

            // Apply class properties
            var classNameMapping = Project.ClassNameMapping as ScriptedMapping;
            foreach (var @class in Project.ClassDefinitions.Values)
            {
                classNameMapping.Globals.Header = @class.Header;
                @class.ManagedName = classNameMapping.Map(@class.Name);

                var @enum = @class as EnumDefinition;
                if (@enum != null)
                {
                    if (@enum.Parent != null &&
                        @enum.Parent.Methods.Count == 0 &&
                        @enum.Parent.Fields.Count == 0 &&
                        @enum.Parent.Classes.Count == 1)
                    {
                        @enum.ManagedName = @enum.Parent.ManagedName;
                    }
                }
            }

            // Set managed method/parameter names
            foreach (var method in Project.ClassDefinitions.Values.SelectMany(c => c.Methods))
            {
                method.ManagedName = GetManagedMethodName(method);

                foreach (var param in method.Parameters)
                {
                    param.ManagedName = GetManagedParameterName(param);
                }
            }
        }

        string GetManagedMethodName(MethodDefinition method)
        {
            if (Project.MethodNameMapping != null)
            {
                string mapping = Project.MethodNameMapping.Map(method.Name);
                if (mapping != null) return mapping;
            }

            if (method.Name.StartsWith("operator"))
            {
                return method.Name;
            }

            if (method.IsConstructor)
            {
                return method.Parent.ManagedName;
            }

            return ToCamelCase(method.Name, true);
        }

        string GetManagedParameterName(ParameterDefinition param)
        {
            if (Project.ParameterNameMapping != null)
            {
                string mapping = Project.ParameterNameMapping.Map(param.Name);
                if (mapping != null) return mapping;
            }

            string managedName = param.Name;
            if (managedName.StartsWith("__unnamed"))
            {
                return managedName;
            }

            return ToCamelCase(param.Name, false);
        }

        // Create default constructor if no explicit C++ constructor exists.
        void CreateDefaultConstructors()
        {
            foreach (var @class in Project.ClassDefinitions.Values)
            {
                if (@class.HidePublicConstructors) continue;
                if (@class.IsStaticClass) continue;
                if (@class is EnumDefinition) continue;
                if (@class.IsPureEnum) continue;

                var constructors = @class.Methods.Where(m => m.IsConstructor && !m.IsExcluded);
                if (!constructors.Any())
                {
                    var constructor = new MethodDefinition(@class.Name, @class, 0);
                    constructor.IsConstructor = true;
                    constructor.ReturnType = new TypeRefDefinition();
                }
            }
        }

        string[] _booleanVerbs = { "Has", "Is", "Needs" };

        // Create getters and setters for fields
        void CreateFieldAccessors()
        {
            foreach (var @class in Project.ClassDefinitions.Values)
            {
                foreach (var field in @class.Fields)
                {
                    string name = field.Name;
                    if (name.StartsWith("m_"))
                    {
                        name = name.Substring(2);
                    }
                    name = char.ToUpper(name[0]) + name.Substring(1); // capitalize
                    string managedName = ToCamelCase(name, true);

                    // Generate getter/setter methods
                    string getterName, setterName;
                    string managedGetterName, managedSetterName;
                    string verb = _booleanVerbs.FirstOrDefault(v => name.StartsWith(v));
                    if (verb != null && "bool".Equals(field.Type.Name))
                    {
                        getterName = name;
                        setterName = "set" + name.Substring(verb.Length);
                        managedGetterName = managedName;
                        managedSetterName = "Set" + managedName.Substring(verb.Length);
                    }
                    else
                    {
                        getterName = "get" + name;
                        setterName = "set" + name;
                        managedGetterName = "Get" + managedName;
                        managedSetterName = "Set" + managedName;
                    }

                    // See if there are already accessor methods for this field
                    MethodDefinition getter = null, setter = null;
                    foreach (var method in @class.Methods)
                    {
                        if (managedGetterName.Equals(method.ManagedName) && method.Parameters.Length == 0)
                        {
                            getter = method;
                            continue;
                        }

                        if (managedSetterName.Equals(method.ManagedName) && method.Parameters.Length == 1)
                        {
                            setter = method;
                        }
                    }

                    if (getter == null)
                    {
                        getter = new MethodDefinition(getterName, @class, 0);
                        getter.ManagedName = managedGetterName;
                        getter.ReturnType = field.Type;
                        getter.Field = field;
                    }

                    var prop = new PropertyDefinition(getter, GetPropertyName(getter));

                    if (setter == null)
                    {
                        CreateFieldSetter(prop, setterName, managedSetterName);
                    }
                }
            }
        }

        void CreateFieldSetter(PropertyDefinition prop, string setterName, string managedSetterName)
        {
            // Can't assign value to reference or constant array
            if (prop.Type.IsReference || prop.Type.IsConstantArray) return;

            if (prop.Type.Name != null && prop.Type.Name.StartsWith("btAlignedObjectArray")) return;

            var type = prop.Getter.ReturnType;

            MethodDefinition setter = new MethodDefinition(setterName, prop.Parent, 1);
            setter.ManagedName = managedSetterName;
            setter.ReturnType = new TypeRefDefinition();
            setter.Field = prop.Getter.Field;
            if (!type.IsBasic && !type.IsPointer)
            {
                type = type.Copy();
                type.IsConst = true;
            }
            setter.Parameters[0] = new ParameterDefinition("value", type);
            setter.Parameters[0].ManagedName = "value";

            prop.Setter = setter;
            prop.Setter.Property = prop;
        }

        string GetPropertyName(MethodDefinition getter)
        {
            string name = getter.ManagedName;

            var propertyType = getter.IsVoid ? getter.Parameters[0].Type : getter.ReturnType;
            if ("bool".Equals(propertyType.Name) && _booleanVerbs.Any(v => name.StartsWith(v)))
            {
                return name;
            }

            if (name.StartsWith("Get"))
            {
                return name.Substring(3);
            }

            throw new NotImplementedException();
        }

        // Turn getters and setters into properties,
        // managed method names have been resolved at this point
        void CreateProperties()
        {
            foreach (var @class in Project.ClassDefinitions.Values)
            {
                // Getters with return type and 0 arguments
                var getterMethods = @class.Methods.Where(m => !m.IsConstructor && !m.IsVoid && m.Parameters.Length == 0);
                foreach (var method in getterMethods)
                {
                    if (method.ManagedName.StartsWith("Get") ||
                        ("bool".Equals(method.ReturnType.Name) &&
                        _booleanVerbs.Any(v => method.ManagedName.StartsWith(v))))
                    {
                        if (method.Property != null) continue;
                        new PropertyDefinition(method, GetPropertyName(method));
                    }
                }

                // Getters with void type and 1 pointer argument for the return value
                // TODO: in general, it is not possible to automatically determine
                // whether such methods can be wrapped by properties or not,
                // so read this info from the project configuration.
                foreach (var method in @class.Methods.Where(m => m.IsVoid && m.Parameters.Length == 1))
                {
                    if (method.ManagedName.StartsWith("Get"))
                    {
                        if (method.Property != null) continue;

                        var paramType = method.Parameters[0].Type;
                        if (paramType.IsPointer || paramType.IsReference)
                        {
                            // TODO: check project configuration
                            //if (true)
                            {
                                new PropertyDefinition(method, GetPropertyName(method));
                            }
                        }
                    }
                }

                // Setters
                foreach (var method in @class.Methods)
                {
                    if (method.Parameters.Length == 1 &&
                        method.Name.StartsWith("set", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string name = method.ManagedName.Substring(3);
                        // Find the property with the matching getter
                        foreach (var prop in @class.Properties)
                        {
                            if (prop.Setter != null)
                            {
                                continue;
                            }

                            if (prop.Name.Equals(name))
                            {
                                prop.Setter = method;
                                method.Property = prop;
                                break;
                            }
                        }
                    }
                }
            }
        }

        void ResolveInclude(TypeRefDefinition typeRef, HeaderDefinition parentHeader)
        {
            if (typeRef.IsPointer || typeRef.IsReference || typeRef.IsConstantArray)
            {
                ResolveInclude(typeRef.Referenced, parentHeader);
            }
            else if (typeRef.SpecializedTemplateType != null)
            {
                ResolveInclude(typeRef.SpecializedTemplateType, parentHeader);
            }
            else if (typeRef.IsIncomplete && typeRef.Target != null)
            {
                parentHeader.Includes.Add(typeRef.Target.Header);
            }
        }

        // Add includes for incomplete types (forward references)
        // Should be done after removing redundant methods.
        void ResolveIncludes()
        {
            var classDefinitionsList = new List<ClassDefinition>(Project.ClassDefinitions.Values);
            foreach (var @class in classDefinitionsList.Where(c => !c.IsExcluded))
            {
                var header = @class.Header;

                // Include header for the base if necessary
                if (@class.BaseClass != null && header != @class.BaseClass.Header)
                {
                    header.Includes.Add(@class.BaseClass.Header);
                }

                // Resolve typedef
                if (@class.TypedefUnderlyingType != null)
                {
                    ResolveInclude(@class.TypedefUnderlyingType, header);
                }

                // Resolve method return type and parameter types
                foreach (var method in @class.Methods)
                {
                    ResolveInclude(method.ReturnType, header);
                    foreach (ParameterDefinition param in method.Parameters)
                    {
                        ResolveInclude(param.Type, header);
                    }
                }

                // Resolve field types
                foreach (var field in @class.Fields)
                {
                    ResolveInclude(field.Type, header);
                }
            }
        }
    }
}
