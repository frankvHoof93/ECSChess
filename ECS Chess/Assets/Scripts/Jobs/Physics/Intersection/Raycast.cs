using ECSChess.Misc.DataTypes;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace ECSChess.Jobs.Physics.Intersection
{
    /// <summary>
    /// Checks for Intersection with a Ray, by using WorldRenderBounds
    /// TODO: Use Mesh instead, so raycast is actually accurate (instead of using a box around the object)
    /// </summary>
    [BurstCompile]
    public struct RayCastJob : IJobForEachWithEntity<WorldRenderBounds>
    {
        #region Variables
        /// <summary>
        /// Ray to intersect with
        /// </summary>
        [ReadOnly]
        private readonly Ray Ray;
        /// <summary>
        /// OUT: Results for intersections
        /// </summary>
        [WriteOnly]
        private NativeArray<RayIntersectionResult> Results;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a RaycastJob
        /// </summary>
        /// <param name="r">Ray to check against</param>
        /// <param name="outputArray">Array for Outputs</param>
        public RayCastJob(Ray r, NativeArray<RayIntersectionResult> outputArray)
        {
            Ray = r;
            Results = outputArray;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Intersects Bounds with Ray
        /// </summary>
        /// <param name="entity">Entity for Bounds</param>
        /// <param name="index">Index for Job (parallel)</param>
        /// <param name="c0">WorldRenderBounds for Entity</param>
        public void Execute([ReadOnly]Entity entity, [ReadOnly]int index, [ReadOnly]ref WorldRenderBounds c0)
        {
            bool result = c0.Value.ToBounds().IntersectRay(Ray, out float distance);
            // Set distance to Float.MaxValue if there was no intersection
            Results[index] = new RayIntersectionResult(result, result ? distance : float.MaxValue, entity);
        }
        #endregion
    }

    /// <summary>
    /// Checks for Intersection with a Ray, by using WorldRenderBounds
    /// TODO: Use Mesh instead, so raycast is actually accurate (instead of using a box around the object)
    /// </summary>
    /// <typeparam name="T">Used to filter on Entities by a (Tag-)Component</typeparam>
    [BurstCompile]
    public struct RayCastJob<T> : IJobForEachWithEntity<WorldRenderBounds, T> where T : struct, IComponentData
    {
        #region Variables
        /// <summary>
        /// Ray to intersect with
        /// </summary>
        [ReadOnly]
        private readonly Ray Ray;
        /// <summary>
        /// OUT: Results for intersections
        /// </summary>
        [WriteOnly]
        private NativeArray<RayIntersectionResult> Results;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a RaycastJob
        /// </summary>
        /// <param name="r">Ray to check against</param>
        /// <param name="outputArray">Array for Outputs</param>
        public RayCastJob(Ray r, NativeArray<RayIntersectionResult> outputArray)
        {
            Ray = r;
            Results = outputArray;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Intersects Bounds with Ray
        /// </summary>
        /// <param name="entity">Entity for Bounds</param>
        /// <param name="index">Index for Job (parallel)</param>
        /// <param name="c0">WorldRenderBounds for Entity</param>
        /// <param name="c1">Filter-Tag for Entity</param>
        public void Execute([ReadOnly]Entity entity, [ReadOnly]int index, [ReadOnly]ref WorldRenderBounds c0, [ReadOnly]ref T c1)
        {
            bool result = c0.Value.ToBounds().IntersectRay(Ray, out float distance);
            // Set distance to Float.MaxValue if there was no intersection
            Results[index] = new RayIntersectionResult(result, result ? distance : float.MaxValue, entity);
        }
        #endregion
    }
}