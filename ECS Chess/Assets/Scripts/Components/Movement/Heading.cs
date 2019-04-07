using System;
using Unity.Entities;
using Unity.Mathematics;

namespace ECSChess.Components.Movement
{
    /// <summary>
    /// Component used to store Heading (including Speed) for a moving Entity
    /// </summary>
    [Serializable]
    public struct Heading : IComponentData
    {
        /// <summary>
        /// Value for Heading (Including Speed)
        /// </summary>
        public float3 Value;
        /// <summary>
        /// Normalized Value for Heading
        /// </summary>
        public float3 Norm { get { return math.normalize(Value); } }
        /// <summary>
        /// Magnitude of Heading (Speed)
        /// </summary>
        public float Mag { get { return math.length(Value); } }
    }
}