using ECSChess.Components.Input;
using ECSChess.Misc.DataTypes;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

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
        #region Variables
        /// <summary>
        /// Array of all IntersectionResults
        /// </summary>
        [ReadOnly]
        public NativeArray<RayIntersectionResult> IntersectionResults;
        /// <summary>
        /// Entities that currently have a Hovered-Component
        /// </summary>
        [ReadOnly, DeallocateOnJobCompletion]
        public NativeArray<Entity> Hovered;
        /// <summary>
        /// Entities that currently have a Selected-Component
        /// </summary>
        [ReadOnly, DeallocateOnJobCompletion]
        public NativeArray<Entity> Selected;
        /// <summary>
        /// Whether a MouseClick occured (Whether Selection should be processed)
        /// </summary>
        [ReadOnly]
        public bool Click;
        /// <summary>
        /// CommandBuffer used to process Add-/Remove-Component calls
        /// </summary>
        [ReadOnly]
        public EntityCommandBuffer Buffer;
        #endregion

        /// <summary>
        /// Adds Selected-Component to closest intersection (if Click is true)
        /// Adds Hovered-Component to closest intersection (always)
        /// Removes Selected- & Hovered-Component from previous selection/hovered
        /// </summary>
        public void Execute()
        {
            RayIntersectionResult result = IntersectionResults[0];
            for (int i = 0; i < Hovered.Length; i++) //  Should be Length 1 at most
                if (!result || Hovered[i] != result.Entity)
                    Buffer.RemoveComponent(Hovered[i], typeof(Hovered)); // Remove previous hovered (if not currently hovered)
            if (result)
            {
                if (!Hovered.Contains(result.Entity))
                    Buffer.AddComponent(result.Entity, new Hovered()); // Add current Hovered (if not yet added previously)
                if (Click)
                {
                    for (int i = 0; i < Selected.Length; i++) //  Should be Length 1 at most
                        if (Selected[i] != result.Entity)
                            Buffer.RemoveComponent(Selected[i], typeof(Selected)); // Remove previous Selected (if not currently selecting)
                    if (!Selected.Contains(result.Entity)) // Only add once
                        Buffer.AddComponent(result.Entity, new Selected()); // Add current Selected (if not yet added previously)
                }
            }
        }
    }
}
