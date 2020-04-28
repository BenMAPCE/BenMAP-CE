using System;
using System.Collections.Generic;
using System.Text;

namespace ESIL.Kriging
{
	public static class CellStrMatch
	{
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

		public static int GetMatchString(string str, string[] strs)
		{
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
					matchIndex = GetMatchString(str, strs, ref flag);
				}

				return matchIndex;
			}
			catch (Exception ex)
			{
				Common.LogError(ex);
				return matchIndex;
			}
		}
	}
}
