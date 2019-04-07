using ECSChess.Misc.DataTypes;
using System;
using Unity.Entities;

namespace ECSChess.Components.Chess
{
    /// <summary>
    /// A Tile is a position on a ChessBoard
    /// </summary>
    [Serializable]
    public struct Tile : IComponentData
    {
        /// <summary>
        /// Value (Position) for Tile
        /// </summary>
        public readonly ChessPosition Value;
        /// <summary>
        /// Creates a Tile
        /// </summary>
        /// <param name="position">Value for Tile</param>
        public Tile(ChessPosition position)
        {
            Value = position;
        }
    }
}