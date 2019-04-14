using ECSChess.Components.Movement;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECSChess.Systems.Movement
{
    [UpdateBefore(typeof(EndFrameCompositeScaleSystem))]
    [UpdateInGroup(typeof(TransformSystemGroup))]
    public class UnFreezeStaticObjectsForCameraSystem : ComponentSystem
    {
        #region Variables
        /// <summary>
        /// Transform for Camera. System only executes when transform has changed
        /// </summary>
        private Transform cameraTransform;
        /// <summary>
        /// Query for Entities with Frozen-Component
        /// </summary>
        private EntityQuery entityQuery;
        #endregion

        #region Methods
        /// <summary>
        /// Called when System is created
        /// Creates EntityQuery
        /// </summary>
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            EntityQueryDesc desc = new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(Frozen) },
                None = new ComponentType[] { typeof(SkipFreeze) }
            };
            entityQuery = GetEntityQuery(desc);
        }

        /// <summary>
        /// Called when system updates.
        /// Removes Frozen-Component from all Entities in Query when camera transform has changed
        /// </summary>
        protected override void OnUpdate()
        {
            if (!cameraTransform)
                cameraTransform = Camera.main.transform;
            if (cameraTransform.hasChanged)
                EntityManager.RemoveComponent(entityQuery, typeof(Frozen));
        }
        #endregion
    }
}