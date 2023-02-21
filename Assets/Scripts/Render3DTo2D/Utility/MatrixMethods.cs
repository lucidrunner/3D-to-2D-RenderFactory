using System.Collections.Generic;
using UnityEngine;

namespace Render3DTo2D.Utility
{
    public static class MatrixMethods
    {

        /// <summary>
        /// Fills a provided Matrix of type T with a provided value. Useful for setting default values.
        /// </summary>
        public static void FillMatrixWithValue<T>(ref T[,] aMatrix, T aValue)
        {
            for(int rowIndex = 0; rowIndex < aMatrix.GetLength(0); rowIndex++)
            {
                for(int dirIndex = 0; dirIndex < aMatrix.GetLength(1); dirIndex++)
                {
                    aMatrix[rowIndex, dirIndex] = aValue;
                }
            }
        }

        /// <summary>
        /// Returns the "actual" length of a line in a jagged matrix
        /// </summary>
        /// <param name="aCheckedLine">The 1-dimension index being checked</param>
        /// <param name="aNonValue">A reference non-value that denotes an invalid value</param>
        /// <returns></returns>
        public static int CheckLineLength<T>(ref T[,] aMatrix, int aCheckedLine, T aNonValue)
        {
            int _length = 0; 
            for(int index = 0; index < aMatrix.GetLength(0); index++)
            {
                if(aMatrix[index, aCheckedLine].Equals(aNonValue))
                {
                    return _length;
                }

                _length++;
            }
            Debug.Log("Reported length in class " + _length);
            Debug.Log("Original dimension 1 " + aMatrix.GetLength(1));
            Debug.Log("Original dimension 0 " + aMatrix.GetLength(0));
            return _length;

        
        }

        public static T[,] CloneMatrix<T>(ref T[,] aOriginMatrix)
        {
            T[,] _clone = new T[aOriginMatrix.GetLength(0), aOriginMatrix.GetLength(1)];
            CopyMatrix(ref aOriginMatrix, ref _clone);
            return _clone;
        }


        /// <summary>
        /// Copies all data from the origin matrix to the destination matrix.
        /// The Destination matrix must be smaller or equal in size to the origin matrix
        /// </summary>
        public static void CopyMatrix<T>(ref T[,] aOriginMatrix, ref T[,] aDestinationMatrix)
        {
            int width = aDestinationMatrix.GetLength(0);
            int height = aDestinationMatrix.GetLength(1);

            if(width > aOriginMatrix.GetLength(0) || height > aOriginMatrix.GetLength(1))
            {
                Debug.Log("Couldn't copy because destination matrix was larger than origin.");
                return;
            }

            for(int lineIndex = 0; lineIndex < height; lineIndex++)
            {
                for(int frameIndex = 0; frameIndex < width; frameIndex++)
                {
                    aDestinationMatrix[frameIndex, lineIndex] = aOriginMatrix[frameIndex, lineIndex];
                }
            }
        }

        public static IEnumerable<List<T>> ToLists<T>(T[,] aMatrix)
        {
            int width = aMatrix.GetLength(0);
            int height = aMatrix.GetLength(1);
            List<List<T>> _toReturn = new List<List<T>>();
            
            for (int _columnIndex = 0; _columnIndex < width; _columnIndex++)
            {
                var _list = new List<T>();
                for (int _lineIndex = 0; _lineIndex < height; _lineIndex++)
                {
                    _list.Add(aMatrix[_columnIndex, _lineIndex]);
                }
                _toReturn.Add(_list);
            }

            return _toReturn;
        }
    }
}
