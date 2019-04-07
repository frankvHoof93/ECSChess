using ECSChess.Misc.DataTypes;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace ECSChess.Jobs
{
    /// <summary>
    /// Checks for Intersection with a Ray, by using WorldRenderBounds
    /// TODO: Use Mesh instead, so raycast is actually accurate (instead of using a box around the object)
    /// </summary>
    [BurstCompile]
    public struct RayCastJob : IJobForEachWithEntity<WorldRenderBounds>
    {
        /// <summary>
        /// Ray to intersect with
        /// </summary>
        [ReadOnly]
        public Ray Ray;
        /// <summary>
        /// OUT: Results for intersections
        /// </summary>
        [WriteOnly]
        public NativeArray<RayIntersectionResult> Results;

        public void Execute([ReadOnly]Entity entity, [ReadOnly]int index, [ReadOnly]ref WorldRenderBounds c0)
        {
            bool result = c0.Value.ToBounds().IntersectRay(Ray, out float distance);
            // Set distance to Float.MaxValue if there was no intersection
            Results[index] = new RayIntersectionResult { Intersection = result, Distance = result ? distance : float.MaxValue, Entity = entity };
        }
    }

    /// <summary>
    /// Checks for Intersection with a Ray, by using WorldRenderBounds
    /// TODO: Use Mesh instead, so raycast is actually accurate (instead of using a box around the object)
    /// </summary>
    /// <typeparam name="T">Used to filter on Entities by a single (Tag-)Component</typeparam>
    [BurstCompile]
    public struct RayCastJob<T> : IJobForEachWithEntity<WorldRenderBounds, T> where T : struct, IComponentData
    {
        /// <summary>
        /// Ray to intersect with
        /// </summary>
        [ReadOnly]
        public Ray Ray;
        /// <summary>
        /// OUT: Results for intersections
        /// </summary>
        [WriteOnly]
        public NativeArray<RayIntersectionResult> Results;

        public void Execute([ReadOnly]Entity entity, [ReadOnly]int index, [ReadOnly]ref WorldRenderBounds c0, [ReadOnly]ref T c1)
        {
            bool result = c0.Value.ToBounds().IntersectRay(Ray, out float distance);
            // Set distance to Float.MaxValue if there was no intersection
            Results[index] = new RayIntersectionResult { Intersection = result, Distance = result ? distance : float.MaxValue, Entity = entity };
        }
    }

    /// <summary>
    /// Sorts Array of IntersectionResults by Distance
    /// </summary>
    [BurstCompile]
    public struct SortIntersectionJob : IJob
    {
        /// <summary>
        /// INOUT: Array to Sort
        /// </summary>
        public NativeArray<RayIntersectionResult> Array;

        /// <summary>
        /// Insertion Sort on Array
        /// </summary>
        public void Execute()
        {
            // Insertion sort on array
            int inner;
            RayIntersectionResult temp;
            for (int outer = 1; outer < Array.Length; outer++)
            {
                if (!Array[outer])
                    continue; // No intersection. Skip
                temp = Array[outer];
                inner = outer;
                while (inner > 0 && Array[inner - 1].Distance >= temp.Distance || (!temp && Array[inner - 1]))
                {
                    Array[inner] = Array[inner - 1];
                    inner -= 1;
                }
                Array[inner] = temp;
            }
        }
    }
}