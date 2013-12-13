using System;
using System.Collections.Generic;
using System.Text;

namespace ESIL.Kriging
{
    public enum HandleSign
    {
        /// <summary>
        /// > 大于
        /// </summary>
        Greater,
        /// <summary>
        /// 大于等于
        /// </summary>
        GreaterOrEqual,
        /// <summary>
        /// 小于
        /// </summary>
        Less,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessOrEqual,
        /// <summary>
        /// 等于==
        /// </summary>
        Equal,
        /// <summary>
        /// 逻辑与
        /// </summary>
        And,
        /// <summary>
        /// 逻辑或
        /// </summary>
        Or
    }
    public static class MatrixCompute
    {
        #region Matlab eye 生成单位矩阵
        /// <summary>
        /// 生成单位矩阵：  1 0 0
        ///                 0 1 0
        ///                 0 0 1  
        /// </summary>
        /// <param name="x">存储单位矩阵的数组</param>
        /// <returns>执行成功返回true，否则false</returns>
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
        #endregion

        #region Matlab det函数 行列式求值
        /// <summary>
        /// matlab中的det函数 求行列式的值
        /// </summary>
        /// <param name="detArray">n阶行列式</param>
        /// <param name="n">行列式的阶数</param>
        /// <returns>执行成功返回det值，否则返回1e-10</returns>
        public static double Det(double[,] detArray)
        {
            try
            {// 行列式的初始值必须设为1，否则在计算过程中永远为0
                if (detArray == null)
                { return 1e-10; }
                double f = 1.0;
                double det = 1.0;
                double q = 0.0;
                double d;
                int iis = 0;
                int js = 0;
                // 0->d对应二维数组的行；1->对应二维数组的列
                int rowCount = detArray.GetLength(0);
                int colCount = detArray.GetLength(1);
                if (rowCount != colCount || rowCount == 0)
                { return det = 1e-10; }
                int n = rowCount;
                // k作为行索引
                for (int k = 0; k <= n - 2; k++)
                { //循环行 i是行索引
                    q = 0.0;
                    for (int i = k; i <= n - 1; i++)
                    {// 循环列 j列索引
                        for (int j = k; j <= n - 1; j++)
                        { // l = i * n + j;
                            d = Math.Abs(detArray[i, j]);
                            if (d > q)
                            {
                                q = d;
                                iis = i;
                                js = j;
                            }
                        }//for_j
                    }//for_i
                    if (q + 1.0 == 1.0)
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
                    }//if_iis
                    if (js != k)
                    {
                        f = -f;
                        for (int i = k; i <= n - 1; i++)
                        {
                            d = detArray[i, js];
                            detArray[i, js] = detArray[i, k];
                            detArray[i, k] = d;
                        }
                    }//if_js
                    det = det * detArray[k, k];
                    for (int i = k + 1; i <= n - 1; i++)
                    {
                        d = detArray[i, k] / detArray[k, k];
                        for (int j = k + 1; j <= n - 1; j++)
                        {
                            detArray[i, j] = detArray[i, j] - d * detArray[k, j];
                        }//for_j
                    }//for_i
                }//for_k
                det = f * det * detArray[n - 1, n - 1];
                return det;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 1e-10;
            }
        }

        /// <summary>
        /// matlab中的det函数:求行列式的值
        /// </summary>
        /// <param name="detArr">n介行列式用一个一维数组存储</param>
        /// <param name="n">行列式的阶数</param>
        /// <returns>执行成功返回det值，否则返回1e-10</returns>
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
                        }//for_j
                    }//for_i
                    if (q + 1.0 == 1.0)
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
                    }//if_iis
                    if (js != k)
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
                    }//if_js
                    l = k * n + k;
                    det = det * a[l];
                    for (int i = k + 1; i <= n - 1; i++)
                    {
                        d = a[i * n + k] / a[l];
                        for (int j = k + 1; j <= n - 1; j++)
                        {
                            u = i * n + j;
                            a[u] = a[u] - d * a[k * n + j];
                        }//for_j
                    }//for_i
                }//for_k
                det = f * det * a[n * n - 1];
                return det;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 1e-10;
            }
        }
        #endregion

        #region 矩阵相加
        /// <summary>
        /// 两个矩阵相加，必须是同型矩阵
        /// </summary>
        /// <param name="matrixI">存放矩阵1的一维数组</param>
        /// <param name="matrixII">存放矩阵2的一维数组</param>
        /// <param name="resultMatrix">结果矩阵，用一维数组存放</param>
        /// <returns>执行成功返回true值，否则返回false</returns>
        public static bool Add(double[] matrixI, double[] matrixII, ref double[] resultMatrix)
        {
            try
            {// 两个矩阵加运算时，这两个矩阵必须是同型矩阵
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

        /// <summary>
        /// 重载1 两个矩阵相加，必须是同型矩阵
        /// </summary>
        /// <param name="matrixI">存放矩阵1的二维数组</param>
        /// <param name="matrixII">存放矩阵2的二维数组</param>
        /// <param name="resultMatrix">结果矩阵，用二维数组存放</param>
        /// <returns>执行成功返回true值，否则返回false</returns>
        public static bool Add(double[,] matrixI, double[,] matrixII, ref double[,] resultMatrix)
        {
            try
            {// 两个矩阵加运算时，这两个矩阵必须是同型矩阵
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);// 0->矩阵的行数
                int colsI = matrixI.GetLength(1);// 1->矩阵的列数
                int rowsII = matrixII.GetLength(0);
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

        /// <summary>
        /// 重载2 矩阵加一个常数
        /// </summary>
        /// <param name="matrixI">存放矩阵1的二维数组</param>
        /// <param name="constant">常数</param>
        /// <param name="resultMatrix">结果矩阵，用二维数组存放</param>
        /// <returns>执行成功返回true值，否则返回false</returns>
        public static bool Add(double[,] matrixI, double constant, ref double[,] resultMatrix)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);// 0->矩阵的行数
                int colsI = matrixI.GetLength(1);// 1->矩阵的列数
                if (rowsI == 0 && colsI == rowsI)
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

        /// <summary>
        /// 重载3 向量加一个常数
        /// </summary>
        /// <param name="matrixI">存放矩阵1的二维数组</param>
        /// <param name="constant">常数</param>
        /// <param name="resultMatrix">结果矩阵，用二维数组存放</param>
        /// <returns>执行成功返回true值，否则返回false</returns>
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
        #endregion

        #region 矩阵相减
        /// <summary>
        /// 两个矩阵相减，必须是同型矩阵
        /// </summary>
        /// <param name="matrixI">存放矩阵1的一维数组</param>
        /// <param name="matrixII">存放矩阵2的一维数组</param>
        /// <param name="resultMatrix">结果矩阵，用一维数组存放</param>
        /// <returns>执行成功返回true值，否则返回false</returns>
        public static bool Subtract(double[] matrixI, double[] matrixII, ref double[] resultMatrix)
        {
            try
            {// 两个矩阵加运算时，这两个矩阵必须是同型矩阵
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

        /// <summary>
        /// 重载1 两个矩阵相减，必须是同型矩阵
        /// </summary>
        /// <param name="matrixI">存放矩阵1的二维数组</param>
        /// <param name="matrixII">存放矩阵2的二维数组</param>
        /// <param name="resultMatrix">结果矩阵，用二维数组存放</param>
        /// <returns>执行成功返回true值，否则返回false</returns>
        public static bool Subtract(double[,] matrixI, double[,] matrixII, ref double[,] resultMatrix)
        {
            try
            {// 两个矩阵加运算时，这两个矩阵必须是同型矩阵
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);// 0->矩阵的行数
                int colsI = matrixI.GetLength(1);// 1->矩阵的列数
                int rowsII = matrixII.GetLength(0);
                int colsII = matrixII.GetLength(1);
                // 判断传入相减的参数是否都有值，全部计算参数有值才计算，否则返回false
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

        /// <summary>
        /// 重载2 矩阵减一个常数
        /// </summary>
        /// <param name="matrixI">存放矩阵1的二维数组</param>
        /// <param name="constant">常数</param>
        /// <param name="resultMatrix">结果矩阵，用二维数组存放</param>
        /// <returns>执行成功返回true值，否则返回false</returns>
        public static bool Subtract(double[,] matrixI, double constant, ref double[,] resultMatrix)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);// 0->矩阵的行数
                int colsI = matrixI.GetLength(1);// 1->矩阵的列数
                if (rowsI == colsI && rowsI == 0)
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
        #endregion

        #region Matlab 矩阵相乘
        #region Multiply
        /// <summary>
        /// 矩阵叉乘 两个二维数组存储的矩阵:m*n乘以n*p=m*p
        /// </summary>
        /// <param name="matrixI">乘数矩阵1:m*n</param>
        /// <param name="matrixII">乘数矩阵2:n*p</param>
        /// <param name="matrixResult">两个矩阵相乘后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool Multiply(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {// 矩阵(matrixI:m*n) *(matrixII:p*k)->m*k新矩阵
            // 矩阵相乘的条件矩阵1的列数等于矩阵二的行数，如果不等，那么矩阵不能相乘 返回false
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);
                int colsI = matrixI.GetLength(1);// 列数
                int rowsII = matrixII.GetLength(0);//行数
                int colsII = matrixII.GetLength(1);
                if ((colsI != rowsII) || (rowsI == 0 && rowsI == colsI) || (rowsII == 0 && rowsII == colsII))
                { return false; }
                matrixResult = new double[rowsI, colsII];
                //循环matrixI的每一列和对应的matrixII的每一行对应相乘得到新的matrixResult
                for (int rowI = 0; rowI < rowsI; rowI++)
                {//循环matrixI的行
                    for (int colII = 0; colII < colsII; colII++)
                    {// 循环matrixII的列
                        for (int colI = 0; colI < colsI; colI++)
                        {// matrixI的列数==matrixII的行数
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

        /// <summary>
        /// 重载1 矩阵叉乘：矩阵m*n乘以列向量n*1=m*1
        /// </summary>
        /// <param name="matrixI">乘数矩阵1:m*n</param>
        /// <param name="matrixII">乘数矩阵2:列向量n*1</param>
        /// <param name="matrixResult">两个矩阵相乘后的结果矩阵:m*1的列向量</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool Multiply(double[,] matrixI, double[] matrixII, ref double[] matrixResult)
        {// 矩阵(matrixI:m*n) *(matrixII:p*1)->m*1新矩阵
            // 矩阵相乘的条件矩阵1的列数等于矩阵二的行数，如果不等，那么矩阵不能相乘 返回false
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);
                int colsI = matrixI.GetLength(1);// 列数
                int lensII = matrixII.Length;
                if ((colsI != lensII) || (rowsI == 0 && rowsI == colsI) || (lensII == 0))
                { return false; }
                matrixResult = new double[rowsI];
                //循环matrixI的每一列和对应的matrixII的每一行对应相乘得到新的matrixResult
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

        /// <summary>
        /// 重载2 矩阵叉乘:列向量m*1乘以行向量1*p=m*p
        /// </summary>
        /// <param name="matrixI">乘数矩阵1:列向量m*1</param>
        /// <param name="matrixII">乘数矩阵2:行向量1*p</param>
        /// <param name="matrixResult">两个矩阵相乘后的结果矩阵:m*p</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool Multiply(double[] matrixI, double[] matrixII, ref double[,] matrixResult)
        { // 矩阵(matrixI:m*1) *(matrixII:1*k)->m*k新矩阵
            // 矩阵相乘的条件矩阵1的列数等于矩阵二的行数，如果不等，那么矩阵不能相乘 返回false
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int cols = matrixI.Length;// 列数
                int rows = matrixII.Length;//行数
                if (cols == 0 && rows == cols)
                { return false; }
                matrixResult = new double[rows, cols];
                //循环matrixI的每一列和对应的matrixII的每一行对应相乘得到新的matrixResult
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

        /// <summary>
        /// 重载3 矩阵叉乘:行向量 1*m乘以列向量m*1=1*1
        /// </summary>
        /// <param name="matrixI">乘数矩阵1:行向量1*m</param>
        /// <param name="matrixII">乘数矩阵2:行向量m*1</param>
        /// <param name="matrixResult">两个矩阵相乘后的结果:</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool Multiply(double[] matrixI, double[] matrixII, ref double matrixResult)
        {// 矩阵(matrixI:m*n) *(matrixII:p*k)->m*k新矩阵
            // 矩阵相乘的条件矩阵1的列数等于矩阵二的行数，如果不等，那么矩阵不能相乘
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int lensI = matrixI.Length;// 列数
                int lensII = matrixII.Length;//行数
                if ((lensI != lensII) || (lensII == 0 && lensII == lensI))
                { return false; }
                matrixResult = 0;
                //循环matrixI的每一列和对应的matrixII的每一行对应相乘得到新的matrixResult
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

        /// <summary>
        /// 重载4 矩阵叉乘：行向量 1*m乘以矩阵 m*p=1*p
        /// </summary>
        /// <param name="matrixI">乘数矩阵1:行向量1*m</param>
        /// <param name="matrixII">乘数矩阵2:矩阵m*p</param>
        /// <param name="matrixResult">两个矩阵相乘后的结果矩阵:1*p</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool Multiply(double[] matrixI, double[,] matrixII, ref double[] matrixResult)
        {// 矩阵(matrixI:1*n) *(matrixII:p*k)->n*k新矩阵
            // 矩阵相乘的条件矩阵1的列数等于矩阵二的行数，如果不等，那么矩阵不能相乘
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int lens = matrixI.Length;// 列数
                int rows = matrixII.GetLength(0);//行数
                int cols = matrixII.GetLength(1);// 返回结构矩阵的长度
                if ((lens != rows) || (rows == 0 && rows == cols))
                { return false; }
                matrixResult = new double[cols];
                //循环matrixI的每一列和对应的matrixII的每一行对应相乘得到新的matrixResult
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
        }//method

        /// <summary>
        /// 重载5 矩阵叉乘:行向量1*m乘以常数
        /// </summary>
        /// <param name="matrixI">乘数矩阵1:行向量1*m</param>
        /// <param name="constant">常数</param>
        /// <param name="matrixResult">两个矩阵相乘后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool Multiply(double[] matrixI, double constant, ref double[] matrixResult)
        {// 矩阵相乘的条件矩阵1的列数等于矩阵二的行数，如果不等，那么矩阵不能相乘
            try
            {
                if (matrixI == null)
                { return false; }
                int length = matrixI.Length;
                if (length == 0)
                { return false; }
                matrixResult = new double[length];
                //循环matrixI的每一列和对应的matrixII的每一行对应相乘得到新的matrixResult
                for (int i = 0; i < length; i++)
                { matrixResult[i] = matrixI[i] * constant; }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }// method

        /// <summary>
        /// 重载6 矩阵叉乘矩阵m*n乘以常数
        /// </summary>
        /// <param name="matrixI">乘数矩阵1:m*n</param>
        /// <param name="constant">乘数矩阵2:常数</param>
        /// <param name="matrixResult">两个矩阵相乘后的结果矩阵:m*1的列向量</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool Multiply(double[,] matrixI, double constant, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int colsI = matrixI.GetLength(1);// 列数
                int rowsI = matrixI.GetLength(0);
                if (rowsI == 0 && rowsI == colsI)
                { return false; }
                matrixResult = new double[rowsI, colsI];
                //循环matrixI的每一列和对应的matrixII的每一行对应相乘得到新的matrixResult
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
        }// method
        #endregion

        #region DotMultiply
        /// <summary>
        /// 矩阵点乘
        /// </summary>
        /// <param name="matrixI">乘数矩阵1</param>
        /// <param name="matrixII">乘数矩阵2</param>
        /// <param name="matrixResult">两个矩阵相乘后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool DotMultiply(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int colsI = matrixI.GetLength(1);// 列数
                int rowsI = matrixI.GetLength(0);
                int colsII = matrixII.GetLength(1);
                int rowsII = matrixII.GetLength(0);//行数
                if ((rowsI != rowsII) || (colsI != colsII) || (rowsI == 0 && rowsI == rowsII))
                { return false; }
                matrixResult = new double[rowsI, colsI];
                //循环matrixI的每一列和对应的matrixII的每一行对应相乘得到新的matrixResult
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

        /// <summary>
        /// 重载1 向量点乘
        /// </summary>
        /// <param name="matrixI">乘数矩阵1</param>
        /// <param name="matrixII">乘数矩阵2</param>
        /// <param name="matrixResult">两个矩阵相乘后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
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

        /// <summary>
        /// 重载2 点乘
        /// </summary>
        /// <param name="matrixI">乘数矩阵1</param>
        /// <param name="matrixII">乘数矩阵2</param>
        /// <param name="matrixResult">两个矩阵相乘后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
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
        }// method

        /// <summary>
        /// 矩阵点乘
        /// </summary>
        /// <param name="matrixI">乘数矩阵1</param>
        /// <param name="matrixII">乘数矩阵2</param>
        /// <param name="matrixResult">两个矩阵相乘后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool DotMultiply(double[,] matrixI, double constant, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int colsI = matrixI.GetLength(1);// 列数
                int rowsI = matrixI.GetLength(0);
                if (rowsI == 0 && rowsI == colsI)
                { return false; }
                matrixResult = new double[rowsI, colsI];
                //循环matrixI的每一列和对应的matrixII的每一行对应相乘得到新的matrixResult
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
        }// method

        /// <summary>
        /// 重载 向量点乘
        /// </summary>
        /// <param name="matrixI">乘数矩阵1</param>
        /// <param name="matrixII">乘数矩阵2</param>
        /// <param name="matrixResult">两个矩阵相乘后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
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
        }// method
        #endregion
        #endregion

        #region 矩阵相除
        #region RDivision
        /// <summary>
        /// 矩阵右除 A/B 等同于 A*Inv(B)
        /// </summary>
        /// <param name="matrixI">矩阵1:m*n</param>
        /// <param name="matrixII">矩阵2:n*n</param>
        /// <param name="matrixResult">两个矩阵相除后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool RDivision(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {// 计算的一个理念：尽可能的不改变传入的计算参数的只，计算结果在赋值前，先初始化
            try
            {// 在右除过程中矩阵，matrixII必须是方正n*n；matrixI如果是m*n
                bool ok = false;
                if (matrixI == null || matrixII == null)
                { return false; }
                // 数组作为参数传递的过程中是地址传递，所以当在改变参数值的情况是，将参数值传递给临时变量
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

        /// <summary>
        /// 重载1 矩阵右除 A/B 等同于 A*Inv(B)
        /// </summary>
        /// <param name="matrixI">矩阵1</param>
        /// <param name="matrixII">矩阵2</param>
        /// <param name="matrixResult">两个矩阵相除后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool RDivision(double[] matrixI, double[,] matrixII, ref double[] matrixResult)
        {
            try
            {// 在右除过程中矩阵，matrixII必须是方正n*n；matrixI如果是m*n
                bool ok = false;
                if (matrixI == null || matrixII == null)
                { return false; }
                int len = matrixI.Length;
                // 数组作为参数传递的过程中是地址传递，所以当在改变参数值的情况是，将参数值传递给临时变量
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

        /// <summary>
        /// 重载2 矩阵右除常数 A/constant 等同于点除
        /// </summary>
        /// <param name="matrixI">矩阵1</param>
        /// <param name="constant">常数</param>
        /// <param name="matrixResult">两个矩阵相除后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool RDivision(double[,] matrixI, double constant, ref double[,] matrixResult)
        {
            try
            {// 在右除过程中矩阵，matrixII必须是方正n*n；matrixI如果是m*n
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
                        {matrixResult[row, col] = matrixI[row, col] / constant;}
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

        /// <summary>
        /// 重载3 矩阵右除常数 A/constant 
        /// </summary>
        /// <param name="matrixI">矩阵1</param>
        /// <param name="constant">常数</param>
        /// <param name="matrixResult">两个矩阵相除后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool RDivision(double[] matrixI, double constant, ref double[] matrixResult)
        {
            try
            {// 在右除过程中矩阵，matrixII必须是方正n*n；matrixI如果是m*n
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
        #endregion

        #region LDivision
        /// <summary>
        /// 矩阵左除 A\B 等同于 Inv(A)*B
        /// </summary>
        /// <param name="matrixI">矩阵1</param>
        /// <param name="matrixII">矩阵2</param>
        /// <param name="matrixResult">两个矩阵相除后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool LDivision(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {// 在左除过程中矩阵，matrixI必须是方正n*n；matrixI如果是N*M;
                bool ok = false;
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);
                int colsI = matrixI.GetLength(1);

                int rowsII = matrixII.GetLength(0);
                int colsII = matrixII.GetLength(1);
                if ((rowsI != colsI) || (rowsI == 0 && rowsI == colsI) || (rowsII == 0 && rowsII == colsII))
                { return false; }
                // 数组作为参数传递的过程中是地址传递，所以当在改变参数值的情况是，将参数值传递给临时变量
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

        /// <summary>
        /// 重载1 矩阵左除 A\B 等同于 Inv(A)*B
        /// </summary>
        /// <param name="matrixI">矩阵1</param>
        /// <param name="matrixII">矩阵2</param>
        /// <param name="matrixResult">两个矩阵相除后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool LDivision(double[,] matrixI, double[] matrixII, ref double[] matrixResult)
        {
            try
            {// 在右除过程中矩阵，matrixI必须是方正n*n；matrixI如果是N*M;
                bool ok = false;
                if (matrixI == null || matrixII == null)
                { return false; }
                // 数组作为参数传递的过程中是地址传递，所以当在改变参数值的情况是，将参数值传递给临时变量
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

        /// <summary>
        /// 重载2 矩阵左除 A\B 等同于 Inv(A)*B
        /// </summary>
        /// <param name="matrixI">矩阵1</param>
        /// <param name="matrixII">单值</param>
        /// <param name="matrixResult">两个矩阵相除后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool LDivision(double[,] matrixI, double value, ref double[,] matrixResult)
        {
            try
            {// 在左除过程中矩阵，matrixI必须是方正n*n；matrixI如果是N*M;
                bool ok = false;
                if (matrixI == null)
                { return false; }
                int rows = matrixI.GetLength(0);
                int cols = matrixI.GetLength(1);
                if ((rows != cols) || (rows == 0 && rows == cols))
                { return false; }
                // 数组作为参数传递的过程中是地址传递，所以当在改变参数值的情况是，将参数值传递给临时变量
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
        #endregion

        #region DotRDivision
        /// <summary>
        /// 数组右除 A./B
        /// </summary>
        /// <param name="matrixI">矩阵1</param>
        /// <param name="matrixII">矩阵2</param>
        /// <param name="matrixResult">两个矩阵相除后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool DotRDivision(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {// 点除的条件是两个矩阵的阶数相等属于同型矩阵
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
        }// method

        /// <summary>
        /// 重载1 数组右除 A./B
        /// </summary>
        /// <param name="matrixI">矩阵1 二维数组存放</param>
        /// <param name="constant">常数</param>
        /// <param name="matrixResult">两个矩阵相除后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool DotRDivision(double[,] matrixI, double constant, ref double[,] matrixResult)
        {
            try
            {// 
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

        /// <summary>
        /// 重载2 数组右除 A./B
        /// </summary>
        /// <param name="matrixI">矩阵1 一维数组</param>
        /// <param name="constant">常数</param>
        /// <param name="matrixResult">两个矩阵相除后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool DotRDivision(double[] matrixI, double constant, ref double[] matrixResult)
        {
            try
            {// 
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

        /// <summary>
        /// 重载3 数组右除 A./B
        /// </summary>
        /// <param name="vectorA">向量A 一维数组</param>
        /// <param name="vectorB">向量B</param>
        /// <param name="matrixResult">两向量点除之后的结果</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool DotRDivision(double[] vectorA, double[] vectorB, ref double[] matrixResult)
        {
            try
            {// 
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
        #endregion

        #region DotLDivision
        /// <summary>
        /// 数组左除 A.\B
        /// </summary>
        /// <param name="matrixI">矩阵1</param>
        /// <param name="matrixII">矩阵2</param>
        /// <param name="matrixResult">两个矩阵相除后的结果矩阵</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool DotLDivision(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {// 点除的条件是两个矩阵的阶数相等属于同型矩阵
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
        #endregion
        #endregion

        #region 矩阵求逆
        //高斯—约当法(全选主元)求逆的步骤如下。
        //首先，对于k从0到n一1作如下几步：
        //(1)从第k行、第k列开始的右下角子阵中选取绝对值最大的元素，并记住此元素所
        //在的行号和列号，再通过行交换与列交换将它交换到主元素位置上。这一步称为全选主
        //(2)1／Akk=>Akk
        //(3)Akj*Akk=>Akj,j=0,1,2,...j!=k
        //(4)Aij-Aik=>Aij,i,j=0,1,2,...j!=k i!=k
        //(5)Aik*Akk=>Aik,i,=0,1,2,...i!=k
        //最后，根据在全选主元过程户所记录的行、列交换的信息进行恢复，恢复的原则如下
        // 在全选主元过程中，先交换的行、列后进行恢复；原来的行(列)交换用列(行)交换来恢复。

        /// <summary>
        /// 一般矩阵的求逆运算
        /// </summary>
        /// <param name="originMatrix">需要求逆的矩阵a，双精度实行二维数组，体积n*n</param>
        /// <param name="invMatrix">求逆后的矩阵</param>
        /// <param name="n">矩阵的阶数</param>
        /// <returns>执行成功返回：true；否则：false</returns>
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
                        {// 元素索引行优先
                            l = i * n + j;
                            p = Math.Abs(invMatrix[l]);
                            if (p > d)
                            {
                                d = p;
                                iis[k] = i;
                                js[k] = j;
                            }
                        }//for_j
                    }//for_i
                    if (d + 1.0 == 1.0)
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
                        }//if
                    }// for_i

                    for (int i = 0; i < n; i++)
                    {
                        if (i != k)
                        {
                            u = i * n + k;
                            invMatrix[u] = -invMatrix[u] * invMatrix[l];
                        }
                    }// for_i
                }//for_k
                for (int k = n - 1; k >= 0; k--)
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

        /// <summary>
        /// 重载1 一般矩阵的求逆运算
        /// </summary>
        /// <param name="originMatrix">需要求逆的矩阵a，双精度实行二维数组，体积n*n</param>
        /// <param name="invMatrix">求逆后的矩阵</param>
        /// <returns></returns>
        public static bool Inv(double[,] originMatrix, ref double[,] invMatrix)
        {
            try
            {// 矩阵求逆的必要条件是矩阵必须是方正
                bool ok = false;
                int n = 0;
                if (originMatrix == null)
                { return false; }
                // 获取矩阵的行数和列数
                int rowCount = originMatrix.GetLength(0);
                int colCount = originMatrix.GetLength(1);
                if (rowCount != colCount || rowCount == 0)
                { return false; }
                n = rowCount;
                invMatrix = new double[n, n];
                //a.CopyTo(invMatrix, 0);
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

        /// <summary>
        /// 重载2 一般矩阵的求逆运算
        /// </summary>
        /// <param name="invMatrix">需要求逆的矩阵invMatrix，双精度实行二维数组，体积n*n,求逆矩阵后返回逆矩阵</param>
        /// <returns></returns>
        public static bool Inv(ref double[,] invMatrix)
        {
            try
            {// 矩阵求逆的必要条件是矩阵必须是方正
                int n = 0;
                if (invMatrix == null)
                { return false; }
                // 获取矩阵的行数和列数
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
                        }//for_j
                    }//for_i
                    if (d + 1.0 == 1.0)
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
                        }//if
                    }// for_i
                    for (int i = 0; i < n; i++)
                    {
                        if (i != k)
                        {
                            invMatrix[i, k] = -invMatrix[i, k] * invMatrix[k, k];
                        }
                    }// for_i
                }//for_k
                for (int k = n - 1; k >= 0; k--)
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
        #endregion

        #region 矩阵分解chol 对称正定矩阵的乔里斯基分解
        /// <summary>
        /// Cholesky矩阵分解法[R,p]=chol(a) == a=R'*R
        /// </summary>
        /// <param name="a">存放矩阵的一维数组</param>
        /// <param name="n">正定矩阵的阶数</param>
        /// <param name="det">行列式的值</param>
        /// <returns>执行成功返回true；否则返回false</returns>
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
                // 将下三角矩阵转换为上三角矩阵
                ok = Transpose(ref a, n);
                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// 重载1 Cholesky矩阵分解法[R,p]=chol(a) == a=R'*R
        /// </summary>
        /// <param name="a">存放矩阵的一维数组</param>
        /// <param name="n">正定矩阵的阶数</param>
        /// <returns>执行成功返回true；否则返回false</returns>
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
                // 将下三角矩阵转换为上三角矩阵
                ok = Transpose(ref a, n);

                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// 重载2 Cholesky矩阵分解法 【R,p】=chol(a) === a=R'*R
        /// </summary>
        /// <param name="a">存放矩阵的二维数组</param>
        /// <returns>执行成功返回true==0=p；否则返回false!=0</returns>
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
                {//u = i * n;
                    a[i, 0] = a[i, 0] / a[0, 0];
                }
                for (int j = 1; j < n; j++)
                {//  l = j * n + j;
                    for (int k = 0; k < j; k++)
                    { //u = j * n + k;
                        a[j, j] = a[j, j] - a[j, k] * a[j, k];
                    }
                    if (a[j, j] + 1.0 == 1.0 || a[j, j] < 0.00)
                    { return false; }
                    a[j, j] = Math.Sqrt(a[j, j]);
                    d = d * a[j, j];
                    for (int i = j + 1; i < n; i++)
                    { //u = i * n + j;
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
                // 将下三角矩阵转换为上三角矩阵
                ok = Transpose(ref a);
                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }
        #endregion

        #region 求矩阵的转置矩阵
        /// <summary>
        /// 用一维数组存放n阶矩阵，对矩阵进行转置
        /// </summary>
        /// <param name="a">存放矩阵的一维数组</param>
        /// <param name="n">矩阵的阶数</param>
        /// <returns>执行成功返回true 否则返回：false</returns>
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

        /// <summary>
        /// 重载1 用二维数组存放n阶矩阵，对矩阵进行转置
        /// </summary>
        /// <param name="a">存放矩阵的二维数组</param>
        /// <returns>执行成功返回true 否则返回：false</returns>
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

        /// <summary>
        /// 重载2 用二维数组存放n阶矩阵，对矩阵进行转置
        ///  方法中去掉rows或者cols==0则返回false的代码，原因是ACTSET存在长度为0的时候
        /// </summary>
        /// <param name="a">存放矩阵的二维数组</param>
        /// <param name="resultMatrix">转置后的矩阵</param>
        /// <returns>执行成功返回true 否则返回：false</returns>
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
        #endregion

        #region rcond 矩阵倒条件数估计
        public static double RCond(double[,] R)
        {
            // TODO: 未实现
            return 10.0;
        }
        #endregion

        #region 矩阵QR分解
        /// <summary>
        /// 矩阵的QR分解
        /// </summary>
        /// <param name="a">要进行QR分解的矩阵，执行成功后返回分解后的R</param>
        /// <param name="m">矩阵的行数</param>
        /// <param name="n">矩阵的列数</param>
        /// <param name="q">QR分解后的q</param>
        /// <returns></returns>
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
                    }//for_j
                }//for_i
                nn = n;
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

                    }//if((u + 1.0) != 1.0)
                }// for_k
                for (int i = 0; i < m - 1; i++)
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

        /// <summary>
        /// 重载1 矩阵的QR分解
        /// </summary>
        /// <param name="matrixA">要进行QR分解的矩阵，执行成功后返回分解后的R</param>
        /// <param name="matrixR">QR分解后矩阵R</param>
        /// <param name="matrixQ">QR分解后的Q</param>
        /// <returns></returns>
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
                    }//for_col
                }//for_row

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

        /// <summary>
        /// 重载2 矩阵的QR分解
        /// </summary>
        /// <param name="matrixA">要进行QR分解的矩阵，执行成功后返回分解后的R</param>
        /// <param name="matrixR">QR分解后矩阵R</param>
        /// <param name="matrixQ">QR分解后的Q</param>
        /// <param name="matrixE">单位矩阵的行列等matrixA的列数</param>
        /// <returns></returns>
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

        /// <summary>
        /// 重载3 向量的QR分解
        /// </summary>
        /// <param name="matrixA">要进行QR分解的矩阵，执行成功后返回分解后的R</param>
        /// <param name="matrixR">QR分解后矩阵R</param>
        /// <param name="matrixQ">QR分解后的Q</param>
        /// <returns></returns>
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
                    //Console.WriteLine(R[i]);
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// 重载4 向量的QR分解
        /// </summary>
        /// <param name="matrixA">要进行QR分解的矩阵，执行成功后返回分解后的R</param>
        /// <param name="matrixR">QR分解后矩阵R</param>
        /// <param name="matrixQ">QR分解后的Q</param>
        /// <param name="matrixE">单位矩阵的行列等matrixA的列数</param>
        /// <returns></returns>
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

        #endregion

        #region qrdelete
        /// <summary>
        /// 删除第j列或者第j行后进QR分解等到的新的QR值
        /// Note：删除行的还没实现
        /// </summary>
        /// <param name="Q">删除前矩阵QR分解后的Q，执行函数成功后返回删除行或者列之后的Q</param>
        /// <param name="R">删除前矩阵QR分解后的R，执行函数成功后返回删除行或者列之后的R</param>
        /// <param name="j">要删除矩阵的行或者列的索引（0开始）</param>
        /// <param name="orient">删除标识符：0（删除列）；1（删除行）</param>
        /// <returns>执行成功返回true；否则返回false</returns>
        public static bool QRdelete(ref double[,] Q, ref double[,] R, int j, int orient)
        {
            #region 函数功能说明
            //  QRDELETE Delete a column or row from QR factorization.
            //  [Q1,R1] = QRDELETE(Q,R,J) returns the QR factorization of the matrix A1,
            //  where A1 is A with the column A(:,J) removed and [Q,R] = QR(A) is the QR
            //   factorization of A. Matrices Q and R can also be generated by 
            //   the "economy size" QR factorization [Q,R] = QR(A,0). 
            //
            //   QRDELETE(Q,R,J,'col') is the same as QRDELETE(Q,R,J).
            //
            //   [Q1,R1] = QRDELETE(Q,R,J,'row') returns the QR factorization of the matrix
            //   A1, where A1 is A with the row A(J,:) removed and [Q,R] = QR(A) is the QR
            //   factorization of A.
            //
            //   Example:
            //      A = magic(5);  [Q,R] = qr(A);
            //      j = 3;
            //      [Q1,R1] = qrdelete(Q,R,j,'row');
            //   returns a valid QR factorization, although possibly different from
            //     A2 = A;  A2(j,:) = [];
            //     [Q2,R2] = qr(A2);
            //
            //  Class support for inputs Q,R:
            //     float: double, single
            // See also QR, QRINSERT, PLANEROT.
            #endregion
            try
            {// 本函数暂时只实现了删除列的功能
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
                // r->row;c->col
                int r = 0;
                int c = 0;
                switch (orient)
                {
                    case 0:// 0->col
                        if (j > nr)
                        { return false; }
                        //  Remove the j-th column.  n = number of columns in modified R.
                        ok = DelColOrRow(ref R, j, 0);
                        if (!ok) { return ok; }
                        mr = R.GetLength(0);
                        nr = R.GetLength(1);
                        // R now has nonzeros below the diagonal in columns j through n.
                        //    R = [x | x x x         [x x x x
                        //         0 | x x x          0 * * x
                        //         0 | + x x    G     0 0 * *
                        //         0 | 0 + x   --->   0 0 0 *
                        //         0 | 0 0 +          0 0 0 0
                        //         0 | 0 0 0]         0 0 0 0]
                        // Use Givens rotations to zero the +'s, one at a time, from left to right.
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
                            { // R(p,k+1:n) = G*R(p,k+1:n);
                                tmps = new double[2, nr - k];
                                for (int i = k; i < nr; i++)
                                {// R(p,k+1:n)
                                    tmps[0, i - k] = R[k, i];
                                    tmps[1, i - k] = R[k + 1, i];
                                }
                                ok = MatrixCompute.Multiply(G, tmps, ref tmps1);
                                if (!ok) { return ok; }
                                for (int i = k; i < nr; i++)
                                {//R(p,k+1:n)
                                    R[k, i] = tmps[0, i - k];
                                    R[k + 1, i] = tmps[1, i - k];
                                }
                            }// if
                            // Q(:,p) = Q(:,p)*G';
                            ok = MatrixCompute.Transpose(ref G);//G'
                            if (!ok) { return ok; }
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
                        { // If Q is not square, Q is from economy size QR(A,0). Both Q and R need further adjustments. 
                            ok = DelColOrRow(ref R, mr, 1);
                            if (!ok) { return ok; }
                            ok = DelColOrRow(ref Q, nq, 0);
                            if (!ok) { return ok; }
                        }
                        break;
                    case 1:// 1->row
                        if (j > mr)
                        { return false; }
                        if (j != 0)
                        {
                            //p=
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
        #endregion

        #region QRinsert
        // QRINSERT Insert a column or row into QR factorization(因式分解，因子分解).
        //   [Q1,R1] = QRINSERT(Q,R,J,X) returns the QR factorization of the matrix A1,where A1 is A=Q*R with an extra column, X, inserted before A(:,J). If A has
        //   N columns and J = N+1, then X is inserted after the last column of A.
        //
        //   QRINSERT(Q,R,J,X,'col') is the same as QRINSERT(Q,R,J,X).
        //
        //   [Q1,R1] = QRINSERT(Q,R,J,X,'row') returns the QR factorization of the matrix A1, where A1 is A=Q*R with an extra row, X, inserted before A(J,:).
        //
        //   Example:
        //      A = magic(5);  [Q,R] = qr(A);
        //      j = 3; x = 1:5;
        //      [Q1,R1] = qrinsert(Q,R,j,x,'row');
        //   returns a valid QR factorization, although possibly different from
        //      A2 = [A(1:j-1,:); x; A(j:end,:)];
        //      [Q2,R2] = qr(A2);
        //
        //   Class support for inputs Q,R,X:
        //      float: double, single
        //
        //   See also QR, QRDELETE, PLANEROT.
        /// <summary>
        /// 添加第j列或者第j行后进QR分解等到的新的QR值
        /// Note：添加行的还没实现，本函数只考虑添加一行或者一列的情况
        /// </summary>
        /// <param name="Q">添加前矩阵QR分解后的Q，执行函数成功后返回删除行或者列之后的Q</param>
        /// <param name="R">添加前矩阵QR分解后的R，执行函数成功后返回删除行或者列之后的R</param>
        /// <param name="j">添加除矩阵的行或者列的索引（0开始）</param>
        /// <param name="x">添加的行或者列</param>
        /// <param name="orient">添加标识符：0（列）；1（行）</param>
        /// <returns>执行成功返回true；否则返回false</returns>
        public static bool QRinsert(ref double[,] Q, ref double[,] R, int j, double[] x, int orient)
        {
            try
            { // 本函数暂时只考虑插入一行的状态
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
                {// R矩阵列数为0时
                    ok = MatrixCompute.QR(x, ref tmpValue, ref Q);
                    if (!ok) { return ok; }
                    R = new double[tmpValue.Length, 1];
                    for (int row = 0; row < tmpValue.Length; row++)
                    {
                        R[row, 0] = tmpValue[row];
                    }
                    return true;
                }
                // TODO：由于本函数暂时只考虑行，所以判断R矩阵函数为0时忽略
                if (mq != nq)
                { return false; }
                if (nq != mr)
                { return false; }
                if (j < 0)
                { return false; }
                switch (orient)
                {
                    case 0:// 列
                        if (j > nr + 1)
                        { return false; }
                        else if ((mx != mr) || nx != 1)
                        { return false; }
                        // Make room and insert x before j-th column.
                        tmps = new double[mr, nr];
                        ok = Common.Init2Dimensions(ref tmps, R);
                        if (!ok)
                        { return false; }
                        R = new double[mr, nr + 1];
                        // R(:,j) = Q'*x;
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
                                // 设置这两个临时变量的原因是：不是每次插入列都是从矩阵的最后一列插入
                                tmpJ++;
                            }
                            else
                            {
                                for (int row = 0; row < mr; row++)
                                { R[row, col] = tmpValue[row]; }
                            }//if
                        }
                        // Now R has nonzeros below the diagonal in the j-th column,
                        // and "extra" zeros on the diagonal in later columns.
                        //    R = [x x x x x         [x x x x x
                        //         0 x x x x    G     0 x x x x
                        //         0 0 + x x   --->   0 0 * * *
                        //         0 0 + 0 x          0 0 0 * *
                        //         0 0 + 0 0]         0 0 0 0 *]
                        // Use Givens rotations to zero the +'s, one at a time, from bottom to top.
                        //
                        // Q is treated to (the transpose of) the same rotations.
                        //    Q = [x x x x x    G'   [x x * * *
                        //         x x x x x   --->   x x * * *
                        //         x x x x x          x x * * *
                        //         x x x x x          x x * * *
                        //         x x x x x]         x x * * *]
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
                            {//R(p,k+1:n) = G*R(p,k+1:n);
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
                            // Q(:,p) = Q(:,p)*G';
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
                    case 1:// 行
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
        #endregion

        #region planerot
        //%PLANEROT Givens plane rotation.
        //%   [G,Y] = PLANEROT(X), where X is a 2-component column vector,
        //%   returns a 2-by-2 orthogonal matrix G so that Y = G*X has Y(2) = 0.
        //%
        //%   Class support for input X:
        //%      float: double, single
        //%
        //%   See also QRINSERT, QRDELETE.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">长度为2的列向量</param>
        /// <param name="G">2*2的矩阵</param>
        /// <returns>执行成功返回true，都在false</returns>
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
        #endregion

        #region Delete Row or Col
        /// <summary>
        /// 删除二维数组的 列或行
        /// </summary>
        /// <param name="x">要删除行或者列的数组</param>
        /// <param name="index">要删除的行或者列的索引从0开始</param>
        /// <param name="flag">0->列；1->行</param>
        /// <returns></returns>
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
                    case 0:// 删除列
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
                                }// if
                            }// for_col
                            r++;
                        }// for_row
                        break;
                    case 1:// 删除行
                        x = new double[rows - 1, cols];
                        for (int col = 0; col < cols; col++)
                        {
                            for (int row = 0; row < rows; row++)
                            {
                                if (row != index)
                                {
                                    x[r, c] = tmpX[row, col];
                                    c++;
                                }// if
                            }// for_row
                            r++;
                        }// for_col
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

        /// <summary>
        /// 删除一维数组的第index个元素
        /// </summary>
        /// <param name="x">要删除行或者列的数组</param>
        /// <param name="index">要删除的行或者列的索引从0开始</param>
        /// <returns></returns>
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

        /// <summary>
        /// 删除一维数组的第index个元素
        /// </summary>
        /// <param name="x">要删除行或者列的数组</param>
        /// <param name="index">要删除的行或者列的索引从0开始</param>
        /// <returns></returns>
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
        #endregion

        #region Add Row or Col
        /// <summary>
        /// 在矩阵指定行或者列前增加一行或者一列
        /// </summary>
        /// <param name="Matrix"></param>
        /// <param name="value"></param>
        /// <param name="flag">0->列；1->行</param>
        /// <returns></returns>
        //public static double[,] AddRowOrCols(ref double[,] matrix, int index, double value, int flag)
        //{
        //    bool ok = false;
        //    double[,] tmps = new double[0, 0];
        //    try
        //    {
        //        int rows = matrix.GetLength(0);
        //        int cols = matrix.GetLength(1);
        //        switch (flag)
        //        {
        //            case 0://添加列
        //                tmps = new double[rows, cols + 1];
        //                for (int row = 0; row < rows; row++)
        //                {
        //                    for (int col = 0; col < cols+1; col++)
        //                    {
        //                        if (col == 0) { matrix[row, col] = value; }
        //                        else { tmps[row, col] = matrix[row, col - 1]; }
        //                    }
        //                }
        //                break;
        //            case 1://添加行
        //                tmps = new double[rows+1, cols];
        //                for (int row = 0; row < rows+1; row++)
        //                {
        //                    for (int col = 0; col < cols; col++)
        //                    {
        //                        if (row == 0) { tmps[row, col] = value; }
        //                        else { tmps[row, col] = matrix[row-1, col]; }
        //                    }
        //                }
        //                break;
        //        }
        //        return tmps;
        //    }
        //    catch (Exception ex)
        //    {
        //        Common.LogError(ex);
        //        return null;
        //    }
        //}
        #endregion

        #region 读取矩阵或者向量的N行或者列
        /// <summary>
        /// 获取矩阵的指定列或行得到新矩阵
        /// </summary>
        /// <param name="origionMatrix">原始矩阵</param>
        /// <param name="rowIndexs">要读取的行索引值（从0开始）如果取源数组的所有行rowIndexs.length=0</param>
        /// <param name="colIndexs">要读取的列索引值(从0开始)如果取源数组的所有列colIndexs.length=0</param>
        /// <param name="newMatrix">得到的新矩阵</param>
        /// <returns></returns>
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
                {// 取原数组的所有行和列
                    newMatrix = new double[rows, cols];
                    ok = Common.Init2Dimensions(ref newMatrix, origionMatrix);
                    if (!ok) { return ok; }
                }
                else if (rowIndexs.Length == 0)
                {//取源数组的指定列的所有行
                    cols = colIndexs.Length;
                    newMatrix = new double[rows, cols];
                    for (int col = 0; col < cols; col++)
                    {
                        for (int row = 0; row < rows; row++)
                        { newMatrix[row, col] = origionMatrix[row, colIndexs[col]]; }
                    }
                }
                else if (colIndexs.Length == 0)
                {// 取源数组的指定行的所有列
                    rows = rowIndexs.Length;
                    newMatrix = new double[rows, cols];
                    for (int row = 0; row < rows; row++)
                    {
                        for (int col = 0; col < cols; col++)
                        { newMatrix[row, col] = origionMatrix[rowIndexs[row], col]; }
                    }
                }
                else
                { // 取源数组的指定行和指定列
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
        #endregion

        #region Norm 求矩阵和向量的范数
        /// <summary>
        /// 求矩阵的范数
        /// </summary>
        /// <param name="matrix">矩阵</param>
        /// <param name="inf">要求的范数类型</param>
        /// <returns>执行成功返回无穷范数，否则返回-1</returns>
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

        /// <summary>
        /// 重载1 求向量的无穷范数
        /// </summary>
        /// <param name="vector">向量</param>
        /// <param name="str">要求的范数类型,inf表示无穷范围,fro表示F范数</param>
        /// <returns>执行成功返回无穷范数，否则返回-1</returns>
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
                else if (str.ToLower() == "fro") // Frobenius,F范数
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

        /// <summary>
        /// 重载2 求向量的2范数，每个元素绝对值的平方和开根号
        /// </summary>
        /// <param name="vector">向量</param>
        /// <returns>执行成功返回无穷范数，否则返回-1</returns>
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
        #endregion

        #region find 找出符合条件的元素的索引
        //FIND   Find indices(index 的复数形式) of nonzero elements.查找所有非零元素的返回索引
        //   I = FIND(X) returns the linear indices corresponding(符合，相配) to the nonzero entries of the array X.  X may be a logical expression. 
        //   Use IND2SUB(SIZE(X),I) to calculate multiple subscripts from the linear indices I.
        //   I = FIND(X,K) returns at most the first K indices corresponding to the nonzero entries of the array X.  K must be a positive integer, but can be of any numeric type.
        //   I = FIND(X,K,'first') is the same as I = FIND(X,K).
        //  I = FIND(X,K,'last') returns at most the last K indices corresponding to the nonzero entries of the array X.
        //   [I,J] = FIND(X,...) returns the row and column indices instead of linear indices into X. This syntax is especially useful when working
        //   with sparse matrices.  If X is an N-dimensional array where N > 2, then  J is a linear index over the N-1 trailing dimensions of X.
        //   [I,J,V] = FIND(X,...) also returns a vector V containing the values that correspond to the row and column indices I and J.
        //   Example:
        //      A = magic(3)
        //      find(A > 5)
        //   finds the linear indices of the 4 entries of the matrix A that aregreater than 5.
        //      [rows,cols,vals] = find(speye(5))
        //   finds the row and column indices and nonzero values of the 5-by-5  sparse identity matrix.
        //   See also SPARSE, IND2SUB, RELOP, NONZEROS.

        /// <summary>
        /// 寻找数组中的非零元素，并返回索引值
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>执行成功返回数组的非零元素的索引值，否则返回null</returns>
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

        /// <summary>
        /// 重载1 寻找数组中的值等于value的元素，并返其回索引值
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>执行成功返回数组的非零元素的索引值，否则返回null</returns>
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
        }// method

        /// <summary>
        /// 重载2 寻找数组中的值等于value的元素，并返其回索引值
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>执行成功返回数组的非零元素的索引值，否则返回null</returns>
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

        /// <summary>
        /// 重载3 寻找数组中的值value的值符合HandleSgn操作元素索引，并返其回索引值
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="mnu">枚举型的操作符</param>
        /// <param name="value">如果是两个数组的比较必须两个数组的长度相等，否则是错误</param>
        /// <returns>执行成功返回数组的非零元素的索引值，否则返回null</returns>
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
                        case HandleSign.Greater:// >
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] > value[0])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.Equal:// ==
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] == value[0])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.GreaterOrEqual:// >=
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] >= value[0])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.Less:// <
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] < value[0])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.LessOrEqual:// <=
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] <= value[0])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.And:// &
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] > 0 & value[0] > 0)
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.Or:// |
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
                        case HandleSign.Greater:// >
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] > value[i])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.Equal:// ==
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] == value[i])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.GreaterOrEqual:// >=
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] >= value[i])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.Less:// <
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] < value[i])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.LessOrEqual:// <=
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] <= value[i])
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.And:// &
                            for (int i = 0; i < len; i++)
                            {
                                if (vector[i] > 0 & value[i] > 0)
                                { lists.Add(i); }
                            }
                            break;
                        case HandleSign.Or:// |
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
        }// method
        #endregion

        #region Zeros 得到全零矩阵或者数组
        /// <summary>
        /// 得到全零向量
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="len">向量长度</param>
        /// <returns>执行成功返回：true，否则false</returns>
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

        /// <summary>
        /// 重载1 得到全零向量
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>执行成功返回：true，否则false</returns>
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

        /// <summary>
        /// 重载2 得到全零矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>执行成功返回：true，否则false</returns>
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
        #endregion

        #region ones 得到全1数组
        /// <summary>
        /// 得到元素值都为1的向量
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
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
        }// method

        /// <summary>
        /// 重载1 得到元素值都为1的矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 得到元素值都为1的向量
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
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
        #endregion

        #region 矩阵求或者向量求和SUM
        /// <summary>
        /// 求向量的和：向量各元素之和
        /// </summary>
        /// <param name="vector">向量</param>
        /// <param name="value">向量的和</param>
        /// <returns></returns>
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

        /// <summary>
        /// 重载1 求矩阵所有元素之和
        /// </summary>
        /// <param name="matrix">矩阵</param>
        /// <param name="value">和</param>
        /// <returns></returns>
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

        /// <summary>
        /// 重载2 求矩阵各行对应元素之和或者矩阵各列对应元素之和
        /// </summary>
        /// <param name="matrix">矩阵</param>
        ///<param name="flag">flag=0求矩阵各列的元素之和返回行向量；flag=1求矩阵各行的元素之和返回列向量</param>
        ///<param name="values">根据相应计算返回相应值</param>
        /// <returns></returns>
        public static bool Sum(double[,] matrix, int flag, ref double[] values)
        {
            try
            {
                if (matrix == null)
                { return false; }
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                double tmp = 0.0;
                // flag=0类似于matlab里面的sum(X)和sum(X,1)
                if (flag == 0)
                {
                    values = new double[cols];
                    for (int col = 0; col < cols; col++)
                    {
                        // 每次计算完成之前必须将临时变量清零，否则得出来的是递加的结果
                        tmp = 0.0;
                        for (int row = 0; row < rows; row++)
                        { tmp += matrix[row, col]; }
                        values[col] = tmp;
                    }
                }
                //flag=1类似于matlab中的sum(X,2)
                else if (flag == 1)
                {
                    values = new double[rows];
                    for (int row = 0; row < rows; row++)
                    {
                        // 每次计算完成之前必须将临时变量清零，否则得出来的是递加的结果
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
        #endregion

        #region repmat(A,M,N)
        // REPMAT Replicate(复制品，重复) and tile an array.
        //   B = repmat(A,M,N) creates a large matrix B consisting of an M-by-N
        //   tiling（瓷砖式显示，并列式窗口；瓷砖式覆盖） of copies of A.
        //   The size of B is [size(A,1)*M, size(A,2)*N].
        //  The statement repmat(A,N) creates an N-by-N tiling. 
        //   B = REPMAT(A,[M N]) accomplishes the same result as repmat(A,M,N).
        //   B = REPMAT(A,[M N P ...]) tiles the array A to produce a 
        //   multidimensional array B composed of copies of A. The size of B is 
        //   [size(A,1)*M, size(A,2)*N, size(A,3)*P, ...].
        //
        //   REPMAT(A,M,N) when A is a scalar(标量) is commonly used to produce an M-by-N
        //   matrix filled with A's value and having A's CLASS. For certain values,
        //   you may achieve the same results using other functions. Namely,
        //      REPMAT(NAN,M,N)           is the same as   NAN(M,N)
        //      REPMAT(SINGLE(INF),M,N)   is the same as   INF(M,N,'single')
        //      REPMAT(INT8(0),M,N)       is the same as   ZEROS(M,N,'int8')
        //      REPMAT(UINT32(1),M,N)     is the same as   ONES(M,N,'uint32')
        //      REPMAT(EPS,M,N)           is the same as   EPS(ONES(M,N))
        //
        //   Example:
        //       repmat(magic(2), 2, 3)
        //       repmat(uint8(5), 2, 3)
        //
        //   Class support for input A:
        //      float: double, single
        //   See also BSXFUN, MESHGRID, ONES, ZEROS, NAN, INF.

        /// <summary>
        /// 获得新矩阵
        /// </summary>
        /// <param name="A">新矩阵的值</param>
        /// <param name="M">新矩阵的行数</param>
        /// <param name="N">新矩阵列数</param>
        /// <param name="B">生成的新矩阵</param>
        /// <returns>获取成功返回true，否则false</returns>
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

        /// <summary>
        /// 重载1 获得新矩阵如果A是一维数组，那么将A作为1*P的二维数组处理
        /// </summary>
        /// <param name="A">新矩阵的值</param>
        /// <param name="M">新矩阵的行参数A.GetLength(0)</param>
        /// <param name="N">新矩阵列参数A.GetLength(1)</param>
        /// <param name="B">生成的新矩阵</param>
        /// <returns>获取成功返回true，否则false</returns>
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
        #endregion

        #region Any
        /// <summary>
        /// 检测数组是否存在值为true的元素
        /// </summary>
        /// <param name="x">检测数组</param>
        /// <returns>存在值为true返回true值，否则返回false</returns>
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
        #endregion

        #region Reshape
        /// <summary>
        /// matlab中reshape（A,ROW,COL）函数是将矩阵A生成新的ROW*COL矩阵
        /// 从原始矩阵读取数据以列优先顺序读取，在新矩阵中也是以列优先存储
        /// </summary>
        /// <param name="x"></param>
        /// <param name="rows">新矩阵的行数</param>
        /// <param name="cols">新矩阵的列数</param>
        /// <returns>执行成功返回新矩阵，否则返回null</returns>
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
                // i->x.row j->x.col
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

        /// <summary>
        ///重载1 matlab中reshape（A,ROW,COL）函数是将矩阵A生成新的ROW*COL矩阵
        /// 从原始矩阵读取数据以列优先顺序读取，在新矩阵中也是以列优先存储
        /// </summary>
        /// <param name="x"></param>
        /// <param name="rows">新矩阵的行数</param>
        /// <param name="cols">新矩阵的列数</param>
        /// <returns>执行成功返回新矩阵，否则返回null</returns>
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
                // i->x.row j->x.col
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
        #endregion

        #region 数据修正
        /// <summary>
        ///  四舍五入保留数据，默认保留小数点后七位
        /// </summary>
        /// <param name="datas">需要四舍五入的数据</param>
        /// <returns>执行成功返回四舍五入的数据，否则返回null</returns>
        public static double[,] ReviseDatas(double[,] datas)
        {// 修正存在的必要性：因为在数据转换过程中数据存在精度的损失，造成不必要的错误
            try
            {// double 默认 数据保留到小数点后六位
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

        /// <summary>
        ///  四舍五入保留数据，默认保留小数点后七位
        /// </summary>
        /// <param name="datas">需要四舍五入的数据</param>
        /// <returns>执行成功返回true，否则返回false</returns>
        public static bool ReviseDatas(ref double[,] datas)
        {// 修正存在的必要性：因为在数据转换过程中数据存在精度的损失，造成不必要的错误
            try
            {// double 默认 数据保留到小数点后六位
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

        /// <summary>
        ///  四舍五入保留数据
        /// </summary>
        /// <param name="datas">需要四舍五入的数据</param>
        /// <param name="decimals">数据保留到小数点后的位数</param>
        /// <returns>执行成功返回四舍五入的数据，否则返回null</returns>
        public static double[,] ReviseDatas(double[,] datas, int decimals)
        {// 修正存在的必要性：因为在数据转换过程中数据存在精度的损失，造成不必要的错误
            try
            {// double 默认 数据保留到小数点后六位
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

        /// <summary>
        ///  四舍五入保留数据
        /// </summary>
        /// <param name="datas">需要四舍五入的数据</param>
        /// <param name="decimals">数据保留到小数点后的位数</param>
        /// <returns></returns>
        public static bool ReviseDatas(ref double[,] datas, int decimals)
        {// 修正存在的必要性：因为在数据转换过程中数据存在精度的损失，造成不必要的错误
            try
            {// double 默认 数据保留到小数点后六位
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
        #endregion

        #region 判断矩阵正定性
        /// <summary>
        ///  矩阵正定性判断，默认误差值1e-8
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>是正定矩阵返回true；否则返回false</returns>
        public static bool JudgePositiveDefinite(double[,] matrix)
        {// 矩阵桥里斯分解的时候需要判断其正定性，只有正定矩阵进行桥里斯分解的数据才有意义，否则没有意义
            try
            {
                double erroFlag = 1e-8;
                double tmp = 0;
                // 判断矩阵是方正
                if (matrix == null) { return false; }
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                if (rows != cols) { return false; }
                // 判断矩阵是否关于主对角线对称
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

        /// <summary>
        ///  矩阵正定性判断
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="erroFlag">两浮点数相等的允许误差范围</param>
        /// <returns>是正定矩阵返回true；否则返回false</returns>
        public static bool JudgePositiveDefinite(double[,] matrix, double erroFlag)
        {// 矩阵桥里斯分解的时候需要判断其正定性，只有正定矩阵进行桥里斯分解的数据才有意义，否则没有意义
            try
            {
                double tmp = 0;
                // 判断矩阵是方正
                if (matrix == null) { return false; }
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                if (rows != cols) { return false; }
                // 判断矩阵是否关于主对角线对称
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
        #endregion
    }//class
}
