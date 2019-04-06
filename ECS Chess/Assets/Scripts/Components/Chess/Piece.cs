using ECSChess.Misc.Enums;
using System;
using Unity.Entities;

namespace ECSChess.Components.Chess
{
    [Serializable]
    [RequireComponentTag(typeof(Team))]
    public struct Piece : IComponentData
    {
        /// <summary>
        /// Type of ChessPiece
        /// </summary>
        public readonly ChessPiece Value;

        public Piece(ChessPiece piece)
        {
            Value = piece;
        }
    }
}