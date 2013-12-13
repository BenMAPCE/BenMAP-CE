using System;
using System.Collections.Generic;
using System.Text;

namespace ESIL.Kriging
{
   public static class CellStrMatch
    {
        #region C:\Program Files\MATLAB\R2009a\toolbox\matlab\strfun\@cell\strmatch
        // STRMATCH Cell array based string matching.
        // Implementation of STRMATCH for cell arrays of strings.  See
        // STRMATCH for more info.

        // Loren Dean 9/19/95
        // $Revision: 1.15.4.5 $
        #endregion

        /// <summary>
        /// 比较字符串是否和数组中的某个元素匹配
        /// </summary>
        /// <param name="str">匹配的字符串</param>
        /// <param name="strs">匹配的数组</param>
        /// <param name="flag">标志参数</param>
        /// <returns></returns>
        public static int GetMatchString(string str, string[] strs, ref int flag)
        {
            try
            {
                int nargin = flag;
                if (nargin < 2 || nargin > 3) { return -1; }

                return 0;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return -1;
            }
        }

        /// <summary>
        /// 重载1
        /// </summary>
        /// <param name="str">匹配的字符串</param>
        /// <param name="strs">匹配的数组</param>
        /// <param name="flag">标志参数</param>
        /// <returns></returns>
        public static int GetMatchString(string str, string[] strs)
        {
            // 与数组匹配的字符串在数字中的索引位置：-1表示不匹配
            int matchIndex = -1;
            int flag = 2;
            try
            {
                if (strs == null || strs.Length == 0)
                { return matchIndex; }
                int nargin = 2;
                if (nargin == 2)
                { matchIndex = Strmatch.GetStrMatch(str, strs); }
                else
                {
                   matchIndex= GetMatchString(str, strs, ref flag);
                }

                return matchIndex;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return matchIndex;
            }
        }
    }//class
}
