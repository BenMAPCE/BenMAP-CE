using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class Logger
    {
        private static string CRLF = "\r\n";           // 结束符
        
        #region LogError
        /// <summary>
        /// 记录发生的错误
        /// 重载1
        /// </summary>
        /// <param name="ex">Exception对象</param>	
        public static void LogError(Exception ex)
        {
            try
            {
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
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + "  " + "Logger.LogError";
                Debug.WriteLine(errMsg);
                Console.WriteLine(errMsg);
            }
        }
        /// <summary>
        /// 记录发生的错误
        /// 重载2
        /// </summary>
        /// <param name="ex">Exception对象</param>
        /// <param name="msg">附加的自定义信息</param>	
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
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + "  " + "Logger.LogError";
                Debug.WriteLine(errMsg);
                Console.WriteLine(errMsg);
            }
        }
        /// <summary>
        /// 记录发生的错误
        /// 重载3
        /// </summary>
        /// <param name="msg">自定义的错误信息</param>	
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
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + CRLF + "Logger.LogError";
                Debug.WriteLine(errMsg);
                Console.WriteLine(errMsg);
            }
        }

        /// <summary>
        /// 从app.config(或web.config)的appSettings节中获取记录错误日志的文件全路径.<br />
        /// 如果配置节不存在,则在DataManager.dll所在目录下创建err_log.log
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>文件全路径</returns>
        public static string GetLogPath(Exception ex)
        {
            try
            {
                string logPath;

                // Get the appSettings.
                // 即使配置文件里不存在appSettings这个节,也不会出错
                //System.Collections.Specialized.NameValueCollection appSettings = ConfigurationManager.AppSettings;

                //logPath = appSettings["errorLogFile"];

                //if (logPath == null)
                //{
                logPath = AppDomain.CurrentDomain.BaseDirectory + "\\err_log.log";
                logPath = logPath.Replace("\\\\", "\\");
                //}
                //CheckAndCreateDir(logPath);
                return logPath;
                //return null;
            }
            catch (Exception myEx)
            {
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + CRLF + "Logger.GetLogPath";
                Debug.WriteLine(errMsg);
                Console.WriteLine(errMsg);
                return "err_log.log";
            }
        }

        /// <summary>
        /// 检查会议的文件全路径,创建所有不存在的目录.(注:不创建文件)
        /// </summary>
        /// <param name="path">文件全路径,必须包含文件名.如: D:\Test\test.txt</param>
        public static void CheckAndCreateDir(string path)
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
                string errMsg = CRLF + DateTime.Now.ToString() + "\\rn" + ex.ToString() + "\r\npath=" + path + CRLF + "Logger.CheckAndCreateDir";
                Debug.WriteLine(errMsg);
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
                StreamWriter writer = new StreamWriter(path, true);
                writer.WriteLine(msg);
                writer.Close();
                Debug.WriteLine(msg);
                Console.WriteLine(msg);
            }
            catch (Exception myEx)
            {
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + CRLF + "Logger.AppendErr";
                Debug.WriteLine(errMsg);
                Console.WriteLine(errMsg);
            }
        }
        #endregion
    }//class
}
