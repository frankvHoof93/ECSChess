using System;
using Unity.Entities;
using Unity.Mathematics;

namespace ECSChess.Components.Movement
{
    /// <summary>
    /// Component used to contain a Destination for a moving Entity
    /// </summary>
    [Serializable]
    public struct Destination : IComponentData
    {
        #region Variables
        /// <summary>
        /// Position for Destination
        /// </summary>
        public readonly float3 Value;
        /// <summary>
        /// Maximum distance to Destination to have to have 'arrived'
        /// </summary>
        public readonly float Distance;
        /// <summary>
        /// Whether to 'Snap' to Destination when 'arrived' (Set Translation to Destination)
        /// </summary>
        public readonly bool Snap;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a Destination
        /// </summary>
        /// <param name="value">Position for Destination</param>
        /// <param name="distance">Maximum distance to Destination to have to have 'arrived'</param>
        /// <param name="snap">Whether to 'Snap' to Destination when 'arrived' (Set Translation to Destination)</param>
        public Destination(float3 value, float distance = 0f, bool snap = false)
        {
            Value = value;
            Distance = distance;
            Snap = snap;
        }
        #endregion
    }
}