#include "main.h"

extern "C"
{
	EXPORT btConvex2dConvex2dAlgorithm_CreateFunc* btConvex2dConvex2dAlgorithm_CreateFunc_new(btVoronoiSimplexSolver* simplexSolver, btConvexPenetrationDepthSolver* pdSolver);
	EXPORT int btConvex2dConvex2dAlgorithm_CreateFunc_getMinimumPointsPerturbationThreshold(btConvex2dConvex2dAlgorithm_CreateFunc* obj);
	EXPORT int btConvex2dConvex2dAlgorithm_CreateFunc_getNumPerturbationIterations(btConvex2dConvex2dAlgorithm_CreateFunc* obj);
	EXPORT btConvexPenetrationDepthSolver* btConvex2dConvex2dAlgorithm_CreateFunc_getPdSolver(btConvex2dConvex2dAlgorithm_CreateFunc* obj);
	EXPORT btVoronoiSimplexSolver* btConvex2dConvex2dAlgorithm_CreateFunc_getSimplexSolver(btConvex2dConvex2dAlgorithm_CreateFunc* obj);
	EXPORT void btConvex2dConvex2dAlgorithm_CreateFunc_setMinimumPointsPerturbationThreshold(btConvex2dConvex2dAlgorithm_CreateFunc* obj, int value);
	EXPORT void btConvex2dConvex2dAlgorithm_CreateFunc_setNumPerturbationIterations(btConvex2dConvex2dAlgorithm_CreateFunc* obj, int value);
	EXPORT void btConvex2dConvex2dAlgorithm_CreateFunc_setPdSolver(btConvex2dConvex2dAlgorithm_CreateFunc* obj, btConvexPenetrationDepthSolver* value);
	EXPORT void btConvex2dConvex2dAlgorithm_CreateFunc_setSimplexSolver(btConvex2dConvex2dAlgorithm_CreateFunc* obj, btVoronoiSimplexSolver* value);

	EXPORT btConvex2dConvex2dAlgorithm* btConvex2dConvex2dAlgorithm_new(btPersistentManifold* mf, const btCollisionAlgorithmConstructionInfo* ci, const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap, btVoronoiSimplexSolver* simplexSolver, btConvexPenetrationDepthSolver* pdSolver, int numPerturbationIterations, int minimumPointsPerturbationThreshold);
	EXPORT const btPersistentManifold* btConvex2dConvex2dAlgorithm_getManifold(btConvex2dConvex2dAlgorithm* obj);
	EXPORT void btConvex2dConvex2dAlgorithm_setLowLevelOfDetail(btConvex2dConvex2dAlgorithm* obj, bool useLowLevel);
}
