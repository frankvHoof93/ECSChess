using ECSChess.Components.Movement;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECSChess.Jobs.Physics.Movement
{
    /// <summary>
    /// Job which moves an Entity, by updating its Translation
    /// <para>
    /// Handles ending movement (through Destination) as well
    /// </para>
    /// </summary>
    public struct TranslationJob : IJobForEachWithEntity<Translation, Heading>
    {
        #region Variables
        /// <summary>
        /// delta-T (TimeSpan)
        /// </summary>
        [ReadOnly]
        public float dT;
        /// <summary>
        /// Destinations for Entities (Where applicable)
        /// </summary>
        [ReadOnly]
        public ComponentDataFromEntity<Destination> destinations;
        /// <summary>
        /// CommandBuffer for Add-/Remove-Commands
        /// </summary>
        [ReadOnly]
        public EntityCommandBuffer.Concurrent buffer;
        #endregion

        #region Methods
        /// <summary>
        /// Moves Entity by Heading
        /// <para>
        /// Handles ending movement (through Destination) as well
        /// </para>
        /// </summary>
        /// <param name="entity">Entity to Move</param>
        /// <param name="index">Index of Job</param>
        /// <param name="c0">Translation for Entity</param>
        /// <param name="c1">Heading for Entity</param>
        public void Execute(Entity entity, int index, ref Translation c0, [ReadOnly]ref Heading c1)
        {
            if (!destinations.Exists(entity)) // Entity does not have Destination
                Move(ref c0, c1, dT);
            else // Handle movement with Destination
            {
                Destination dest = destinations[entity];
                if (math.distance(c0.Value, dest.Value) > dest.Distance)
                    Move(ref c0, c1, dT); // Destination not reached. Perform regular movement
                else
                {
                    if (dest.Snap) // Snap to destination
                        c0.Value = dest.Value;
                    // Remove Heading & Destination
                    buffer.RemoveComponent<Destination>(index, entity);
                    buffer.RemoveComponent<Heading>(index, entity);
                }
            }
        }

        /// <summary>
        /// Moves a Translation by Heading 
        /// <para>
        /// X(t) = X(t-1) + H * dT
        /// </para>
        /// </summary>
        /// <param name="c0">ref: Translation to modify</param>
        /// <param name="c1">Movement to perform (Heading with Speed)</param>
        /// <param name="dT">TimeSpan for Movement</param>
        private void Move(ref Translation c0, Heading c1, float dT)
        {
            c0.Value = c0.Value + c1.Value * dT;
        }
        #endregion
    }
}