using System;
using System.Collections.Generic;
using System.Text;

namespace ESIL.Kriging
{
    public static class Strmatch
    {
        public static int GetStrMatch(string str, string[] strs, int flag)
        {
            int matchIndex = -1;
            try
            {
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

        public static int GetStrMatch(string str, string[] strs)
        {
            int matchIndex = -1;
            try
            {
                int m = strs.Length;
                int n = str.Length;
                int len = str.Length;
                bool exactMatch = false;

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

        public static int[] GetStrMatch(string str, string[] strs, ref bool isOK)
        {
            List<int> match = new List<int>();
            int[] matchValues = new int[0];
            try
            {
                int m = strs.Length;

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

        public static int GetStrMatch(string str, string[] strs, string flag)
        {
            int matchIndex = -1;
            try
            {
                int m = strs.Length;
                int n = str.Length;
                int len = str.Length;
                bool exactMatch = false;

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

        public static bool StrCmp(string str, string[] strs)
        {
            bool match = false;
            try
            {
                int m = strs.Length;
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

        public static bool StrCmp(string str1, string str2)
        {
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

        public static int Strcmpi(string str1, string str2)
        {
            int match = -1;
            try
            {
                str1 = str1.ToLower().Trim();
                str2 = str2.ToLower().Trim();
                if (str1 == str2)
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

        public static bool Strcmpi(string str1, string str2, bool flag)
        {
            bool match = false;
            try
            {
                str1 = str1.ToLower().Trim();
                str2 = str2.ToLower().Trim();
                if (str1 == str2)
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

    }
}
