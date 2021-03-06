#include "main.h"

extern "C"
{
	EXPORT btTetrahedronShapeEx* btTetrahedronShapeEx_new();
	EXPORT void btTetrahedronShapeEx_setVertices(btTetrahedronShapeEx* obj, const btScalar* v0, const btScalar* v1, const btScalar* v2, const btScalar* v3);

	EXPORT bool btGImpactShapeInterface_childrenHasTransform(btGImpactShapeInterface* obj);
	EXPORT const btGImpactBoxSet* btGImpactShapeInterface_getBoxSet(btGImpactShapeInterface* obj);
	EXPORT void btGImpactShapeInterface_getBulletTetrahedron(btGImpactShapeInterface* obj, int prim_index, btTetrahedronShapeEx* tetrahedron);
	EXPORT void btGImpactShapeInterface_getBulletTriangle(btGImpactShapeInterface* obj, int prim_index, btTriangleShapeEx* triangle);
	EXPORT void btGImpactShapeInterface_getChildAabb(btGImpactShapeInterface* obj, int child_index, const btScalar* t, btScalar* aabbMin, btScalar* aabbMax);
	EXPORT btCollisionShape* btGImpactShapeInterface_getChildShape(btGImpactShapeInterface* obj, int index);
	EXPORT void btGImpactShapeInterface_getChildTransform(btGImpactShapeInterface* obj, int index, btScalar* value);
	EXPORT eGIMPACT_SHAPE_TYPE btGImpactShapeInterface_getGImpactShapeType(btGImpactShapeInterface* obj);
	EXPORT const btAABB* btGImpactShapeInterface_getLocalBox(btGImpactShapeInterface* obj);
	EXPORT int btGImpactShapeInterface_getNumChildShapes(btGImpactShapeInterface* obj);
	EXPORT const btPrimitiveManagerBase* btGImpactShapeInterface_getPrimitiveManager(btGImpactShapeInterface* obj);
	EXPORT void btGImpactShapeInterface_getPrimitiveTriangle(btGImpactShapeInterface* obj, int index, btPrimitiveTriangle* triangle);
	EXPORT bool btGImpactShapeInterface_hasBoxSet(btGImpactShapeInterface* obj);
	EXPORT void btGImpactShapeInterface_lockChildShapes(btGImpactShapeInterface* obj);
	EXPORT bool btGImpactShapeInterface_needsRetrieveTetrahedrons(btGImpactShapeInterface* obj);
	EXPORT bool btGImpactShapeInterface_needsRetrieveTriangles(btGImpactShapeInterface* obj);
	EXPORT void btGImpactShapeInterface_postUpdate(btGImpactShapeInterface* obj);
	EXPORT void btGImpactShapeInterface_processAllTrianglesRay(btGImpactShapeInterface* obj, btTriangleCallback* __unnamed0, const btScalar* __unnamed1, const btScalar* __unnamed2);
	EXPORT void btGImpactShapeInterface_rayTest(btGImpactShapeInterface* obj, const btScalar* rayFrom, const btScalar* rayTo, btCollisionWorld_RayResultCallback* resultCallback);
	EXPORT void btGImpactShapeInterface_setChildTransform(btGImpactShapeInterface* obj, int index, const btScalar* transform);
	EXPORT void btGImpactShapeInterface_unlockChildShapes(btGImpactShapeInterface* obj);
	EXPORT void btGImpactShapeInterface_updateBound(btGImpactShapeInterface* obj);

	EXPORT btGImpactCompoundShape_CompoundPrimitiveManager* btGImpactCompoundShape_CompoundPrimitiveManager_new(const btGImpactCompoundShape_CompoundPrimitiveManager* compound);
	EXPORT btGImpactCompoundShape_CompoundPrimitiveManager* btGImpactCompoundShape_CompoundPrimitiveManager_new2(btGImpactCompoundShape* compoundShape);
	EXPORT btGImpactCompoundShape_CompoundPrimitiveManager* btGImpactCompoundShape_CompoundPrimitiveManager_new3();
	EXPORT btGImpactCompoundShape* btGImpactCompoundShape_CompoundPrimitiveManager_getCompoundShape(btGImpactCompoundShape_CompoundPrimitiveManager* obj);
	EXPORT void btGImpactCompoundShape_CompoundPrimitiveManager_setCompoundShape(btGImpactCompoundShape_CompoundPrimitiveManager* obj, btGImpactCompoundShape* value);

	EXPORT btGImpactCompoundShape* btGImpactCompoundShape_new();
	EXPORT btGImpactCompoundShape* btGImpactCompoundShape_new2(bool children_has_transform);
	EXPORT void btGImpactCompoundShape_addChildShape(btGImpactCompoundShape* obj, const btScalar* localTransform, btCollisionShape* shape);
	EXPORT void btGImpactCompoundShape_addChildShape2(btGImpactCompoundShape* obj, btCollisionShape* shape);
	EXPORT btGImpactCompoundShape_CompoundPrimitiveManager* btGImpactCompoundShape_getCompoundPrimitiveManager(btGImpactCompoundShape* obj);

	EXPORT btGImpactMeshShapePart_TrimeshPrimitiveManager* btGImpactMeshShapePart_TrimeshPrimitiveManager_new(btStridingMeshInterface* meshInterface, int part);
	EXPORT btGImpactMeshShapePart_TrimeshPrimitiveManager* btGImpactMeshShapePart_TrimeshPrimitiveManager_new2(const btGImpactMeshShapePart_TrimeshPrimitiveManager* manager);
	EXPORT btGImpactMeshShapePart_TrimeshPrimitiveManager* btGImpactMeshShapePart_TrimeshPrimitiveManager_new3();
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_get_bullet_triangle(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, int prim_index, btTriangleShapeEx* triangle);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_get_indices(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, int face_index, unsigned int* i0, unsigned int* i1, unsigned int* i2);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_get_vertex(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, unsigned int vertex_index, btScalar* vertex);
	EXPORT int btGImpactMeshShapePart_TrimeshPrimitiveManager_get_vertex_count(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT const unsigned char* btGImpactMeshShapePart_TrimeshPrimitiveManager_getIndexbase(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT int btGImpactMeshShapePart_TrimeshPrimitiveManager_getIndexstride(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT PHY_ScalarType btGImpactMeshShapePart_TrimeshPrimitiveManager_getIndicestype(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT int btGImpactMeshShapePart_TrimeshPrimitiveManager_getLock_count(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT btScalar btGImpactMeshShapePart_TrimeshPrimitiveManager_getMargin(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT btStridingMeshInterface* btGImpactMeshShapePart_TrimeshPrimitiveManager_getMeshInterface(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT int btGImpactMeshShapePart_TrimeshPrimitiveManager_getNumfaces(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT int btGImpactMeshShapePart_TrimeshPrimitiveManager_getNumverts(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT int btGImpactMeshShapePart_TrimeshPrimitiveManager_getPart(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_getScale(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, btScalar* value);
	EXPORT int btGImpactMeshShapePart_TrimeshPrimitiveManager_getStride(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT PHY_ScalarType btGImpactMeshShapePart_TrimeshPrimitiveManager_getType(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT const unsigned char* btGImpactMeshShapePart_TrimeshPrimitiveManager_getVertexbase(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_lock(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setIndexbase(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, const unsigned char* value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setIndexstride(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, int value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setIndicestype(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, PHY_ScalarType value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setLock_count(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, int value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setMargin(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, btScalar value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setMeshInterface(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, btStridingMeshInterface* value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setNumfaces(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, int value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setNumverts(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, int value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setPart(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, int value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setScale(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, const btScalar* value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setStride(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, int value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setType(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, PHY_ScalarType value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_setVertexbase(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj, const unsigned char* value);
	EXPORT void btGImpactMeshShapePart_TrimeshPrimitiveManager_unlock(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj);

	EXPORT btGImpactMeshShapePart* btGImpactMeshShapePart_new();
	EXPORT btGImpactMeshShapePart* btGImpactMeshShapePart_new2(btStridingMeshInterface* meshInterface, int part);
	EXPORT int btGImpactMeshShapePart_getPart(btGImpactMeshShapePart* obj);
	EXPORT btGImpactMeshShapePart_TrimeshPrimitiveManager* btGImpactMeshShapePart_getTrimeshPrimitiveManager(btGImpactMeshShapePart* obj);
	EXPORT void btGImpactMeshShapePart_getVertex(btGImpactMeshShapePart* obj, int vertex_index, btScalar* vertex);
	EXPORT int btGImpactMeshShapePart_getVertexCount(btGImpactMeshShapePart* obj);

	EXPORT btGImpactMeshShape* btGImpactMeshShape_new(btStridingMeshInterface* meshInterface);
	EXPORT btStridingMeshInterface* btGImpactMeshShape_getMeshInterface(btGImpactMeshShape* obj);
	EXPORT btGImpactMeshShapePart* btGImpactMeshShape_getMeshPart(btGImpactMeshShape* obj, int index);
	EXPORT int btGImpactMeshShape_getMeshPartCount(btGImpactMeshShape* obj);
}
