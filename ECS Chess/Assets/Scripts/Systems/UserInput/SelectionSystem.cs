using ECSChess.Components.Input;
using ECSChess.Jobs;
using ECSChess.Misc.DataTypes;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ECSChess.Systems.UserInput
{
    /// <summary>
    /// System used to Select a ChessPiece
    /// <para>
    ///     Uses MouseInput and Raycasting to Select ChessPieces
    /// </para>
    /// </summary>
    public class SelectionSystem : JobComponentSystem
    {
        #region InnerClasses
        /// <summary>
        /// System that processes EntityCommands for the SelectionSystem
        /// </summary>
        [UpdateAfter(typeof(SelectionSystem))]
        public class SelectionCommandBufferSystem : EntityCommandBufferSystem { }
        #endregion

        #region Variables
        /// <summary>
        /// Camera used to process Mouse-Input
        /// </summary>
        private Camera camera;
        /// <summary>
        /// Query of all Selectable Entities
        /// </summary>
        private EntityQuery selectables;
        /// <summary>
        /// Query of all currently Hovered Entities
        /// </summary>
        private EntityQuery currentHovered;
        /// <summary>
        /// Query of all currently Selected Entities
        /// </summary>
        private EntityQuery currentSelected;
        /// <summary>
        /// CommandBufferSystem used to issue EntityCommands using an EntityCommandBuffer
        /// </summary>
        private SelectionCommandBufferSystem commandBufferSystem;

        #region NativeArrays
        /// <summary>
        /// Results of Intersection of Raycast
        /// </summary>
        private NativeArray<RayIntersectionResult> intersectionResults;
        #endregion
        #endregion
        
        #region Methods
        #region Unity
        /// <summary>
        /// Runs when SelectionSystem is Created. Sets up EntityQueries & CommandBufferSystem
        /// </summary>
        protected override void OnCreateManager()
        {
            commandBufferSystem = World.Active.GetOrCreateSystem<SelectionCommandBufferSystem>();
            selectables = GetEntityQuery(typeof(Selectable));
            currentHovered = GetEntityQuery(typeof(Hovered));
            currentSelected = GetEntityQuery(typeof(Selected));
            base.OnCreateManager();
        }

        /// <summary>
        /// Runs when SelectionSystem is Destroyed. Cleans up NativeArrays
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
            inputDeps.Complete();
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
            // Handle Keyboard
            HandleKeyboardInput();
            // Get position of Mouse on Screen
            Vector2 mousePos = Input.mousePosition;
            // Check whether MousePos is in Screen
            if (mousePos.x >= 0 && mousePos.x <= Screen.width
                && mousePos.y >= 0 && mousePos.y <= Screen.height)
                return HandleMouseInput(inputDeps, mousePos); // Handle Mouse
            else return inputDeps; // Nothing to do, return inputDeps
        }
        #endregion

        #region Private
        /// <summary>
        /// Handles KeyboardInput (For Deselection)
        /// </summary>
        private void HandleKeyboardInput()
        {
            // Deselect
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                using (NativeArray<Entity> selectedEntities = currentSelected.ToEntityArray(Allocator.TempJob)) // Must be allocated as TempJob, because we're in a JobComponentSystem?
                    foreach (Entity entity in selectedEntities)
                        EntityManager.RemoveComponent(entity, typeof(Selected));
        }

        /// <summary>
        /// Handles MouseInput (for Hovering & Selection)
        /// </summary>
        /// <param name="inputDeps">InputDependencies for Jobs</param>
        /// <param name="mousePos">MousePosition on Screen</param>
        /// <returns>Output-Dependencies for Jobs</returns>
        private JobHandle HandleMouseInput(JobHandle inputDeps, Vector2 mousePos)
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
            // Create Buffer to send EntityCommands to.
            EntityCommandBuffer buffer = commandBufferSystem.CreateCommandBuffer();
            // Create SelectionJob
            SelectClosestIntersectionJob selectionJob = new SelectClosestIntersectionJob
            {
                IntersectionResults = intersectionResults,
                Buffer = buffer,
                Click = Input.GetMouseButtonDown(0),
                Hovered = currentHovered.ToEntityArray(Allocator.TempJob), // Deallocated by Job itself
                Selected = currentSelected.ToEntityArray(Allocator.TempJob) // Deallocated by Job itself
            };
            // Create final handle (SelectionJob with dependency on SortingJob)
            returnHandle = selectionJob.Schedule(returnHandle);
            // Add dependency to buffer
            commandBufferSystem.AddJobHandleForProducer(returnHandle);
            return returnHandle;
        }
        #endregion
        #endregion
    }
}