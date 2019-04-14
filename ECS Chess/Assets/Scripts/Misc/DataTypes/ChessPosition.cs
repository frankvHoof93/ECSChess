using ECSChess.Misc.Enums;
using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ECSChess.Misc.DataTypes
{
    /// <summary>
    /// A Chess-Position is a Position on a ChessBoard, usually described as a Letter (Column) and a Number (Row), e.g. A1 (Bottom-Left)
    /// </summary>
    [Serializable]
    public struct ChessPosition : IEquatable<ChessPosition>
    {
        #region Variables
        #region Public
        /// <summary>
        /// Position in WorldSpace, where (0,0) is A1 (bottom-left), and (7,0) is A8 (bottom-right)
        /// </summary>
        public int2 Value
        {
            get => value;
            set
            {
                if (!IsValid(value))
                    throw new ArgumentOutOfRangeException("value", "Both axes for value must be between 1 and 8 (inclusive)");
                this.value = value;
            }
        }
        /// <summary>
        /// Column for Position (A-H)
        /// </summary>
        public TileLetter FileLetter => (TileLetter)value.x;
        /// <summary>
        /// Column for Position (1-8)
        /// </summary>
        public int File => value.x + 1; // +1 for Indexing-Offset
        /// <summary>
        /// Row for Position (1-8)
        /// </summary>
        public int Rank => value.y + 1; // +1 for Indexing-Offset
        #endregion

        #region Private
        /// <summary>
        /// Position in WorldSpace, where (0,0) is A1 (bottom-left), and (7,0) is A8 (bottom-right)
        /// </summary>
        private int2 value;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a ChessPosition from a (classic) Chess-Based Position
        /// </summary>
        /// <param name="file">Column (A-H)</param>
        /// <param name="rank">Row (1-8)</param>
        public ChessPosition(TileLetter file, int rank)
        {
            int2 position = new int2((int)file, rank - 1);// -1 to remove indexing-offset
            if (!IsValid(position))
            {
                if (rank < 1 || rank > 8)
                    throw new ArgumentOutOfRangeException("rank", "Rank must be between 1 and 8 (inclusive)");
                else
                    throw new ArgumentOutOfRangeException("file", "File must be between A and H (inclusive)");
            }
            value = position;
        }

        /// <summary>
        /// Creates a ChessPosition from a (classic) Chess-Based Position
        /// </summary>
        /// <param name="file">Column (1-8)</param>
        /// <param name="rank">Row (1-8)</param>
        public ChessPosition(int rank, int file)
        {
            int2 position = new int2(file - 1, rank - 1);// -1 to remove indexing-offset
            if (!IsValid(position))
            {
                if (file < 1 || file > 8)
                    throw new ArgumentOutOfRangeException("file", "File must be between 1 and 8 (inclusive)");
                else
                    throw new ArgumentOutOfRangeException("rank", "Rank must be between 1 and 8 (inclusive)");
            }
            value = position;
        }

        /// <summary>
        /// Creates a ChessPostion from a (0-indexed) WorldPosition
        /// </summary>
        /// <param name="worldPosition">WorldPosition for ChessPosition</param>
        public ChessPosition(int2 worldPosition)
        {
            if (!IsValid(worldPosition))
                throw new ArgumentException("worldPosition", "Position must be between 0 and 7 (inclusive) for both axes");
            value = worldPosition;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Checks if a Position is Valid (both axes between 0 and 7 (inclusive))
        /// </summary>
        /// <param name="position">Position to Check</param>
        /// <returns>True if both X and Y are between 0 and 7 (inclusive)</returns>
        private static bool IsValid(int2 position)
        {
            return position.x >= 0 && position.x < 8
                && position.y >= 0 && position.y < 8;
        }

        /// <summary>
        /// Checks if a Position is Valid (both axes between 0 and 7 (inclusive))
        /// </summary>
        /// <param name="file">File for Position (Column)</param>
        /// <param name="rank">Rank for Position (Row, 1-indexed (1-8))</param>
        /// <returns>True if Position is on Board</returns>
        private static bool IsValid(TileLetter file, int rank)
        {
            return IsValid((int)file + 1, rank);
        }

        /// <summary>
        /// Checks if a Position is Valid (both axes between 0 and 7 (inclusive))
        /// </summary>
        /// <param name="file">File for Position (Column, 1-Indexed (1-8))</param>
        /// <param name="rank">Rank for Position (Row, 1-indexed (1-8))</param>
        /// <returns>True if Position is on Board</returns>
        private static bool IsValid(int file, int rank)
        {
            return file >= 1 && file < 9
                && rank >= 1 && file < 9;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Checks for Equality between Object and ChessPosition
        /// </summary>
        /// <param name="obj">Object to check against</param>
        /// <returns>True if <paramref name="obj"/> is a ChessPosition, and Values are Equal</returns>
        public override bool Equals(object obj)
        {
            return obj is ChessPosition && ((ChessPosition)obj).Equals(this);
        }
        /// <summary>
        /// Checks for Equality with another ChessPosition
        /// </summary>
        /// <param name="other">ChessPosition to check against</param>
        /// <returns>True if Values are Equal</returns>
        public bool Equals(ChessPosition other)
        {
            return other != null && other == this;
        }
        /// <summary>
        /// Returns Chess-Based String-Representation of Position (e.g. A1)
        /// </summary>
        /// <returns>Chess-Based String-Representation of Position (e.g. A1)</returns>
        public override string ToString()
        {
            return FileLetter.ToString() + Rank;
        }
        /// <summary>
        /// Returns HashCode based on Value
        /// </summary>
        /// <returns>HashCode based on Value</returns>
        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<int2>.Default.GetHashCode(value);
        }
        #endregion

        #region Operators
        #region Cast
        /// <summary>
        /// Explicit Cast from ChessPosition to int2-Position (0-indexed)
        /// </summary>
        /// <param name="pos">ChessPosition to cast</param>
        public static explicit operator int2(ChessPosition pos) => pos.value;
        /// <summary>
        /// Explicit Cast from int2-Position (0-indexed) to ChessPosition
        /// </summary>
        /// <param name="pos">int2-Position to cast</param>
        public static explicit operator ChessPosition(int2 pos) => new ChessPosition { value = pos };
        #endregion

        #region Equality
        public static bool operator ==(ChessPosition lhs, ChessPosition rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ChessPosition lhs, ChessPosition rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static bool operator ==(ChessPosition lhs, int2 rhs)
        {
            return lhs.value.Equals(rhs);
        }

        public static bool operator !=(ChessPosition lhs, int2 rhs)
        {
            return !lhs.value.Equals(rhs);
        }

        public static bool operator ==(int2 lhs, ChessPosition rhs)
        {
            return lhs.Equals(rhs.value);
        }

        public static bool operator !=(int2 lhs, ChessPosition rhs)
        {
            return !lhs.Equals(rhs.value);
        }
        #endregion

        #region Addition/Subtraction
        public static ChessPosition operator +(ChessPosition lhs, ChessPosition rhs)
        {
            return new ChessPosition(lhs.value + rhs.value);
        }

        public static ChessPosition operator -(ChessPosition lhs, ChessPosition rhs)
        {
            return new ChessPosition(lhs.value - rhs.value);
        }

        public static ChessPosition operator +(ChessPosition lhs, int2 rhs)
        {
            return new ChessPosition(lhs.value + rhs);
        }

        public static ChessPosition operator -(ChessPosition lhs, int2 rhs)
        {
            return new ChessPosition(lhs.value - rhs);
        }

        public static int2 operator -(int2 lhs, ChessPosition rhs)
        {
            return lhs - rhs.value;
        }
        #endregion
        #endregion
    }
}
