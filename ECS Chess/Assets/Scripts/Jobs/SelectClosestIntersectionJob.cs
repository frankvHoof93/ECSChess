using ECSChess.Components.Input;
using ECSChess.Misc.DataTypes;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ECSChess.Jobs
{
    /// <summary>
    /// Job that selects the First RayIntersectionResult in an Array
    /// <para>
    ///     Only selects if RayIntersectionResult actually had Intersection (and Click is true).
    ///     Deselects all previously selected Entities in Selected (if Click is true).
    /// </para>
    /// <para>
    ///     Deallocates Arrays upon completion (Except IntersectionResults)
    ///     Adds/Removes Hovered to Entities as well
    /// </para>
    /// </summary>
    public struct SelectClosestIntersectionJob : IJob
    {
        [ReadOnly]
        public NativeArray<RayIntersectionResult> IntersectionResults;
        [ReadOnly, DeallocateOnJobCompletion]
        public NativeArray<Entity> Hovered;
        [ReadOnly, DeallocateOnJobCompletion]
        public NativeArray<Entity> Selected;
        [ReadOnly]
        public bool Click;
        public EntityCommandBuffer Buffer;
        
        public void Execute()
        {
            RayIntersectionResult result = IntersectionResults[0];
            for (int i = 0; i < Hovered.Length; i++) //  Should be Length 1 at most
                if (!result || Hovered[i] != result.Entity)
                    Buffer.RemoveComponent(Hovered[i], typeof(Hovered));
            if (result)
            {
                if (!Hovered.Contains(result.Entity))
                {
                    Buffer.AddComponent(result.Entity, new Hovered());
                }
                if (Click)
                {
                    for (int i = 0; i < Selected.Length; i++) //  Should be Length 1 at most
                        if (Selected[i] != result.Entity)
                            Buffer.RemoveComponent(Selected[i], typeof(Selected));
                    if (!Selected.Contains(result.Entity)) // Only add once
                    {
                        Buffer.AddComponent(result.Entity, new Selected());
                    }
                }
            }
        }
    }
}
