using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace ECSChess.Jobs.Misc
{
    /// <summary>
    /// Sorts Array of IntersectionResults by Distance
    /// </summary>
    [BurstCompile]
    public struct InsertionSortJob<T> : IJob where T : struct, IComparable<T>
    {
        #region Variables
        /// <summary>
        /// INOUT: Array to Sort
        /// </summary>
        private NativeArray<T> array;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for an InsertionSortJob
        /// </summary>
        /// <param name="toSort">Array to Sort</param>
        public InsertionSortJob(NativeArray<T> toSort)
        {
            array = toSort;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Insertion Sort on Array
        /// </summary>
        public void Execute()
        {
            // Insertion sort on array  
            int inner;
            T temp;
            for (int outer = 1; outer < array.Length; outer++)
            {
                temp = array[outer];
                inner = outer;
                while (inner > 0 && array[inner - 1].CompareTo(temp) > 0)
                {
                    array[inner] = array[inner - 1];
                    inner -= 1;
                }
                array[inner] = temp;
            }
        }
        #endregion
    }
}
