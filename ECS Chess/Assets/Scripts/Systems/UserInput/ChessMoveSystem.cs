using ECSChess.Components.Input;
using ECSChess.Components.Movement;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECSChess.Systems.UserInput
{
    /// <summary>
    /// System used to issue a movement to a ChessPiece
    /// </summary>
    public class ChessMoveSystem : ComponentSystem
    {
        /// <summary>
        /// Selected Entities
        /// </summary>
        private EntityQuery selected;

        #region Methods
        /// <summary>
        /// Initializes EntityQuery
        /// </summary>
        protected override void OnCreateManager()
        {
            selected = GetEntityQuery(typeof(Selected), typeof(Translation));
            base.OnCreateManager();
        }

        /// <summary>
        /// Issues Movement-Commands
        /// </summary>
        protected override void OnUpdate()
        {
            // Debug move
            if (Input.GetKeyDown(KeyCode.F1))
                using (NativeArray<Entity> selectedEntities = selected.ToEntityArray(Allocator.TempJob)) // Must be allocated as TempJob, because we're in a JobComponentSystem?
                {
                    EntityManager.AddComponent(selectedEntities, typeof(Heading));
                    foreach (Entity entity in selectedEntities)
                        EntityManager.SetComponentData(entity, new Heading { Value = new float3(1, 0, 0) });
                }
            // Debug Move with Destination
            if (Input.GetKeyDown(KeyCode.F2))
                using (NativeArray<Entity> selectedEntities = selected.ToEntityArray(Allocator.TempJob)) // Must be allocated as TempJob, because we're in a JobComponentSystem?
                using (NativeArray<Translation> translations = selected.ToComponentDataArray<Translation>(Allocator.TempJob))
                {
                    Destination dest = new Destination(new float3(4, 0, 4), .1f, true);
                    EntityManager.AddComponent(selectedEntities, typeof(Heading));
                    EntityManager.AddComponent(selectedEntities, typeof(Destination));
                    EntityManager.RemoveComponent(selectedEntities, typeof(Selected));
                    float duration = 2f; // Duration for movement is 2 seconds
                    for (int i = 0; i < selectedEntities.Length; i++)
                    {
                        float3 pos = translations[i].Value;
                        Heading h = new Heading(dest.Value - pos);
                        float dist = h.Mag; // Distance to Destination
                        float speed = dist / duration; // Set Speed
                        h.Mag = speed;
                        EntityManager.SetComponentData(selectedEntities[i], h);
                        EntityManager.SetComponentData(selectedEntities[i], dest);
                    }
                }
        }
        #endregion
    }
}