using ECSChess.Misc.Enums;
using System;
using Unity.Entities;

namespace ECSChess.Components.Chess
{
    /// <summary>
    /// A Piece is an Object on a ChessBoard which can be moved (controlled) by a Player
    /// </summary>
    [Serializable]
    [RequireComponentTag(typeof(Team))]
    public struct Piece : IComponentData
    {
        /// <summary>
        /// Type of ChessPiece
        /// </summary>
        public readonly ChessPiece Value;
        /// <summary>
        /// Creates a Piece
        /// </summary>
        /// <param name="piece">ChessPiece to create</param>
        public Piece(ChessPiece piece)
        {
            Value = piece;
        }
    }
}