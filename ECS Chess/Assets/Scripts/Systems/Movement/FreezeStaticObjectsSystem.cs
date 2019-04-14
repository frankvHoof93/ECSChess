using ECSChess.Components.Movement;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace ECSChess.Systems.Movement
{
    /// <summary>
    /// This system adds a Frozen-Component to any Entity without a Heading.
    /// <para>
    /// <para>-</para>
    /// The purpose of this operation is to have Entities skip the UpdateLoop of 
    /// Unity's Groups (when not moving), thereby speeding it up.
    /// </para>
    /// <para>
    /// By adding the component at the end of the frame, it allows the renderbounds to be updated 
    /// for any final movement before removal of the component as well.
    /// </para>
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(RenderMeshSystemV2))]
    public class FreezeStaticObjectsSystem : ComponentSystem
    {
        private Transform cameraTransform;
        /// <summary>
        /// Query of Entities for System
        /// </summary>
        private EntityQuery entityQuery;

        #region Methods
        /// <summary>
        /// Creates Query
        /// </summary>
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            EntityQueryDesc desc = new EntityQueryDesc
            {
                All = new ComponentType[] { ComponentType.ReadOnly<WorldRenderBounds>(), ComponentType.ReadOnly<RenderMesh>() },
                None = new ComponentType[] { typeof(Frozen), typeof(SkipFreeze), typeof(Heading) }
            };
            entityQuery = GetEntityQuery(desc);
            //Enabled = false; // DEMO
        }

        /// <summary>
        /// Adds a Frozen-Component to all Entities in the Query
        /// </summary>
        protected override void OnUpdate()
        {
            if (!cameraTransform)
                cameraTransform = Camera.main.transform;
            if (!cameraTransform.hasChanged)
                EntityManager.AddComponent(entityQuery, typeof(Frozen));
            else
                cameraTransform.hasChanged = false;
        }
        #endregion
    }
}