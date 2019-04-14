using ECSChess.Components.Input;
using ECSChess.Misc.DataTypes;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ECSChess.Jobs.Selection
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
        /// Array of all IntersectionResults (sorted by distance)
        /// </summary>
        [ReadOnly]
        private readonly NativeArray<RayIntersectionResult> intersectionResults;
        /// <summary>
        /// Entities that currently have a Hovered-Component
        /// </summary>
        [ReadOnly, DeallocateOnJobCompletion]
        private readonly NativeArray<Entity> hovered;
        /// <summary>
        /// Entities that currently have a Selected-Component
        /// </summary>
        [ReadOnly, DeallocateOnJobCompletion]
        private readonly NativeArray<Entity> selected;
        /// <summary>
        /// Whether a MouseClick occured (Whether Selection should be processed)
        /// </summary>
        [ReadOnly]
        private readonly bool click;
        /// <summary>
        /// CommandBuffer used to process Add-/Remove-Component calls
        /// </summary>
        [ReadOnly]
        private readonly EntityCommandBuffer buffer;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a SelectClosestIntersectionJob
        /// </summary>
        /// <param name="raycastResults">Array of IntersectionResults (first one is only one used)</param>
        /// <param name="hoveredEntities">Array of currently hovered Entities</param>
        /// <param name="selectedEntities">Array of currently selected Entities</param>
        /// <param name="mouseClick">Whether a mouseclick occurred</param>
        /// <param name="cmd">CommandBuffer used to process Add-/Remove-Component calls</param>
        public SelectClosestIntersectionJob(NativeArray<RayIntersectionResult> raycastResults, NativeArray<Entity> hoveredEntities,
                                            NativeArray<Entity> selectedEntities, bool mouseClick, EntityCommandBuffer cmd)
        {
            intersectionResults = raycastResults;
            hovered = hoveredEntities;
            selected = selectedEntities;
            click = mouseClick;
            buffer = cmd;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds Selected-Component to closest intersection (if Click is true)
        /// Adds Hovered-Component to closest intersection (always)
        /// Removes Selected- & Hovered-Component from previous selection/hovered
        /// </summary>
        public void Execute()
        {
            RayIntersectionResult result = intersectionResults[0];
            for (int i = 0; i < hovered.Length; i++) //  Should be Length 1 at most
                if (!result || hovered[i] != result.Entity)
                    buffer.RemoveComponent(hovered[i], typeof(Hovered)); // Remove previous hovered (if not currently hovered)
            if (result)
            {
                if (!hovered.Contains(result.Entity))
                    buffer.AddComponent(result.Entity, new Hovered()); // Add current Hovered (if not yet added previously)
                if (click)
                {
                    for (int i = 0; i < selected.Length; i++) //  Should be Length 1 at most
                        if (selected[i] != result.Entity)
                            buffer.RemoveComponent(selected[i], typeof(Selected)); // Remove previous Selected (if not currently selecting)
                    if (!selected.Contains(result.Entity)) // Only add once
                        buffer.AddComponent(result.Entity, new Selected()); // Add current Selected (if not yet added previously)
                }
            }
        }
        #endregion
    }
}
