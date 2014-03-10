using System;
using System.Collections.Generic;
using System.Text;

namespace ESIL.Kriging
{
    public static class Statgetargs
    {
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
                string pname = "";

                if (nargs % 2 != 0)
                {
                    eid = "WrongNumberArgs";
                    emsg = "Wrong number of arguments.";
                }
                else
                {
                    for (int j = 1; j <= nargs; j++)
                    {
                        pname = pnames[j];

                        j++;
                    }
                }

                statgetarg.Add("eid", eid);
                statgetarg.Add("emsg", emsg);
                statgetarg.Add("maxiter", maxiter);
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

        public static string StatGetargs(string[] pnames, string[] dflts, ref string eid, ref string crit, ref string dosmooth)
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
                string pname = "";
                string i = "";

                if (nargs % 2 != 0)
                {
                    eid = "WrongNumberArgs";
                }
                else
                {
                    for (int j = 1; j <= nargs; j++)
                    {
                        pname = pnames[j];
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
    }
}
