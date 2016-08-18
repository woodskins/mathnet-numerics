﻿// <copyright file="ManagedLinearAlgebraProvider.Complex.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2015 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using MathNet.Numerics.LinearAlgebra.Complex.Factorization;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.Properties;
using MathNet.Numerics.Threading;

namespace MathNet.Numerics.Providers.LinearAlgebra
{

#if !NOSYSNUMERICS
    using Complex = System.Numerics.Complex;
#endif

    /// <summary>
    /// The managed linear algebra provider.
    /// </summary>
    public partial class ManagedLinearAlgebraProvider
    {
        /// <summary>
        /// Adds a scaled vector to another: <c>result = y + alpha*x</c>.
        /// </summary>
        /// <param name="y">The vector to update.</param>
        /// <param name="alpha">The value to scale <paramref name="x"/> by.</param>
        /// <param name="x">The vector to add to <paramref name="y"/>.</param>
        /// <param name="result">The result of the addition.</param>
        /// <remarks>This is similar to the AXPY BLAS routine.</remarks>
        public virtual void AddVectorToScaledVector(Complex[] y, Complex alpha, Complex[] x, Complex[] result)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (y.Length != x.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength);
            }

            if (y.Length != x.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength);
            }

            if (alpha.IsZero())
            {
                y.Copy(result);
            }
            else if (alpha.IsOne())
            {
                CommonParallel.For(0, y.Length, 4096, (a, b) =>
                {
                    for (int i = a; i < b; i++)
                    {
                        result[i] = y[i] + x[i];
                    }
                });
            }
            else
            {
                CommonParallel.For(0, y.Length, 4096, (a, b) =>
                {
                    for (int i = a; i < b; i++)
                    {
                        result[i] = y[i] + (alpha*x[i]);
                    }
                });
            }
        }

        /// <summary>
        /// Scales an array. Can be used to scale a vector and a matrix.
        /// </summary>
        /// <param name="alpha">The scalar.</param>
        /// <param name="x">The values to scale.</param>
        /// <param name="result">This result of the scaling.</param>
        /// <remarks>This is similar to the SCAL BLAS routine.</remarks>
        public virtual void ScaleArray(Complex alpha, Complex[] x, Complex[] result)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (alpha.IsZero())
            {
                Array.Clear(result, 0, result.Length);
            }
            else if (alpha.IsOne())
            {
                x.Copy(result);
            }
            else
            {
                CommonParallel.For(0, x.Length, 4096, (a, b) =>
                {
                    for (int i = a; i < b; i++)
                    {
                        result[i] = alpha*x[i];
                    }
                });
            }
        }

        /// <summary>
        /// Conjugates an array. Can be used to conjugate a vector and a matrix.
        /// </summary>
        /// <param name="x">The values to conjugate.</param>
        /// <param name="result">This result of the conjugation.</param>
        public virtual void ConjugateArray(Complex[] x, Complex[] result)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            CommonParallel.For(0, x.Length, 4096, (a, b) =>
            {
                for (int i = a; i < b; i++)
                {
                    result[i] = x[i].Conjugate();
                }
            });
        }

        /// <summary>
        /// Computes the dot product of x and y.
        /// </summary>
        /// <param name="x">The vector x.</param>
        /// <param name="y">The vector y.</param>
        /// <returns>The dot product of x and y.</returns>
        /// <remarks>This is equivalent to the DOT BLAS routine.</remarks>
        public virtual Complex DotProduct(Complex[] x, Complex[] y)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (y.Length != x.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength);
            }

            var dot = Complex.Zero;
            for (var index = 0; index < y.Length; index++)
            {
                dot += y[index]*x[index];
            }

            return dot;
        }

        /// <summary>
        /// Does a point wise add of two arrays <c>z = x + y</c>. This can be used
        /// to add vectors or matrices.
        /// </summary>
        /// <param name="x">The array x.</param>
        /// <param name="y">The array y.</param>
        /// <param name="result">The result of the addition.</param>
        /// <remarks>There is no equivalent BLAS routine, but many libraries
        /// provide optimized (parallel and/or vectorized) versions of this
        /// routine.</remarks>
        public virtual void AddArrays(Complex[] x, Complex[] y, Complex[] result)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (y.Length != x.Length || y.Length != result.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength);
            }

            CommonParallel.For(0, y.Length, 4096, (a, b) =>
            {
                for (int i = a; i < b; i++)
                {
                    result[i] = x[i] + y[i];
                }
            });
        }

        /// <summary>
        /// Does a point wise subtraction of two arrays <c>z = x - y</c>. This can be used
        /// to subtract vectors or matrices.
        /// </summary>
        /// <param name="x">The array x.</param>
        /// <param name="y">The array y.</param>
        /// <param name="result">The result of the subtraction.</param>
        /// <remarks>There is no equivalent BLAS routine, but many libraries
        /// provide optimized (parallel and/or vectorized) versions of this
        /// routine.</remarks>
        public virtual void SubtractArrays(Complex[] x, Complex[] y, Complex[] result)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (y.Length != x.Length || y.Length != result.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength);
            }

            CommonParallel.For(0, y.Length, 4096, (a, b) =>
            {
                for (int i = a; i < b; i++)
                {
                    result[i] = x[i] - y[i];
                }
            });
        }

        /// <summary>
        /// Does a point wise multiplication of two arrays <c>z = x * y</c>. This can be used
        /// to multiple elements of vectors or matrices.
        /// </summary>
        /// <param name="x">The array x.</param>
        /// <param name="y">The array y.</param>
        /// <param name="result">The result of the point wise multiplication.</param>
        /// <remarks>There is no equivalent BLAS routine, but many libraries
        /// provide optimized (parallel and/or vectorized) versions of this
        /// routine.</remarks>
        public virtual void PointWiseMultiplyArrays(Complex[] x, Complex[] y, Complex[] result)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (y.Length != x.Length || y.Length != result.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength);
            }

            CommonParallel.For(0, y.Length, 4096, (a, b) =>
            {
                for (int i = a; i < b; i++)
                {
                    result[i] = x[i] * y[i];
                }
            });
        }

        /// <summary>
        /// Does a point wise division of two arrays <c>z = x / y</c>. This can be used
        /// to divide elements of vectors or matrices.
        /// </summary>
        /// <param name="x">The array x.</param>
        /// <param name="y">The array y.</param>
        /// <param name="result">The result of the point wise division.</param>
        /// <remarks>There is no equivalent BLAS routine, but many libraries
        /// provide optimized (parallel and/or vectorized) versions of this
        /// routine.</remarks>
        public virtual void PointWiseDivideArrays(Complex[] x, Complex[] y, Complex[] result)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (y.Length != x.Length || y.Length != result.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength);
            }

            CommonParallel.For(0, y.Length, 4096, (a, b) =>
            {
                for (int i = a; i < b; i++)
                {
                    result[i] = x[i] / y[i];
                }
            });
        }

        /// <summary>
        /// Computes the requested <see cref="Norm"/> of the matrix.
        /// </summary>
        /// <param name="norm">The type of norm to compute.</param>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        /// <param name="matrix">The matrix to compute the norm from.</param>
        /// <returns>
        /// The requested <see cref="Norm"/> of the matrix.
        /// </returns>
        public virtual double MatrixNorm(Norm norm, int rows, int columns, Complex[] matrix)
        {
            switch (norm)
            {
                case Norm.OneNorm:
                    var norm1 = 0d;
                    for (var j = 0; j < columns; j++)
                    {
                        var s = 0.0;
                        for (var i = 0; i < rows; i++)
                        {
                            s += matrix[(j*rows) + i].Magnitude;
                        }
                        norm1 = Math.Max(norm1, s);
                    }
                    return norm1;
                case Norm.LargestAbsoluteValue:
                    var normMax = 0d;
                    for (var j = 0; j < columns; j++)
                    {
                        for (var i = 0; i < rows; i++)
                        {
                            normMax = Math.Max(matrix[(j * rows) + i].Magnitude, normMax);
                        }
                    }
                    return normMax;
                case Norm.InfinityNorm:
                    var r = new double[rows];
                    for (var j = 0; j < columns; j++)
                    {
                        for (var i = 0; i < rows; i++)
                        {
                            r[i] += matrix[(j * rows) + i].Magnitude;
                        }
                    }
                    // TODO: reuse
                    var max = r[0];
                    for (int i = 0; i < r.Length; i++)
                    {
                        if (r[i] > max)
                        {
                            max = r[i];
                        }
                    }
                    return max;
                case Norm.FrobeniusNorm:
                    var aat = new Complex[rows*rows];
                    MatrixMultiplyWithUpdate(Transpose.DontTranspose, Transpose.ConjugateTranspose, 1.0, matrix, rows, columns, matrix, rows, columns, 0.0, aat);
                    var normF = 0d;
                    for (var i = 0; i < rows; i++)
                    {
                        normF += aat[(i * rows) + i].Magnitude;
                    }
                    return Math.Sqrt(normF);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Multiples two matrices. <c>result = x * y</c>
        /// </summary>
        /// <param name="x">The x matrix.</param>
        /// <param name="rowsX">The number of rows in the x matrix.</param>
        /// <param name="columnsX">The number of columns in the x matrix.</param>
        /// <param name="y">The y matrix.</param>
        /// <param name="rowsY">The number of rows in the y matrix.</param>
        /// <param name="columnsY">The number of columns in the y matrix.</param>
        /// <param name="result">Where to store the result of the multiplication.</param>
        /// <remarks>This is a simplified version of the BLAS GEMM routine with alpha
        /// set to 1.0 and beta set to 0.0, and x and y are not transposed.</remarks>
        public virtual void MatrixMultiply(Complex[] x, int rowsX, int columnsX, Complex[] y, int rowsY, int columnsY, Complex[] result)
        {
            // First check some basic requirement on the parameters of the matrix multiplication.
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (rowsX*columnsX != x.Length)
            {
                throw new ArgumentException("x.Length != xRows * xColumns");
            }

            if (rowsY*columnsY != y.Length)
            {
                throw new ArgumentException("y.Length != yRows * yColumns");
            }

            if (columnsX != rowsY)
            {
                throw new ArgumentException("xColumns != yRows");
            }

            if (rowsX*columnsY != result.Length)
            {
                throw new ArgumentException("xRows * yColumns != result.Length");
            }

            // Check whether we will be overwriting any of our inputs and make copies if necessary.
            // TODO - we can don't have to allocate a completely new matrix when x or y point to the same memory
            // as result, we can do it on a row wise basis. We should investigate this.
            Complex[] xdata;
            if (ReferenceEquals(x, result))
            {
                xdata = (Complex[]) x.Clone();
            }
            else
            {
                xdata = x;
            }

            Complex[] ydata;
            if (ReferenceEquals(y, result))
            {
                ydata = (Complex[]) y.Clone();
            }
            else
            {
                ydata = y;
            }

            Array.Clear(result, 0, result.Length);

            CacheObliviousMatrixMultiply(Transpose.DontTranspose, Transpose.DontTranspose, Complex.One, xdata, 0, 0, ydata, 0, 0, result, 0, 0, rowsX, columnsY, columnsX, rowsX, columnsY, columnsX, true);
        }

        /// <summary>
        /// Multiplies two matrices and updates another with the result. <c>c = alpha*op(a)*op(b) + beta*c</c>
        /// </summary>
        /// <param name="transposeA">How to transpose the <paramref name="a"/> matrix.</param>
        /// <param name="transposeB">How to transpose the <paramref name="b"/> matrix.</param>
        /// <param name="alpha">The value to scale <paramref name="a"/> matrix.</param>
        /// <param name="a">The a matrix.</param>
        /// <param name="rowsA">The number of rows in the <paramref name="a"/> matrix.</param>
        /// <param name="columnsA">The number of columns in the <paramref name="a"/> matrix.</param>
        /// <param name="b">The b matrix</param>
        /// <param name="rowsB">The number of rows in the <paramref name="b"/> matrix.</param>
        /// <param name="columnsB">The number of columns in the <paramref name="b"/> matrix.</param>
        /// <param name="beta">The value to scale the <paramref name="c"/> matrix.</param>
        /// <param name="c">The c matrix.</param>
        public virtual void MatrixMultiplyWithUpdate(Transpose transposeA, Transpose transposeB, Complex alpha, Complex[] a, int rowsA, int columnsA, Complex[] b, int rowsB, int columnsB, Complex beta, Complex[] c)
        {
            int m; // The number of rows of matrix op(A) and of the matrix C.
            int n; // The number of columns of matrix op(B) and of the matrix C.
            int k; // The number of columns of matrix op(A) and the rows of the matrix op(B).

            // First check some basic requirement on the parameters of the matrix multiplication.
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if ((int) transposeA > 111 && (int) transposeB > 111)
            {
                if (rowsA != columnsB)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (columnsA*rowsB != c.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                m = columnsA;
                n = rowsB;
                k = rowsA;
            }
            else if ((int) transposeA > 111)
            {
                if (rowsA != rowsB)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (columnsA*columnsB != c.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                m = columnsA;
                n = columnsB;
                k = rowsA;
            }
            else if ((int) transposeB > 111)
            {
                if (columnsA != columnsB)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (rowsA*rowsB != c.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                m = rowsA;
                n = rowsB;
                k = columnsA;
            }
            else
            {
                if (columnsA != rowsB)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (rowsA*columnsB != c.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                m = rowsA;
                n = columnsB;
                k = columnsA;
            }

            if (alpha.IsZero() && beta.IsZero())
            {
                Array.Clear(c, 0, c.Length);
                return;
            }

            // Check whether we will be overwriting any of our inputs and make copies if necessary.
            // TODO - we can don't have to allocate a completely new matrix when x or y point to the same memory
            // as result, we can do it on a row wise basis. We should investigate this.
            Complex[] adata;
            if (ReferenceEquals(a, c))
            {
                adata = (Complex[]) a.Clone();
            }
            else
            {
                adata = a;
            }

            Complex[] bdata;
            if (ReferenceEquals(b, c))
            {
                bdata = (Complex[]) b.Clone();
            }
            else
            {
                bdata = b;
            }

            if (beta.IsZero())
            {
                Array.Clear(c, 0, c.Length);
            }
            else if (!beta.IsOne())
            {
                ScaleArray(beta, c, c);
            }

            if (alpha.IsZero())
            {
                return;
            }

            CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, adata, 0, 0, bdata, 0, 0, c, 0, 0, m, n, k, m, n, k, true);
        }

        /// <summary>
        /// Cache-Oblivious Matrix Multiplication
        /// </summary>
        /// <param name="transposeA">if set to <c>true</c> transpose matrix A.</param>
        /// <param name="transposeB">if set to <c>true</c> transpose matrix B.</param>
        /// <param name="alpha">The value to scale the matrix A with.</param>
        /// <param name="matrixA">The matrix A.</param>
        /// <param name="shiftArow">Row-shift of the left matrix</param>
        /// <param name="shiftAcol">Column-shift of the left matrix</param>
        /// <param name="matrixB">The matrix B.</param>
        /// <param name="shiftBrow">Row-shift of the right matrix</param>
        /// <param name="shiftBcol">Column-shift of the right matrix</param>
        /// <param name="result">The matrix C.</param>
        /// <param name="shiftCrow">Row-shift of the result matrix</param>
        /// <param name="shiftCcol">Column-shift of the result matrix</param>
        /// <param name="m">The number of rows of matrix op(A) and of the matrix C.</param>
        /// <param name="n">The number of columns of matrix op(B) and of the matrix C.</param>
        /// <param name="k">The number of columns of matrix op(A) and the rows of the matrix op(B).</param>
        /// <param name="constM">The constant number of rows of matrix op(A) and of the matrix C.</param>
        /// <param name="constN">The constant number of columns of matrix op(B) and of the matrix C.</param>
        /// <param name="constK">The constant number of columns of matrix op(A) and the rows of the matrix op(B).</param>
        /// <param name="first">Indicates if this is the first recursion.</param>
        static void CacheObliviousMatrixMultiply(Transpose transposeA, Transpose transposeB, Complex alpha, Complex[] matrixA, int shiftArow, int shiftAcol, Complex[] matrixB, int shiftBrow, int shiftBcol, Complex[] result, int shiftCrow, int shiftCcol, int m, int n, int k, int constM, int constN, int constK, bool first)
        {
            if (m + n <= Control.ParallelizeOrder || m == 1 || n == 1 || k == 1)
            {
                if ((int) transposeA > 111 && (int) transposeB > 111)
                {
                    if ((int) transposeA > 112 && (int) transposeB > 112)
                    {
                        for (var m1 = 0; m1 < m; m1++)
                        {
                            var matArowPos = m1 + shiftArow;
                            var matCrowPos = m1 + shiftCrow;
                            for (var n1 = 0; n1 < n; ++n1)
                            {
                                var matBcolPos = n1 + shiftBcol;
                                var sum = Complex.Zero;
                                for (var k1 = 0; k1 < k; ++k1)
                                {
                                    sum += matrixA[(matArowPos*constK) + k1 + shiftAcol].Conjugate()*
                                        matrixB[((k1 + shiftBrow)*constN) + matBcolPos].Conjugate();
                                }

                                result[((n1 + shiftCcol)*constM) + matCrowPos] += alpha*sum;
                            }
                        }
                    }
                    else if ((int) transposeA > 112)
                    {
                        for (var m1 = 0; m1 < m; m1++)
                        {
                            var matArowPos = m1 + shiftArow;
                            var matCrowPos = m1 + shiftCrow;
                            for (var n1 = 0; n1 < n; ++n1)
                            {
                                var matBcolPos = n1 + shiftBcol;
                                var sum = Complex.Zero;
                                for (var k1 = 0; k1 < k; ++k1)
                                {
                                    sum += matrixA[(matArowPos*constK) + k1 + shiftAcol].Conjugate()*
                                        matrixB[((k1 + shiftBrow)*constN) + matBcolPos];
                                }

                                result[((n1 + shiftCcol)*constM) + matCrowPos] += alpha*sum;
                            }
                        }
                    }
                    else if ((int) transposeB > 112)
                    {
                        for (var m1 = 0; m1 < m; m1++)
                        {
                            var matArowPos = m1 + shiftArow;
                            var matCrowPos = m1 + shiftCrow;
                            for (var n1 = 0; n1 < n; ++n1)
                            {
                                var matBcolPos = n1 + shiftBcol;
                                var sum = Complex.Zero;
                                for (var k1 = 0; k1 < k; ++k1)
                                {
                                    sum += matrixA[(matArowPos*constK) + k1 + shiftAcol]*
                                        matrixB[((k1 + shiftBrow)*constN) + matBcolPos].Conjugate();
                                }

                                result[((n1 + shiftCcol)*constM) + matCrowPos] += alpha*sum;
                            }
                        }
                    }
                    else
                    {
                        for (var m1 = 0; m1 < m; m1++)
                        {
                            var matArowPos = m1 + shiftArow;
                            var matCrowPos = m1 + shiftCrow;
                            for (var n1 = 0; n1 < n; ++n1)
                            {
                                var matBcolPos = n1 + shiftBcol;
                                var sum = Complex.Zero;
                                for (var k1 = 0; k1 < k; ++k1)
                                {
                                    sum += matrixA[(matArowPos*constK) + k1 + shiftAcol]*
                                        matrixB[((k1 + shiftBrow)*constN) + matBcolPos];
                                }

                                result[((n1 + shiftCcol)*constM) + matCrowPos] += alpha*sum;
                            }
                        }
                    }
                }
                else if ((int) transposeA > 111)
                {
                    if ((int) transposeA > 112)
                    {
                        for (var m1 = 0; m1 < m; m1++)
                        {
                            var matArowPos = m1 + shiftArow;
                            var matCrowPos = m1 + shiftCrow;
                            for (var n1 = 0; n1 < n; ++n1)
                            {
                                var matBcolPos = n1 + shiftBcol;
                                var sum = Complex.Zero;
                                for (var k1 = 0; k1 < k; ++k1)
                                {
                                    sum += matrixA[(matArowPos*constK) + k1 + shiftAcol].Conjugate()*
                                        matrixB[(matBcolPos*constK) + k1 + shiftBrow];
                                }

                                result[((n1 + shiftCcol)*constM) + matCrowPos] += alpha*sum;
                            }
                        }
                    }
                    else
                    {
                        for (var m1 = 0; m1 < m; m1++)
                        {
                            var matArowPos = m1 + shiftArow;
                            var matCrowPos = m1 + shiftCrow;
                            for (var n1 = 0; n1 < n; ++n1)
                            {
                                var matBcolPos = n1 + shiftBcol;
                                var sum = Complex.Zero;
                                for (var k1 = 0; k1 < k; ++k1)
                                {
                                    sum += matrixA[(matArowPos*constK) + k1 + shiftAcol]*
                                        matrixB[(matBcolPos*constK) + k1 + shiftBrow];
                                }

                                result[((n1 + shiftCcol)*constM) + matCrowPos] += alpha*sum;
                            }
                        }
                    }
                }
                else if ((int) transposeB > 111)
                {
                    if ((int) transposeB > 112)
                    {
                        for (var m1 = 0; m1 < m; m1++)
                        {
                            var matArowPos = m1 + shiftArow;
                            var matCrowPos = m1 + shiftCrow;
                            for (var n1 = 0; n1 < n; ++n1)
                            {
                                var matBcolPos = n1 + shiftBcol;
                                var sum = Complex.Zero;
                                for (var k1 = 0; k1 < k; ++k1)
                                {
                                    sum += matrixA[((k1 + shiftAcol)*constM) + matArowPos]*
                                        matrixB[((k1 + shiftBrow)*constN) + matBcolPos].Conjugate();
                                }

                                result[((n1 + shiftCcol)*constM) + matCrowPos] += alpha*sum;
                            }
                        }
                    }
                    else
                    {
                        for (var m1 = 0; m1 < m; m1++)
                        {
                            var matArowPos = m1 + shiftArow;
                            var matCrowPos = m1 + shiftCrow;
                            for (var n1 = 0; n1 < n; ++n1)
                            {
                                var matBcolPos = n1 + shiftBcol;
                                var sum = Complex.Zero;
                                for (var k1 = 0; k1 < k; ++k1)
                                {
                                    sum += matrixA[((k1 + shiftAcol)*constM) + matArowPos]*
                                        matrixB[((k1 + shiftBrow)*constN) + matBcolPos];
                                }

                                result[((n1 + shiftCcol)*constM) + matCrowPos] += alpha*sum;
                            }
                        }
                    }
                }
                else
                {
                    for (var m1 = 0; m1 < m; m1++)
                    {
                        var matArowPos = m1 + shiftArow;
                        var matCrowPos = m1 + shiftCrow;
                        for (var n1 = 0; n1 < n; ++n1)
                        {
                            var matBcolPos = n1 + shiftBcol;
                            var sum = Complex.Zero;
                            for (var k1 = 0; k1 < k; ++k1)
                            {
                                sum += matrixA[((k1 + shiftAcol)*constM) + matArowPos]*
                                    matrixB[(matBcolPos*constK) + k1 + shiftBrow];
                            }

                            result[((n1 + shiftCcol)*constM) + matCrowPos] += alpha*sum;
                        }
                    }
                }
            }
            else
            {
                // divide and conquer
                int m2 = m/2, n2 = n/2, k2 = k/2;

                if (first)
                {
                    CommonParallel.Invoke(
                        () => CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow, shiftAcol, matrixB, shiftBrow, shiftBcol, result, shiftCrow, shiftCcol, m2, n2, k2, constM, constN, constK, false),
                        () => CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow, shiftAcol, matrixB, shiftBrow, shiftBcol + n2, result, shiftCrow, shiftCcol + n2, m2, n - n2, k2, constM, constN, constK, false),
                        () => CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow + m2, shiftAcol, matrixB, shiftBrow, shiftBcol, result, shiftCrow + m2, shiftCcol, m - m2, n2, k2, constM, constN, constK, false),
                        () => CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow + m2, shiftAcol, matrixB, shiftBrow, shiftBcol + n2, result, shiftCrow + m2, shiftCcol + n2, m - m2, n - n2, k2, constM, constN, constK, false));

                    CommonParallel.Invoke(
                        () => CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow, shiftAcol + k2, matrixB, shiftBrow + k2, shiftBcol, result, shiftCrow, shiftCcol, m2, n2, k - k2, constM, constN, constK, false),
                        () => CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow, shiftAcol + k2, matrixB, shiftBrow + k2, shiftBcol + n2, result, shiftCrow, shiftCcol + n2, m2, n - n2, k - k2, constM, constN, constK, false),
                        () => CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow + m2, shiftAcol + k2, matrixB, shiftBrow + k2, shiftBcol, result, shiftCrow + m2, shiftCcol, m - m2, n2, k - k2, constM, constN, constK, false),
                        () => CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow + m2, shiftAcol + k2, matrixB, shiftBrow + k2, shiftBcol + n2, result, shiftCrow + m2, shiftCcol + n2, m - m2, n - n2, k - k2, constM, constN, constK, false));
                }
                else
                {
                    CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow, shiftAcol, matrixB, shiftBrow, shiftBcol, result, shiftCrow, shiftCcol, m2, n2, k2, constM, constN, constK, false);
                    CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow, shiftAcol, matrixB, shiftBrow, shiftBcol + n2, result, shiftCrow, shiftCcol + n2, m2, n - n2, k2, constM, constN, constK, false);

                    CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow, shiftAcol + k2, matrixB, shiftBrow + k2, shiftBcol, result, shiftCrow, shiftCcol, m2, n2, k - k2, constM, constN, constK, false);
                    CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow, shiftAcol + k2, matrixB, shiftBrow + k2, shiftBcol + n2, result, shiftCrow, shiftCcol + n2, m2, n - n2, k - k2, constM, constN, constK, false);

                    CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow + m2, shiftAcol, matrixB, shiftBrow, shiftBcol, result, shiftCrow + m2, shiftCcol, m - m2, n2, k2, constM, constN, constK, false);
                    CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow + m2, shiftAcol, matrixB, shiftBrow, shiftBcol + n2, result, shiftCrow + m2, shiftCcol + n2, m - m2, n - n2, k2, constM, constN, constK, false);

                    CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow + m2, shiftAcol + k2, matrixB, shiftBrow + k2, shiftBcol, result, shiftCrow + m2, shiftCcol, m - m2, n2, k - k2, constM, constN, constK, false);
                    CacheObliviousMatrixMultiply(transposeA, transposeB, alpha, matrixA, shiftArow + m2, shiftAcol + k2, matrixB, shiftBrow + k2, shiftBcol + n2, result, shiftCrow + m2, shiftCcol + n2, m - m2, n - n2, k - k2, constM, constN, constK, false);
                }
            }
        }

        /// <summary>
        /// Computes the LUP factorization of A. P*A = L*U.
        /// </summary>
        /// <param name="data">An <paramref name="order"/> by <paramref name="order"/> matrix. The matrix is overwritten with the
        /// the LU factorization on exit. The lower triangular factor L is stored in under the diagonal of <paramref name="data"/> (the diagonal is always 1.0
        /// for the L factor). The upper triangular factor U is stored on and above the diagonal of <paramref name="data"/>.</param>
        /// <param name="order">The order of the square matrix <paramref name="data"/>.</param>
        /// <param name="ipiv">On exit, it contains the pivot indices. The size of the array must be <paramref name="order"/>.</param>
        /// <remarks>This is equivalent to the GETRF LAPACK routine.</remarks>
        public virtual void LUFactor(Complex[] data, int order, int[] ipiv)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (ipiv == null)
            {
                throw new ArgumentNullException("ipiv");
            }

            if (data.Length != order*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "data");
            }

            if (ipiv.Length != order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "ipiv");
            }

            // Initialize the pivot matrix to the identity permutation.
            for (var i = 0; i < order; i++)
            {
                ipiv[i] = i;
            }

            var vecLUcolj = new Complex[order];

            // Outer loop.
            for (var j = 0; j < order; j++)
            {
                var indexj = j*order;
                var indexjj = indexj + j;

                // Make a copy of the j-th column to localize references.
                for (var i = 0; i < order; i++)
                {
                    vecLUcolj[i] = data[indexj + i];
                }

                // Apply previous transformations.
                for (var i = 0; i < order; i++)
                {
                    // Most of the time is spent in the following dot product.
                    var kmax = Math.Min(i, j);
                    var s = Complex.Zero;
                    for (var k = 0; k < kmax; k++)
                    {
                        s += data[(k*order) + i]*vecLUcolj[k];
                    }

                    data[indexj + i] = vecLUcolj[i] -= s;
                }

                // Find pivot and exchange if necessary.
                var p = j;
                for (var i = j + 1; i < order; i++)
                {
                    if (vecLUcolj[i].Magnitude > vecLUcolj[p].Magnitude)
                    {
                        p = i;
                    }
                }

                if (p != j)
                {
                    for (var k = 0; k < order; k++)
                    {
                        var indexk = k*order;
                        var indexkp = indexk + p;
                        var indexkj = indexk + j;
                        var temp = data[indexkp];
                        data[indexkp] = data[indexkj];
                        data[indexkj] = temp;
                    }

                    ipiv[j] = p;
                }

                // Compute multipliers.
                if (j < order & data[indexjj] != 0.0)
                {
                    for (var i = j + 1; i < order; i++)
                    {
                        data[indexj + i] /= data[indexjj];
                    }
                }
            }
        }

        /// <summary>
        /// Computes the inverse of matrix using LU factorization.
        /// </summary>
        /// <param name="a">The N by N matrix to invert. Contains the inverse On exit.</param>
        /// <param name="order">The order of the square matrix <paramref name="a"/>.</param>
        /// <remarks>This is equivalent to the GETRF and GETRI LAPACK routines.</remarks>
        public virtual void LUInverse(Complex[] a, int order)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (a.Length != order*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            var ipiv = new int[order];
            LUFactor(a, order, ipiv);
            LUInverseFactored(a, order, ipiv);
        }

        /// <summary>
        /// Computes the inverse of a previously factored matrix.
        /// </summary>
        /// <param name="a">The LU factored N by N matrix.  Contains the inverse On exit.</param>
        /// <param name="order">The order of the square matrix <paramref name="a"/>.</param>
        /// <param name="ipiv">The pivot indices of <paramref name="a"/>.</param>
        /// <remarks>This is equivalent to the GETRI LAPACK routine.</remarks>
        public virtual void LUInverseFactored(Complex[] a, int order, int[] ipiv)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (ipiv == null)
            {
                throw new ArgumentNullException("ipiv");
            }

            if (a.Length != order*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (ipiv.Length != order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "ipiv");
            }

            var inverse = new Complex[a.Length];
            for (var i = 0; i < order; i++)
            {
                inverse[i + (order*i)] = Complex.One;
            }

            LUSolveFactored(order, a, order, ipiv, inverse);
            inverse.Copy(a);
        }

        /// <summary>
        /// Solves A*X=B for X using LU factorization.
        /// </summary>
        /// <param name="columnsOfB">The number of columns of B.</param>
        /// <param name="a">The square matrix A.</param>
        /// <param name="order">The order of the square matrix <paramref name="a"/>.</param>
        /// <param name="b">On entry the B matrix; on exit the X matrix.</param>
        /// <remarks>This is equivalent to the GETRF and GETRS LAPACK routines.</remarks>
        public virtual void LUSolve(int columnsOfB, Complex[] a, int order, Complex[] b)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (a.Length != order*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (b.Length != order*columnsOfB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            var ipiv = new int[order];
            var clone = new Complex[a.Length];
            a.Copy(clone);
            LUFactor(clone, order, ipiv);
            LUSolveFactored(columnsOfB, clone, order, ipiv, b);
        }

        /// <summary>
        /// Solves A*X=B for X using a previously factored A matrix.
        /// </summary>
        /// <param name="columnsOfB">The number of columns of B.</param>
        /// <param name="a">The factored A matrix.</param>
        /// <param name="order">The order of the square matrix <paramref name="a"/>.</param>
        /// <param name="ipiv">The pivot indices of <paramref name="a"/>.</param>
        /// <param name="b">On entry the B matrix; on exit the X matrix.</param>
        /// <remarks>This is equivalent to the GETRS LAPACK routine.</remarks>
        public virtual void LUSolveFactored(int columnsOfB, Complex[] a, int order, int[] ipiv, Complex[] b)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (ipiv == null)
            {
                throw new ArgumentNullException("ipiv");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (a.Length != order*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (ipiv.Length != order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "ipiv");
            }

            if (b.Length != order*columnsOfB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            // Compute the column vector  P*B
            for (var i = 0; i < ipiv.Length; i++)
            {
                if (ipiv[i] == i)
                {
                    continue;
                }

                var p = ipiv[i];
                for (var j = 0; j < columnsOfB; j++)
                {
                    var indexk = j*order;
                    var indexkp = indexk + p;
                    var indexkj = indexk + i;
                    var temp = b[indexkp];
                    b[indexkp] = b[indexkj];
                    b[indexkj] = temp;
                }
            }

            // Solve L*Y = P*B
            for (var k = 0; k < order; k++)
            {
                var korder = k*order;
                for (var i = k + 1; i < order; i++)
                {
                    for (var j = 0; j < columnsOfB; j++)
                    {
                        var index = j*order;
                        b[i + index] -= b[k + index]*a[i + korder];
                    }
                }
            }

            // Solve U*X = Y;
            for (var k = order - 1; k >= 0; k--)
            {
                var korder = k + (k*order);
                for (var j = 0; j < columnsOfB; j++)
                {
                    b[k + (j*order)] /= a[korder];
                }

                korder = k*order;
                for (var i = 0; i < k; i++)
                {
                    for (var j = 0; j < columnsOfB; j++)
                    {
                        var index = j*order;
                        b[i + index] -= b[k + index]*a[i + korder];
                    }
                }
            }
        }

        /// <summary>
        /// Computes the Cholesky factorization of A.
        /// </summary>
        /// <param name="a">On entry, a square, positive definite matrix. On exit, the matrix is overwritten with the
        /// the Cholesky factorization.</param>
        /// <param name="order">The number of rows or columns in the matrix.</param>
        /// <remarks>This is equivalent to the POTRF LAPACK routine.</remarks>
        public virtual void CholeskyFactor(Complex[] a, int order)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            var tmpColumn = new Complex[order];

            // Main loop - along the diagonal
            for (var ij = 0; ij < order; ij++)
            {
                // "Pivot" element
                var tmpVal = a[(ij*order) + ij];

                if (tmpVal.Real > 0.0)
                {
                    tmpVal = tmpVal.SquareRoot();
                    a[(ij*order) + ij] = tmpVal;
                    tmpColumn[ij] = tmpVal;

                    // Calculate multipliers and copy to local column
                    // Current column, below the diagonal
                    for (var i = ij + 1; i < order; i++)
                    {
                        a[(ij*order) + i] /= tmpVal;
                        tmpColumn[i] = a[(ij*order) + i];
                    }

                    // Remaining columns, below the diagonal
                    DoCholeskyStep(a, order, ij + 1, order, tmpColumn, Control.MaxDegreeOfParallelism);
                }
                else
                {
                    throw new ArgumentException(Resources.ArgumentMatrixPositiveDefinite);
                }

                for (var i = ij + 1; i < order; i++)
                {
                    a[(i*order) + ij] = 0.0;
                }
            }
        }

        /// <summary>
        /// Calculate Cholesky step
        /// </summary>
        /// <param name="data">Factor matrix</param>
        /// <param name="rowDim">Number of rows</param>
        /// <param name="firstCol">Column start</param>
        /// <param name="colLimit">Total columns</param>
        /// <param name="multipliers">Multipliers calculated previously</param>
        /// <param name="availableCores">Number of available processors</param>
        static void DoCholeskyStep(Complex[] data, int rowDim, int firstCol, int colLimit, Complex[] multipliers, int availableCores)
        {
            var tmpColCount = colLimit - firstCol;

            if ((availableCores > 1) && (tmpColCount > Control.ParallelizeElements))
            {
                var tmpSplit = firstCol + (tmpColCount/3);
                var tmpCores = availableCores/2;

                CommonParallel.Invoke(
                    () => DoCholeskyStep(data, rowDim, firstCol, tmpSplit, multipliers, tmpCores),
                    () => DoCholeskyStep(data, rowDim, tmpSplit, colLimit, multipliers, tmpCores));
            }
            else
            {
                for (var j = firstCol; j < colLimit; j++)
                {
                    var tmpVal = multipliers[j];
                    for (var i = j; i < rowDim; i++)
                    {
                        data[(j*rowDim) + i] -= multipliers[i]*tmpVal.Conjugate();
                    }
                }
            }
        }

        /// <summary>
        /// Solves A*X=B for X using Cholesky factorization.
        /// </summary>
        /// <param name="a">The square, positive definite matrix A.</param>
        /// <param name="orderA">The number of rows and columns in A.</param>
        /// <param name="b">On entry the B matrix; on exit the X matrix.</param>
        /// <param name="columnsB">The number of columns in the B matrix.</param>
        /// <remarks>This is equivalent to the POTRF add POTRS LAPACK routines.</remarks>
        public virtual void CholeskySolve(Complex[] a, int orderA, Complex[] b, int columnsB)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (b.Length != orderA*columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            var clone = new Complex[a.Length];
            a.Copy(clone);
            CholeskyFactor(clone, orderA);
            CholeskySolveFactored(clone, orderA, b, columnsB);
        }

        /// <summary>
        /// Solves A*X=B for X using a previously factored A matrix.
        /// </summary>
        /// <param name="a">The square, positive definite matrix A.</param>
        /// <param name="orderA">The number of rows and columns in A.</param>
        /// <param name="b">The B matrix.</param>
        /// <param name="columnsB">The number of columns in the B matrix.</param>
        /// <remarks>This is equivalent to the POTRS LAPACK routine.</remarks>
        public virtual void CholeskySolveFactored(Complex[] a, int orderA, Complex[] b, int columnsB)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (b.Length != orderA*columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            CommonParallel.For(0, columnsB, (u, v) =>
                {
                    for (int i = u; i < v; i++)
                    {
                        DoCholeskySolve(a, orderA, b, i);
                    }
                });
        }

        /// <summary>
        /// Solves A*X=B for X using a previously factored A matrix.
        /// </summary>
        /// <param name="a">The square, positive definite matrix A. Has to be different than <paramref name="b"/>.</param>
        /// <param name="orderA">The number of rows and columns in A.</param>
        /// <param name="b">On entry the B matrix; on exit the X matrix.</param>
        /// <param name="index">The column to solve for.</param>
        static void DoCholeskySolve(Complex[] a, int orderA, Complex[] b, int index)
        {
            var cindex = index*orderA;

            // Solve L*Y = B;
            Complex sum;
            for (var i = 0; i < orderA; i++)
            {
                sum = b[cindex + i];
                for (var k = i - 1; k >= 0; k--)
                {
                    sum -= a[(k*orderA) + i]*b[cindex + k];
                }

                b[cindex + i] = sum/a[(i*orderA) + i];
            }

            // Solve L'*X = Y;
            for (var i = orderA - 1; i >= 0; i--)
            {
                sum = b[cindex + i];
                var iindex = i*orderA;
                for (var k = i + 1; k < orderA; k++)
                {
                    sum -= a[iindex + k].Conjugate()*b[cindex + k];
                }

                b[cindex + i] = sum/a[iindex + i];
            }
        }

        /// <summary>
        /// Computes the QR factorization of A.
        /// </summary>
        /// <param name="r">On entry, it is the M by N A matrix to factor. On exit,
        /// it is overwritten with the R matrix of the QR factorization. </param>
        /// <param name="rowsR">The number of rows in the A matrix.</param>
        /// <param name="columnsR">The number of columns in the A matrix.</param>
        /// <param name="q">On exit, A M by M matrix that holds the Q matrix of the
        /// QR factorization.</param>
        /// <param name="tau">A min(m,n) vector. On exit, contains additional information
        /// to be used by the QR solve routine.</param>
        /// <remarks>This is similar to the GEQRF and ORGQR LAPACK routines.</remarks>
        public virtual void QRFactor(Complex[] r, int rowsR, int columnsR, Complex[] q, Complex[] tau)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r");
            }

            if (q == null)
            {
                throw new ArgumentNullException("q");
            }

            if (r.Length != rowsR*columnsR)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, "rowsR * columnsR"), "r");
            }

            if (tau.Length < Math.Min(rowsR, columnsR))
            {
                throw new ArgumentException(string.Format(Resources.ArrayTooSmall, "min(m,n)"), "tau");
            }

            if (q.Length != rowsR*rowsR)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, "rowsR * rowsR"), "q");
            }

            var work = columnsR > rowsR ? new Complex[rowsR*rowsR] : new Complex[rowsR*columnsR];

            CommonParallel.For(0, rowsR, (a, b) =>
                {
                    for (int i = a; i < b; i++)
                    {
                        q[(i*rowsR) + i] = Complex.One;
                    }
                });

            var minmn = Math.Min(rowsR, columnsR);
            for (var i = 0; i < minmn; i++)
            {
                GenerateColumn(work, r, rowsR, i, i);
                ComputeQR(work, i, r, i, rowsR, i + 1, columnsR, Control.MaxDegreeOfParallelism);
            }

            for (var i = minmn - 1; i >= 0; i--)
            {
                ComputeQR(work, i, q, i, rowsR, i, rowsR, Control.MaxDegreeOfParallelism);
            }
        }

        /// <summary>
        /// Computes the QR factorization of A.
        /// </summary>
        /// <param name="a">On entry, it is the M by N A matrix to factor. On exit,
        /// it is overwritten with the Q matrix of the QR factorization.</param>
        /// <param name="rowsA">The number of rows in the A matrix.</param>
        /// <param name="columnsA">The number of columns in the A matrix.</param>
        /// <param name="r">On exit, A N by N matrix that holds the R matrix of the
        /// QR factorization.</param>
        /// <param name="tau">A min(m,n) vector. On exit, contains additional information
        /// to be used by the QR solve routine.</param>
        /// <remarks>This is similar to the GEQRF and ORGQR LAPACK routines.</remarks>
        public virtual void ThinQRFactor(Complex[] a, int rowsA, int columnsA, Complex[] r, Complex[] tau)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r");
            }

            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (a.Length != rowsA*columnsA)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, "rowsR * columnsR"), "a");
            }

            if (tau.Length < Math.Min(rowsA, columnsA))
            {
                throw new ArgumentException(string.Format(Resources.ArrayTooSmall, "min(m,n)"), "tau");
            }

            if (r.Length != columnsA*columnsA)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, "columnsA * columnsA"), "r");
            }

            var work = new Complex[rowsA*columnsA];

            var minmn = Math.Min(rowsA, columnsA);
            for (var i = 0; i < minmn; i++)
            {
                GenerateColumn(work, a, rowsA, i, i);
                ComputeQR(work, i, a, i, rowsA, i + 1, columnsA, Control.MaxDegreeOfParallelism);
            }

            //copy R
            for (var j = 0; j < columnsA; j++)
            {
                var rIndex = j*columnsA;
                var aIndex = j*rowsA;
                for (var i = 0; i < columnsA; i++)
                {
                    r[rIndex + i] = a[aIndex + i];
                }
            }

            //clear A and set diagonals to 1
            Array.Clear(a, 0, a.Length);
            for (var i = 0; i < columnsA; i++)
            {
                a[i*rowsA + i] = Complex.One;
            }

            for (var i = minmn - 1; i >= 0; i--)
            {
                ComputeQR(work, i, a, i, rowsA, i, columnsA, Control.MaxDegreeOfParallelism);
            }
        }


        #region QR Factor Helper functions

        /// <summary>
        /// Perform calculation of Q or R
        /// </summary>
        /// <param name="work">Work array</param>
        /// <param name="workIndex">Index of column in work array</param>
        /// <param name="a">Q or R matrices</param>
        /// <param name="rowStart">The first row in </param>
        /// <param name="rowCount">The last row</param>
        /// <param name="columnStart">The first column</param>
        /// <param name="columnCount">The last column</param>
        /// <param name="availableCores">Number of available CPUs</param>
        static void ComputeQR(Complex[] work, int workIndex, Complex[] a, int rowStart, int rowCount, int columnStart, int columnCount, int availableCores)
        {
            if (rowStart > rowCount || columnStart > columnCount)
            {
                return;
            }

            var tmpColCount = columnCount - columnStart;

            if ((availableCores > 1) && (tmpColCount > 200))
            {
                var tmpSplit = columnStart + (tmpColCount/2);
                var tmpCores = availableCores/2;

                CommonParallel.Invoke(
                    () => ComputeQR(work, workIndex, a, rowStart, rowCount, columnStart, tmpSplit, tmpCores),
                    () => ComputeQR(work, workIndex, a, rowStart, rowCount, tmpSplit, columnCount, tmpCores));
            }
            else
            {
                for (var j = columnStart; j < columnCount; j++)
                {
                    var scale = Complex.Zero;
                    for (var i = rowStart; i < rowCount; i++)
                    {
                        scale += work[(workIndex*rowCount) + i - rowStart]*a[(j*rowCount) + i];
                    }

                    for (var i = rowStart; i < rowCount; i++)
                    {
                        a[(j*rowCount) + i] -= work[(workIndex*rowCount) + i - rowStart].Conjugate()*scale;
                    }
                }
            }
        }

        /// <summary>
        /// Generate column from initial matrix to work array
        /// </summary>
        /// <param name="work">Work array</param>
        /// <param name="a">Initial matrix</param>
        /// <param name="rowCount">The number of rows in matrix</param>
        /// <param name="row">The first row</param>
        /// <param name="column">Column index</param>
        static void GenerateColumn(Complex[] work, Complex[] a, int rowCount, int row, int column)
        {
            var tmp = column*rowCount;
            var index = tmp + row;

            CommonParallel.For(row, rowCount, (u, v) =>
                {
                    for (int i = u; i < v; i++)
                    {
                        var iIndex = tmp + i;
                        work[iIndex - row] = a[iIndex];
                        a[iIndex] = Complex.Zero;
                    }
                });

            var norm = Complex.Zero;
            for (var i = 0; i < rowCount - row; ++i)
            {
                var index1 = tmp + i;
                norm += work[index1].Magnitude*work[index1].Magnitude;
            }

            norm = norm.SquareRoot();
            if (row == rowCount - 1 || norm.Magnitude == 0)
            {
                a[index] = -work[tmp];
                work[tmp] = new Complex(2.0, 0).SquareRoot();
                return;
            }

            if (work[tmp].Magnitude != 0.0)
            {
                norm = norm.Magnitude*(work[tmp]/work[tmp].Magnitude);
            }

            a[index] = -norm;
            CommonParallel.For(0, rowCount - row, 4096, (u, v) =>
                {
                    for (int i = u; i < v; i++)
                    {
                        work[tmp + i] /= norm;
                    }
                });
            work[tmp] += 1.0;

            var s = (1.0/work[tmp]).SquareRoot();
            CommonParallel.For(0, rowCount - row, 4096, (u, v) =>
                {
                    for (int i = u; i < v; i++)
                    {
                        work[tmp + i] = work[tmp + i].Conjugate()*s;
                    }
                });
        }

        #endregion

        /// <summary>
        /// Solves A*X=B for X using QR factorization of A.
        /// </summary>
        /// <param name="a">The A matrix.</param>
        /// <param name="rows">The number of rows in the A matrix.</param>
        /// <param name="columns">The number of columns in the A matrix.</param>
        /// <param name="b">The B matrix.</param>
        /// <param name="columnsB">The number of columns of B.</param>
        /// <param name="x">On exit, the solution matrix.</param>
        /// <param name="method">The type of QR factorization to perform. <seealso cref="QRMethod"/></param>
        /// <remarks>Rows must be greater or equal to columns.</remarks>
        public virtual void QRSolve(Complex[] a, int rows, int columns, Complex[] b, int columnsB, Complex[] x, QRMethod method = QRMethod.Full)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }



            if (a.Length != rows*columns)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (b.Length != rows*columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (rows < columns)
            {
                throw new ArgumentException(Resources.RowsLessThanColumns);
            }

            if (x.Length != columns*columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "x");
            }

            var work = new Complex[rows * columns];

            var clone = new Complex[a.Length];
            a.Copy(clone);

            if (method == QRMethod.Full)
            {
                var q = new Complex[rows*rows];
                QRFactor(clone, rows, columns, q, work);
                QRSolveFactored(q, clone, rows, columns, null, b, columnsB, x, method);
            }
            else
            {
                var r = new Complex[columns*columns];
                ThinQRFactor(clone, rows, columns, r, work);
                QRSolveFactored(clone, r, rows, columns, null, b, columnsB, x, method);
            }
        }

        /// <summary>
        /// Solves A*X=B for X using a previously QR factored matrix.
        /// </summary>
        /// <param name="q">The Q matrix obtained by calling <see cref="QRFactor(Complex[],int,int,Complex[],Complex[])"/>.</param>
        /// <param name="r">The R matrix obtained by calling <see cref="QRFactor(Complex[],int,int,Complex[],Complex[])"/>. </param>
        /// <param name="rowsA">The number of rows in the A matrix.</param>
        /// <param name="columnsA">The number of columns in the A matrix.</param>
        /// <param name="tau">Contains additional information on Q. Only used for the native solver
        /// and can be <c>null</c> for the managed provider.</param>
        /// <param name="b">The B matrix.</param>
        /// <param name="columnsB">The number of columns of B.</param>
        /// <param name="x">On exit, the solution matrix.</param>
        /// <param name="method">The type of QR factorization to perform. <seealso cref="QRMethod"/></param>
        /// <remarks>Rows must be greater or equal to columns.</remarks>
        public virtual void QRSolveFactored(Complex[] q, Complex[] r, int rowsA, int columnsA, Complex[] tau, Complex[] b, int columnsB, Complex[] x, QRMethod method = QRMethod.Full)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r");
            }

            if (q == null)
            {
                throw new ArgumentNullException("q");
            }

            if (b == null)
            {
                throw new ArgumentNullException("q");
            }

            if (x == null)
            {
                throw new ArgumentNullException("q");
            }

            if (rowsA < columnsA)
            {
                throw new ArgumentException(Resources.RowsLessThanColumns);
            }

            int rowsQ, columnsQ, rowsR, columnsR;
            if (method == QRMethod.Full)
            {
                rowsQ = columnsQ = rowsR = rowsA;
                columnsR = columnsA;
            }
            else
            {
                rowsQ = rowsA;
                columnsQ = rowsR = columnsR = columnsA;
            }

            if (r.Length != rowsR*columnsR)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, rowsR*columnsR), "r");
            }

            if (q.Length != rowsQ*columnsQ)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, rowsQ*columnsQ), "q");
            }

            if (b.Length != rowsA*columnsB)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, rowsA*columnsB), "b");
            }

            if (x.Length != columnsA*columnsB)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, columnsA*columnsB), "x");
            }

            var sol = new Complex[b.Length];

            // Copy B matrix to "sol", so B data will not be changed
            Array.Copy(b, 0, sol, 0, b.Length);

            // Compute Y = transpose(Q)*B
            var column = new Complex[rowsA];
            for (var j = 0; j < columnsB; j++)
            {
                var jm = j*rowsA;
                Array.Copy(sol, jm, column, 0, rowsA);
                CommonParallel.For(0, columnsA, (u, v) =>
                    {
                        for (int i = u; i < v; i++)
                        {
                            var im = i*rowsA;

                            var sum = Complex.Zero;
                            for (var k = 0; k < rowsA; k++)
                            {
                                sum += q[im + k].Conjugate()*column[k];
                            }

                            sol[jm + i] = sum;
                        }
                    });
            }

            // Solve R*X = Y;
            for (var k = columnsA - 1; k >= 0; k--)
            {
                var km = k*rowsR;
                for (var j = 0; j < columnsB; j++)
                {
                    sol[(j*rowsA) + k] /= r[km + k];
                }

                for (var i = 0; i < k; i++)
                {
                    for (var j = 0; j < columnsB; j++)
                    {
                        var jm = j*rowsA;
                        sol[jm + i] -= sol[jm + k]*r[km + i];
                    }
                }
            }

            // Fill result matrix
            for (var col = 0; col < columnsB; col++)
            {
                Array.Copy(sol, col*rowsA, x, col*columnsA, columnsR);
            }
        }

        /// <summary>
        /// Computes the singular value decomposition of A.
        /// </summary>
        /// <param name="computeVectors">Compute the singular U and VT vectors or not.</param>
        /// <param name="a">On entry, the M by N matrix to decompose. On exit, A may be overwritten.</param>
        /// <param name="rowsA">The number of rows in the A matrix.</param>
        /// <param name="columnsA">The number of columns in the A matrix.</param>
        /// <param name="s">The singular values of A in ascending value.</param>
        /// <param name="u">If <paramref name="computeVectors"/> is <c>true</c>, on exit U contains the left
        /// singular vectors.</param>
        /// <param name="vt">If <paramref name="computeVectors"/> is <c>true</c>, on exit VT contains the transposed
        /// right singular vectors.</param>
        /// <remarks>This is equivalent to the GESVD LAPACK routine.</remarks>
        public virtual void SingularValueDecomposition(bool computeVectors, Complex[] a, int rowsA, int columnsA, Complex[] s, Complex[] u, Complex[] vt)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            if (u == null)
            {
                throw new ArgumentNullException("u");
            }

            if (vt == null)
            {
                throw new ArgumentNullException("vt");
            }

            if (u.Length != rowsA*rowsA)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "u");
            }

            if (vt.Length != columnsA*columnsA)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "vt");
            }

            if (s.Length != Math.Min(rowsA, columnsA))
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "s");
            }

            var work = new Complex[rowsA];

            const int maxiter = 1000;

            var e = new Complex[columnsA];
            var v = new Complex[vt.Length];
            var stemp = new Complex[Math.Min(rowsA + 1, columnsA)];

            int i, j, l, lp1;

            Complex t;

            var ncu = rowsA;

            // Reduce matrix to bidiagonal form, storing the diagonal elements
            // in "s" and the super-diagonal elements in "e".
            var nct = Math.Min(rowsA - 1, columnsA);
            var nrt = Math.Max(0, Math.Min(columnsA - 2, rowsA));
            var lu = Math.Max(nct, nrt);

            for (l = 0; l < lu; l++)
            {
                lp1 = l + 1;
                if (l < nct)
                {
                    // Compute the transformation for the l-th column and
                    // place the l-th diagonal in vector s[l].
                    var sum = 0.0;
                    for (i = l; i < rowsA; i++)
                    {
                        sum += a[(l*rowsA) + i].Magnitude*a[(l*rowsA) + i].Magnitude;
                    }

                    stemp[l] = Math.Sqrt(sum);
                    if (stemp[l] != 0.0)
                    {
                        if (a[(l*rowsA) + l] != 0.0)
                        {
                            stemp[l] = stemp[l].Magnitude*(a[(l*rowsA) + l]/a[(l*rowsA) + l].Magnitude);
                        }

                        // A part of column "l" of Matrix A from row "l" to end multiply by 1.0 / s[l]
                        for (i = l; i < rowsA; i++)
                        {
                            a[(l*rowsA) + i] = a[(l*rowsA) + i]*(1.0/stemp[l]);
                        }

                        a[(l*rowsA) + l] = 1.0 + a[(l*rowsA) + l];
                    }

                    stemp[l] = -stemp[l];
                }

                for (j = lp1; j < columnsA; j++)
                {
                    if (l < nct)
                    {
                        if (stemp[l] != 0.0)
                        {
                            // Apply the transformation.
                            t = 0.0;
                            for (i = l; i < rowsA; i++)
                            {
                                t += a[(l*rowsA) + i].Conjugate()*a[(j*rowsA) + i];
                            }

                            t = -t/a[(l*rowsA) + l];

                            for (var ii = l; ii < rowsA; ii++)
                            {
                                a[(j*rowsA) + ii] += t*a[(l*rowsA) + ii];
                            }
                        }
                    }

                    // Place the l-th row of matrix into "e" for the
                    // subsequent calculation of the row transformation.
                    e[j] = a[(j*rowsA) + l].Conjugate();
                }

                if (computeVectors && l < nct)
                {
                    // Place the transformation in "u" for subsequent back multiplication.
                    for (i = l; i < rowsA; i++)
                    {
                        u[(l*rowsA) + i] = a[(l*rowsA) + i];
                    }
                }

                if (l >= nrt)
                {
                    continue;
                }

                // Compute the l-th row transformation and place the l-th super-diagonal in e(l).
                var enorm = 0.0;
                for (i = lp1; i < e.Length; i++)
                {
                    enorm += e[i].Magnitude*e[i].Magnitude;
                }

                e[l] = Math.Sqrt(enorm);
                if (e[l] != 0.0)
                {
                    if (e[lp1] != 0.0)
                    {
                        e[l] = e[l].Magnitude*(e[lp1]/e[lp1].Magnitude);
                    }

                    // Scale vector "e" from "lp1" by 1.0 / e[l]
                    for (i = lp1; i < e.Length; i++)
                    {
                        e[i] = e[i]*(1.0/e[l]);
                    }

                    e[lp1] = 1.0 + e[lp1];
                }

                e[l] = -e[l].Conjugate();

                if (lp1 < rowsA && e[l] != 0.0)
                {
                    // Apply the transformation.
                    for (i = lp1; i < rowsA; i++)
                    {
                        work[i] = 0.0;
                    }

                    for (j = lp1; j < columnsA; j++)
                    {
                        for (var ii = lp1; ii < rowsA; ii++)
                        {
                            work[ii] += e[j]*a[(j*rowsA) + ii];
                        }
                    }

                    for (j = lp1; j < columnsA; j++)
                    {
                        var ww = (-e[j]/e[lp1]).Conjugate();
                        for (var ii = lp1; ii < rowsA; ii++)
                        {
                            a[(j*rowsA) + ii] += ww*work[ii];
                        }
                    }
                }

                if (!computeVectors)
                {
                    continue;
                }

                // Place the transformation in v for subsequent back multiplication.
                for (i = lp1; i < columnsA; i++)
                {
                    v[(l*columnsA) + i] = e[i];
                }
            }

            // Set up the final bidiagonal matrix or order m.
            var m = Math.Min(columnsA, rowsA + 1);
            var nctp1 = nct + 1;
            var nrtp1 = nrt + 1;
            if (nct < columnsA)
            {
                stemp[nctp1 - 1] = a[((nctp1 - 1)*rowsA) + (nctp1 - 1)];
            }

            if (rowsA < m)
            {
                stemp[m - 1] = 0.0;
            }

            if (nrtp1 < m)
            {
                e[nrtp1 - 1] = a[((m - 1)*rowsA) + (nrtp1 - 1)];
            }

            e[m - 1] = 0.0;

            // If required, generate "u".
            if (computeVectors)
            {
                for (j = nctp1 - 1; j < ncu; j++)
                {
                    for (i = 0; i < rowsA; i++)
                    {
                        u[(j*rowsA) + i] = 0.0;
                    }

                    u[(j*rowsA) + j] = 1.0;
                }

                for (l = nct - 1; l >= 0; l--)
                {
                    if (stemp[l] != 0.0)
                    {
                        for (j = l + 1; j < ncu; j++)
                        {
                            t = 0.0;
                            for (i = l; i < rowsA; i++)
                            {
                                t += u[(l*rowsA) + i].Conjugate()*u[(j*rowsA) + i];
                            }

                            t = -t/u[(l*rowsA) + l];
                            for (var ii = l; ii < rowsA; ii++)
                            {
                                u[(j*rowsA) + ii] += t*u[(l*rowsA) + ii];
                            }
                        }

                        // A part of column "l" of matrix A from row "l" to end multiply by -1.0
                        for (i = l; i < rowsA; i++)
                        {
                            u[(l*rowsA) + i] = u[(l*rowsA) + i]*-1.0;
                        }

                        u[(l*rowsA) + l] = 1.0 + u[(l*rowsA) + l];
                        for (i = 0; i < l; i++)
                        {
                            u[(l*rowsA) + i] = 0.0;
                        }
                    }
                    else
                    {
                        for (i = 0; i < rowsA; i++)
                        {
                            u[(l*rowsA) + i] = 0.0;
                        }

                        u[(l*rowsA) + l] = 1.0;
                    }
                }
            }

            // If it is required, generate v.
            if (computeVectors)
            {
                for (l = columnsA - 1; l >= 0; l--)
                {
                    lp1 = l + 1;
                    if (l < nrt)
                    {
                        if (e[l] != 0.0)
                        {
                            for (j = lp1; j < columnsA; j++)
                            {
                                t = 0.0;
                                for (i = lp1; i < columnsA; i++)
                                {
                                    t += v[(l*columnsA) + i].Conjugate()*v[(j*columnsA) + i];
                                }

                                t = -t/v[(l*columnsA) + lp1];
                                for (var ii = l; ii < columnsA; ii++)
                                {
                                    v[(j*columnsA) + ii] += t*v[(l*columnsA) + ii];
                                }
                            }
                        }
                    }

                    for (i = 0; i < columnsA; i++)
                    {
                        v[(l*columnsA) + i] = 0.0;
                    }

                    v[(l*columnsA) + l] = 1.0;
                }
            }

            // Transform "s" and "e" so that they are double
            for (i = 0; i < m; i++)
            {
                Complex r;
                if (stemp[i] != 0.0)
                {
                    t = stemp[i].Magnitude;
                    r = stemp[i]/t;
                    stemp[i] = t;
                    if (i < m - 1)
                    {
                        e[i] = e[i]/r;
                    }

                    if (computeVectors)
                    {
                        // A part of column "i" of matrix U from row 0 to end multiply by r
                        for (j = 0; j < rowsA; j++)
                        {
                            u[(i*rowsA) + j] = u[(i*rowsA) + j]*r;
                        }
                    }
                }

                // Exit
                if (i == m - 1)
                {
                    break;
                }

                if (e[i] == 0.0)
                {
                    continue;
                }

                t = e[i].Magnitude;
                r = t/e[i];
                e[i] = t;
                stemp[i + 1] = stemp[i + 1]*r;
                if (!computeVectors)
                {
                    continue;
                }

                // A part of column "i+1" of matrix VT from row 0 to end multiply by r
                for (j = 0; j < columnsA; j++)
                {
                    v[((i + 1)*columnsA) + j] = v[((i + 1)*columnsA) + j]*r;
                }
            }

            // Main iteration loop for the singular values.
            var mn = m;
            var iter = 0;

            while (m > 0)
            {
                // Quit if all the singular values have been found.
                // If too many iterations have been performed throw exception.
                if (iter >= maxiter)
                {
                    throw new NonConvergenceException();
                }

                // This section of the program inspects for negligible elements in the s and e arrays,
                // on completion the variables kase and l are set as follows:
                // kase = 1: if mS[m] and e[l-1] are negligible and l < m
                // kase = 2: if mS[l] is negligible and l < m
                // kase = 3: if e[l-1] is negligible, l < m, and mS[l, ..., mS[m] are not negligible (qr step).
                // kase = 4: if e[m-1] is negligible (convergence).
                double ztest;
                double test;
                for (l = m - 2; l >= 0; l--)
                {
                    test = stemp[l].Magnitude + stemp[l + 1].Magnitude;
                    ztest = test + e[l].Magnitude;
                    if (ztest.AlmostEqualRelative(test, 15))
                    {
                        e[l] = 0.0;
                        break;
                    }
                }

                int kase;
                if (l == m - 2)
                {
                    kase = 4;
                }
                else
                {
                    int ls;
                    for (ls = m - 1; ls > l; ls--)
                    {
                        test = 0.0;
                        if (ls != m - 1)
                        {
                            test = test + e[ls].Magnitude;
                        }

                        if (ls != l + 1)
                        {
                            test = test + e[ls - 1].Magnitude;
                        }

                        ztest = test + stemp[ls].Magnitude;
                        if (ztest.AlmostEqualRelative(test, 15))
                        {
                            stemp[ls] = 0.0;
                            break;
                        }
                    }

                    if (ls == l)
                    {
                        kase = 3;
                    }
                    else if (ls == m - 1)
                    {
                        kase = 1;
                    }
                    else
                    {
                        kase = 2;
                        l = ls;
                    }
                }

                l = l + 1;

                // Perform the task indicated by kase.
                int k;
                double f;
                double cs;
                double sn;
                switch (kase)
                {
                        // Deflate negligible s[m].
                    case 1:
                        f = e[m - 2].Real;
                        e[m - 2] = 0.0;
                        double t1;
                        for (var kk = l; kk < m - 1; kk++)
                        {
                            k = m - 2 - kk + l;
                            t1 = stemp[k].Real;
                            Drotg(ref t1, ref f, out cs, out sn);
                            stemp[k] = t1;
                            if (k != l)
                            {
                                f = -sn*e[k - 1].Real;
                                e[k - 1] = cs*e[k - 1];
                            }

                            if (computeVectors)
                            {
                                // Rotate
                                for (i = 0; i < columnsA; i++)
                                {
                                    var z = (cs*v[(k*columnsA) + i]) + (sn*v[((m - 1)*columnsA) + i]);
                                    v[((m - 1)*columnsA) + i] = (cs*v[((m - 1)*columnsA) + i]) - (sn*v[(k*columnsA) + i]);
                                    v[(k*columnsA) + i] = z;
                                }
                            }
                        }

                        break;

                        // Split at negligible s[l].
                    case 2:
                        f = e[l - 1].Real;
                        e[l - 1] = 0.0;
                        for (k = l; k < m; k++)
                        {
                            t1 = stemp[k].Real;
                            Drotg(ref t1, ref f, out cs, out sn);
                            stemp[k] = t1;
                            f = -sn*e[k].Real;
                            e[k] = cs*e[k];
                            if (computeVectors)
                            {
                                // Rotate
                                for (i = 0; i < rowsA; i++)
                                {
                                    var z = (cs*u[(k*rowsA) + i]) + (sn*u[((l - 1)*rowsA) + i]);
                                    u[((l - 1)*rowsA) + i] = (cs*u[((l - 1)*rowsA) + i]) - (sn*u[(k*rowsA) + i]);
                                    u[(k*rowsA) + i] = z;
                                }
                            }
                        }

                        break;

                        // Perform one qr step.
                    case 3:
                        // calculate the shift.
                        var scale = 0.0;
                        scale = Math.Max(scale, stemp[m - 1].Magnitude);
                        scale = Math.Max(scale, stemp[m - 2].Magnitude);
                        scale = Math.Max(scale, e[m - 2].Magnitude);
                        scale = Math.Max(scale, stemp[l].Magnitude);
                        scale = Math.Max(scale, e[l].Magnitude);
                        var sm = stemp[m - 1].Real/scale;
                        var smm1 = stemp[m - 2].Real/scale;
                        var emm1 = e[m - 2].Real/scale;
                        var sl = stemp[l].Real/scale;
                        var el = e[l].Real/scale;
                        var b = (((smm1 + sm)*(smm1 - sm)) + (emm1*emm1))/2.0;
                        var c = (sm*emm1)*(sm*emm1);
                        var shift = 0.0;
                        if (b != 0.0 || c != 0.0)
                        {
                            shift = Math.Sqrt((b*b) + c);
                            if (b < 0.0)
                            {
                                shift = -shift;
                            }

                            shift = c/(b + shift);
                        }

                        f = ((sl + sm)*(sl - sm)) + shift;
                        var g = sl*el;

                        // Chase zeros
                        for (k = l; k < m - 1; k++)
                        {
                            Drotg(ref f, ref g, out cs, out sn);
                            if (k != l)
                            {
                                e[k - 1] = f;
                            }

                            f = (cs*stemp[k].Real) + (sn*e[k].Real);
                            e[k] = (cs*e[k]) - (sn*stemp[k]);
                            g = sn*stemp[k + 1].Real;
                            stemp[k + 1] = cs*stemp[k + 1];
                            if (computeVectors)
                            {
                                for (i = 0; i < columnsA; i++)
                                {
                                    var z = (cs*v[(k*columnsA) + i]) + (sn*v[((k + 1)*columnsA) + i]);
                                    v[((k + 1)*columnsA) + i] = (cs*v[((k + 1)*columnsA) + i]) - (sn*v[(k*columnsA) + i]);
                                    v[(k*columnsA) + i] = z;
                                }
                            }

                            Drotg(ref f, ref g, out cs, out sn);
                            stemp[k] = f;
                            f = (cs*e[k].Real) + (sn*stemp[k + 1].Real);
                            stemp[k + 1] = -(sn*e[k]) + (cs*stemp[k + 1]);
                            g = sn*e[k + 1].Real;
                            e[k + 1] = cs*e[k + 1];
                            if (computeVectors && k < rowsA)
                            {
                                for (i = 0; i < rowsA; i++)
                                {
                                    var z = (cs*u[(k*rowsA) + i]) + (sn*u[((k + 1)*rowsA) + i]);
                                    u[((k + 1)*rowsA) + i] = (cs*u[((k + 1)*rowsA) + i]) - (sn*u[(k*rowsA) + i]);
                                    u[(k*rowsA) + i] = z;
                                }
                            }
                        }

                        e[m - 2] = f;
                        iter = iter + 1;
                        break;

                        // Convergence
                    case 4:

                        // Make the singular value  positive
                        if (stemp[l].Real < 0.0)
                        {
                            stemp[l] = -stemp[l];
                            if (computeVectors)
                            {
                                // A part of column "l" of matrix VT from row 0 to end multiply by -1
                                for (i = 0; i < columnsA; i++)
                                {
                                    v[(l*columnsA) + i] = v[(l*columnsA) + i]*-1.0;
                                }
                            }
                        }

                        // Order the singular value.
                        while (l != mn - 1)
                        {
                            if (stemp[l].Real >= stemp[l + 1].Real)
                            {
                                break;
                            }

                            t = stemp[l];
                            stemp[l] = stemp[l + 1];
                            stemp[l + 1] = t;
                            if (computeVectors && l < columnsA)
                            {
                                // Swap columns l, l + 1
                                for (i = 0; i < columnsA; i++)
                                {
                                    var z = v[(l*columnsA) + i];
                                    v[(l*columnsA) + i] = v[((l + 1)*columnsA) + i];
                                    v[((l + 1)*columnsA) + i] = z;
                                }
                            }

                            if (computeVectors && l < rowsA)
                            {
                                // Swap columns l, l + 1
                                for (i = 0; i < rowsA; i++)
                                {
                                    var z = u[(l*rowsA) + i];
                                    u[(l*rowsA) + i] = u[((l + 1)*rowsA) + i];
                                    u[((l + 1)*rowsA) + i] = z;
                                }
                            }

                            l = l + 1;
                        }

                        iter = 0;
                        m = m - 1;
                        break;
                }
            }

            if (computeVectors)
            {
                // Finally transpose "v" to get "vt" matrix
                for (i = 0; i < columnsA; i++)
                {
                    for (j = 0; j < columnsA; j++)
                    {
                        vt[(j*columnsA) + i] = v[(i*columnsA) + j].Conjugate();
                    }
                }
            }

            // Copy stemp to s with size adjustment. We are using ported copy of linpack's svd code and it uses
            // a singular vector of length rows+1 when rows < columns. The last element is not used and needs to be removed.
            // We should port lapack's svd routine to remove this problem.
            Array.Copy(stemp, 0, s, 0, Math.Min(rowsA, columnsA));
        }

        /// <summary>
        /// Solves A*X=B for X using the singular value decomposition of A.
        /// </summary>
        /// <param name="a">On entry, the M by N matrix to decompose.</param>
        /// <param name="rowsA">The number of rows in the A matrix.</param>
        /// <param name="columnsA">The number of columns in the A matrix.</param>
        /// <param name="b">The B matrix.</param>
        /// <param name="columnsB">The number of columns of B.</param>
        /// <param name="x">On exit, the solution matrix.</param>
        public virtual void SvdSolve(Complex[] a, int rowsA, int columnsA, Complex[] b, int columnsB, Complex[] x)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (b.Length != rowsA*columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (x.Length != columnsA*columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            var s = new Complex[Math.Min(rowsA, columnsA)];
            var u = new Complex[rowsA*rowsA];
            var vt = new Complex[columnsA*columnsA];

            var clone = new Complex[a.Length];
            a.Copy(clone);
            SingularValueDecomposition(true, clone, rowsA, columnsA, s, u, vt);
            SvdSolveFactored(rowsA, columnsA, s, u, vt, b, columnsB, x);
        }

        /// <summary>
        /// Solves A*X=B for X using a previously SVD decomposed matrix.
        /// </summary>
        /// <param name="rowsA">The number of rows in the A matrix.</param>
        /// <param name="columnsA">The number of columns in the A matrix.</param>
        /// <param name="s">The s values returned by <see cref="SingularValueDecomposition(bool,Complex[],int,int,Complex[],Complex[],Complex[])"/>.</param>
        /// <param name="u">The left singular vectors returned by  <see cref="SingularValueDecomposition(bool,Complex[],int,int,Complex[],Complex[],Complex[])"/>.</param>
        /// <param name="vt">The right singular  vectors returned by  <see cref="SingularValueDecomposition(bool,Complex[],int,int,Complex[],Complex[],Complex[])"/>.</param>
        /// <param name="b">The B matrix.</param>
        /// <param name="columnsB">The number of columns of B.</param>
        /// <param name="x">On exit, the solution matrix.</param>
        public virtual void SvdSolveFactored(int rowsA, int columnsA, Complex[] s, Complex[] u, Complex[] vt, Complex[] b, int columnsB, Complex[] x)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            if (u == null)
            {
                throw new ArgumentNullException("u");
            }

            if (vt == null)
            {
                throw new ArgumentNullException("vt");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (u.Length != rowsA*rowsA)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "u");
            }

            if (vt.Length != columnsA*columnsA)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "vt");
            }

            if (s.Length != Math.Min(rowsA, columnsA))
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "s");
            }

            if (b.Length != rowsA*columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (x.Length != columnsA*columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            var mn = Math.Min(rowsA, columnsA);
            var tmp = new Complex[columnsA];

            for (var k = 0; k < columnsB; k++)
            {
                for (var j = 0; j < columnsA; j++)
                {
                    var value = Complex.Zero;
                    if (j < mn)
                    {
                        for (var i = 0; i < rowsA; i++)
                        {
                            value += u[(j*rowsA) + i].Conjugate()*b[(k*rowsA) + i];
                        }

                        value /= s[j];
                    }

                    tmp[j] = value;
                }

                for (var j = 0; j < columnsA; j++)
                {
                    var value = Complex.Zero;
                    for (var i = 0; i < columnsA; i++)
                    {
                        value += vt[(j*columnsA) + i].Conjugate()*tmp[i];
                    }

                    x[(k*columnsA) + j] = value;
                }
            }
        }

        /// <summary>
        /// Computes the eigenvalues and eigenvectors of a matrix.
        /// </summary>
        /// <param name="isSymmetric">Whether the matrix is symmetric or not.</param>
        /// <param name="order">The order of the matrix.</param>
        /// <param name="matrix">The matrix to decompose. The lenth of the array must be order * order.</param>
        /// <param name="matrixEv">On output, the matrix contains the eigen vectors. The lenth of the array must be order * order.</param>
        /// <param name="vectorEv">On output, the eigen values (λ) of matrix in ascending value. The length of the arry must <paramref name="order"/>.</param>
        /// <param name="matrixD">On output, the block diagonal eigenvalue matrix. The lenth of the array must be order * order.</param>
        public virtual void EigenDecomp(bool isSymmetric, int order, Complex[] matrix, Complex[] matrixEv, Complex[] vectorEv, Complex[] matrixD)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException("matrix");
            }

            if (matrix.Length != order*order)
            {
                throw new ArgumentException(String.Format(Resources.ArgumentArrayWrongLength, order*order), "matrix");
            }

            if (matrixEv == null)
            {
                throw new ArgumentNullException("matrixEv");
            }

            if (matrixEv.Length != order*order)
            {
                throw new ArgumentException(String.Format(Resources.ArgumentArrayWrongLength, order*order), "matrixEv");
            }

            if (vectorEv == null)
            {
                throw new ArgumentNullException("vectorEv");
            }

            if (vectorEv.Length != order)
            {
                throw new ArgumentException(String.Format(Resources.ArgumentArrayWrongLength, order), "vectorEv");
            }

            if (matrixD == null)
            {
                throw new ArgumentNullException("matrixD");
            }

            if (matrixD.Length != order*order)
            {
                throw new ArgumentException(String.Format(Resources.ArgumentArrayWrongLength, order*order), "matrixD");
            }
            var matrixCopy = new Complex[matrix.Length];
            Array.Copy(matrix, 0, matrixCopy, 0, matrix.Length);

            if (isSymmetric)
            {
                var tau = new Complex[order];
                var d = new double[order];
                var e = new double[order];

                DenseEvd.SymmetricTridiagonalize(matrixCopy, d, e, tau, order);
                DenseEvd.SymmetricDiagonalize(matrixEv, d, e, order);
                DenseEvd.SymmetricUntridiagonalize(matrixEv, matrixCopy, tau, order);

                for (var i = 0; i < order; i++)
                {
                    vectorEv[i] = new Complex(d[i], e[i]);
                }
            }
            else
            {
                DenseEvd.NonsymmetricReduceToHessenberg(matrixEv, matrixCopy, order);
                DenseEvd.NonsymmetricReduceHessenberToRealSchur(vectorEv, matrixEv, matrixCopy, order);
            }

            for (var i = 0; i < order; i++)
            {
                matrixD[i*order + i] = vectorEv[i];
            }
        }
    }
}
