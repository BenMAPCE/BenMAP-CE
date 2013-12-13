using System;
using System.Collections.Generic;
using System.Text;

namespace ESIL.Kriging
{
    public enum HandleSign
    {
        /// <summary>
        /// > ����
        /// </summary>
        Greater,
        /// <summary>
        /// ���ڵ���
        /// </summary>
        GreaterOrEqual,
        /// <summary>
        /// С��
        /// </summary>
        Less,
        /// <summary>
        /// С�ڵ���
        /// </summary>
        LessOrEqual,
        /// <summary>
        /// ����==
        /// </summary>
        Equal,
        /// <summary>
        /// �߼���
        /// </summary>
        And,
        /// <summary>
        /// �߼���
        /// </summary>
        Or
    }
    public static class MatrixCompute
    {
        #region Matlab eye ���ɵ�λ����
        /// <summary>
        /// ���ɵ�λ����  1 0 0
        ///                 0 1 0
        ///                 0 0 1  
        /// </summary>
        /// <param name="x">�洢��λ���������</param>
        /// <returns>ִ�гɹ�����true������false</returns>
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

        #region Matlab det���� ����ʽ��ֵ
        /// <summary>
        /// matlab�е�det���� ������ʽ��ֵ
        /// </summary>
        /// <param name="detArray">n������ʽ</param>
        /// <param name="n">����ʽ�Ľ���</param>
        /// <returns>ִ�гɹ�����detֵ�����򷵻�1e-10</returns>
        public static double Det(double[,] detArray)
        {
            try
            {// ����ʽ�ĳ�ʼֵ������Ϊ1�������ڼ����������ԶΪ0
                if (detArray == null)
                { return 1e-10; }
                double f = 1.0;
                double det = 1.0;
                double q = 0.0;
                double d;
                int iis = 0;
                int js = 0;
                // 0->d��Ӧ��ά������У�1->��Ӧ��ά�������
                int rowCount = detArray.GetLength(0);
                int colCount = detArray.GetLength(1);
                if (rowCount != colCount || rowCount == 0)
                { return det = 1e-10; }
                int n = rowCount;
                // k��Ϊ������
                for (int k = 0; k <= n - 2; k++)
                { //ѭ���� i��������
                    q = 0.0;
                    for (int i = k; i <= n - 1; i++)
                    {// ѭ���� j������
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
        /// matlab�е�det����:������ʽ��ֵ
        /// </summary>
        /// <param name="detArr">n������ʽ��һ��һά����洢</param>
        /// <param name="n">����ʽ�Ľ���</param>
        /// <returns>ִ�гɹ�����detֵ�����򷵻�1e-10</returns>
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

        #region �������
        /// <summary>
        /// ����������ӣ�������ͬ�;���
        /// </summary>
        /// <param name="matrixI">��ž���1��һά����</param>
        /// <param name="matrixII">��ž���2��һά����</param>
        /// <param name="resultMatrix">���������һά������</param>
        /// <returns>ִ�гɹ�����trueֵ�����򷵻�false</returns>
        public static bool Add(double[] matrixI, double[] matrixII, ref double[] resultMatrix)
        {
            try
            {// �������������ʱ�����������������ͬ�;���
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
        /// ����1 ����������ӣ�������ͬ�;���
        /// </summary>
        /// <param name="matrixI">��ž���1�Ķ�ά����</param>
        /// <param name="matrixII">��ž���2�Ķ�ά����</param>
        /// <param name="resultMatrix">��������ö�ά������</param>
        /// <returns>ִ�гɹ�����trueֵ�����򷵻�false</returns>
        public static bool Add(double[,] matrixI, double[,] matrixII, ref double[,] resultMatrix)
        {
            try
            {// �������������ʱ�����������������ͬ�;���
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);// 0->���������
                int colsI = matrixI.GetLength(1);// 1->���������
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
        /// ����2 �����һ������
        /// </summary>
        /// <param name="matrixI">��ž���1�Ķ�ά����</param>
        /// <param name="constant">����</param>
        /// <param name="resultMatrix">��������ö�ά������</param>
        /// <returns>ִ�гɹ�����trueֵ�����򷵻�false</returns>
        public static bool Add(double[,] matrixI, double constant, ref double[,] resultMatrix)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);// 0->���������
                int colsI = matrixI.GetLength(1);// 1->���������
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
        /// ����3 ������һ������
        /// </summary>
        /// <param name="matrixI">��ž���1�Ķ�ά����</param>
        /// <param name="constant">����</param>
        /// <param name="resultMatrix">��������ö�ά������</param>
        /// <returns>ִ�гɹ�����trueֵ�����򷵻�false</returns>
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

        #region �������
        /// <summary>
        /// �������������������ͬ�;���
        /// </summary>
        /// <param name="matrixI">��ž���1��һά����</param>
        /// <param name="matrixII">��ž���2��һά����</param>
        /// <param name="resultMatrix">���������һά������</param>
        /// <returns>ִ�гɹ�����trueֵ�����򷵻�false</returns>
        public static bool Subtract(double[] matrixI, double[] matrixII, ref double[] resultMatrix)
        {
            try
            {// �������������ʱ�����������������ͬ�;���
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
        /// ����1 �������������������ͬ�;���
        /// </summary>
        /// <param name="matrixI">��ž���1�Ķ�ά����</param>
        /// <param name="matrixII">��ž���2�Ķ�ά����</param>
        /// <param name="resultMatrix">��������ö�ά������</param>
        /// <returns>ִ�гɹ�����trueֵ�����򷵻�false</returns>
        public static bool Subtract(double[,] matrixI, double[,] matrixII, ref double[,] resultMatrix)
        {
            try
            {// �������������ʱ�����������������ͬ�;���
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);// 0->���������
                int colsI = matrixI.GetLength(1);// 1->���������
                int rowsII = matrixII.GetLength(0);
                int colsII = matrixII.GetLength(1);
                // �жϴ�������Ĳ����Ƿ���ֵ��ȫ�����������ֵ�ż��㣬���򷵻�false
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
        /// ����2 �����һ������
        /// </summary>
        /// <param name="matrixI">��ž���1�Ķ�ά����</param>
        /// <param name="constant">����</param>
        /// <param name="resultMatrix">��������ö�ά������</param>
        /// <returns>ִ�гɹ�����trueֵ�����򷵻�false</returns>
        public static bool Subtract(double[,] matrixI, double constant, ref double[,] resultMatrix)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);// 0->���������
                int colsI = matrixI.GetLength(1);// 1->���������
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

        #region Matlab �������
        #region Multiply
        /// <summary>
        /// ������ ������ά����洢�ľ���:m*n����n*p=m*p
        /// </summary>
        /// <param name="matrixI">��������1:m*n</param>
        /// <param name="matrixII">��������2:n*p</param>
        /// <param name="matrixResult">����������˺�Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool Multiply(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {// ����(matrixI:m*n) *(matrixII:p*k)->m*k�¾���
            // ������˵���������1���������ھ������������������ȣ���ô��������� ����false
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);
                int colsI = matrixI.GetLength(1);// ����
                int rowsII = matrixII.GetLength(0);//����
                int colsII = matrixII.GetLength(1);
                if ((colsI != rowsII) || (rowsI == 0 && rowsI == colsI) || (rowsII == 0 && rowsII == colsII))
                { return false; }
                matrixResult = new double[rowsI, colsII];
                //ѭ��matrixI��ÿһ�кͶ�Ӧ��matrixII��ÿһ�ж�Ӧ��˵õ��µ�matrixResult
                for (int rowI = 0; rowI < rowsI; rowI++)
                {//ѭ��matrixI����
                    for (int colII = 0; colII < colsII; colII++)
                    {// ѭ��matrixII����
                        for (int colI = 0; colI < colsI; colI++)
                        {// matrixI������==matrixII������
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
        /// ����1 �����ˣ�����m*n����������n*1=m*1
        /// </summary>
        /// <param name="matrixI">��������1:m*n</param>
        /// <param name="matrixII">��������2:������n*1</param>
        /// <param name="matrixResult">����������˺�Ľ������:m*1��������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool Multiply(double[,] matrixI, double[] matrixII, ref double[] matrixResult)
        {// ����(matrixI:m*n) *(matrixII:p*1)->m*1�¾���
            // ������˵���������1���������ھ������������������ȣ���ô��������� ����false
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);
                int colsI = matrixI.GetLength(1);// ����
                int lensII = matrixII.Length;
                if ((colsI != lensII) || (rowsI == 0 && rowsI == colsI) || (lensII == 0))
                { return false; }
                matrixResult = new double[rowsI];
                //ѭ��matrixI��ÿһ�кͶ�Ӧ��matrixII��ÿһ�ж�Ӧ��˵õ��µ�matrixResult
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
        /// ����2 ������:������m*1����������1*p=m*p
        /// </summary>
        /// <param name="matrixI">��������1:������m*1</param>
        /// <param name="matrixII">��������2:������1*p</param>
        /// <param name="matrixResult">����������˺�Ľ������:m*p</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool Multiply(double[] matrixI, double[] matrixII, ref double[,] matrixResult)
        { // ����(matrixI:m*1) *(matrixII:1*k)->m*k�¾���
            // ������˵���������1���������ھ������������������ȣ���ô��������� ����false
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int cols = matrixI.Length;// ����
                int rows = matrixII.Length;//����
                if (cols == 0 && rows == cols)
                { return false; }
                matrixResult = new double[rows, cols];
                //ѭ��matrixI��ÿһ�кͶ�Ӧ��matrixII��ÿһ�ж�Ӧ��˵õ��µ�matrixResult
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
        /// ����3 ������:������ 1*m����������m*1=1*1
        /// </summary>
        /// <param name="matrixI">��������1:������1*m</param>
        /// <param name="matrixII">��������2:������m*1</param>
        /// <param name="matrixResult">����������˺�Ľ��:</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool Multiply(double[] matrixI, double[] matrixII, ref double matrixResult)
        {// ����(matrixI:m*n) *(matrixII:p*k)->m*k�¾���
            // ������˵���������1���������ھ������������������ȣ���ô���������
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int lensI = matrixI.Length;// ����
                int lensII = matrixII.Length;//����
                if ((lensI != lensII) || (lensII == 0 && lensII == lensI))
                { return false; }
                matrixResult = 0;
                //ѭ��matrixI��ÿһ�кͶ�Ӧ��matrixII��ÿһ�ж�Ӧ��˵õ��µ�matrixResult
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
        /// ����4 �����ˣ������� 1*m���Ծ��� m*p=1*p
        /// </summary>
        /// <param name="matrixI">��������1:������1*m</param>
        /// <param name="matrixII">��������2:����m*p</param>
        /// <param name="matrixResult">����������˺�Ľ������:1*p</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool Multiply(double[] matrixI, double[,] matrixII, ref double[] matrixResult)
        {// ����(matrixI:1*n) *(matrixII:p*k)->n*k�¾���
            // ������˵���������1���������ھ������������������ȣ���ô���������
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int lens = matrixI.Length;// ����
                int rows = matrixII.GetLength(0);//����
                int cols = matrixII.GetLength(1);// ���ؽṹ����ĳ���
                if ((lens != rows) || (rows == 0 && rows == cols))
                { return false; }
                matrixResult = new double[cols];
                //ѭ��matrixI��ÿһ�кͶ�Ӧ��matrixII��ÿһ�ж�Ӧ��˵õ��µ�matrixResult
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
        /// ����5 ������:������1*m���Գ���
        /// </summary>
        /// <param name="matrixI">��������1:������1*m</param>
        /// <param name="constant">����</param>
        /// <param name="matrixResult">����������˺�Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool Multiply(double[] matrixI, double constant, ref double[] matrixResult)
        {// ������˵���������1���������ھ������������������ȣ���ô���������
            try
            {
                if (matrixI == null)
                { return false; }
                int length = matrixI.Length;
                if (length == 0)
                { return false; }
                matrixResult = new double[length];
                //ѭ��matrixI��ÿһ�кͶ�Ӧ��matrixII��ÿһ�ж�Ӧ��˵õ��µ�matrixResult
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
        /// ����6 �����˾���m*n���Գ���
        /// </summary>
        /// <param name="matrixI">��������1:m*n</param>
        /// <param name="constant">��������2:����</param>
        /// <param name="matrixResult">����������˺�Ľ������:m*1��������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool Multiply(double[,] matrixI, double constant, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int colsI = matrixI.GetLength(1);// ����
                int rowsI = matrixI.GetLength(0);
                if (rowsI == 0 && rowsI == colsI)
                { return false; }
                matrixResult = new double[rowsI, colsI];
                //ѭ��matrixI��ÿһ�кͶ�Ӧ��matrixII��ÿһ�ж�Ӧ��˵õ��µ�matrixResult
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
        /// ������
        /// </summary>
        /// <param name="matrixI">��������1</param>
        /// <param name="matrixII">��������2</param>
        /// <param name="matrixResult">����������˺�Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool DotMultiply(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null || matrixII == null)
                { return false; }
                int colsI = matrixI.GetLength(1);// ����
                int rowsI = matrixI.GetLength(0);
                int colsII = matrixII.GetLength(1);
                int rowsII = matrixII.GetLength(0);//����
                if ((rowsI != rowsII) || (colsI != colsII) || (rowsI == 0 && rowsI == rowsII))
                { return false; }
                matrixResult = new double[rowsI, colsI];
                //ѭ��matrixI��ÿһ�кͶ�Ӧ��matrixII��ÿһ�ж�Ӧ��˵õ��µ�matrixResult
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
        /// ����1 �������
        /// </summary>
        /// <param name="matrixI">��������1</param>
        /// <param name="matrixII">��������2</param>
        /// <param name="matrixResult">����������˺�Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
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
        /// ����2 ���
        /// </summary>
        /// <param name="matrixI">��������1</param>
        /// <param name="matrixII">��������2</param>
        /// <param name="matrixResult">����������˺�Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
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
        /// ������
        /// </summary>
        /// <param name="matrixI">��������1</param>
        /// <param name="matrixII">��������2</param>
        /// <param name="matrixResult">����������˺�Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool DotMultiply(double[,] matrixI, double constant, ref double[,] matrixResult)
        {
            try
            {
                if (matrixI == null)
                { return false; }
                int colsI = matrixI.GetLength(1);// ����
                int rowsI = matrixI.GetLength(0);
                if (rowsI == 0 && rowsI == colsI)
                { return false; }
                matrixResult = new double[rowsI, colsI];
                //ѭ��matrixI��ÿһ�кͶ�Ӧ��matrixII��ÿһ�ж�Ӧ��˵õ��µ�matrixResult
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
        /// ���� �������
        /// </summary>
        /// <param name="matrixI">��������1</param>
        /// <param name="matrixII">��������2</param>
        /// <param name="matrixResult">����������˺�Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
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

        #region �������
        #region RDivision
        /// <summary>
        /// �����ҳ� A/B ��ͬ�� A*Inv(B)
        /// </summary>
        /// <param name="matrixI">����1:m*n</param>
        /// <param name="matrixII">����2:n*n</param>
        /// <param name="matrixResult">�������������Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool RDivision(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {// �����һ����������ܵĲ��ı䴫��ļ��������ֻ���������ڸ�ֵǰ���ȳ�ʼ��
            try
            {// ���ҳ������о���matrixII�����Ƿ���n*n��matrixI�����m*n
                bool ok = false;
                if (matrixI == null || matrixII == null)
                { return false; }
                // ������Ϊ�������ݵĹ������ǵ�ַ���ݣ����Ե��ڸı����ֵ������ǣ�������ֵ���ݸ���ʱ����
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
        /// ����1 �����ҳ� A/B ��ͬ�� A*Inv(B)
        /// </summary>
        /// <param name="matrixI">����1</param>
        /// <param name="matrixII">����2</param>
        /// <param name="matrixResult">�������������Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool RDivision(double[] matrixI, double[,] matrixII, ref double[] matrixResult)
        {
            try
            {// ���ҳ������о���matrixII�����Ƿ���n*n��matrixI�����m*n
                bool ok = false;
                if (matrixI == null || matrixII == null)
                { return false; }
                int len = matrixI.Length;
                // ������Ϊ�������ݵĹ������ǵ�ַ���ݣ����Ե��ڸı����ֵ������ǣ�������ֵ���ݸ���ʱ����
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
        /// ����2 �����ҳ����� A/constant ��ͬ�ڵ��
        /// </summary>
        /// <param name="matrixI">����1</param>
        /// <param name="constant">����</param>
        /// <param name="matrixResult">�������������Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool RDivision(double[,] matrixI, double constant, ref double[,] matrixResult)
        {
            try
            {// ���ҳ������о���matrixII�����Ƿ���n*n��matrixI�����m*n
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
        /// ����3 �����ҳ����� A/constant 
        /// </summary>
        /// <param name="matrixI">����1</param>
        /// <param name="constant">����</param>
        /// <param name="matrixResult">�������������Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool RDivision(double[] matrixI, double constant, ref double[] matrixResult)
        {
            try
            {// ���ҳ������о���matrixII�����Ƿ���n*n��matrixI�����m*n
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
        /// ������� A\B ��ͬ�� Inv(A)*B
        /// </summary>
        /// <param name="matrixI">����1</param>
        /// <param name="matrixII">����2</param>
        /// <param name="matrixResult">�������������Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool LDivision(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {// ����������о���matrixI�����Ƿ���n*n��matrixI�����N*M;
                bool ok = false;
                if (matrixI == null || matrixII == null)
                { return false; }
                int rowsI = matrixI.GetLength(0);
                int colsI = matrixI.GetLength(1);

                int rowsII = matrixII.GetLength(0);
                int colsII = matrixII.GetLength(1);
                if ((rowsI != colsI) || (rowsI == 0 && rowsI == colsI) || (rowsII == 0 && rowsII == colsII))
                { return false; }
                // ������Ϊ�������ݵĹ������ǵ�ַ���ݣ����Ե��ڸı����ֵ������ǣ�������ֵ���ݸ���ʱ����
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
        /// ����1 ������� A\B ��ͬ�� Inv(A)*B
        /// </summary>
        /// <param name="matrixI">����1</param>
        /// <param name="matrixII">����2</param>
        /// <param name="matrixResult">�������������Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool LDivision(double[,] matrixI, double[] matrixII, ref double[] matrixResult)
        {
            try
            {// ���ҳ������о���matrixI�����Ƿ���n*n��matrixI�����N*M;
                bool ok = false;
                if (matrixI == null || matrixII == null)
                { return false; }
                // ������Ϊ�������ݵĹ������ǵ�ַ���ݣ����Ե��ڸı����ֵ������ǣ�������ֵ���ݸ���ʱ����
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
        /// ����2 ������� A\B ��ͬ�� Inv(A)*B
        /// </summary>
        /// <param name="matrixI">����1</param>
        /// <param name="matrixII">��ֵ</param>
        /// <param name="matrixResult">�������������Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool LDivision(double[,] matrixI, double value, ref double[,] matrixResult)
        {
            try
            {// ����������о���matrixI�����Ƿ���n*n��matrixI�����N*M;
                bool ok = false;
                if (matrixI == null)
                { return false; }
                int rows = matrixI.GetLength(0);
                int cols = matrixI.GetLength(1);
                if ((rows != cols) || (rows == 0 && rows == cols))
                { return false; }
                // ������Ϊ�������ݵĹ������ǵ�ַ���ݣ����Ե��ڸı����ֵ������ǣ�������ֵ���ݸ���ʱ����
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
        /// �����ҳ� A./B
        /// </summary>
        /// <param name="matrixI">����1</param>
        /// <param name="matrixII">����2</param>
        /// <param name="matrixResult">�������������Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool DotRDivision(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {// �������������������Ľ����������ͬ�;���
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
        /// ����1 �����ҳ� A./B
        /// </summary>
        /// <param name="matrixI">����1 ��ά������</param>
        /// <param name="constant">����</param>
        /// <param name="matrixResult">�������������Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
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
        /// ����2 �����ҳ� A./B
        /// </summary>
        /// <param name="matrixI">����1 һά����</param>
        /// <param name="constant">����</param>
        /// <param name="matrixResult">�������������Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
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
        /// ����3 �����ҳ� A./B
        /// </summary>
        /// <param name="vectorA">����A һά����</param>
        /// <param name="vectorB">����B</param>
        /// <param name="matrixResult">���������֮��Ľ��</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
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
        /// ������� A.\B
        /// </summary>
        /// <param name="matrixI">����1</param>
        /// <param name="matrixII">����2</param>
        /// <param name="matrixResult">�������������Ľ������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
        public static bool DotLDivision(double[,] matrixI, double[,] matrixII, ref double[,] matrixResult)
        {
            try
            {// �������������������Ľ����������ͬ�;���
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

        #region ��������
        //��˹��Լ����(ȫѡ��Ԫ)����Ĳ������¡�
        //���ȣ�����k��0��nһ1�����¼�����
        //(1)�ӵ�k�С���k�п�ʼ�����½�������ѡȡ����ֵ����Ԫ�أ�����ס��Ԫ����
        //�ڵ��кź��кţ���ͨ���н������н���������������Ԫ��λ���ϡ���һ����Ϊȫѡ��
        //(2)1��Akk=>Akk
        //(3)Akj*Akk=>Akj,j=0,1,2,...j!=k
        //(4)Aij-Aik=>Aij,i,j=0,1,2,...j!=k i!=k
        //(5)Aik*Akk=>Aik,i,=0,1,2,...i!=k
        //��󣬸�����ȫѡ��Ԫ���̻�����¼���С��н�������Ϣ���лָ����ָ���ԭ������
        // ��ȫѡ��Ԫ�����У��Ƚ������С��к���лָ���ԭ������(��)��������(��)�������ָ���

        /// <summary>
        /// һ��������������
        /// </summary>
        /// <param name="originMatrix">��Ҫ����ľ���a��˫����ʵ�ж�ά���飬���n*n</param>
        /// <param name="invMatrix">�����ľ���</param>
        /// <param name="n">����Ľ���</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
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
                        {// Ԫ������������
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
        /// ����1 һ��������������
        /// </summary>
        /// <param name="originMatrix">��Ҫ����ľ���a��˫����ʵ�ж�ά���飬���n*n</param>
        /// <param name="invMatrix">�����ľ���</param>
        /// <returns></returns>
        public static bool Inv(double[,] originMatrix, ref double[,] invMatrix)
        {
            try
            {// ��������ı�Ҫ�����Ǿ�������Ƿ���
                bool ok = false;
                int n = 0;
                if (originMatrix == null)
                { return false; }
                // ��ȡ���������������
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
        /// ����2 һ��������������
        /// </summary>
        /// <param name="invMatrix">��Ҫ����ľ���invMatrix��˫����ʵ�ж�ά���飬���n*n,�������󷵻������</param>
        /// <returns></returns>
        public static bool Inv(ref double[,] invMatrix)
        {
            try
            {// ��������ı�Ҫ�����Ǿ�������Ƿ���
                int n = 0;
                if (invMatrix == null)
                { return false; }
                // ��ȡ���������������
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

        #region ����ֽ�chol �Գ��������������˹���ֽ�
        /// <summary>
        /// Cholesky����ֽⷨ[R,p]=chol(a) == a=R'*R
        /// </summary>
        /// <param name="a">��ž����һά����</param>
        /// <param name="n">��������Ľ���</param>
        /// <param name="det">����ʽ��ֵ</param>
        /// <returns>ִ�гɹ�����true�����򷵻�false</returns>
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
                // �������Ǿ���ת��Ϊ�����Ǿ���
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
        /// ����1 Cholesky����ֽⷨ[R,p]=chol(a) == a=R'*R
        /// </summary>
        /// <param name="a">��ž����һά����</param>
        /// <param name="n">��������Ľ���</param>
        /// <returns>ִ�гɹ�����true�����򷵻�false</returns>
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
                // �������Ǿ���ת��Ϊ�����Ǿ���
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
        /// ����2 Cholesky����ֽⷨ ��R,p��=chol(a) === a=R'*R
        /// </summary>
        /// <param name="a">��ž���Ķ�ά����</param>
        /// <returns>ִ�гɹ�����true==0=p�����򷵻�false!=0</returns>
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
                // �������Ǿ���ת��Ϊ�����Ǿ���
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

        #region ������ת�þ���
        /// <summary>
        /// ��һά������n�׾��󣬶Ծ������ת��
        /// </summary>
        /// <param name="a">��ž����һά����</param>
        /// <param name="n">����Ľ���</param>
        /// <returns>ִ�гɹ�����true ���򷵻أ�false</returns>
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
        /// ����1 �ö�ά������n�׾��󣬶Ծ������ת��
        /// </summary>
        /// <param name="a">��ž���Ķ�ά����</param>
        /// <returns>ִ�гɹ�����true ���򷵻أ�false</returns>
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
        /// ����2 �ö�ά������n�׾��󣬶Ծ������ת��
        ///  ������ȥ��rows����cols==0�򷵻�false�Ĵ��룬ԭ����ACTSET���ڳ���Ϊ0��ʱ��
        /// </summary>
        /// <param name="a">��ž���Ķ�ά����</param>
        /// <param name="resultMatrix">ת�ú�ľ���</param>
        /// <returns>ִ�гɹ�����true ���򷵻أ�false</returns>
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

        #region rcond ��������������
        public static double RCond(double[,] R)
        {
            // TODO: δʵ��
            return 10.0;
        }
        #endregion

        #region ����QR�ֽ�
        /// <summary>
        /// �����QR�ֽ�
        /// </summary>
        /// <param name="a">Ҫ����QR�ֽ�ľ���ִ�гɹ��󷵻طֽ���R</param>
        /// <param name="m">���������</param>
        /// <param name="n">���������</param>
        /// <param name="q">QR�ֽ���q</param>
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
        /// ����1 �����QR�ֽ�
        /// </summary>
        /// <param name="matrixA">Ҫ����QR�ֽ�ľ���ִ�гɹ��󷵻طֽ���R</param>
        /// <param name="matrixR">QR�ֽ�����R</param>
        /// <param name="matrixQ">QR�ֽ���Q</param>
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
        /// ����2 �����QR�ֽ�
        /// </summary>
        /// <param name="matrixA">Ҫ����QR�ֽ�ľ���ִ�гɹ��󷵻طֽ���R</param>
        /// <param name="matrixR">QR�ֽ�����R</param>
        /// <param name="matrixQ">QR�ֽ���Q</param>
        /// <param name="matrixE">��λ��������е�matrixA������</param>
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
        /// ����3 ������QR�ֽ�
        /// </summary>
        /// <param name="matrixA">Ҫ����QR�ֽ�ľ���ִ�гɹ��󷵻طֽ���R</param>
        /// <param name="matrixR">QR�ֽ�����R</param>
        /// <param name="matrixQ">QR�ֽ���Q</param>
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
        /// ����4 ������QR�ֽ�
        /// </summary>
        /// <param name="matrixA">Ҫ����QR�ֽ�ľ���ִ�гɹ��󷵻طֽ���R</param>
        /// <param name="matrixR">QR�ֽ�����R</param>
        /// <param name="matrixQ">QR�ֽ���Q</param>
        /// <param name="matrixE">��λ��������е�matrixA������</param>
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
        /// ɾ����j�л��ߵ�j�к��QR�ֽ�ȵ����µ�QRֵ
        /// Note��ɾ���еĻ�ûʵ��
        /// </summary>
        /// <param name="Q">ɾ��ǰ����QR�ֽ���Q��ִ�к����ɹ��󷵻�ɾ���л�����֮���Q</param>
        /// <param name="R">ɾ��ǰ����QR�ֽ���R��ִ�к����ɹ��󷵻�ɾ���л�����֮���R</param>
        /// <param name="j">Ҫɾ��������л����е�������0��ʼ��</param>
        /// <param name="orient">ɾ����ʶ����0��ɾ���У���1��ɾ���У�</param>
        /// <returns>ִ�гɹ�����true�����򷵻�false</returns>
        public static bool QRdelete(ref double[,] Q, ref double[,] R, int j, int orient)
        {
            #region ��������˵��
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
            {// ��������ʱֻʵ����ɾ���еĹ���
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
        // QRINSERT Insert a column or row into QR factorization(��ʽ�ֽ⣬���ӷֽ�).
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
        /// ��ӵ�j�л��ߵ�j�к��QR�ֽ�ȵ����µ�QRֵ
        /// Note������еĻ�ûʵ�֣�������ֻ�������һ�л���һ�е����
        /// </summary>
        /// <param name="Q">���ǰ����QR�ֽ���Q��ִ�к����ɹ��󷵻�ɾ���л�����֮���Q</param>
        /// <param name="R">���ǰ����QR�ֽ���R��ִ�к����ɹ��󷵻�ɾ���л�����֮���R</param>
        /// <param name="j">��ӳ�������л����е�������0��ʼ��</param>
        /// <param name="x">��ӵ��л�����</param>
        /// <param name="orient">��ӱ�ʶ����0���У���1���У�</param>
        /// <returns>ִ�гɹ�����true�����򷵻�false</returns>
        public static bool QRinsert(ref double[,] Q, ref double[,] R, int j, double[] x, int orient)
        {
            try
            { // ��������ʱֻ���ǲ���һ�е�״̬
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
                {// R��������Ϊ0ʱ
                    ok = MatrixCompute.QR(x, ref tmpValue, ref Q);
                    if (!ok) { return ok; }
                    R = new double[tmpValue.Length, 1];
                    for (int row = 0; row < tmpValue.Length; row++)
                    {
                        R[row, 0] = tmpValue[row];
                    }
                    return true;
                }
                // TODO�����ڱ�������ʱֻ�����У������ж�R������Ϊ0ʱ����
                if (mq != nq)
                { return false; }
                if (nq != mr)
                { return false; }
                if (j < 0)
                { return false; }
                switch (orient)
                {
                    case 0:// ��
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
                                // ������������ʱ������ԭ���ǣ�����ÿ�β����ж��ǴӾ�������һ�в���
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
                    case 1:// ��
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
        /// <param name="x">����Ϊ2��������</param>
        /// <param name="G">2*2�ľ���</param>
        /// <returns>ִ�гɹ�����true������false</returns>
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
        /// ɾ����ά����� �л���
        /// </summary>
        /// <param name="x">Ҫɾ���л����е�����</param>
        /// <param name="index">Ҫɾ�����л����е�������0��ʼ</param>
        /// <param name="flag">0->�У�1->��</param>
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
                    case 0:// ɾ����
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
                    case 1:// ɾ����
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
        /// ɾ��һά����ĵ�index��Ԫ��
        /// </summary>
        /// <param name="x">Ҫɾ���л����е�����</param>
        /// <param name="index">Ҫɾ�����л����е�������0��ʼ</param>
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
        /// ɾ��һά����ĵ�index��Ԫ��
        /// </summary>
        /// <param name="x">Ҫɾ���л����е�����</param>
        /// <param name="index">Ҫɾ�����л����е�������0��ʼ</param>
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
        /// �ھ���ָ���л�����ǰ����һ�л���һ��
        /// </summary>
        /// <param name="Matrix"></param>
        /// <param name="value"></param>
        /// <param name="flag">0->�У�1->��</param>
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
        //            case 0://�����
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
        //            case 1://�����
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

        #region ��ȡ�������������N�л�����
        /// <summary>
        /// ��ȡ�����ָ���л��еõ��¾���
        /// </summary>
        /// <param name="origionMatrix">ԭʼ����</param>
        /// <param name="rowIndexs">Ҫ��ȡ��������ֵ����0��ʼ�����ȡԴ�����������rowIndexs.length=0</param>
        /// <param name="colIndexs">Ҫ��ȡ��������ֵ(��0��ʼ)���ȡԴ�����������colIndexs.length=0</param>
        /// <param name="newMatrix">�õ����¾���</param>
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
                {// ȡԭ����������к���
                    newMatrix = new double[rows, cols];
                    ok = Common.Init2Dimensions(ref newMatrix, origionMatrix);
                    if (!ok) { return ok; }
                }
                else if (rowIndexs.Length == 0)
                {//ȡԴ�����ָ���е�������
                    cols = colIndexs.Length;
                    newMatrix = new double[rows, cols];
                    for (int col = 0; col < cols; col++)
                    {
                        for (int row = 0; row < rows; row++)
                        { newMatrix[row, col] = origionMatrix[row, colIndexs[col]]; }
                    }
                }
                else if (colIndexs.Length == 0)
                {// ȡԴ�����ָ���е�������
                    rows = rowIndexs.Length;
                    newMatrix = new double[rows, cols];
                    for (int row = 0; row < rows; row++)
                    {
                        for (int col = 0; col < cols; col++)
                        { newMatrix[row, col] = origionMatrix[rowIndexs[row], col]; }
                    }
                }
                else
                { // ȡԴ�����ָ���к�ָ����
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

        #region Norm �����������ķ���
        /// <summary>
        /// �����ķ���
        /// </summary>
        /// <param name="matrix">����</param>
        /// <param name="inf">Ҫ��ķ�������</param>
        /// <returns>ִ�гɹ���������������򷵻�-1</returns>
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
        /// ����1 �������������
        /// </summary>
        /// <param name="vector">����</param>
        /// <param name="str">Ҫ��ķ�������,inf��ʾ���Χ,fro��ʾF����</param>
        /// <returns>ִ�гɹ���������������򷵻�-1</returns>
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
                else if (str.ToLower() == "fro") // Frobenius,F����
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
        /// ����2 ��������2������ÿ��Ԫ�ؾ���ֵ��ƽ���Ϳ�����
        /// </summary>
        /// <param name="vector">����</param>
        /// <returns>ִ�гɹ���������������򷵻�-1</returns>
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

        #region find �ҳ�����������Ԫ�ص�����
        //FIND   Find indices(index �ĸ�����ʽ) of nonzero elements.�������з���Ԫ�صķ�������
        //   I = FIND(X) returns the linear indices corresponding(���ϣ�����) to the nonzero entries of the array X.  X may be a logical expression. 
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
        /// Ѱ�������еķ���Ԫ�أ�����������ֵ
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>ִ�гɹ���������ķ���Ԫ�ص�����ֵ�����򷵻�null</returns>
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
        /// ����1 Ѱ�������е�ֵ����value��Ԫ�أ������������ֵ
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>ִ�гɹ���������ķ���Ԫ�ص�����ֵ�����򷵻�null</returns>
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
        /// ����2 Ѱ�������е�ֵ����value��Ԫ�أ������������ֵ
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>ִ�гɹ���������ķ���Ԫ�ص�����ֵ�����򷵻�null</returns>
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
        /// ����3 Ѱ�������е�ֵvalue��ֵ����HandleSgn����Ԫ�������������������ֵ
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="mnu">ö���͵Ĳ�����</param>
        /// <param name="value">�������������ıȽϱ�����������ĳ�����ȣ������Ǵ���</param>
        /// <returns>ִ�гɹ���������ķ���Ԫ�ص�����ֵ�����򷵻�null</returns>
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

        #region Zeros �õ�ȫ������������
        /// <summary>
        /// �õ�ȫ������
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="len">��������</param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
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
        /// ����1 �õ�ȫ������
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
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
        /// ����2 �õ�ȫ�����
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>ִ�гɹ����أ�true������false</returns>
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

        #region ones �õ�ȫ1����
        /// <summary>
        /// �õ�Ԫ��ֵ��Ϊ1������
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
        /// ����1 �õ�Ԫ��ֵ��Ϊ1�ľ���
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
        /// �õ�Ԫ��ֵ��Ϊ1������
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

        #region ����������������SUM
        /// <summary>
        /// �������ĺͣ�������Ԫ��֮��
        /// </summary>
        /// <param name="vector">����</param>
        /// <param name="value">�����ĺ�</param>
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
        /// ����1 ���������Ԫ��֮��
        /// </summary>
        /// <param name="matrix">����</param>
        /// <param name="value">��</param>
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
        /// ����2 �������ж�ӦԪ��֮�ͻ��߾�����ж�ӦԪ��֮��
        /// </summary>
        /// <param name="matrix">����</param>
        ///<param name="flag">flag=0�������е�Ԫ��֮�ͷ�����������flag=1�������е�Ԫ��֮�ͷ���������</param>
        ///<param name="values">������Ӧ���㷵����Ӧֵ</param>
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
                // flag=0������matlab�����sum(X)��sum(X,1)
                if (flag == 0)
                {
                    values = new double[cols];
                    for (int col = 0; col < cols; col++)
                    {
                        // ÿ�μ������֮ǰ���뽫��ʱ�������㣬����ó������ǵݼӵĽ��
                        tmp = 0.0;
                        for (int row = 0; row < rows; row++)
                        { tmp += matrix[row, col]; }
                        values[col] = tmp;
                    }
                }
                //flag=1������matlab�е�sum(X,2)
                else if (flag == 1)
                {
                    values = new double[rows];
                    for (int row = 0; row < rows; row++)
                    {
                        // ÿ�μ������֮ǰ���뽫��ʱ�������㣬����ó������ǵݼӵĽ��
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
        // REPMAT Replicate(����Ʒ���ظ�) and tile an array.
        //   B = repmat(A,M,N) creates a large matrix B consisting of an M-by-N
        //   tiling����שʽ��ʾ������ʽ���ڣ���שʽ���ǣ� of copies of A.
        //   The size of B is [size(A,1)*M, size(A,2)*N].
        //  The statement repmat(A,N) creates an N-by-N tiling. 
        //   B = REPMAT(A,[M N]) accomplishes the same result as repmat(A,M,N).
        //   B = REPMAT(A,[M N P ...]) tiles the array A to produce a 
        //   multidimensional array B composed of copies of A. The size of B is 
        //   [size(A,1)*M, size(A,2)*N, size(A,3)*P, ...].
        //
        //   REPMAT(A,M,N) when A is a scalar(����) is commonly used to produce an M-by-N
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
        /// ����¾���
        /// </summary>
        /// <param name="A">�¾����ֵ</param>
        /// <param name="M">�¾��������</param>
        /// <param name="N">�¾�������</param>
        /// <param name="B">���ɵ��¾���</param>
        /// <returns>��ȡ�ɹ�����true������false</returns>
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
        /// ����1 ����¾������A��һά���飬��ô��A��Ϊ1*P�Ķ�ά���鴦��
        /// </summary>
        /// <param name="A">�¾����ֵ</param>
        /// <param name="M">�¾�����в���A.GetLength(0)</param>
        /// <param name="N">�¾����в���A.GetLength(1)</param>
        /// <param name="B">���ɵ��¾���</param>
        /// <returns>��ȡ�ɹ�����true������false</returns>
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
        /// ��������Ƿ����ֵΪtrue��Ԫ��
        /// </summary>
        /// <param name="x">�������</param>
        /// <returns>����ֵΪtrue����trueֵ�����򷵻�false</returns>
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
        /// matlab��reshape��A,ROW,COL�������ǽ�����A�����µ�ROW*COL����
        /// ��ԭʼ�����ȡ������������˳���ȡ�����¾�����Ҳ���������ȴ洢
        /// </summary>
        /// <param name="x"></param>
        /// <param name="rows">�¾��������</param>
        /// <param name="cols">�¾��������</param>
        /// <returns>ִ�гɹ������¾��󣬷��򷵻�null</returns>
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
        ///����1 matlab��reshape��A,ROW,COL�������ǽ�����A�����µ�ROW*COL����
        /// ��ԭʼ�����ȡ������������˳���ȡ�����¾�����Ҳ���������ȴ洢
        /// </summary>
        /// <param name="x"></param>
        /// <param name="rows">�¾��������</param>
        /// <param name="cols">�¾��������</param>
        /// <returns>ִ�гɹ������¾��󣬷��򷵻�null</returns>
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

        #region ��������
        /// <summary>
        ///  �������뱣�����ݣ�Ĭ�ϱ���С�������λ
        /// </summary>
        /// <param name="datas">��Ҫ�������������</param>
        /// <returns>ִ�гɹ�����������������ݣ����򷵻�null</returns>
        public static double[,] ReviseDatas(double[,] datas)
        {// �������ڵı�Ҫ�ԣ���Ϊ������ת�����������ݴ��ھ��ȵ���ʧ����ɲ���Ҫ�Ĵ���
            try
            {// double Ĭ�� ���ݱ�����С�������λ
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
        ///  �������뱣�����ݣ�Ĭ�ϱ���С�������λ
        /// </summary>
        /// <param name="datas">��Ҫ�������������</param>
        /// <returns>ִ�гɹ�����true�����򷵻�false</returns>
        public static bool ReviseDatas(ref double[,] datas)
        {// �������ڵı�Ҫ�ԣ���Ϊ������ת�����������ݴ��ھ��ȵ���ʧ����ɲ���Ҫ�Ĵ���
            try
            {// double Ĭ�� ���ݱ�����С�������λ
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
        ///  �������뱣������
        /// </summary>
        /// <param name="datas">��Ҫ�������������</param>
        /// <param name="decimals">���ݱ�����С������λ��</param>
        /// <returns>ִ�гɹ�����������������ݣ����򷵻�null</returns>
        public static double[,] ReviseDatas(double[,] datas, int decimals)
        {// �������ڵı�Ҫ�ԣ���Ϊ������ת�����������ݴ��ھ��ȵ���ʧ����ɲ���Ҫ�Ĵ���
            try
            {// double Ĭ�� ���ݱ�����С�������λ
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
        ///  �������뱣������
        /// </summary>
        /// <param name="datas">��Ҫ�������������</param>
        /// <param name="decimals">���ݱ�����С������λ��</param>
        /// <returns></returns>
        public static bool ReviseDatas(ref double[,] datas, int decimals)
        {// �������ڵı�Ҫ�ԣ���Ϊ������ת�����������ݴ��ھ��ȵ���ʧ����ɲ���Ҫ�Ĵ���
            try
            {// double Ĭ�� ���ݱ�����С�������λ
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

        #region �жϾ���������
        /// <summary>
        ///  �����������жϣ�Ĭ�����ֵ1e-8
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>���������󷵻�true�����򷵻�false</returns>
        public static bool JudgePositiveDefinite(double[,] matrix)
        {// ��������˹�ֽ��ʱ����Ҫ�ж��������ԣ�ֻ�����������������˹�ֽ�����ݲ������壬����û������
            try
            {
                double erroFlag = 1e-8;
                double tmp = 0;
                // �жϾ����Ƿ���
                if (matrix == null) { return false; }
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                if (rows != cols) { return false; }
                // �жϾ����Ƿ�������Խ��߶Գ�
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
        ///  �����������ж�
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="erroFlag">����������ȵ�������Χ</param>
        /// <returns>���������󷵻�true�����򷵻�false</returns>
        public static bool JudgePositiveDefinite(double[,] matrix, double erroFlag)
        {// ��������˹�ֽ��ʱ����Ҫ�ж��������ԣ�ֻ�����������������˹�ֽ�����ݲ������壬����û������
            try
            {
                double tmp = 0;
                // �жϾ����Ƿ���
                if (matrix == null) { return false; }
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                if (rows != cols) { return false; }
                // �жϾ����Ƿ�������Խ��߶Գ�
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
