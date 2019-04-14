using ECSChess.Components.Movement;
using ECSChess.Jobs.Physics.Movement;
using ECSChess.Systems.UserInput;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ECSChess.Systems.Movement
{
    [UpdateBefore(typeof(SelectionSystem))]
    public class TranslationSystem : JobComponentSystem
    {
        /// <summary>
        /// CommandBufferSystem for (concurrent) Remove-Commands
        /// </summary>
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        #region Methods
        /// <summary>
        /// Grabs reference to CommandBufferSystem
        /// </summary>
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            commandBufferSystem = World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        /// <summary>
        /// Creates & Schedules TranslationJob
        /// </summary>
        /// <param name="inputDeps">InputDependecies</param>
        /// <returns>OutputDependencies</returns>
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // Grab Destinations (Where applicable)
            ComponentDataFromEntity<Destination> destinations = GetComponentDataFromEntity<Destination>(true);
            // Create TranslationJob
            TranslationJob job = new TranslationJob(Time.deltaTime, destinations, commandBufferSystem.CreateCommandBuffer().ToConcurrent());
            // Schedule Job
            JobHandle returnHandle = job.Schedule(this, inputDeps);
            // Add JobHandle to producer of CommandBuffer
            commandBufferSystem.AddJobHandleForProducer(returnHandle);
            return returnHandle;
        }
        #endregion
    }
}