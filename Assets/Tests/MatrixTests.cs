using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Render3DTo2D.Utility;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MatrixTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void MatrixTestsSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator MatrixTestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }

        [Test]
        public void CheckLineLengthTest()
        {
            float[,] _f = {
                {1, 1, 1, -1f},
                {2, 2, 2, 2},
                {3, 3, -3, -3f},
                {4, -4f, 4, 4 }
            };

            int _t1, _t2, _t3, _t4;
            _t1 = MatrixMethods.CheckLineLength(ref _f, 0, -1f);
            Debug.Log(_t1);
            _t2 = MatrixMethods.CheckLineLength(ref _f, 1, -2f);
            Debug.Log(_t2);
            _t3 = MatrixMethods.CheckLineLength(ref _f, 2, -3f);
            Debug.Log(_t3);
            _t4 = MatrixMethods.CheckLineLength(ref _f, 3, -4f);
            Debug.Log(_t4);


            int[] _expected = { 3, 4, 2, 1 };
            int[] _actual = { _t1, _t2, _t3, _t4 };

            Assert.AreEqual(3, _t1);
            Assert.AreEqual(4, _t2);
            Assert.AreEqual(2, _t3);
            Assert.AreEqual(1, _t4);

            CollectionAssert.AreEqual(_expected, _actual);

            float[,] _emF = {
                {1 },{2},{3},{4}
            };

            MatrixMethods.CheckLineLength(ref _emF, 0, -1);
        }

        [Test]
        public void FillMatrixTest()
        {
            float[,] _f = new float[4, 4];
            float[,] _ = { { 4, 4, 4, 4 }, { 4, 4, 4, 4 }, { 4, 4, 4, 4 }, { 4, 4, 4, 4 } };

            MatrixMethods.FillMatrixWithValue(ref _f, 4f);

            CollectionAssert.AreEqual(_, _f);
        }
    }
}
