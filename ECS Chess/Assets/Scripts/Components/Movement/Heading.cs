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
        #region Variables
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
        public float Mag { get { return math.length(Value); } set { Value = Norm * value; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a Heading
        /// </summary>
        /// <param name="value">Value for Heading (including magnitude)</param>
        public Heading(float3 value)
        {
            Value = value;
        }

        /// <summary>
        /// Constructor for a Heading
        /// </summary>
        /// <param name="dir">Direction for Heading</param>
        /// <param name="mag">Magnitude for Heading</param>
        public Heading(float3 dir, float mag)
        {
            Value = math.normalize(dir) * mag;
        }
        #endregion
    }
}