using System;
using Unity.Entities;

namespace ECSChess.Misc.DataTypes
{
    /// <summary>
    /// A RayIntersectionResult holds the result of checking a Raycast against an Entity
    /// </summary>
    [Serializable]
    public struct RayIntersectionResult
    {
        #region Variables
        /// <summary>
        /// Whether an Intersection occured
        /// <para>
        ///     Due to invalid intersections being sometimes possible (e.g. first-frame), this also checks whether the distance is between 0 and float.MaxValue
        /// </para>
        /// </summary>
        public bool Value { get { return Intersection && Distance >= 0 && Distance < float.MaxValue; } }
        /// <summary>
        /// Outcome of Intersection (does not check distance)
        /// </summary>
        public bool Intersection;
        /// <summary>
        /// Distance from Ray-Origin
        /// </summary>
        public float Distance;
        /// <summary>
        /// Entity which was Intersected
        /// </summary>
        public Entity Entity;
        #endregion

        #region Methods
        #region Operators
        /// <summary>
        /// Implicit Cast from RayIntersectionResult to Boolean (by Value)
        /// </summary>
        /// <param name="res">RayIntersectionResult to Cast</param>
        public static implicit operator bool(RayIntersectionResult res) => res.Value;
        #endregion

        #region Overrides
        /// <summary>
        /// String-Representation of RayIntersectionResult
        /// </summary>
        /// <returns>Entity: [E], Intersection: [I], Distance: [D]</returns>
        public override string ToString()
        {
            return string.Format("Entity: [{0}], Intersection: [{1}], Distance: [{2}]", World.Active.EntityManager.GetName(Entity), Value, Distance);
        }
        #endregion
        #endregion
    }
}