using ECSChess.Misc.Enums;
using System;
using Unity.Entities;

namespace ECSChess.Components.Chess
{
    [Serializable]
    public struct Team : IComponentData
    {
        /// <summary>
        /// Team in Chess (White/Black)
        /// </summary>
        public readonly ChessTeam Value;

        public Team(ChessTeam team)
        {
            Value = team;
        }
    }
}