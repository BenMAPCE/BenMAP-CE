using System;
using System.Collections.Generic;
using System.Text;

namespace ESIL.Kriging
{
    public static class Strmatch
    {
        #region C:\Program Files\MATLAB\R2009a\toolbox\matlab\strfun\strmatch.m
        //  STRMATCH Find possible matches for string.
        //  I = STRMATCH(STR, STRARRAY) looks through the rows of the character
        //  array or cell array of strings STRARRAY to find strings that begin
        //  with the string contained in STR, and returns the matching row indices. 
        //  Any trailing space characters in STR or STRARRAY are ignored when 
        //  matching. STRMATCH is fastest when STRARRAY is a character array. 

        //  I = STRMATCH(STR, STRARRAY, 'exact') compares STR with each row of
        //  STRARRAY, looking for an exact match of the entire strings. Any 
        //  trailing space characters in STR or STRARRAY are ignored when matching.
        //  Examples
        //    i = strmatch('max',strvcat('max','minimax','maximum'))
        //  returns i = [1; 3] since rows 1 and 3 begin with 'max', and
        //    i = strmatch('max',strvcat('max','minimax','maximum'),'exact')
        //  returns i = 1, since only row 1 matches 'max' exactly.

        //  See also STRFIND, STRVCAT, STRCMP, STRNCMP, REGEXP.


        //The cell array implementation is in @cell/strmatch.m
        #endregion

        /// <summary>
        /// 在字符串数组中寻找指定字符串匹配的元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="strs">字符数组</param>
        /// <param name="flag"></param>
        /// <returns>执行成功返回字符串在数组中的索引，否则返回-1</returns>
        public static int GetStrMatch(string str, string[] strs, int flag)
        {
            // 与数组匹配的字符串在数字中的索引位置：-1表示不匹配
            int matchIndex = -1;
            try
            {
                // 在matlab中此数组时列向量即三行一列的数组
                int m = strs.Length;
                int n = str.Length;
                int len = str.Length;

                return matchIndex;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return matchIndex;
            }
        }

        /// <summary>
        /// 重载1
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strs"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static int GetStrMatch(string str, string[] strs)
        {
            // 与数组匹配的字符串在数字中的索引位置：-1表示不匹配
            int matchIndex = -1;
            try
            {
                // 在matlab中此数组时列向量即三行一列的数组
                int m = strs.Length;
                int n = str.Length;
                int len = str.Length;
                bool exactMatch = false;

                //% Special treatment for empty STR or STRS to avoid
                //% warnings and error below
                if (len == 0)
                { str = "iterations"; }
                if (len > n)
                {
                    matchIndex = -1;
                    return matchIndex;
                }
                else
                {

                    if (exactMatch && (len < n))
                    {
                        return matchIndex;
                    }
                }

                // % walk from end of strs array and search for row starting with str.
                for (int i = 0; i < strs.Length; i++)
                {
                    if (strs[i].ToLower() == str)
                    {
                        matchIndex = i;
                        break;
                    }
                }
                return matchIndex;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return matchIndex;
            }
        }

        /// <summary>
        /// 重载2 根据匹配情况返回数据，没有匹配返回第一个元素=-1
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strs"></param>
        /// <param name="isOK">有匹配返回true，没找到匹配元素返回false</param>
        /// <returns></returns>
        public static int[] GetStrMatch(string str, string[] strs, ref bool isOK)
        {
            // 与数组匹配的字符串在数字中的索引位置：-1表示不匹配
            List<int> match = new List<int>();
            int[] matchValues = new int[0];
            try
            {
                // 在matlab中此数组时列向量即三行一列的数组
                int m = strs.Length;

                // % walk from end of strs array and search for row starting with str.
                for (int i = 0; i < strs.Length; i++)
                {
                    if (strs[i].ToLower() == str)
                    {
                        match.Add(i);
                    }
                }
                if (match.Count == 0)
                {
                    match.Add(-1);
                    matchValues = new int[1];
                    matchValues[0] = -1;
                    isOK = false;
                }
                else
                {
                    matchValues = new int[match.Count];
                    match.CopyTo(matchValues, 0);
                    isOK = true;
                }
                return matchValues;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                isOK = false;
                return matchValues;
            }
        }

        /// <summary>
        /// 重载3 但字符串在字符串数组中 有多个匹配元素时调用
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strs"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static int GetStrMatch(string str, string[] strs, string flag)
        {
            // 与数组匹配的字符串在数字中的索引位置：-1表示不匹配
            int matchIndex = -1;
            try
            {
                // 在matlab中此数组时列向量即三行一列的数组
                int m = strs.Length;
                int n = str.Length;
                int len = str.Length;
                bool exactMatch = false;

                //% Special treatment for empty STR or STRS to avoid
                //% warnings and error below
                if (len == 0)
                { str = "iterations"; }
                if (len > n)
                {
                    matchIndex = -1;
                    return matchIndex;
                }
                else
                {

                    if (exactMatch && (len < n))
                    {
                        return matchIndex;
                    }
                }

                // % walk from end of strs array and search for row starting with str.
                for (int i = 0; i < strs.Length; i++)
                {
                    if (strs[i].ToLower() == str)
                    {
                        matchIndex = i;
                        break;
                    }
                }
                return matchIndex;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return matchIndex;
            }
        }

        /// <summary>
        /// 字符串是否与数组中的元素匹配
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strs"></param>
        /// <returns>执行成功返回true；否则false</returns>
        public static bool StrCmp(string str, string[] strs)
        {
            // 字符串和数组匹配与否：如果数组中有元素和str匹配返回true；否则返回false
            bool match = false;
            try
            {
                int m = strs.Length;
                // % walk from end of strs array and search for row starting with str.
                for (int i = 0; i < m; i++)
                {
                    if (strs[i].ToLower() == str)
                    {
                        match = true;
                        break;
                    }
                }
                return match;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return match;
            }
        }

        /// <summary>
        /// 重载1 字符串是否与数组中的元素匹配
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strs"></param>
        /// <returns>执行成功返回true；否则false</returns>
        public static bool StrCmp(string str1, string str2)
        { // 字符串和数组匹配与否：如果数组中有元素和str匹配返回true；否则返回false
            bool match = false;
            try
            {
                if (str1 == str2)
                { match = true; }
                return match;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return match;
            }
        }

        /// <summary>
        /// 判断两个字符串是否相等（忽略大小写）：-1 出现异常；0 不匹配；1 相等
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns>执行成功返回true；否则false</returns>
        public static int Strcmpi(string str1, string str2)
        { // 判断两个字符串是否相等（忽略大小写）：-1 出现异常；0 不匹配；1 相等
            int match = -1;
            try
            {
                str1 = str1.ToLower().Trim();
                str2 = str2.ToLower().Trim();
                if (str1==str2)
                {
                    match = 1;
                }
                else
                {
                    match = 0;
                }
                return match;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return match;
            }
        }

        /// <summary>
        /// 重载1 判断两个字符串是否相等（忽略大小写）：-1 出现异常；0 不匹配；1 相等
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="flag">有flag的获取的返回值是bool值</param>
        /// <returns>执行成功返回true；否则false</returns>
        public static bool Strcmpi(string str1, string str2, bool flag)
        {
            // 判断两个字符串是否相等（忽略大小写）：-1 出现异常；0 不匹配；1 相等
            bool match = false;
            try
            {
                str1 = str1.ToLower().Trim();
                str2 = str2.ToLower().Trim();
                if (str1==str2)
                {
                    match = true;
                }
                else
                {
                    match = false;
                }
                return match;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return match;
            }
        }

    }//class
}
