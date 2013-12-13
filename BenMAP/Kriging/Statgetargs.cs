using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace ESIL.Kriging
{
    public static class Statgetargs
    {
        #region 类的说明
        //  STATGETARGS Process parameter name/value pairs for statistics functions
        //  [EID,EMSG,A,B,...]=STATGETARGS(PNAMES,DFLTS,'NAME1',VAL1,'NAME2',VAL2,...)
        //  accepts a cell array PNAMES of valid parameter names, a cell array
        //  DFLTS of default values for the parameters named in PNAMES, and
        //  additional parameter name/value pairs.  Returns parameter values A,B,...
        //  in the same order as the names in PNAMES.  Outputs corresponding to
        //  entries in PNAMES that are not specified in the name/value pairs are
        //  set to the corresponding value from DFLTS.  If nargout is equal to
        //  length(PNAMES)+1, then unrecognized name/value pairs are an error.  If
        //  nargout is equal to length(PNAMES)+2, then all unrecognized name/value
        //  pairs are returned in a single cell array following any other outputs.

        //  EID and EMSG are empty if the arguments are valid.  If an error occurs,
        //  EMSG is the text of an error message and EID is the final component
        //  of an error message id.  STATGETARGS does not actually throw any errors,
        //  but rather returns EID and EMSG so that the caller may throw the error.
        //  Outputs will be partially processed after an error occurs.

        //  This utility is used by some Statistics Toolbox functions to process
        //  name/value pair arguments.

        //  Example:
        //     pnames = {'color' 'linestyle', 'linewidth'}
        //      dflts  = {    'r'         '_'          '1'}
        //      varargin = {{'linew' 2 'nonesuch' [1 2 3] 'linestyle' ':'}
        //      [eid,emsg,c,ls,lw] = statgetargs(pnames,dflts,varargin{:})    % error
        //      [eid,emsg,c,ls,lw,ur] = statgetargs(pnames,dflts,varargin{:}) % ok

        //  $Revision: 1.4.2.2 $  $Date: 2008/03/13 17:42:40 $ 

        //We always create (nparams+2) outputs:
        //   one each for emsg and eid
        //   nparams varargs for values corresponding to names in pnames
        //If they ask for one more (nargout == nparams+3), it's for unrecognized
        //names/values
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pnames"></param>
        /// <param name="dflts">一次调用时默认的返回值</param>
        /// <param name="varargin">输入参数的个数，与2求模不为0就提示出错</param>
        /// <returns></returns>
        public static Dictionary<string, string> StatGetargs(string[] pnames, string[] dflts, int varargin)
        {
            try
            {
                Dictionary<string, string> statgetarg = new Dictionary<string, string>();
                string emsg = "";
                string eid = "";
                int nparams = pnames.Length;
                string[] varargout = new string[dflts.Length];
                dflts.CopyTo(varargout, 0);
                string[] unrecog = new string[1];
                int nargs = varargin;
                string maxiter = dflts[0];
                string crit = dflts[1];
                string dosmooth = dflts[2];
                // 用于存储参数panames中任意一个元素
                string pname = "";
                //string i = "";

                // Must have name/value pairs
                if (nargs % 2 != 0)
                {
                    eid = "WrongNumberArgs";
                    emsg = "Wrong number of arguments.";
                }
                else
                { // Process name/value pairs
                    for (int j = 1; j <= nargs; j++)
                    {
                        pname = pnames[j];
                        // 调用C:\Program Files\MATLAB\R2009a\toolbox\matlab\strfun\@cell

                        j++;
                    }
                }

                statgetarg.Add("eid", eid);
                statgetarg.Add("emsg", emsg);
                statgetarg.Add("maxiter",maxiter);
                statgetarg.Add("crit", crit);
                statgetarg.Add("dosmooth", dosmooth);
                return statgetarg;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// 重载1（第一次调用） emsg只是错误提示，所以在本程序代码中省略
        /// </summary>
        /// <param name="pnames"></param>
        /// <param name="dflts">一次调用时默认的返回值</param>
        /// <param name="varargin">输入的可变参数：本次重载为字符串数组值为：“iterations”“50”</param>
        /// <returns>执行成功返回maxiter的值，否则返回false</returns>
        public static string StatGetargs(string[] pnames, string[] dflts,ref string eid,ref string crit,ref string dosmooth)
        {
            string maxiter = "";
            try
            {
                eid = "";
                int nargs = 0;
                int nparams = pnames.Length;
                string[] varargout = new string[dflts.Length];
                dflts.CopyTo(varargout, 0);
                string[] unrecog = new string[1];
                 crit = dflts[1];
                dosmooth = dflts[2];
                // 用于存储参数panames中任意一个元素
                string pname = "";
                string i = "";

                // Must have name/value pairs
                if (nargs % 2 != 0)
                {
                    eid = "WrongNumberArgs";
                }
                else
                { // Process name/value pairs
                    for (int j = 1; j <= nargs; j++)
                    {
                        pname = pnames[j];
                       // 调用C:\Program Files\MATLAB\R2009a\toolbox\matlab\strfun\@cell\strmatch.m
                       // %i=1
                       //i = strmatch(lower(pname),pnames);
                        CellStrMatch.GetMatchString(pname.ToLower(), pnames);
                        j++;
                    }
                }
                return maxiter;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return null;
            }
        }
    }//class
}
