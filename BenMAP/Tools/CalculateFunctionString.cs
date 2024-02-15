using System;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using Microsoft.CSharp;
using System.Linq;
using System.Collections.Generic;

namespace BenMAP.Tools
{
    class CalculateFunctionString
    {
        private string _CharpCode = "";
        public object BaseLineEval(string crid, string cCharpCode, double a, double b, double c, double beta, double deltaq, double q0, double q1, double incidence, double pop, double prevalence, Dictionary<string, double> dicSetupVariables)
        {
            try
            {
                MethodInfo mi = null;
                object tmp = null;
                List<object> lstParam = new List<object>() { a, b, c, beta, deltaq, q0, q1, incidence, pop, prevalence };
                if (dicSetupVariables != null && dicSetupVariables.Count > 0)
                {
                    int j = 0;
                    while (j < dicSetupVariables.Count)
                    {
                        lstParam.Add(dicSetupVariables.ToList()[j].Value);
                        j++;
                    }
                }

                tmp = dicBaselineMethodInfo[crid];
                Type type = tmp.GetType();
                mi = type.GetMethod("myMethod");

                object result = mi.Invoke(tmp, lstParam.ToArray());
                // debug code
                if (CommonClass.getDebugValue() && CommonClass.debugGridCell)
                {
                    Logger.debuggingOut.Append("Baseline,");

                    foreach (object i in lstParam)
                    {

                        Logger.debuggingOut.Append(i.ToString() + ",");
                    }
                    //System.Console.Write(",");

                    Logger.debuggingOut.Append(result + "\n");
                }
                return result;

            }
            catch (Exception ex)
            {
                return -999999999;
            }
        }
        public void CreateAllBaselineEvalObjects(Dictionary<string, string> dicFunction, Dictionary<string, string> dicSetupVariables)
        {
            try
            {
                if (dicBaselineMethodInfo != null)
                    dicBaselineMethodInfo.Clear();
                else
                    dicBaselineMethodInfo = new Dictionary<string, object>();


                foreach (KeyValuePair<string, string> k in dicFunction)
                {
                    try
                    {
                        string strVariables = "";
                        int i = 0;
                        if (dicSetupVariables != null && dicSetupVariables.Count > 0 && dicSetupVariables.ContainsKey(k.Key))
                        {
                            foreach (KeyValuePair<string, string> l in dicSetupVariables)
                            {
                                if (l.Key == k.Key)
                                    strVariables = l.Value;
                            }
                        }
                        CSharpCodeProvider csharpCodeProvider = new CSharpCodeProvider();
                        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

                        CompilerParameters cp = new CompilerParameters();
                        cp.ReferencedAssemblies.Add("System.dll");
                        cp.CompilerOptions = "/t:library";
                        cp.GenerateInMemory = true;
                        Random rm = new Random();
                        cp.OutputAssembly = CommonClass.DataFilePath + "\\Tmp\\" + System.DateTime.Now.Year + System.DateTime.Now.Month + System.DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute +
                            DateTime.Now.Second + DateTime.Now.Millisecond + rm.Next(2000) + ".dll";
                        StringBuilder myCode = new StringBuilder();
                        myCode.Append("using System;");
                        myCode.Append("namespace CoustomEval{");
                        myCode.Append("class myLibBaseLine" + k.Key + " { public double myPow(double a) { return Math.Pow(a,2);} public double myMethod(double a, double b, double c, double beta, double deltaq, double q0, double q1, double incidence, double pop, double prevalence" + (strVariables.Equals("") ? "" : ", " + strVariables) +
        "){try{" + k.Value + "} catch (Exception ex) { return -999999999; }}}");
                        myCode.Append("}");

                        CompilerResults cr = provider.CompileAssemblyFromSource(cp, myCode.ToString());
                        Assembly assembly = cr.CompiledAssembly;
                        Type[] types = new Type[] { typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double) };

                        object tmp = assembly.CreateInstance("CoustomEval.myLibBaseLine" + k.Key);
                        dicBaselineMethodInfo.Add(k.Key, tmp);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void CreateAllPointEstimateEvalObjects(Dictionary<string, string> dicFunction, Dictionary<string, string> dicSetupVariables)

        {
            try
            {
                if (dicPointEstimateMethodInfo != null)
                    dicPointEstimateMethodInfo.Clear();
                else
                    dicPointEstimateMethodInfo = new Dictionary<string, object>();

                foreach (KeyValuePair<string, string> k in dicFunction)
                {
                    try
                    {
                        string strVariables = "";
                        int i = 0;
                        if (dicSetupVariables != null && dicSetupVariables.Count > 0 && dicSetupVariables.ContainsKey(k.Key))
                        {
                            foreach (KeyValuePair<string, string> l in dicSetupVariables)
                            {
                                if (l.Key == k.Key)
                                    strVariables = l.Value;
                            }
                        }   

                        int icount = dicPointEstimateMethodInfo.Count;


                        CSharpCodeProvider csharpCodeProvider = new CSharpCodeProvider();
                        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

                        CompilerParameters cp = new CompilerParameters();
                        cp.ReferencedAssemblies.Add("System.dll");
                        cp.CompilerOptions = "/t:library";
                        cp.GenerateInMemory = true;
                        Random rm = new Random();
                        cp.OutputAssembly = CommonClass.DataFilePath + "\\Tmp\\" + System.DateTime.Now.Year + System.DateTime.Now.Month + System.DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute +
                            DateTime.Now.Second + DateTime.Now.Millisecond + rm.Next(2000) + ".dll";
                        StringBuilder myCode = new StringBuilder();
                        myCode.Append("using System;");
                        myCode.Append("namespace CoustomEval{");
                        myCode.Append("class myLibPointEstimate" + k.Key + " { public double myPow(double a) { return Math.Pow(a,2);}  public double myMethod(double a, double b, double c, double beta, double deltaq, double q0, double q1, double incidence, double pop, double prevalence" + (strVariables.Equals("") ? "" : ", " + strVariables) +
        "){ try{" + k.Value + "} catch (Exception ex) { return -999999999; }}}");
                        myCode.Append("}");

                        CompilerResults cr = csharpCodeProvider.CompileAssemblyFromSource(cp, myCode.ToString());

                        Assembly assembly = cr.CompiledAssembly;
                        Type[] types = new Type[] { typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double) };

                        object tmp = assembly.CreateInstance("CoustomEval.myLibPointEstimate" + k.Key);
                        dicPointEstimateMethodInfo.Add(k.Key, tmp);
                    }
                    catch(Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    public void CreateAllPopulationEvalObjects(Dictionary<string, string> dicFunction, Dictionary<string, string> dicSetupVariables)
    {
      //BENMAP-541
      //convert Population function into C# code.
      //input dicFunction is HI function. 

      //step 1: Prepare dicPopFunction --- Extract from POE functions (dicFunction) 
      //currently we assume population function = POP * Variable1, Ideally there is a better way to extract population function from HI funcion.
      Dictionary<string, string> dicPopFunction = new Dictionary<string, string>();
      Dictionary<string, string> dicSetupPopVariables = new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> k in dicFunction)
			{
				try
				{
          string strVariables = "";
          string funCode = " return pop;";
          int i = 0;
          if (dicSetupVariables != null && dicSetupVariables.Count > 0 && dicSetupVariables.ContainsKey(k.Key))
          {
            foreach (KeyValuePair<string, string> l in dicSetupVariables)
            {
              if (l.Key == k.Key)
							{
                strVariables = l.Value;
                string variableName = strVariables.Trim().Split(' ')[1];
                //check if the variable name is a population variable (range >=0 and <= 1)
                string commandText = String.Format(@"SELECT COUNT(*) as CountRec
FROM SETUPGEOGRAPHICVARIABLES r 
INNER JOIN SETUPVARIABLES s ON r.SETUPVARIABLEID=s.SETUPVARIABLEID
where lower(s.SETUPVARIABLENAME) = '{0}'
GROUP BY s.SETUPVARIABLENAME
HAVING min(r.VVALUE) >=0 AND max(r.VVALUE) <=1", variableName.ToLower());
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                int count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
								if (count > 0)
								{
                  funCode = " return pop*" + variableName + ";";
                  //dicPopFunction.Add(k.Key,"return pop*"+variableName+";");
                  dicSetupPopVariables.Add(l.Key, l.Value);
                }
              }              
            }
          }
          dicPopFunction.Add(k.Key, funCode);
        }
        catch(Exception ex)
				{

				}
			}

      //step 2: Compile c# code
      try
      {
        if (dicPopulationMethodInfo != null)
          dicPopulationMethodInfo.Clear();
        else
          dicPopulationMethodInfo = new Dictionary<string, object>();


        foreach (KeyValuePair<string, string> k in dicPopFunction)
        {
          try
          {
            string strVariables = "";
            int i = 0;
            if (dicSetupPopVariables != null && dicSetupPopVariables.Count > 0 && dicSetupPopVariables.ContainsKey(k.Key))
            {
              foreach (KeyValuePair<string, string> l in dicSetupPopVariables)
              {
                if (l.Key == k.Key)
                  strVariables = l.Value;
              }
            }
            CSharpCodeProvider csharpCodeProvider = new CSharpCodeProvider();
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            CompilerParameters cp = new CompilerParameters();
            cp.ReferencedAssemblies.Add("System.dll");
            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;
            Random rm = new Random();
            cp.OutputAssembly = CommonClass.DataFilePath + "\\Tmp\\" + System.DateTime.Now.Year + System.DateTime.Now.Month + System.DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute +
                DateTime.Now.Second + DateTime.Now.Millisecond + rm.Next(2000) + ".dll";
            StringBuilder myCode = new StringBuilder();
            myCode.Append("using System;");
            myCode.Append("namespace CoustomEval{");
            myCode.Append("class myLibPopulation" + k.Key + " { public double myPow(double a) { return Math.Pow(a,2);} public double myMethod(double pop" + (strVariables.Equals("") ? "" : ", " + strVariables) +
"){try{" + k.Value + "} catch (Exception ex) { return -999999999; }}}");
            myCode.Append("}");

            CompilerResults cr = provider.CompileAssemblyFromSource(cp, myCode.ToString());
            Assembly assembly = cr.CompiledAssembly;
            Type[] types = new Type[] { typeof(double) };

            object tmp = assembly.CreateInstance("CoustomEval.myLibPopulation" + k.Key);
            dicPopulationMethodInfo.Add(k.Key, tmp);
          }
          catch (Exception ex)
          {
            Logger.LogError(ex);
          }
        }
      }
      catch (Exception ex)
      {
      }
    }
    public object PointEstimateEval(string crID, string cCharpCode, double a, double b, double c, double beta, double deltaq, double q0, double q1, double incidence, double pop, double prevalence, Dictionary<string, double> dicSetupVariables)
        {
            try
            {
                MethodInfo mi = null;
                object tmp = null;
                List<object> lstParam = new List<object>() { a, b, c, beta, deltaq, q0, q1, incidence, pop, prevalence };
                if (dicSetupVariables != null && dicSetupVariables.Count > 0)
                {
                    int j = 0;
                    while (j < dicSetupVariables.Count)
                    {
                        lstParam.Add(dicSetupVariables.ToList()[j].Value);
                        j++;
                    }


                }

                tmp = dicPointEstimateMethodInfo[crID.ToString()];
                Type type = tmp.GetType();
                mi = type.GetMethod("myMethod");

                object result = mi.Invoke(tmp, lstParam.ToArray());
                if (CommonClass.getDebugValue() && CommonClass.debugGridCell)
                {

                    Logger.debuggingOut.Append("PointEstimateValue,");
                    foreach (object i in lstParam)
                    {
                        Logger.debuggingOut.Append(i.ToString() + ",");

                    }
                    Logger.debuggingOut.Append(result + "\n");

                }

                return result;

            }
            catch (Exception ex)
            {
                return -999999999;
            }
        }

    public object PopulationEval(string crID, string cCharpCode, double pop, Dictionary<string, double> dicSetupVariables)
    {
      //BENMAP-541
      //Calculate population when population variables are applied. 
      //input cCharpCode is not used. It is kept there just to be consistent with BaselineEval and PointOfEstimateEval
      //The compiled code object used for each function is compiled by function CreateAllPopulationEvalObjects and stores in dicPopulationMethodInfo.  
      try
      {
        MethodInfo mi = null;
        object tmp = null;
        List<object> lstParam = new List<object>() { pop };
        if (dicSetupVariables != null && dicSetupVariables.Count > 0)
        {
          int j = 0;
          while (j < dicSetupVariables.Count)
          {
            lstParam.Add(dicSetupVariables.ToList()[j].Value);
            j++;
          }
        }
        tmp = dicPopulationMethodInfo[crID.ToString()];
        Type type = tmp.GetType();
        mi = type.GetMethod("myMethod");

        object result = mi.Invoke(tmp, lstParam.ToArray());
        if (CommonClass.getDebugValue() && CommonClass.debugGridCell)
        {

          Logger.debuggingOut.Append("PopulationValue,");
          foreach (object i in lstParam)
          {
            Logger.debuggingOut.Append(i.ToString() + ",");

          }
          Logger.debuggingOut.Append(result + "\n");

        }

        return result;

      }
      catch (Exception ex)
      {
        return -999999999;
      }
    }


    public static Dictionary<string, object> dicPointEstimateMethodInfo = new Dictionary<string, object>();
        public static Dictionary<string, object> dicBaselineMethodInfo = new Dictionary<string, object>();
    public static Dictionary<string, object> dicPopulationMethodInfo = new Dictionary<string, object>();
    public static Dictionary<string, object> dicValuationMethodInfo = new Dictionary<string, object>();

        public object ValuationEval(string cCharpCode, double a, double b, double c, double d, double allgoodsindex, double medicalcostindex, double wageindex, double lagadjustment, Dictionary<string, double> dicSetupVariables)
        {
            try
            {
                MethodInfo mi = null;
                object tmp = null;
                List<object> lstParam = new List<object>() { a, b, c, d, allgoodsindex, medicalcostindex, wageindex, lagadjustment };
                if (dicSetupVariables != null && dicSetupVariables.Count > 0)
                {
                    int j = 0;
                    while (j < dicSetupVariables.Count)
                    {
                        lstParam.Add(dicSetupVariables.ToList()[j].Value);
                        j++;
                    }


                }
                if (!dicValuationMethodInfo.Keys.Contains(cCharpCode))
                {


                    string strVariables = "";
                    int i = 0;

                    if (dicSetupVariables != null && dicSetupVariables.Count > 0)
                    {
                        while (i < dicSetupVariables.Count)
                        {
                            strVariables = strVariables + ",double " + dicSetupVariables.ToList()[i].Key.ToLower();
                            i++;
                        }


                    }
                    if (a != 0)
                    { }
                    _CharpCode = cCharpCode;
                    int icount = dicValuationMethodInfo.Count;
                    CSharpCodeProvider csharpCodeProvider = new CSharpCodeProvider();
                    CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

                    CompilerParameters cp = new CompilerParameters();
                    cp.ReferencedAssemblies.Add("system.dll");
                    cp.CompilerOptions = "/t:library";
                    cp.GenerateInMemory = true;
                    Random rm = new Random();
                    cp.OutputAssembly = CommonClass.DataFilePath + "\\Tmp\\" + System.DateTime.Now.Year + System.DateTime.Now.Month + System.DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute +
                        DateTime.Now.Second + DateTime.Now.Millisecond + rm.Next(2000) + ".dll";
                    StringBuilder myCode = new StringBuilder();
                    myCode.Append("using System;");
                    myCode.Append("namespace CoustomEval{");
                    myCode.Append("class myLibValuation" + icount + " {public double myPow(double a) { return Math.Pow(a,2);}   public double myMethod(double a, double b, double c, double d, double allgoodsindex, double medicalcostindex, double wageindex, double lagadjustment" + strVariables +
    "){ try{" + cCharpCode + "} catch (Exception ex) { return -999999999; }}}");
                    myCode.Append("}");
                    CompilerResults cr = provider.CompileAssemblyFromSource(cp, myCode.ToString());
                    Assembly assembly = cr.CompiledAssembly;
                    Type[] types = new Type[] { typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double) };

                    tmp = assembly.CreateInstance("CoustomEval.myLibValuation" + icount);
                    dicValuationMethodInfo.Add(cCharpCode, tmp);


                }
                else
                {
                    tmp = dicValuationMethodInfo[cCharpCode];
                }
                Type type = tmp.GetType();
                mi = type.GetMethod("myMethod");
                object result = mi.Invoke(tmp, lstParam.ToArray());
                return result;

            }
            catch (Exception ex)
            {
                return -999999999;
            }

        }
    }
}
