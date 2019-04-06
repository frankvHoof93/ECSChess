using ECSChess.Components.Input;
using ECSChess.Jobs;
using ECSChess.Misc.DataTypes;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ECSChess.Systems
{
    /// <summary>
    /// System used to Select a ChessPiece
    /// <para>
    ///     Uses MouseInput and Raycasting to Select ChessPieces
    /// </para>
    /// </summary>
    public class SelectionSystem : JobComponentSystem
    {
        #region Variables
        /// <summary>
        /// Camera used to process Mouse-Input
        /// </summary>
        private Camera camera;
        /// <summary>
        /// Query of all Selectable Entities
        /// </summary>
        private EntityQuery selectables;

        #region NativeArrays
        /// <summary>
        /// Results of Intersection of Raycast
        /// </summary>
        private NativeArray<RayIntersectionResult> intersectionResults;
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Runs when SelectionSystem is Created. Sets up EntityQuery
        /// </summary>
        protected override void OnCreateManager()
        {
            EntityQueryDesc query = new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(Selectable) }
            };
            selectables = GetEntityQuery(query);
            base.OnCreateManager();
        }

        /// <summary>
        /// Runs when SelectionSystem is Destroyed. Cleans up NativeArray
        /// </summary>
        protected override void OnDestroyManager()
        {
            if (intersectionResults.IsCreated)
                intersectionResults.Dispose();
            base.OnDestroyManager();
        }

        /// <summary>
        /// Handles Selection
        /// </summary>
        /// <param name="inputDeps">Input-Dependencies</param>
        /// <returns>JobHandle for final job</returns>
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (camera == null)
                camera = Camera.main;
            int amountOfIntersectables = selectables.CalculateLength();
            // Check if creation of (new) NativeArray is required
            if (!intersectionResults.IsCreated || !intersectionResults.Length.Equals(amountOfIntersectables))
            {
                // Dispose previous if it exists
                if (intersectionResults.IsCreated)
                    intersectionResults.Dispose();
                // Create new array. Persistent because it can be re-used as long as the amount of selectables does not change (i.e. Pieces get slain)
                // Size of array should only go down during play
                intersectionResults = new NativeArray<RayIntersectionResult>(amountOfIntersectables, Allocator.Persistent);
            }
            // Get position of Mouse on Screen
            Vector2 mousePos = Input.mousePosition;
            // Check whether MousePos is in Screen
            if (mousePos.x >= 0 && mousePos.x <= Screen.width
                && mousePos.y >= 0 && mousePos.y <= Screen.height)
            {
                Ray r = camera.ScreenPointToRay(mousePos);
                // Create RaycastJob (Parallel)
                RayCastJob<Selectable> jobRayCast = new RayCastJob<Selectable>
                {
                    Ray = r,
                    Results = intersectionResults
                };
                // Schedule RaycastJob
                JobHandle returnHandle = jobRayCast.Schedule(this, inputDeps);
                // Create SortingJob (Not Parallel)
                SortIntersectionJob sortingJob = new SortIntersectionJob
                {
                    Array = intersectionResults
                };
                // Schedule SortingJob with dependency on RaycastJob
                returnHandle = sortingJob.Schedule(returnHandle);

                // Return final handle
                return returnHandle;
            }
            else return inputDeps; // Nothing to do, return inputDeps
        }
        #endregion
    }
}