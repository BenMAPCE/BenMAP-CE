using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace ESIL.Kriging
{

    public static class LHSDesign
    {
        #region C:\Program Files\MATLAB\R2009a\toolbox\stats
        // LHSDESIGN Generate a latin hypercube sample.
        // X=LHSDESIGN(N,P) generates a latin hypercube sample X containing N
        // values on each of P variables.  For each column, the N values are
        // randomly(随意地，任意地) distributed with one from each interval (0,1/N), (1/N,2/N),
        //..., (1-1/N,1), and they are randomly permuted（排列，变更）.

        // X=LHSDESIGN(...,'PARAM1',val1,'PARAM2',val2,...) specifies parameter
        // name/value pairs to control the sample generation.  Valid parameters
        // are the following:

        //    Parameter    Value
        //    'smooth'     'on' (the default) to produce points as above, or
        //                 'off' to produces points at the midpoints of
        //                 the above intervals:  .5/N, 1.5/N, ..., 1-.5/N.
        //    'iterations' The maximum number of iterations to perform in an
        //                 attempt to improve the design (default=5)
        //    'criterion'  The criterion to use to measure design improvement,
        //                 chosen from 'maximin' (the default) to maximize the
        //                 minimum distance between points, 'correlation' to
        //                 reduce correlation, or 'none' to do no iteration.

        // Latin hypercube（超立方体） designs are useful when you need a sample that is
        // random but that is guaranteed（有保证的） to be relatively uniformly 一致的 distributed
        // over each dimension.

        // Example:  The following commands show that the output from lhsdesign
        //           looks uniformly distributed in two dimensions, but too
        //           uniform (non-random) in each single dimension.  Repeat the
        //           same commands with x=rand(100,2) to see the difference.

        //    x = lhsdesign(100,2);
        //    subplot(2,2,1); plot(x(:,1), x(:,2), 'o');
        //    subplot(2,2,2); hist(x(:,2));
        //    subplot(2,2,3); hist(x(:,1));

        //  See also LHSNORM, UNIFRND.
        //  $Revision: 1.6.4.5 $  $Date: 2008/06/20 09:05:12 $
        #endregion

        #region LhsDesign函数和其重载函数

        /// <summary>
        /// 第一次调用
        /// </summary>
        /// <param name="n">行数：每个变量对应的值</param>
        /// <param name="p">列数：varables 变量数</param>
        /// <param name="varargin">输入变量个数：</param>
        /// <returns></returns>
        public static double[,] LhsDesign(int n, int p)
        {
            //double[,] lhsDesign = new double[5 * p, p];
            try
            {
                bool ok = false;
                string eid = "";
                string emsg = "";
                string tmp = "";
                string crit = "";
                string dosmooth = "";
                // 匹配索引，默认为没有匹配：-1
                int matchIndex = -1;
                string[] okargs = { "iterations", "criterion", "'smooth" };
                string[] defaults = { "NaN", "maximin", "on" };
                int maxiter = -1;

                // 调用C:\Program Files\MATLAB\R2009a\toolbox\stats\private\statgetargs.m
                tmp = Statgetargs.StatGetargs(okargs, defaults, ref eid, ref crit, ref dosmooth);
                if ((tmp == null) || (tmp.Length == 0))
                { tmp = "NaN"; }
                else if (tmp == "NaN")
                { // maxiter的整型变量值
                    maxiter = 5;
                }
                else
                {
                    ok = int.TryParse(tmp, out maxiter);
                    if (!ok)
                    { return null; }
                }

                string[] okcrit = { "none", "maximin", "correlation" };
                matchIndex = CellStrMatch.GetMatchString(crit.ToLower(), okcrit);
                crit = okcrit[matchIndex];
                if (dosmooth.Length == 0) { dosmooth = "on"; }

                // Start with a plain lhs sample over a grid
                double[,] X = GetSample(n, p, dosmooth);

                // Create designs, save best one
                if (crit == "none" || Common.Get2DimensionsSize(X, 0) < 2)
                { maxiter = 0; }
                double bestScore, newscore;
                double[,] x = new double[n, p];
                //double[,] aaa = new double[p, 2];
                //Common.Get2DimensionMaxMin(X, ref aaa);
                switch (crit)
                {
                    case "maximin":
                        bestScore = GetScore(X, crit);

                        for (int j = 2; j <= maxiter; j++)
                        {
                            x = GetSample(n, p, dosmooth);
                            newscore = GetScore(x, crit);
                            if (newscore > bestScore)
                            {
                                X = new double[n, p];
                                ok = Common.Init2Dimensions(ref X, x);
                                if (!ok)
                                { return null; }
                                bestScore = newscore;
                            }
                        }
                        break;
                    case "correlation":
                        break;
                    case "":
                        break;

                }
                return X;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }
        #endregion



        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">二维数组的行数</param>
        /// <param name="p">二维数组的列数</param>
        /// <param name="dosmooth"></param>
        /// <returns>返回得到的二维数组</returns>
        public static double[,] GetSample(int n, int p, string dosmooth)
        {
            double[,] x = new double[n, p];
            try
            {
                // 用0-1之间的double随机数初始化二维数组
                Common.Init2Dimensions(ref x, n, p);
                double[] r = new double[p];
                for (int i = 0; i < p; i++)
                {
                    double[] tmpColValues = new double[n];
                    for (int row = 0; row < n; row++)
                    {
                        tmpColValues[row] = x[row, i];
                    }
                    // 循环调用rank函数处理x的每列
                    r = Rank(tmpColValues);
                    for (int row = 0; row < n; row++)
                    {
                        x[row, i] = r[row];
                    }

                }
                // 判断dosmooth=on是否成立,等于执行if，否则执行else
                if (dosmooth == "on")
                {
                    // 对x处理:对应二维数组减去一个0~1之间的浮点数
                    double[,] tmp2Dims = new double[n, p];
                    Common.Init2Dimensions(ref tmp2Dims, n, p);
                    for (int row = 0; row < n; row++)
                    {
                        for (int col = 0; col < p; col++)
                        {
                            x[row, col] = (x[row, col] - tmp2Dims[row, col]) / n;
                        }
                    }
                }
                else
                {
                    // 二维数组每个元素减去0.5
                    for (int row = 0; row < n; row++)
                    {
                        for (int col = 0; col < p; col++)
                        {
                            x[row, col] = x[row, col] - 0.5;
                        }
                    }
                }
                //// 二维数组每个元素除以n
                //for (int row = 0; row < n; row++)
                //{
                //    for (int col = 0; col < p; col++)
                //    {
                //        x[row, col] = x[row, col] / n;
                //    }
                //}

                return x;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// % compute score function, larger is better
        /// </summary>
        /// <param name="x"></param>
        /// <param name="crit"></param>
        /// <returns></returns>
        public static double GetScore(double[,] x, string crit)
        {
            double s = 0;
            try
            {
                int n = Common.Get2DimensionsSize(x, 0);
                if (n < 2)
                {
                    // % score is meaningless with just one point
                    s = 0;
                    return s;
                }
                switch (crit)
                {
                    //  % Minimize the sum of between-column squared correlations
                    case "correlation":

                        break;
                    case "maximin":
                        // 调用C:\Program Files\MATLAB\R2009a\toolbox\stats\pdist.m
                        double tmp = 0;
                        Common.GetArrayMaxMin(Pdist.GetPDist(x), ref s, ref tmp);
                        break;
                }
                return s;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return s;
            }
        }

        /// <summary>
        /// % 传入的参数必须是一位数组此函数才能正确调用,对getsample的返回值X每一列转换
        ///% Similar to tiedrank, but no adjustment for ties here
        /// </summary>
        /// <param name="x">getsample的返回值的一列</param>
        /// <returns>执行成功返回：转换后的数组，否则返回null</returns>
        public static double[] Rank(double[] x)
        {
            double[] r = new double[x.Length];
            double[] sx = new double[x.Length];
            int[] rowidx = new int[x.Length];
            try
            {
                Common.SortArray(x, ref sx, ref rowidx);
                for (int i = 0; i < rowidx.Length; i++)
                {
                    r[rowidx[i]] = i + 1;
                }
                return r;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }


    }//
}
