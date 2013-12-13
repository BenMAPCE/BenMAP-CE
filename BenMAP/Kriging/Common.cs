using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;

namespace ESIL.Kriging
{
    #region Struct
    /// <summary>
    /// 设置参数,确定克里金算法采用何种拟合模型
    /// </summary>
    public struct CorParms
    {
        /// <summary>
        /// 1*因子数的数组
        /// </summary>
        public double[] scale;

        /// <summary>
        /// 因子数*1的double数组
        /// </summary>
        public double[] smoothness;

        /// <summary>
        /// correlation estimation method 相关估计方法：0->MLE;1->REML
        /// </summary>
        public int fittype;

        /// <summary>
        /// 拟合选择的模型:0->Gaussian;1->PowerExponential;2->Cubic
        /// </summary>
        public int ctype;

        /// <summary>
        /// 
        /// </summary>
        public double[] theta;

        /// <summary>
        /// 
        /// </summary>
        public double[] power;

    }
    #endregion



    public static class Common
    {
        /// <summary>
        /// eps 叫做机器的浮点运算误差限！PC机上eps的默认值为2.2204*10^(-16)
        /// 若某个量的绝对值小于eps，就认为这个量为0
        /// </summary>
        public static double eps = 2.2204e-016;

        /// <summary>
        /// 克里金算法拟合模型选择的依据参数
        /// 默认情况下：scale=无穷大；smoothness=无穷大；fittype=0；ctype=1
        /// </summary>
        public static CorParms corparms;

        #region LogError
        private static string CRLF = "\r\n";           // 结束符

        /// <summary>
        /// 重载1
        /// 记录发生的错误
        /// </summary>
        /// <param name="ex">Exception对象</param>,
        public static void LogError(Exception ex)
        {
            try
            {
                Console.WriteLine(ex.ToString());
                string place = ex.StackTrace.ToString();
                int pos = place.LastIndexOf("\n");
                //Debug.WriteLine(ex.Message+"\n"+msg+"\n"+ex.StackTrace.ToString());
                // 如果找不到,pos=-1,C#中字符索引是从0开始的
                string msg = CRLF + DateTime.Now.ToString() + CRLF + ex.Message + CRLF + place.Substring(pos + 1);
                //Debug.WriteLine(msg);
                AppendErr(GetLogPath(ex), msg);
                //Debug.WriteLine(System.Threading.Thread.GetDomain().GetAssemblies().ToString());
            }
            catch (Exception myEx)
            {
                //string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + "  " + "CommonClass.LogError,1";
                string errMsg = string.Format("{0}{1}{0}{2} CommonClass.LogError,1", CRLF, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), myEx.Message);
                Console.WriteLine(errMsg);
            }
        }
        /// <summary>
        /// 重载2
        /// 记录发生的错误
        /// </summary>
        /// <param name="ex">Exception对象</param>
        /// <param name="msg">附加的自定义信息</param>,
        public static void LogError(Exception ex, string msg)
        {
            try
            {
                string place = ex.StackTrace.ToString();
                int pos = place.LastIndexOf("\n");

                //Debug.WriteLine(ex.Message+"\n"+msg+"\n"+ex.StackTrace.ToString());
                // 如果找不到,pos=-1,C#中字符索引是从0开始的
                string myMsg = CRLF + DateTime.Now.ToString() + CRLF + ex.Message + CRLF + msg + CRLF + place.Substring(pos + 1);
                //Debug.WriteLine(myMsg);
                AppendErr(GetLogPath(ex), myMsg);
            }
            catch (Exception myEx)
            {
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + "  " + "CommonClass.LogError,2";
                Console.WriteLine(errMsg);
            }
        }

        /// <summary>
        /// 重载3
        /// 记录发生的错误
        /// </summary>
        /// <param name="msg">自定义的错误信息</param>,
        public static void LogError(string msg)
        {
            try
            {
                //Debug.WriteLine("\n"+msg+"  "+DateTime.Now.ToString());
                string myMsg = CRLF + DateTime.Now.ToString() + CRLF + msg;
                // 必须保证 c:\Err_log\目录存在,并且asp_net有权写入
                //AppendErr("c:\\Err_Log\\Err_.log",myMsg);
                AppendErr(GetLogPath(null), myMsg);
            }
            catch (Exception myEx)
            {
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + CRLF + "CommonClass.LogError,3";
                Console.WriteLine(errMsg);
            }
        }

        /// <summary>
        /// 从app.config(或web.config)的appSettings节中获取记录错误日志的文件全路径.<br />
        /// 如果配置节不存在,则在DataManager.dll所在目录下创建err_log.log
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>文件全路径</returns>
        private static string GetLogPath(Exception ex)
        {
            try
            {
                string logPath;
                // 用于Web程序必须保证 c:\Err_log\目录存在,并且asp_net有权写入
                //logPath="c:\\Err_Log\\Err_"+ex.Source+".log";

                // 以下两个路径都指向c:\windows\system32
                //logPath=Directory.GetCurrentDirectory()+"\\Err_"+ex.Source+".log";
                //logPath=ex.Source+".log";

                // 此做法将在组件所在目录记录错误日志
                //logPath = "Err_" + ex.Source + ".log";
                //logPath = Directory.GetCurrentDirectory() + "\\ErrorLog.log";
                //logPath = "ErrorLog.log";

                // Get the appSettings.
                // 即使配置文件里不存在appSettings这个节,也不会出错
                //System.Collections.Specialized.NameValueCollection appSettings = ConfigurationManager.AppSettings;

                //logPath = appSettings["errorLogFile"];

                //if (logPath == null)
                //{
                logPath = AppDomain.CurrentDomain.BaseDirectory + "\\err_log.log";
                logPath = logPath.Replace("\\\\", "\\");
                //}
                CheckAndCreateDir(logPath);
                return logPath;
            }
            catch (Exception myEx)
            {
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + CRLF + "CommonClass.GetLogPath";
                Console.WriteLine(errMsg);
                return "err_log.log";
            }
        }

        /// <summary>
        /// 检查会议的文件全路径,创建所有不存在的目录.(注:不创建文件)
        /// </summary>
        /// <param name="path">文件全路径,必须包含文件名.如: D:\Test\test.txt</param>
        private static void CheckAndCreateDir(string path)
        {
            try
            {
                string dir;
                int pos = path.LastIndexOf("\\");
                dir = path.Substring(0, pos);
                if (Directory.Exists(dir) == false)
                {
                    // 创建路径中 所有 不存在的目录
                    Directory.CreateDirectory(dir);
                }
                //return;
            }
            catch (Exception ex)
            {
                string errMsg = CRLF + DateTime.Now.ToString() + "\\rn" + ex.ToString() + "\r\npath=" + path + CRLF + "CommonClass.CheckAndCreateDir";
                Console.WriteLine(errMsg);
            }
        }

        /// <summary>
        /// 将错误信息追加到错误日志
        /// </summary>
        /// <param name="path">错误日志文件位置,如:"c:\\Err_Log\\err_myUtil.log"</param>
        /// <param name="msg">错误信息</param>
        private static void AppendErr(string path, string msg)
        {
            try
            {
                // 路径里的文件可以不存在,但目录必须存在,否则会报错?
                //,,,,StreamWriter writer;
                //,,,,if (File.Exists(path)) {
                //,,,,,writer=File.AppendText(path);
                //,,,,}else{
                //,,,,,writer=File.CreateText(path);
                //,,,,}
                StreamWriter writer = new StreamWriter(path, true);
                writer.WriteLine(msg);
                writer.Close();
                Console.WriteLine(msg);
            }
            catch (Exception myEx)
            {
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + CRLF + "CommonClass.AppendErr";
                Console.WriteLine(errMsg);
            }
        }
        #endregion

        #region 常用函数

        #region 二维数组相关的处理函数
        /// <summary>
        /// 初始化二维数组
        /// </summary>
        /// <param name="x">需要初始化的二维数组</param>
        /// <param name="n">二维数组的行数</param>
        /// <param name="p">二维数组的列数</param>
        /// <param name="initValue">二维数组的初始值</param>
        /// <returns>执行成功返回：true；否则返回：false</returns>
        public static bool Init2Dimensions(ref double[,] x, int n, int p, double initValue)
        {
            try
            {
                x = new double[n, p];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < p; j++)
                    {
                        x[i, j] = initValue;
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
        /// 重载1 用0到1之间的随机浮点数初始化二维数组
        /// </summary>
        /// <param name="x">需要初始化的二维数组</param>
        /// <param name="n">二维数组的行数</param>
        /// <param name="p">二维数组的列数</param>
        /// <returns>执行成功返回：true；否则返回：false</returns>
        public static bool Init2Dimensions(ref double[,] x, int n, int p)
        {
            try
            {
                Random random = new Random();
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < p; j++)
                    {
                        x[i, j] = random.NextDouble();
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
        /// 重载2 初始化二维数组
        /// </summary>
        /// <param name="x">需要初始化的二维数组</param>
        /// <param name="initValue">二维数组的初始值</param>
        /// <returns>执行成功返回：true；否则返回：false</returns>
        public static bool Init2Dimensions(ref double[,] x, double initValue)
        {
            try
            {
                int rowCount = x.GetLength(0);
                int colCount = x.GetLength(1);
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        x[i, j] = initValue;
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
        /// 重载3 初始化的过程中被初始化的数组长度可以大于或者等于初始值二维数组，
        /// 所以被初始化的二维数组，在传入时必须初始化行数和列数
        /// </summary>
        /// <param name="x">被初始化的二维数组</param>
        /// <param name="initValue">二维数组的初始值</param>
        /// <returns>执行成功返回：true；否则返回：false</returns>
        public static bool Init2Dimensions(ref double[,] x, double[,] initValues)
        {
            try
            {
                // 被初始化的矩阵的行列数
                int rowX = x.GetLength(0);
                int colX = x.GetLength(1);
                // 初始值矩阵的行列数
                int rows = initValues.GetLength(0);
                int cols = initValues.GetLength(1);
                if ((rowX < rows) || (colX < cols))
                { return false; }
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        x[i, j] = initValues[i, j];
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
        /// 获取二维数组的行数和列数
        /// </summary>
        /// <param name="x">需要获取行数和列数的二维数组</param>
        /// <returns></returns>
        public static int[] Get2DimensionsSize(double[,] x)
        {
            // 返回二维数组的行数和列数0->行数；1->对应列数
            int[] rowAndColCount = new int[2];
            try
            {
                rowAndColCount[0] = x.GetLength(0);
                rowAndColCount[1] = x.GetLength(1);
                return rowAndColCount;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return rowAndColCount;
            }
        }

        /// <summary>
        /// 获取二维数组的行数或者列数
        /// </summary>
        /// <param name="x">需要获取行数和列数的二维数组</param>
        /// <param name="rowOrCol">需要获取获取的行或者列：0->行；1->列</param>
        /// <returns></returns>
        public static int Get2DimensionsSize(double[,] x, int rowOrCol)
        {
            int count = 0;
            try
            {
                if (rowOrCol != 0 && rowOrCol != 1)
                { return count; }
                count = x.GetLength(rowOrCol);
                return count;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return count;
            }
        }

        /// <summary>
        /// 获得二维数组每列的最大值和最小值
        /// </summary>
        /// <param name="x">要获取每列最大值和最小值的二维数组</param>
        /// <param name="ranges">每列的最小值作为第一个元素，最大值作为第二个元素</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool Get2DimensionMaxMin(double[,] x, ref double[,] ranges)
        {
            try
            {
                // 二维数组的行数
                int n = Get2DimensionsSize(x, 0);
                // 二维数组的列数
                int p = Get2DimensionsSize(x, 1);
                for (int i = 0; i < p; i++)
                {
                    double min, max;
                    min = x[0, i];
                    max = x[0, i];
                    // 求x每列的最小和最大值
                    for (int j = 0; j < n; j++)
                    {
                        if (min > x[j, i])
                        { min = x[j, i]; }
                        if (max < x[j, i])
                        { max = x[j, i]; }
                    }
                    ranges[i, 0] = min;
                    ranges[i, 1] = max;
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
        ///重载1 获得二维数组每列的最大值和最小值
        /// </summary>
        /// <param name="x">要获取每列最大值和最小值的二维数组</param>
        /// <param name="minValues">二维数组每列的最小值</param>
        /// <param name="maxValues">二维数组每列的最大值</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool Get2DimensionMaxMin(double[,] x, ref double[] minValues, ref double[] maxValues)
        {
            try
            {
                int n = x.GetLength(0);
                int p = x.GetLength(1);
                minValues=new double[p];
                maxValues=new double[p];
                for (int i = 0; i < p; i++)
                {// 列
                    double min, max;
                    min = x[0, i];
                    max = x[0, i];
                    for (int j = 0; j < n; j++)
                    {// 循环行
                        if (min > x[j, i])
                        { min = x[j, i]; }
                        if (max < x[j, i])
                        { max = x[j, i]; }
                    }
                    minValues[i] = min;
                    maxValues[i] = max;
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

        #region 一维数组
        /// <summary>
        /// 初始化数组
        /// </summary>
        /// <param name="x">数组</param>
        /// <param name="value">值</param>
        /// <returns>初始化成功返回true；否则false</returns>
        public static bool InitArray(ref double[] x, double value)
        {
            try
            {
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = value;
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
        /// 重载1 用一个0~1之间的double数初始化数组
        /// </summary>
        /// <param name="x">数组</param>
        /// <returns>初始化成功返回true；否则false</returns>
        public static bool InitArray(ref double[] x)
        {
            try
            {
                Random random = new Random();
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = random.NextDouble();
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
        /// 重载2 初始化数组
        /// </summary>
        /// <param name="x">数组</param>
        /// <param name="value">值</param>
        /// <returns>初始化成功返回true；否则false</returns>
        public static bool InitArray(ref string[] x, string value)
        {
            try
            {
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = value;
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
        /// 重载3 初始化数组
        /// </summary>
        /// <param name="x">数组</param>
        /// <param name="value">值</param>
        /// <returns>初始化成功返回true；否则false</returns>
        public static bool InitArray(ref int[] x, int value)
        {
            try
            {
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = value;
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
        /// 获得数组的最大值和最小值
        /// </summary>
        /// <param name="x">要获取最大值和最小值的数组</param>
        /// <param name="minValues">数组的最小值</param>
        /// <param name="maxValues">数组的最大值</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool GetArrayMaxMin(double[] x, ref double minValues, ref double maxValues)
        {
            try
            {
                int count = x.Length;
                minValues = x[0];
                maxValues = x[0];
                for (int i = 0; i < count; i++)
                {
                    if (x[i] < minValues)
                    {
                        minValues = x[i];
                    }
                    if (x[i] > maxValues)
                    {
                        maxValues = x[i];
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
        /// 获得数组的最大值或者最小值
        /// </summary>
        /// <param name="x">要获取最大值或最小值的数组</param>
        /// <param name="isMax">是否为获取数组的最大值：true 获取最大值，false获取最小值</param>
        /// <param name="minmax">数组的最值</param>
        /// <param name="index">最值元素在数组中的索引</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool GetArrayMaxMin(double[] x, bool isMax, ref double minmax, ref int index)
        {
            try
            {
                int count = x.Length;
                if (count == 0)
                { return false; }
                minmax = x[0];
                index = 0;
                if (isMax)
                {// 求最大值
                    for (int i = 0; i < count; i++)
                    {
                        if (x[i] > minmax)
                        {
                            minmax = x[i];
                            index = i;
                        }
                    }
                }
                else
                {// 求最小值
                    for (int i = 0; i < count; i++)
                    {
                        if (x[i] < minmax)
                        {
                            minmax = x[i];
                            index = i;
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
        /// 一维数组排序
        /// </summary>
        /// <param name="x">需要排序数组</param>
        /// <param name="newArray">排序后的数组</param>
        /// <param name="originIndexs">新数组:newArray每个元素在原来数组:x中的索引值</param>
        /// <returns>执行成功返回：true；否则：false</returns>
        public static bool SortArray(double[] x, ref double[] newArray, ref int[] originIndexs)
        {
            bool ok = false;
            try
            {
                newArray = new double[x.Length];
                x.CopyTo(newArray, 0);
                ok = BubbleSort(ref newArray);
                if (!ok) { return ok; }
                for (int i = 0; i < newArray.Length; i++)
                {
                    // 新数组的元素在原来数组中的索引值：初始值-1表示没有找到对应元素
                    int tmpIndex = -1;
                    double tmpValue = newArray[i];
                    for (int j = 0; j < x.Length; j++)
                    {
                        if (x[j].CompareTo(tmpValue) == 0)
                        {
                            tmpIndex = j;
                            break;
                        }
                    }
                    originIndexs[i] = tmpIndex;
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
        /// 冒泡排序：将一位数组按从小到大排序
        /// </summary>
        /// <param name="x">待排序的一维数组</param>
        /// <returns>执行成功返回true；否则false</returns>
        public static bool BubbleSort(ref double[] x)
        {
            try
            {
                int i, j;
                int m = x.Length - 1;
                int k = 0;
                double tmp = 0.0;
                while (k < m)
                {
                    j = m - 1;
                    m = 0;
                    for (i = k; i <= j; i++)
                    {
                        if (x[i] > x[i + 1])
                        {
                            tmp = x[i];
                            x[i] = x[i + 1];
                            x[i + 1] = tmp;
                            m = i;
                        }// if
                    }// for_i
                    j = k + 1;
                    k = 0;
                    for (i = m; i >= j; i--)
                    {
                        if (x[i - 1] > x[i])
                        {
                            tmp = x[i];
                            x[i] = x[i - 1];
                            x[i - 1] = tmp;
                            k = i;
                        }// if
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
        /// 直接插入法排序， 暂时还不完善
        /// </summary>
        /// <param name="x"></param>
        /// <param name="newArray"></param>
        /// <param name="originIndexs"></param>
        /// <returns></returns>
        public static bool InsertSort(double[] x, ref double[] newArray, ref int[] originIndexs)
        {
            try
            {
                bool ok = false;
                newArray = new double[x.Length];


                return ok;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return false;
            }
        }
        #endregion

        #region Matalab 中的一些简短M文件

        /// <summary>
        /// 遍历数组中每个元素，如果元素>=0返回1，否则返回-1
        /// </summary>
        /// <param name="x"></param>
        /// <returns>执行成功返回遍历后的数组，否则返回null</returns>
        public static int[] Nonzerosign(double[] x)
        {
            // %NONZEROSIGN Signum function excluding zero
            //%   For each element of X, NONZEROSIGN(X) returns 1 if the element
            //%   is greater than or equal to zero and -1 if it is
            //%   less than zero. NONZEROSIGN differs from SIGN in that NONZEROSIGN(0)
            //%   returns 1, while SIGN(0) returns 0.
            bool ok = false;
            int count = x.Length;
            int[] y = new int[count];
            try
            {
                ok = MatrixCompute.Ones(ref y);
                if (!ok)
                { return null;}
                for (int i = 0; i < count; i++)
                {
                    if (x[i] >= 0)
                    { y[i] = 1; }
                    else
                    { y[i] = -1; }
                }
                return y;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// C:\Program Files\MATLAB\R2009a\toolbox\shared\optimlib\fwdFinDiffInsideBnds.m
        /// </summary>
        /// <param name="xC"></param>
        /// <param name="lb"></param>
        /// <param name="ub"></param>
        /// <param name="delta"></param>
        /// <param name="dim"></param>
        /// <param name="DiffMinChange"></param>
        /// <returns></returns>
        public static double FwdFinDiffInsideBnds(double xC, double lb, double ub, double delta, int dim, double DiffMinChange)
        {
            //TODO:del 是delta的简写，为了避免参数和返回值的混搅
            double del = delta;
            try
            {
                // fwdFinDiffInsideBnds Helper function for forward finite differences.
                // It attempts to keep the perturbed point in the forward finite-difference calculation inside the bounds lb and ub. 
                // Inputs xC and delta are scalars, thus this function is meant to be called in a loop, once for each dimension dim of the decision variables.

                //% Forward differences
                //% Need lb ~= ub, and lb <= xC <= ub to enforce bounds.
                if (lb != ub && xC >= lb && xC <= ub)
                {
                    if (xC + delta > ub || xC + delta < lb)
                    {// % outside bound?
                        delta = -delta;
                        if (xC + delta > ub || xC + delta < lb)
                        {// % outside other bound?
                            double newDelta = 0;
                            int indsign = 0;
                            if ((xC - lb) < (ub - xC))
                            {
                                newDelta = ub - xC;
                                indsign = 2;
                            }
                            else
                            {
                                newDelta = xC - lb;
                                indsign = 1;
                            }
                            if (newDelta >= DiffMinChange)
                            {// % make sure sign is correct

                                del = Math.Pow(-1, indsign) * newDelta;

                            }
                            else
                            {
                                return 0.0;
                            }
                        }
                    }
                }
                return del;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return del;
            }
        }
        #endregion

        #endregion

    }
}
