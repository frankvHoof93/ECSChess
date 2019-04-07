using ECSChess.Misc.Enums;
using System;
using Unity.Entities;

namespace ECSChess.Components.Chess
{
    /// <summary>
    /// A Team is a group that a ChessPiece belongs to (White/Black)
    /// Chess consists of 2 opposing Teams
    /// </summary>
    [Serializable]
    public struct Team : IComponentData
    {
        /// <summary>
        /// Team in Chess (White/Black)
        /// </summary>
        public readonly ChessTeam Value;
        /// <summary>
        /// Creates a Team
        /// </summary>
        /// <param name="team">Value for Team</param>
        public Team(ChessTeam team)
        {
            Value = team;
        }
    }
}