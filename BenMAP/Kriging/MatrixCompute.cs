using System;
using System.Collections.Generic;
using System.Text;

namespace ESIL.Kriging
{
    public enum HandleSign
    {
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,
        Equal,
        And,
        Or
    }
    public static class MatrixCompute
    {
        public static bool Eye(ref double[,] x)
        {
            try
            {
                if (x == null)
                { return false; }
                int rows = x.GetLength(0);
                int cols = x.GetLength(1);
                if (rows == 0 && rows == cols)
                { return false; }
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (row != col)
                        { x[row, col] = 0.0; }
                        else
                        { x[row, col] = 1.0; }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static double Det(double[,] detArray)
        {
            try
            {
                if (detArray == null)
                { return 1e-10; }
                double f = 1.0;
                double det = 1.0;
                double q = 0.0;
                double d;
                int iis = 0;
                int js = 0;
                int rowCount = detArray.GetLength(0);
                int colCount = detArray.GetLength(1);
                if (rowCount != colCount || rowCount == 0)
                { return det = 1e-10; }
                int n = rowCount;
                for (int k = 0; k <= n - 2; k++)
                {
                    q = 0.0;
                    for (int i = k; i <= n - 1; i++)
                    {
                        for (int j = k; j <= n - 1; j++)
                        {
                            d = Math.Abs(detArray[i, j]);
                            if (d > q)
                            {
                                q = d;
                                iis = i;
                                js = j;
                            }
                        }
                    } if (q + 1.0 == 1.0)
                    {
                        det = 1e-10;
                        return det;
                    }
                    if (iis != k)
                    {
                        f = -f;
                        for (int j = k; j <= n - 1; j++)
                        {
                            d = detArray[k, j];
                            detArray[k, j] = detArray[iis, j];
                            detArray[iis, j] = d;
                        }
                    } if (js != k)
                    {
                        f = -f;
                        for (int i = k; i <= n - 1; i++)
                        {
                            d = detArray[i, js];
                            detArray[i, js] = detArray[i, k];
                            detArray[i, k] = d;
                        }
                    } det = det * detArray[k, k];
                    for (int i = k + 1; i <= n - 1; i++)
                    {
                        d = detArray[i, k] / detArray[k, k];
                        for (int j = k + 1; j <= n - 1; j++)
                        {
                            detArray[i, j] = detArray[i, j] - d * detArray[k, j];
                        }
                    }
                } det = f * det * detArray[n - 1, n - 1];
                return det;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 1e-10;
            }
        }

        public static double Det(double[] a, int n)
        {
            try
            {
                if (a == null)
                { return 1e-10; }
                double f = 1.0;
                double det = 1.0;
                double q = 0.0;
                double d;
                int l = 0;
                int iis = 0;
                int js = 0;
                int u = 0;
                int v = 0;
                for (int k = 0; k <= n - 2; k++)
                {
                    q = 0.0;
                    for (int i = k; i <= n - 1; i++)
                    {
                        for (int j = k; j <= n - 1; j++)
                        {
                            l = i * n + j;
                            d = Math.Abs(a[l]);
                            if (d > q)
                            {
                                q = d;
                                iis = i;
                                js = j;
                            }
                        }
                    } if (q + 1.0 == 1.0)
                    {
                        det = 1e-10;
                        return det;
                    }
                    if (iis != k)
                    {
                        f = -f;
                        for (int j = k; j <= n - 1; j++)
                        {
                            u = k * n + j;
                            v = iis * n + j;
                            d = a[u];
                            a[u] = a[v];
                            a[v] = d;
                        }
                    } if (js != k)
                    {
                        f = -f;
                        for (int i = k; i <= n - 1; i++)
                        {
                            u = i * n + js;
                            v = i * n + k;
                            d = a[u];
                            a[u] = a[v];
                            a[v] = d;
                        }
                    } l = k * n + k;
                    det = det * a[l];
                    for (int i = k + 1; i <= n - 1; i++)
                    {
                        d = a[i * n + k] / a[l];
                        for (int j = k + 1; j <= n - 1; j++)
                        {
                            u = i * n + j;
                            a[u] = a[u] - d * a[k * n + j];
                        }
                    }
                } det = f * det * a[n * n - 1];
                return det;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 1e-10;
            }
        }

        public static bool Add(double[] matrixI, double[] matrixII, ref double[] resultMatrix)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int lensI = matrixI.Length;
                int lensII = matrixII.Length;
                if (lensI != lensII || lensI == 0)
                { return false; }
                resultMatrix = new double[lensI];
                for (int i = 0; i < lensI; i++)
                { resultMatrix[i] = matrixI[i] + matrixII[i]; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Add(double[,] matrixI, double[,] matrixII, ref double[,] resultMatrix)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0); int colsI = matrixI.GetLength(1); int rowsII = matrixII.GetLength(0);
                int colsII = matrixII.GetLength(1);
                if ((rowsI != rowsII) || (colsI != colsII) || (rowsII == 0 && rowsII == rowsI))
                { return false; }
                resultMatrix = new double[rowsI, colsI];
                for (int i = 0; i < rowsI; i++)
                {
                    for (int j = 0; j < colsI; j++)
                    { resultMatrix[i, j] = matrixI[i, j] + matrixII[i, j]; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Add(double[,] matrixI, double constant, ref double[,] resultMatrix)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int rowsI = matrixI.GetLength(0); int colsI = matrixI.GetLength(1); if (rowsI == 0 && colsI == rowsI)
                { return false; }
                resultMatrix = new double[rowsI, colsI];
                for (int i = 0; i < rowsI; i++)
                {
                    for (int j = 0; j < colsI; j++)
                    { resultMatrix[i, j] = matrixI[i, j] + constant; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Add(double[] vector, double constant, ref double[] values)
        {
            try
            {
                if (vector == null)
                { return false; }
                int len = vector.Length;
                if (len == 0) { return false; }
                values = new double[len];
                for (int j = 0; j < len; j++)
                { values[j] = vector[j] + constant; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Subtract(double[] matrixI, double[] matrixII, ref double[] resultMatrix)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int lensI = matrixI.Length;
                int lensII = matrixII.Length;
                if ((lensI != lensII) || (lensI == 0 && lensI == lensII))
                { return false; }
                resultMatrix = new double[matrixI.Length];
                for (int i = 0; i < lensI; i++)
                { resultMatrix[i] = matrixI[i] - matrixII[i]; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Subtract(double[,] matrixI, double[,] matrixII, ref double[,] resultMatrix)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0); int colsI = matrixI.GetLength(1); int rowsII = matrixII.GetLength(0);
                int colsII = matrixII.GetLength(1);
                if ((rowsI != rowsII) || (colsI != colsII) || (rowsI == 0 && rowsI == colsI) || (rowsII == 0 && rowsII == colsII))
                { return false; }
                resultMatrix = new double[rowsI, colsI];
                for (int i = 0; i < rowsI; i++)
                {
                    for (int j = 0; j < colsI; j++)
                    { resultMatrix[i, j] = matrixI[i, j] - matrixII[i, j]; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Subtract(double[,] matrixI, double constant, ref double[,] resultMatrix)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int rowsI = matrixI.GetLength(0); int colsI = matrixI.GetLength(1); if (rowsI == colsI && rowsI == 0)
                { return false; }
                resultMatrix = new double[rowsI, colsI];
                for (int i = 0; i < rowsI; i++)
                {
                    for (int j = 0; j < colsI; j++)
                    { resultMatrix[i, j] = matrixI[i, j] - constant; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Multiply(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);
                int colsI = matrixI.GetLength(1); int rowsII = matrixII.GetLength(0); int colsII = matrixII.GetLength(1);
                if ((colsI != rowsII) || (rowsI == 0 && rowsI == colsI) || (rowsII == 0 && rowsII == colsII))
                { return false; }
                matrixResult = new double[rowsI, colsII];
                for (int rowI = 0; rowI < rowsI; rowI++)
                {
                    for (int colII = 0; colII < colsII; colII++)
                    {
                        for (int colI = 0; colI < colsI; colI++)
                        {
                            matrixResult[rowI, colII] += matrixI[rowI, colI] * matrixII[colI, colII];
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Multiply(double[,] matrixI, double[] matrixII, ref double[] matrixResult)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);
                int colsI = matrixI.GetLength(1); int lensII = matrixII.Length;
                if ((colsI != lensII) || (rowsI == 0 && rowsI == colsI) || (lensII == 0))
                { return false; }
                matrixResult = new double[rowsI];
                for (int rowI = 0; rowI < rowsI; rowI++)
                {
                    for (int colI = 0; colI < colsI; colI++)
                    { matrixResult[rowI] += matrixI[rowI, colI] * matrixII[colI]; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Multiply(double[] matrixI, double[] matrixII, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int cols = matrixI.Length; int rows = matrixII.Length; if (cols == 0 && rows == cols)
                { return false; }
                matrixResult = new double[rows, cols];
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        matrixResult[row, col] = matrixI[col] * matrixII[row];
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Multiply(double[] matrixI, double[] matrixII, ref double matrixResult)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int lensI = matrixI.Length; int lensII = matrixII.Length; if ((lensI != lensII) || (lensII == 0 && lensII == lensI))
                { return false; }
                matrixResult = 0;
                for (int i = 0; i < lensI; i++)
                { matrixResult += matrixI[i] * matrixII[i]; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Multiply(double[] matrixI, double[,] matrixII, ref double[] matrixResult)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int lens = matrixI.Length; int rows = matrixII.GetLength(0); int cols = matrixII.GetLength(1); if ((lens != rows) || (rows == 0 && rows == cols))
                { return false; }
                matrixResult = new double[cols];
                for (int p = 0; p < cols; p++)
                {
                    for (int col = 0; col < lens; col++)
                    {
                        for (int row = 0; row < rows; row++)
                        { matrixResult[p] += matrixI[row] * matrixII[row, col]; }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }
        public static bool Multiply(double[] matrixI, double constant, ref double[] matrixResult)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int length = matrixI.Length;
                if (length == 0)
                { return false; }
                matrixResult = new double[length];
                for (int i = 0; i < length; i++)
                { matrixResult[i] = matrixI[i] * constant; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }
        public static bool Multiply(double[,] matrixI, double constant, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int colsI = matrixI.GetLength(1); int rowsI = matrixI.GetLength(0);
                if (rowsI == 0 && rowsI == colsI)
                { return false; }
                matrixResult = new double[rowsI, colsI];
                for (int rowI = 0; rowI < rowsI; rowI++)
                {
                    for (int colI = 0; colI < colsI; colI++)
                    {
                        matrixResult[rowI, colI] = matrixI[rowI, colI] * constant;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }
        public static bool DotMultiply(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int colsI = matrixI.GetLength(1); int rowsI = matrixI.GetLength(0);
                int colsII = matrixII.GetLength(1);
                int rowsII = matrixII.GetLength(0); if ((rowsI != rowsII) || (colsI != colsII) || (rowsI == 0 && rowsI == rowsII))
                { return false; }
                matrixResult = new double[rowsI, colsI];
                for (int rowI = 0; rowI < rowsI; rowI++)
                {
                    for (int colI = 0; colI < colsI; colI++)
                    {
                        matrixResult[rowI, colI] = matrixI[rowI, colI] * matrixII[rowI, colI];
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool DotMultiply(double[] vector1, double[] vector2, ref double[] resultValues)
        {
            try
            {
                if (vector1 == null || vector2 == null)
                { return false; }
                int len1 = vector1.Length;
                int len2 = vector2.Length;
                if ((len1 != len2) || (len1 == len2 && len2 == 0))
                { return false; }
                int len = len1;
                resultValues = new double[len];
                for (int i = 0; i < len; i++)
                {
                    resultValues[i] = vector1[i] * vector2[i];
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static double[] DotMultiply(double[] vector1, double[] vector2)
        {
            double[] resultValues = new double[0];
            try
            {
                if (vector1 == null || vector2 == null)
                { return null; }
                int len1 = vector1.Length;
                int len2 = vector2.Length;
                if ((len1 != len2) || (len1 == len2 && len2 == 0))
                { return null; }
                int len = len1;
                resultValues = new double[len];
                for (int i = 0; i < len; i++)
                {
                    resultValues[i] = vector1[i] * vector2[i];
                }
                return resultValues;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }
        public static bool DotMultiply(double[,] matrixI, double constant, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int colsI = matrixI.GetLength(1); int rowsI = matrixI.GetLength(0);
                if (rowsI == 0 && rowsI == colsI)
                { return false; }
                matrixResult = new double[rowsI, colsI];
                for (int rowI = 0; rowI < rowsI; rowI++)
                {
                    for (int colI = 0; colI < colsI; colI++)
                    {
                        matrixResult[rowI, colI] = matrixI[rowI, colI] * constant;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }
        public static double[] DotMultiply(double[] vector1, double constant)
        {
            double[] resultValues = new double[0];
            try
            {
                if (vector1 == null)
                { return null; }
                int len1 = vector1.Length;
                if (len1 == 0)
                { return null; }
                int len = len1;
                resultValues = new double[len];
                for (int i = 0; i < len; i++)
                { resultValues[i] = vector1[i] * constant; }
                return resultValues;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }
        public static bool RDivision(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {
                bool ok = false;
                if (matrixI == null || matrixII == null)
                { return false; }
                int rows = matrixII.GetLength(0);
                int cols = matrixII.GetLength(1);
                if ((rows != cols) || (rows == 0 && rows == cols))
                { return false; }
                double[,] tmps = new double[rows, cols];
                ok = Common.Init2Dimensions(ref tmps, matrixII);
                if (!ok) { return false; }
                ok = Inv(ref tmps);
                if (!ok) { return false; }
                ok = Multiply(matrixI, tmps, ref matrixResult);
                if (!ok) { return false; }
                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool RDivision(double[] matrixI, double[,] matrixII, ref double[] matrixResult)
        {
            try
            {
                bool ok = false;
                if (matrixI == null || matrixII == null)
                { return false; }
                int len = matrixI.Length;
                int rows = matrixII.GetLength(0);
                int cols = matrixII.GetLength(1);
                if (len == 0 || (rows != cols) || (rows == 0 && rows == cols))
                { return false; }
                double[,] tmps = new double[rows, cols];
                ok = Common.Init2Dimensions(ref tmps, matrixII);
                if (!ok) { return false; }
                ok = Inv(ref tmps);
                if (!ok) { return false; }
                ok = Multiply(matrixI, tmps, ref matrixResult);
                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool RDivision(double[,] matrixI, double constant, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int rows = matrixI.GetLength(0);
                int cols = matrixI.GetLength(1);
                if (rows != cols || (rows == 0 && rows == cols))
                { return false; }
                matrixResult = new double[rows, cols];
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (matrixI[row, col] == 0)
                        { matrixResult[row, col] = 0.0; }
                        else
                        { matrixResult[row, col] = matrixI[row, col] / constant; }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool RDivision(double[] matrixI, double constant, ref double[] matrixResult)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int len = matrixI.Length;
                if (len == 0)
                { return false; }
                matrixResult = new double[len];
                for (int row = 0; row < len; row++)
                {
                    matrixResult[row] = matrixI[row] / constant;
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool LDivision(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {
                bool ok = false;
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);
                int colsI = matrixI.GetLength(1);

                int rowsII = matrixII.GetLength(0);
                int colsII = matrixII.GetLength(1);
                if ((rowsI != colsI) || (rowsI == 0 && rowsI == colsI) || (rowsII == 0 && rowsII == colsII))
                { return false; }
                double[,] tmps = new double[rowsI, colsI];
                ok = Common.Init2Dimensions(ref tmps, matrixI);
                if (!ok) { return false; }
                ok = Inv(ref tmps);
                if (!ok) { return false; }
                ok = Multiply(tmps, matrixII, ref matrixResult);
                if (!ok) { return false; }
                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool LDivision(double[,] matrixI, double[] matrixII, ref double[] matrixResult)
        {
            try
            {
                bool ok = false;
                if (matrixI == null || matrixII == null)
                { return false; }
                int rows = matrixI.GetLength(0);
                int cols = matrixI.GetLength(1);
                int len = matrixII.Length;
                if (rows != cols || (rows == 0 && cols == rows) || len == 0)
                { return false; }
                double[,] tmps = new double[rows, cols];
                ok = Common.Init2Dimensions(ref tmps, matrixI);
                if (!ok)
                { return false; }
                ok = Inv(ref tmps);
                if (!ok)
                { return false; }
                ok = Multiply(tmps, matrixII, ref matrixResult);
                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool LDivision(double[,] matrixI, double value, ref double[,] matrixResult)
        {
            try
            {
                bool ok = false;
                if (matrixI == null)
                { return false; }
                int rows = matrixI.GetLength(0);
                int cols = matrixI.GetLength(1);
                if ((rows != cols) || (rows == 0 && rows == cols))
                { return false; }
                double[,] tmps = new double[rows, cols];
                ok = Common.Init2Dimensions(ref tmps, matrixI);
                if (!ok) { return false; }
                ok = Inv(ref tmps);
                if (!ok) { return false; }
                ok = Multiply(tmps, value, ref matrixResult);
                if (!ok) { return false; }
                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool DotRDivision(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);
                int colsI = matrixI.GetLength(1);
                int rowsII = matrixII.GetLength(0);
                int colsII = matrixII.GetLength(1);
                if ((rowsI != rowsII) || (colsI != colsII) || (rowsI == 0 && rowsI == colsI) || (rowsII == 0 && rowsII == colsII))
                { return false; }
                matrixResult = new double[rowsI, colsI];
                for (int i = 0; i < rowsI; i++)
                {
                    for (int j = 0; j < colsI; j++)
                    {
                        matrixResult[i, j] = matrixI[i, j] / matrixII[i, j];
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }
        public static bool DotRDivision(double[,] matrixI, double constant, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int rows = matrixI.GetLength(0);
                int cols = matrixI.GetLength(1);
                if (rows == 0 && rows == cols)
                { return false; }
                matrixResult = new double[rows, cols];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        matrixResult[i, j] = matrixI[i, j] / constant;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool DotRDivision(double[] matrixI, double constant, ref double[] matrixResult)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int len = matrixI.Length;
                if (len == 0)
                { return false; }
                matrixResult = new double[len];
                for (int i = 0; i < len; i++)
                {
                    matrixResult[i] = matrixI[i] / constant;
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool DotRDivision(double[] vectorA, double[] vectorB, ref double[] matrixResult)
        {
            try
            {
                if (vectorA == null || vectorB == null)
                { return false; }
                int lenA = vectorA.Length;
                int lenB = vectorB.Length;
                if ((lenA != lenB) || (lenA == 0 || lenB == 0))
                { return false; }
                matrixResult = new double[lenA];
                for (int i = 0; i < lenA; i++)
                {
                    matrixResult[i] = vectorA[i] / vectorB[i];
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool DotLDivision(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);
                int colsI = matrixI.GetLength(1);
                int rowsII = matrixII.GetLength(0);
                int colsII = matrixII.GetLength(1);
                if ((rowsI != rowsII) || (colsI != colsII) || rowsI == 0 || rowsII == 0)
                { return false; }
                matrixResult = new double[rowsI, colsI];
                for (int i = 0; i < rowsI; i++)
                {
                    for (int j = 0; j < colsI; j++)
                    {
                        matrixResult[i, j] = matrixII[i, j] / matrixI[i, j];
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }


        public static bool Inv(double[] originMatrix, ref double[] invMatrix, int n)
        {
            try
            {
                if (originMatrix == null || originMatrix.Length == 0)
                { return false; }
                invMatrix = new double[originMatrix.Length];
                originMatrix.CopyTo(invMatrix, 0);
                int[] iis = new int[n];
                int[] js = new int[n];
                int l = -1;
                int u = -1;
                int v = -1;
                double d = 0.0;
                double p;
                for (int k = 0; k < n; k++)
                {
                    d = 0.0;
                    for (int i = k; i < n; i++)
                    {
                        for (int j = k; j < n; j++)
                        {
                            l = i * n + j;
                            p = Math.Abs(invMatrix[l]);
                            if (p > d)
                            {
                                d = p;
                                iis[k] = i;
                                js[k] = j;
                            }
                        }
                    } if (d + 1.0 == 1.0)
                    {
                        iis = new int[0];
                        js = new int[0];
                        return false;
                    }
                    if (iis[k] != k)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            u = k * n + j;
                            v = iis[k] * n + j;
                            p = invMatrix[u];
                            invMatrix[u] = invMatrix[v];
                            invMatrix[v] = p;
                        }
                    }
                    if (js[k] != k)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            u = i * n + k;
                            v = i * n + js[k];
                            p = invMatrix[u];
                            invMatrix[u] = invMatrix[v];
                            invMatrix[v] = p;
                        }
                    }
                    l = k * n + k;
                    invMatrix[l] = 1.0 / invMatrix[l];
                    for (int j = 0; j < n; j++)
                    {
                        if (j != k)
                        {
                            u = k * n + j;
                            invMatrix[u] = invMatrix[u] * invMatrix[l];
                        }
                    }
                    for (int i = 0; i < n; i++)
                    {
                        if (i != k)
                        {
                            for (int j = 0; j < n; j++)
                            {
                                if (j != k)
                                {
                                    u = i * n + j;
                                    invMatrix[u] = invMatrix[u] - invMatrix[i * n + k] * invMatrix[k * n + j];
                                }
                            }
                        }
                    }
                    for (int i = 0; i < n; i++)
                    {
                        if (i != k)
                        {
                            u = i * n + k;
                            invMatrix[u] = -invMatrix[u] * invMatrix[l];
                        }
                    }
                } for (int k = n - 1; k >= 0; k--)
                {
                    if (js[k] != k)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            u = k * n + j;
                            v = js[k] * n + j;
                            p = invMatrix[u];
                            invMatrix[u] = invMatrix[v];
                            invMatrix[v] = p;
                        }
                    }
                    if (iis[k] != k)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            u = i * n + k;
                            v = i * n + iis[k];
                            p = invMatrix[u];
                            invMatrix[u] = invMatrix[v];
                            invMatrix[v] = p;
                        }
                    }
                }
                iis = new int[0];
                js = new int[0];
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Inv(double[,] originMatrix, ref double[,] invMatrix)
        {
            try
            {
                bool ok = false;
                int n = 0;
                if (originMatrix == null)
                { return false; }
                int rowCount = originMatrix.GetLength(0);
                int colCount = originMatrix.GetLength(1);
                if (rowCount != colCount || rowCount == 0)
                { return false; }
                n = rowCount;
                invMatrix = new double[n, n];
                ok = Common.Init2Dimensions(ref invMatrix, originMatrix);
                if (!ok)
                { return ok; }
                ok = Inv(ref invMatrix);
                if (!ok)
                { return ok; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Inv(ref double[,] invMatrix)
        {
            try
            {
                int n = 0;
                if (invMatrix == null)
                { return false; }
                int rowCount = invMatrix.GetLength(0);
                int colCount = invMatrix.GetLength(1);
                if (rowCount != colCount || rowCount == 0)
                { return false; }
                n = rowCount;
                int[] rows = new int[n];
                int[] cols = new int[n];
                double d;
                double p;
                for (int k = 0; k < n; k++)
                {
                    d = 0.0;
                    for (int i = k; i < n; i++)
                    {
                        for (int j = k; j < n; j++)
                        {
                            p = Math.Abs(invMatrix[i, j]);
                            if (p > d)
                            {
                                d = p;
                                rows[k] = i;
                                cols[k] = j;
                            }
                        }
                    } if (d + 1.0 == 1.0)
                    {
                        rows = new int[0];
                        cols = new int[0];
                        return false;
                    }
                    if (rows[k] != k)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            p = invMatrix[k, j];
                            invMatrix[k, j] = invMatrix[rows[k], j];
                            invMatrix[rows[k], j] = p;
                        }
                    }
                    if (cols[k] != k)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            p = invMatrix[i, k];
                            invMatrix[i, k] = invMatrix[i, cols[k]];
                            invMatrix[i, cols[k]] = p;
                        }
                    }
                    invMatrix[k, k] = 1.0 / invMatrix[k, k];
                    for (int j = 0; j < n; j++)
                    {
                        if (j != k)
                        {
                            invMatrix[k, j] = invMatrix[k, j] * invMatrix[k, k];
                        }
                    }
                    for (int i = 0; i < n; i++)
                    {
                        if (i != k)
                        {
                            for (int j = 0; j < n; j++)
                            {
                                if (j != k)
                                {
                                    invMatrix[i, j] = invMatrix[i, j] - invMatrix[i, k] * invMatrix[k, j];
                                }
                            }
                        }
                    } for (int i = 0; i < n; i++)
                    {
                        if (i != k)
                        {
                            invMatrix[i, k] = -invMatrix[i, k] * invMatrix[k, k];
                        }
                    }
                } for (int k = n - 1; k >= 0; k--)
                {
                    if (cols[k] != k)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            p = invMatrix[k, j];
                            invMatrix[k, j] = invMatrix[cols[k], j];
                            invMatrix[cols[k], j] = p;
                        }
                    }
                    if (rows[k] != k)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            p = invMatrix[i, k];
                            invMatrix[i, k] = invMatrix[i, rows[k]];
                            invMatrix[i, rows[k]] = p;
                        }
                    }
                }
                rows = new int[0];
                cols = new int[0];
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Chol(ref double[] a, int n, ref double det)
        {
            bool ok = false;
            try
            {

                if (a == null)
                { return false; }
                int u = 0;
                int l = 0;
                double d = 0;

                if ((a[0] + 1.0 == 1.0) || (a[0] < 0.0))
                { return false; }
                a[0] = Math.Sqrt(a[0]);
                d = a[0];
                for (int i = 1; i < n; i++)
                {
                    u = i * n;
                    a[u] = a[u] / a[0];
                }
                for (int j = 1; j < n; j++)
                {
                    l = j * n + j;
                    for (int k = 0; k < j; k++)
                    {
                        u = j * n + k;
                        a[l] = a[l] - a[u] * a[u];
                    }
                    if (a[l] + 1.0 == 1.0 || a[l] < 0.00)
                    { return false; }
                    a[l] = Math.Sqrt(a[l]);
                    d = d * a[l];
                    for (int i = j + 1; i < n; i++)
                    {
                        u = i * n + j;
                        for (int k = 0; k < j; k++)
                        {
                            a[u] = a[u] - a[i * n + k] * a[j * n + k];
                        }
                        a[u] = a[u] / a[l];
                    }
                }
                det = d * d;
                for (int i = 0; i < n - 1; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        a[i * n + j] = 0.0;
                    }
                }
                ok = Transpose(ref a, n);
                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Chol(ref double[] a, int n)
        {
            bool ok = false;
            try
            {
                if (a == null)
                { return false; }
                int u = 0;
                int l = 0;
                double d = 0;
                if ((a[0] + 1.0 == 1.0) || (a[0] < 0.0))
                { return false; }
                a[0] = Math.Sqrt(a[0]);
                d = a[0];
                for (int i = 1; i < n; i++)
                {
                    u = i * n;
                    a[u] = a[u] / a[0];
                }
                for (int j = 1; j < n; j++)
                {
                    l = j * n + j;
                    for (int k = 0; k < j; k++)
                    {
                        u = j * n + k;
                        a[l] = a[l] - a[u] * a[u];
                    }
                    if (a[l] + 1.0 == 1.0 || a[l] < 0.00)
                    { return false; }
                    a[l] = Math.Sqrt(a[l]);
                    d = d * a[l];
                    for (int i = j + 1; i < n; i++)
                    {
                        u = i * n + j;
                        for (int k = 0; k < j; k++)
                        {
                            a[u] = a[u] - a[i * n + k] * a[j * n + k];
                        }
                        a[u] = a[u] / a[l];
                    }
                }
                for (int i = 0; i < n - 1; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        a[i * n + j] = 0.0;
                    }
                }
                ok = Transpose(ref a, n);

                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Chol(ref double[,] a)
        {
            bool ok = false;
            try
            {
                if (a == null)
                { return false; }
                double d = 0;
                int n = a.GetLength(0);
                if ((a[0, 0] + 1.0 == 1.0) || (a[0, 0] < 0.0))
                { return false; }
                a[0, 0] = Math.Sqrt(a[0, 0]);
                d = a[0, 0];
                for (int i = 1; i < n; i++)
                {
                    a[i, 0] = a[i, 0] / a[0, 0];
                }
                for (int j = 1; j < n; j++)
                {
                    for (int k = 0; k < j; k++)
                    {
                        a[j, j] = a[j, j] - a[j, k] * a[j, k];
                    }
                    if (a[j, j] + 1.0 == 1.0 || a[j, j] < 0.00)
                    { return false; }
                    a[j, j] = Math.Sqrt(a[j, j]);
                    d = d * a[j, j];
                    for (int i = j + 1; i < n; i++)
                    {
                        for (int k = 0; k < j; k++)
                        { a[i, j] = a[i, j] - a[i, k] * a[j, k]; }
                        a[i, j] = a[i, j] / a[j, j];
                    }
                }
                for (int i = 0; i < n - 1; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    { a[i, j] = 0.0; }
                }
                ok = Transpose(ref a);
                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Transpose(ref double[] a, int n)
        {
            try
            {
                if (a == null)
                { return false; }
                int u;
                int v;
                double d = 0.0;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        u = i * n + j;
                        v = j * n + i;
                        d = a[u];
                        a[u] = a[v];
                        a[v] = d;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Transpose(ref double[,] a)
        {
            try
            {
                if (a == null)
                { return false; }
                int rowCount = a.GetLength(0);
                int colCount = a.GetLength(1);
                if (rowCount == 0 || colCount == 0)
                { return false; }
                double tmp = 0.0;
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        tmp = a[i, j];
                        a[i, j] = a[j, i];
                        a[j, i] = tmp;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Transpose(double[,] a, ref double[,] resultMatrix)
        {
            try
            {
                if (a == null)
                { return false; }
                int rows = a.GetLength(0);
                int cols = a.GetLength(1);
                resultMatrix = new double[cols, rows];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    { resultMatrix[j, i] = a[i, j]; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static double RCond(double[,] R)
        {
            return 10.0;
        }

        public static bool QR(ref double[] a, int m, int n, ref double[] q)
        {
            try
            {
                if (a == null)
                { return false; }
                int l, nn, p;
                double u, alpha, w, t;
                if (m < n)
                { return false; }
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        l = i * m + j;
                        q[l] = 0.0;
                        if (i == j)
                        {
                            q[l] = 1.0;
                        }
                    }
                } nn = n;
                if (m == n)
                {
                    nn = m - 1;
                }
                for (int k = 0; k < nn; k++)
                {
                    u = 0.0;
                    l = k * n + k;
                    for (int i = k; i < m; i++)
                    {
                        w = Math.Abs(a[i * n + k]);
                        if (w > u)
                        { u = w; }
                    }
                    alpha = 0.0;
                    for (int i = k; i < m; i++)
                    {
                        t = a[i * n + k] / u;
                        alpha = alpha + t * t;
                    }
                    if (a[l] > 0.0)
                    { u = -u; }
                    alpha = u * Math.Sqrt(alpha);
                    if (Math.Abs(alpha) + 1.0 == 1.0)
                    { return false; }
                    u = Math.Sqrt(2.0 * alpha * (alpha - a[l]));
                    if ((u + 1.0) != 1.0)
                    {
                        a[l] = (a[l] - alpha) / u;
                        for (int i = k + 1; i < m; i++)
                        {
                            p = i * n + k;
                            a[p] = a[p] / u;
                        }
                        for (int j = 0; j < m; j++)
                        {
                            t = 0.0;
                            for (int jj = k; jj < m; jj++)
                            {
                                t = t + a[jj * n + k] * q[jj * m + j];
                            }
                            for (int i = k; i < m; i++)
                            {
                                p = i * m + j;
                                q[p] = q[p] - 2.0 * t * a[i * n + k];

                            }
                        }
                        for (int j = k + 1; j < n; j++)
                        {
                            t = 0.0;
                            for (int jj = k; jj < m; jj++)
                            {
                                t = t + a[jj * n + k] * a[jj * n + j];
                            }
                            for (int i = k; i < m; i++)
                            {
                                p = i * n + j;
                                a[p] = a[p] - 2.0 * t * a[i * n + k];
                            }
                        }
                        a[l] = alpha;
                        for (int i = k + 1; i < m; i++)
                        {
                            a[i * n + k] = 0.0;
                        }

                    }
                } for (int i = 0; i < m - 1; i++)
                {
                    for (int j = i + 1; j < m; j++)
                    {
                        p = i * m + j;
                        l = j * m + i;
                        t = q[p];
                        q[p] = q[l];
                        q[l] = t;
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool QR(double[,] matrixA, ref double[,] matrixR, ref  double[,] matrixQ)
        {
            try
            {
                bool ok = false;
                int nn;
                double u, alpha, w, t;
                int rowCount = matrixA.GetLength(0);
                int colCount = matrixA.GetLength(1);
                if (rowCount < colCount)
                { return false; }

                matrixR = new double[rowCount, colCount];
                ok = Common.Init2Dimensions(ref matrixR, matrixA);
                if (!ok) { return false; }
                matrixQ = new double[rowCount, rowCount];
                if (!ok) { return false; }
                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = 0; col < rowCount; col++)
                    {
                        matrixQ[row, col] = 0.0;
                        if (row == col)
                        {
                            matrixQ[row, col] = 1.0;
                        }
                    }
                }
                nn = colCount;
                if (rowCount == colCount)
                {
                    nn = rowCount - 1;
                }
                for (int k = 0; k < nn; k++)
                {
                    u = 0.0;
                    for (int i = k; i < rowCount; i++)
                    {
                        w = Math.Abs(matrixR[i, k]);
                        if (w > u)
                        { u = w; }
                    }
                    alpha = 0.0;
                    for (int i = k; i < rowCount; i++)
                    {
                        t = matrixR[i, k] / u;
                        alpha = alpha + t * t;
                    }
                    if (matrixR[k, k] > 0.0)
                    { u = -u; }
                    alpha = u * Math.Sqrt(alpha);
                    if (Math.Abs(alpha) + 1.0 == 1.0)
                    { return false; }
                    u = Math.Sqrt(2.0 * alpha * (alpha - matrixR[k, k]));
                    if ((u + 1.0) != 1.0)
                    {
                        matrixR[k, k] = (matrixR[k, k] - alpha) / u;
                        for (int i = k + 1; i < rowCount; i++)
                        {
                            matrixR[i, k] = matrixR[i, k] / u;
                        }
                        for (int j = 0; j < rowCount; j++)
                        {
                            t = 0.0;
                            for (int jj = k; jj < rowCount; jj++)
                            {
                                t = t + matrixR[jj, k] * matrixQ[jj, j];
                            }
                            for (int i = k; i < rowCount; i++)
                            {
                                matrixQ[i, j] = matrixQ[i, j] - 2.0 * t * matrixR[i, k];
                            }
                        }
                        for (int j = k + 1; j < colCount; j++)
                        {
                            t = 0.0;
                            for (int jj = k; jj < rowCount; jj++)
                            {
                                t = t + matrixR[jj, k] * matrixR[jj, j];
                            }
                            for (int i = k; i < rowCount; i++)
                            {
                                matrixR[i, j] = matrixR[i, j] - 2.0 * t * matrixR[i, k];
                            }
                        }
                        matrixR[k, k] = alpha;
                        for (int i = k + 1; i < rowCount; i++)
                        {
                            matrixR[i, k] = 0.0;
                        }
                    }
                }
                for (int i = 0; i < rowCount - 1; i++)
                {
                    for (int j = i + 1; j < rowCount; j++)
                    {
                        t = matrixQ[i, j];
                        matrixQ[i, j] = matrixQ[j, i];
                        matrixQ[j, i] = t;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool QR(double[,] matrixA, ref  double[,] matrixR, ref  double[,] matrixQ, ref  double[,] matrixE)
        {
            try
            {
                bool ok = false;
                int rowCount = matrixA.GetLength(0);
                int colCount = matrixA.GetLength(1);
                if (rowCount < colCount)
                { return false; }
                matrixE = new double[colCount, colCount];
                ok = MatrixCompute.Eye(ref matrixE);
                if (!ok)
                { return false; }
                ok = QR(matrixA, ref matrixR, ref matrixQ);
                if (!ok)
                { return false; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool QR(double[] A, ref double[] R, ref double[,] Q)
        {
            try
            {
                bool ok = false;
                int rowCount = A.Length;
                int colCount = 1;
                if (rowCount < colCount)
                { return false; }

                R = new double[rowCount];
                Q = new double[rowCount, rowCount];
                double[,] tmpR = new double[rowCount, colCount];
                double[,] tmpA = new double[rowCount, colCount];
                for (int i = 0; i < rowCount; i++)
                {
                    tmpR[i, 0] = A[i];
                    tmpA[i, 0] = A[i];
                }
                ok = QR(tmpA, ref tmpR, ref Q);
                if (!ok)
                { return false; }
                for (int i = 0; i < rowCount; i++)
                {
                    R[i] = tmpR[i, 0];
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool QR(double[] A, ref double[] R, ref double[,] Q, ref double E)
        {
            try
            {
                bool ok = false;
                int rowCount = A.Length;
                int colCount = 1;
                if (rowCount < colCount)
                { return false; }
                E = 1;
                ok = QR(A, ref R, ref Q);
                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }


        public static bool QRdelete(ref double[,] Q, ref double[,] R, int j, int orient)
        {
            try
            {
                if (Q == null || R == null)
                { return false; }
                bool ok = false;
                int mq = Q.GetLength(0);
                int nq = Q.GetLength(1);
                int mr = R.GetLength(0);
                int nr = R.GetLength(1);
                int[] p = new int[2];
                double[,] G = new double[2, 2];
                if (mq != nq)
                { return false; }
                else if (nq != mr)
                { return false; }
                else if (j < 0)
                { return false; }
                int r = 0;
                int c = 0;
                switch (orient)
                {
                    case 0:
                        if (j > nr)
                        { return false; }
                        ok = DelColOrRow(ref R, j, 0);
                        if (!ok) { return ok; }
                        mr = R.GetLength(0);
                        nr = R.GetLength(1);
                        int tmp = -1;
                        if (nr > mr - 1)
                        { tmp = mr - 1; }
                        else
                        { tmp = nr; }
                        for (int k = j - 1; k < tmp; k++)
                        {
                            p[0] = k;
                            p[1] = k + 1;
                            double[] tmpValue = new double[2];
                            double[,] tmps = new double[0, 0];
                            double[,] tmps1 = new double[0, 0];
                            double[,] tmps2 = new double[0, 0];
                            tmpValue[0] = R[k, k];
                            tmpValue[1] = R[k + 1, k];
                            ok = Planerot(ref tmpValue, ref G);
                            if (!ok)
                            { return ok; }
                            R[k, k] = tmpValue[0];
                            R[k + 1, k] = tmpValue[1];
                            if (k < nr)
                            {
                                tmps = new double[2, nr - k];
                                for (int i = k; i < nr; i++)
                                {
                                    tmps[0, i - k] = R[k, i];
                                    tmps[1, i - k] = R[k + 1, i];
                                }
                                ok = MatrixCompute.Multiply(G, tmps, ref tmps1);
                                if (!ok) { return ok; }
                                for (int i = k; i < nr; i++)
                                {
                                    R[k, i] = tmps[0, i - k];
                                    R[k + 1, i] = tmps[1, i - k];
                                }
                            } ok = MatrixCompute.Transpose(ref G); if (!ok) { return ok; }
                            tmps = new double[Q.GetLength(0), 2];
                            for (int i = 0; i < Q.GetLength(0); i++)
                            {
                                tmps[i, 0] = Q[i, k];
                                tmps[i, 1] = Q[i, k + 1];
                            }
                            ok = MatrixCompute.Multiply(tmps, G, ref tmps1);
                            if (!ok)
                            { return ok; }
                            for (int i = 0; i < Q.GetLength(0); i++)
                            {
                                Q[i, k] = tmps[i, 0];
                                Q[i, k + 1] = tmps[i, 1];
                            }
                        }
                        if (mq != nq)
                        {
                            ok = DelColOrRow(ref R, mr, 1);
                            if (!ok) { return ok; }
                            ok = DelColOrRow(ref Q, nq, 0);
                            if (!ok) { return ok; }
                        }
                        break;
                    case 1:
                        if (j > mr)
                        { return false; }
                        if (j != 0)
                        {
                        }
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool QRinsert(ref double[,] Q, ref double[,] R, int j, double[] x, int orient)
        {
            try
            {
                bool ok = false;
                int mx = x.Length;
                int nx = 1;
                int mq = Q.GetLength(0);
                int nq = Q.GetLength(1);
                int mr = R.GetLength(0);
                int nr = R.GetLength(1);
                double[] tmpValue = new double[0];
                double[,] tmps = new double[0, 0];
                double[,] tmps1 = new double[0, 0];
                double[,] tmps2 = new double[0, 0];
                int[] p = new int[0];
                if (orient == 0 && nr == 0)
                {
                    ok = MatrixCompute.QR(x, ref tmpValue, ref Q);
                    if (!ok) { return ok; }
                    R = new double[tmpValue.Length, 1];
                    for (int row = 0; row < tmpValue.Length; row++)
                    {
                        R[row, 0] = tmpValue[row];
                    }
                    return true;
                }
                if (mq != nq)
                { return false; }
                if (nq != mr)
                { return false; }
                if (j < 0)
                { return false; }
                switch (orient)
                {
                    case 0:
                        if (j > nr + 1)
                        { return false; }
                        else if ((mx != mr) || nx != 1)
                        { return false; }
                        tmps = new double[mr, nr];
                        ok = Common.Init2Dimensions(ref tmps, R);
                        if (!ok)
                        { return false; }
                        R = new double[mr, nr + 1];
                        ok = Transpose(Q, ref tmps1);
                        if (!ok)
                        { return false; }
                        ok = Multiply(tmps1, x, ref tmpValue);
                        if (!ok)
                        { return false; }
                        nr = nr + 1;
                        int tmpJ = 0;
                        for (int col = 0; col < nr; col++)
                        {
                            if (col != j)
                            {
                                for (int row = 0; row < mr; row++)
                                { R[row, col] = tmps[row, tmpJ]; }
                                tmpJ++;
                            }
                            else
                            {
                                for (int row = 0; row < mr; row++)
                                { R[row, col] = tmpValue[row]; }
                            }
                        }
                        for (int k = mr - 2; k >= j - 1; k--)
                        {
                            p = new int[2];
                            p[0] = k;
                            p[1] = k + 1;
                            int[] colIdex = new int[1];
                            colIdex[0] = j;
                            tmpValue = new double[2];
                            tmpValue[0] = R[p[0], j];
                            tmpValue[1] = R[p[1], j];
                            double[,] G = new double[0, 0];
                            ok = Planerot(ref tmpValue, ref G);
                            if (!ok)
                            { return false; }
                            if (k < nr)
                            {
                                colIdex = new int[nr - k];
                                for (int i = k; i < nr; i++)
                                { colIdex[i - k] = i; }
                                ok = GetNewMatrix(R, p, colIdex, ref tmps);
                                if (!ok)
                                { return false; }
                                ok = Multiply(G, tmps, ref tmps1);
                                if (!ok)
                                { return false; }
                                for (int row = 0; row < p.Length; row++)
                                {
                                    for (int col = k; col < nr; col++)
                                    { R[p[row], col] = tmps1[row, col - k]; }
                                }
                            }
                            int[] rowIndex = new int[0];
                            colIdex = new int[2];
                            colIdex[0] = k;
                            colIdex[1] = k + 1;
                            ok = GetNewMatrix(Q, rowIndex, colIdex, ref tmps2);
                            if (!ok)
                            { return false; }
                            ok = Transpose(ref G);
                            if (!ok)
                            { return false; }
                            ok = Multiply(tmps2, G, ref tmps);
                            if (!ok)
                            { return false; }
                            for (int row = 0; row < Q.GetLength(0); row++)
                            {
                                for (int col = 0; col < p.Length; col++)
                                { Q[row, p[col]] = tmps[row, col]; }
                            }
                        }
                        break;
                    case 1:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Planerot(ref double[] x, ref double[,] G)
        {
            try
            {
                bool ok = false;
                double r = -1;
                G = new double[2, 2];
                if (x[1] != 0)
                {
                    r = MatrixCompute.Norm(x);
                    G[0, 0] = x[0];
                    G[0, 1] = x[1];
                    G[1, 0] = -x[1];
                    G[1, 1] = x[0];
                    x[0] = r;
                    x[1] = 0;
                }
                else
                {
                    ok = MatrixCompute.Eye(ref G);
                    if (!ok) { return ok; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool DelColOrRow(ref double[,] x, int index, int flag)
        {
            try
            {
                if (x == null)
                { return false; }
                bool ok = false;
                int r = 0;
                int c = 0;
                int rows = x.GetLength(0);
                int cols = x.GetLength(1);
                double[,] tmpX = new double[rows, cols];
                ok = Common.Init2Dimensions(ref tmpX, x);
                if (!ok)
                { return ok; }
                switch (flag)
                {
                    case 0:
                        x = new double[rows, cols - 1];
                        r = 0;
                        for (int row = 0; row < rows; row++)
                        {
                            c = 0;
                            for (int col = 0; col < cols; col++)
                            {
                                if (col != index)
                                {
                                    x[r, c] = tmpX[row, col];
                                    c++;
                                }
                            } r++;
                        } break;
                    case 1:
                        x = new double[rows - 1, cols];
                        for (int col = 0; col < cols; col++)
                        {
                            for (int row = 0; row < rows; row++)
                            {
                                if (row != index)
                                {
                                    x[r, c] = tmpX[row, col];
                                    c++;
                                }
                            } r++;
                        } break;
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool DelColOrRow(ref double[] x, int index)
        {
            try
            {
                if (x == null)
                { return false; }
                bool ok = false;
                double[] tmpX = new double[0];
                x.CopyTo(tmpX, 0);
                if (!ok) { return ok; }
                x = new double[tmpX.Length - 1];
                int count = 0;
                for (int i = 0; i < tmpX.Length; i++)
                {
                    if (i != index)
                    { x[count] = tmpX[i]; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool DelColOrRow(ref int[] x, int index)
        {
            try
            {
                if (x == null)
                { return false; }
                bool ok = false;
                int[] tmpX = new int[0];
                x.CopyTo(tmpX, 0);
                if (!ok) { return ok; }
                x = new int[tmpX.Length - 1];
                int count = 0;
                for (int i = 0; i < tmpX.Length; i++)
                {
                    if (i != index)
                    { x[count] = tmpX[i]; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }


        public static bool GetNewMatrix(double[,] origionMatrix, int[] rowIndexs, int[] colIndexs, ref double[,] newMatrix)
        {
            try
            {
                if (origionMatrix == null)
                { return false; }
                bool ok = false;
                int rows = origionMatrix.GetLength(0);
                int cols = origionMatrix.GetLength(1);
                if (rows == 0 && cols == 0)
                { return false; }
                if (rowIndexs.Length == 0 && colIndexs.Length == 0)
                {
                    newMatrix = new double[rows, cols];
                    ok = Common.Init2Dimensions(ref newMatrix, origionMatrix);
                    if (!ok) { return ok; }
                }
                else if (rowIndexs.Length == 0)
                {
                    cols = colIndexs.Length;
                    newMatrix = new double[rows, cols];
                    for (int col = 0; col < cols; col++)
                    {
                        for (int row = 0; row < rows; row++)
                        { newMatrix[row, col] = origionMatrix[row, colIndexs[col]]; }
                    }
                }
                else if (colIndexs.Length == 0)
                {
                    rows = rowIndexs.Length;
                    newMatrix = new double[rows, cols];
                    for (int row = 0; row < rows; row++)
                    {
                        for (int col = 0; col < cols; col++)
                        { newMatrix[row, col] = origionMatrix[rowIndexs[row], col]; }
                    }
                }
                else
                {
                    rows = rowIndexs.Length;
                    cols = colIndexs.Length;
                    newMatrix = new double[rows, cols];
                    for (int row = 0; row < rows; row++)
                    {
                        for (int col = 0; col < cols; col++)
                        { newMatrix[row, col] = origionMatrix[rowIndexs[row], colIndexs[col]]; }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static double Norm(double[,] matrix, string str)
        {
            double value = -1;
            bool ok = false;
            try
            {
                if (matrix == null)
                { return -1; }
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                if (rows == 0 && cols == rows)
                { return -1; }
                double[] valueTmp = new double[rows];
                double tmp = 0;
                if (str.ToLower() == "inf")
                {
                    for (int row = 0; row < rows; row++)
                    {
                        tmp = 0;
                        for (int col = 0; col < cols; col++)
                        { tmp += Math.Abs(matrix[row, col]); }
                        valueTmp[row] = tmp;
                    }
                    int index = -1;
                    ok = Common.GetArrayMaxMin(valueTmp, true, ref value, ref index);
                    if (!ok)
                    { return -1; }
                }
                else if (str.ToLower() == "fro")
                {
                    tmp = 0;
                    for (int row = 0; row < rows; row++)
                    {
                        for (int col = 0; col < cols; col++)
                        { tmp += Math.Pow(matrix[row, col], 2); }
                    }
                    value = Math.Sqrt(tmp);
                }
                return value;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return value;
            }
        }

        public static double Norm(double[] vector, string str)
        {
            double value = -1;
            bool ok = false;
            try
            {
                if (vector == null)
                { return -1; }
                int len = vector.Length;
                if (len == 0)
                { return -1; }
                double[] valueTmp = new double[len];
                double tmp = 0;
                if (str.ToLower() == "inf")
                {
                    tmp = 0;
                    for (int row = 0; row < len; row++)
                    {
                        tmp = Math.Abs(vector[row]);
                        valueTmp[row] = tmp;
                    }
                    int index = -1;
                    ok = Common.GetArrayMaxMin(valueTmp, true, ref value, ref index);
                    if (!ok)
                    { return -1; }
                }
                else if (str.ToLower() == "fro")
                {
                    tmp = 0;
                    for (int row = 0; row < len; row++)
                    { tmp += Math.Pow(Math.Abs(vector[row]), 2); }
                    value = Math.Sqrt(tmp);
                }
                return value;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return value;
            }
        }

        public static double Norm(double[] vector)
        {
            double value = -1;
            try
            {
                if (vector == null)
                { return -1; }
                int len = vector.Length;
                if (len == 0)
                { return -1; }
                double[] valueTmp = new double[len];
                double tmp = 0;
                for (int row = 0; row < len; row++)
                { tmp += Math.Pow(Math.Abs(vector[row]), 2); }
                value = Math.Sqrt(tmp);
                return value;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return value;
            }
        }


        public static int[] Find(double[] vector)
        {
            if (vector == null)
            { return null; }
            int[] findResult = new int[0];
            List<int> lists = new List<int>();
            try
            {
                int len = vector.Length;
                if (len == 0)
                { return null; }
                for (int i = 0; i < len; i++)
                {
                    if (vector[i] != 0)
                    { lists.Add(i); }
                }
                findResult = new int[lists.Count];
                lists.CopyTo(findResult, 0);
                return findResult;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }

        public static int[] Find(double[] vector, double value)
        {
            int[] findResult = new int[0];
            List<int> lists = new List<int>();
            try
            {
                if (vector == null)
                { return null; }
                int len = vector.Length;
                if (len == 0)
                { return null; }
                for (int i = 0; i < len; i++)
                {
                    if (vector[i] == value)
                    { lists.Add(i); }
                }
                findResult = new int[lists.Count];
                lists.CopyTo(findResult, 0);
                return findResult;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }
        public static int[] Find(int[] vector, int value)
        {
            int[] findResult = new int[0];
            List<int> lists = new List<int>();
            try
            {
                if (vector == null)
                { return null; }
                int len = vector.Length;
                if (len == 0)
                { return null; }
                for (int i = 0; i < len; i++)
                {
                    if (vector[i] == value)
                    { lists.Add(i); }
                }
                findResult = new int[lists.Count];
                lists.CopyTo(findResult, 0);
                return findResult;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }

        public static int[] Find(double[] vector, HandleSign mnu, double[] value)
        {
            int[] findResult = new int[0];
            List<int> lists = new List<int>();
            try
            {
                if (vector == null)
                { return null; }
                int len = value.Length;
                if (len == 0)
                { return findResult; }
                if (len == 1)
                {
                    if (vector.Length == 0)
                    { return findResult; }
                    len = vector.Length;
                    switch (mnu)
                    {
                        case HandleSign.Greater:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] > value[0])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.Equal:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] == value[0])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.GreaterOrEqual:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] >= value[0])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.Less:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] < value[0])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.LessOrEqual:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] <= value[0])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.And:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] > 0 & value[0] > 0)
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.Or:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] > 0 | value[0] > 0)
                                { lists.Add(i); }
                            }
                            break;
                    }
                }
                else if (len > 1)
                {
                    if (len != vector.Length)
                    { return findResult; }
                    switch (mnu)
                    {
                        case HandleSign.Greater:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] > value[i])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.Equal:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] == value[i])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.GreaterOrEqual:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] >= value[i])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.Less:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] < value[i])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.LessOrEqual:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] <= value[i])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.And:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] > 0 & value[i] > 0)
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.Or:
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] > 0 | value[i] > 0)
                                { lists.Add(i); }
                            }
                            break;
                    }
                }
                if (lists.Count != 0)
                {
                    findResult = new int[lists.Count];
                    lists.CopyTo(findResult, 0);
                }
                return findResult;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }
        public static bool Zeros(ref double[] vector, int len)
        {
            try
            {
                vector = new double[len];
                for (int i = 0; i < len; i++)
                { vector[i] = 0; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Zeros(ref double[] vector)
        {
            try
            {
                if (vector == null)
                { return false; }
                int len = vector.Length;
                for (int i = 0; i < len; i++)
                { vector[i] = 0; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Zeros(ref double[,] matrix)
        {
            try
            {
                if (matrix == null)
                { return false; }
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    { matrix[row, col] = 0; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Ones(ref double[] vector)
        {
            try
            {
                if (vector == null)
                { return false; }
                int len = vector.Length;
                for (int i = 0; i < len; i++)
                { vector[i] = 1.0; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }
        public static bool Ones(ref double[,] matrix)
        {
            try
            {
                if (matrix == null)
                { return false; }
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    { matrix[row, col] = 1.0; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Ones(ref int[] vector)
        {
            try
            {
                if (vector == null)
                { return false; }
                int len = vector.Length;
                for (int i = 0; i < len; i++)
                { vector[i] = 1; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Sum(double[] vector, ref double value)
        {
            try
            {
                value = 0;
                if (vector == null)
                { return false; }
                int len = vector.Length;
                for (int i = 0; i < len; i++)
                { value += vector[i]; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Sum(double[,] matrix, ref double value)
        {
            try
            {
                value = 0;
                if (matrix == null)
                { return false; }
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    { value += matrix[i, j]; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Sum(double[,] matrix, int flag, ref double[] values)
        {
            try
            {
                if (matrix == null)
                { return false; }
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                double tmp = 0.0;
                if (flag == 0)
                {
                    values = new double[cols];
                    for (int col = 0; col < cols; col++)
                    {
                        tmp = 0.0;
                        for (int row = 0; row < rows; row++)
                        { tmp += matrix[row, col]; }
                        values[col] = tmp;
                    }
                }
                else if (flag == 1)
                {
                    values = new double[rows];
                    for (int row = 0; row < rows; row++)
                    {
                        tmp = 0.0;
                        for (int col = 0; col < cols; col++)
                        { tmp += matrix[row, col]; }
                        values[row] = tmp;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }


        public static bool Repmat(double A, int M, int N, ref double[,] B)
        {
            try
            {
                B = new double[M, N];
                for (int row = 0; row < M; row++)
                {
                    for (int col = 0; col < N; col++)
                    { B[row, col] = A; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Repmat(double[,] A, int M, int N, ref double[,] B)
        {
            try
            {
                if (A == null)
                { return false; }
                int rowCount = A.GetLength(0);
                int colCount = A.GetLength(1);
                int rows = rowCount * M;
                int cols = colCount * N;
                B = new double[rows, cols];
                for (int row = 0; row < rows; row++)
                {
                    int i = row % rowCount;
                    for (int col = 0; col < cols; col++)
                    {
                        int j = col % colCount;
                        B[row, col] = A[i, j];
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool Any(bool[] x)
        {
            try
            {
                if (x == null)
                { return false; }
                bool ok = false;
                foreach (bool item in x)
                {
                    if (item)
                    { ok = item; }
                }
                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static double[,] Reshape(double[,] x, int rows, int cols)
        {
            double[,] reshpe = new double[rows, cols];
            try
            {
                if (x == null)
                { return null; }
                int rowCount = x.GetLength(0);
                int colCount = x.GetLength(1);
                if (rowCount * colCount != rows * cols)
                { return null; }
                int i = 0;
                int j = 0;
                for (int col = 0; col < cols; col++)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        reshpe[row, col] = x[i, j];
                        i++;
                        if (i == rowCount)
                        {
                            i = 0;
                            j++;
                        }
                    }
                }
                return reshpe;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }

        public static double[,] Reshape(double[] x, int rows, int cols)
        {
            double[,] reshpe = new double[rows, cols];
            try
            {
                if (x == null)
                { return null; }
                int rowCount = x.GetLength(0);
                int colCount = x.GetLength(1);
                if (rowCount * colCount != rows * cols)
                { return null; }
                int i = 0;
                int j = 0;
                for (int col = 0; col < cols; col++)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        reshpe[row, col] = x[i];
                        i++;
                        if (i == rowCount)
                        {
                            i = 0;
                            j++;
                        }
                    }
                }
                return reshpe;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }

        public static double[,] ReviseDatas(double[,] datas)
        {
            try
            {
                double[,] revisedDatas = new double[0, 0];
                if (datas == null) { return null; }
                int rows = datas.GetLength(0);
                int cols = datas.GetLength(1);
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        revisedDatas[row, col] = Math.Round(datas[row, col], 6);
                    }
                }
                return revisedDatas;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }

        public static bool ReviseDatas(ref double[,] datas)
        {
            try
            {
                double[,] revisedDatas = new double[0, 0];
                if (datas == null) { return false; }
                int rows = datas.GetLength(0);
                int cols = datas.GetLength(1);
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        datas[row, col] = Math.Round(revisedDatas[row, col], 6);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static double[,] ReviseDatas(double[,] datas, int decimals)
        {
            try
            {
                double[,] revisedDatas = new double[0, 0];
                if (datas == null) { return null; }
                int rows = datas.GetLength(0);
                int cols = datas.GetLength(1);
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        revisedDatas[row, col] = Math.Round(datas[row, col], decimals);
                    }
                }
                return revisedDatas;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }

        public static bool ReviseDatas(ref double[,] datas, int decimals)
        {
            try
            {
                double[,] revisedDatas = new double[0, 0];
                if (datas == null) { return false; ; }
                int rows = datas.GetLength(0);
                int cols = datas.GetLength(1);
                revisedDatas = new double[rows, cols];
                bool ok = Common.Init2Dimensions(ref revisedDatas, datas);
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        datas[row, col] = Math.Round(revisedDatas[row, col], decimals);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool JudgePositiveDefinite(double[,] matrix)
        {
            try
            {
                double erroFlag = 1e-8;
                double tmp = 0;
                if (matrix == null) { return false; }
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                if (rows != cols) { return false; }
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        tmp = matrix[row, col] - matrix[col, row];
                        if (tmp > erroFlag) { return false; }
                    }
                }
                tmp = Det(matrix);
                if (tmp < 0) { return false; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        public static bool JudgePositiveDefinite(double[,] matrix, double erroFlag)
        {
            try
            {
                double tmp = 0;
                if (matrix == null) { return false; }
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                if (rows != cols) { return false; }
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        tmp = matrix[row, col] - matrix[col, row];
                        if (tmp > erroFlag) { return false; }
                    }
                }
                tmp = Det(matrix);
                if (tmp < 0) { return false; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }
    }
}
