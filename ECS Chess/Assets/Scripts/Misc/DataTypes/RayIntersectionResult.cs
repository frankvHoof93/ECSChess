using System;
using Unity.Entities;

namespace ECSChess.Misc.DataTypes
{
    /// <summary>
    /// A RayIntersectionResult holds the result of checking a Raycast against an Entity
    /// </summary>
    [Serializable]
    public struct RayIntersectionResult : IEquatable<RayIntersectionResult>, IComparable<RayIntersectionResult>
    {
        #region Variables
        /// <summary>
        /// Whether an Intersection occured
        /// <para>
        ///     Due to invalid intersections being sometimes possible (e.g. first-frame), this also checks whether the distance is between 0 and float.MaxValue
        /// </para>
        /// </summary>
        public bool Value => Intersection && Distance >= 0 && Distance < float.MaxValue;
        /// <summary>
        /// Outcome of Intersection (does not check distance)
        /// </summary>
        public readonly bool Intersection;
        /// <summary>
        /// Distance from Ray-Origin
        /// </summary>
        public readonly float Distance;
        /// <summary>
        /// Entity which was Intersected
        /// </summary>
        public readonly Entity Entity;
        #endregion

        public RayIntersectionResult(bool hit, float dist, Entity entity)
        {
            Intersection = hit;
            Distance = dist;
            Entity = entity;
        }

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
        /// <summary>
        /// Equality-Comparison for RayIntersectionResult.
        /// <para>
        /// Checks EntityIndex, Distance and Result
        /// </para>
        /// </summary>
        /// <param name="other">RayIntersectionResult to equate to</param>
        /// <returns></returns>
        public bool Equals(RayIntersectionResult other)
        {
            // Bool, then Int, then Float (optimized speed for early outs)
            return other.Intersection == Intersection && other.Entity.Index == Entity.Index && other.Distance.Equals(Distance);
        }
        /// <summary>
        /// Comparison for RayIntersectionResults
        /// <para>
        /// Result with no Intersection are always placed later. If both results have an Intersection, they are ordered by Distance
        /// </para>
        /// </summary>
        /// <param name="other">RayIntersectionResult to compare to</param>
        /// <returns></returns>
        public int CompareTo(RayIntersectionResult other)
        {
            if (Intersection != other.Intersection) // Non-intersections return 1 (bigger)
                return other.Intersection ? 1 : -1;
            if (!Intersection) // Both false
                return 0;
            return Distance.CompareTo(other.Distance);
        }
        #endregion
        #endregion
    }
}