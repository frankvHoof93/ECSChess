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
            ComponentDataFromEntity<Destination> destinations = GetComponentDataFromEntity<Destination>(true);
            TranslationJob job = new TranslationJob
            {
                destinations = destinations,
                dT = Time.deltaTime,
                buffer = commandBufferSystem.CreateCommandBuffer().ToConcurrent()
            };
            JobHandle returnHandle = job.Schedule(this, inputDeps);
            commandBufferSystem.AddJobHandleForProducer(returnHandle);
            return returnHandle;
        }
    }
}