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
        public  enum Level : int {Error=1, DEBUG=2 } ;
        public static StringBuilder debuggingOut = new StringBuilder();
        /*
         * the default location the log files are written to if Logger is instantiated with this field null
         **/
        private static string defaultLoggingPath = CommonClass.DataFilePath;
        private static string debugFile = "\\debug_log.log";
        private static string errorFile = "\\err_log.log";
        private static StreamWriter debugWriter;
        private static string CRLF = "\r\n";
        public static void LogError(Exception ex)
        {
            try
            {
                string place = ex.StackTrace.ToString();
                int pos = place.LastIndexOf("\n");
                string msg = CRLF + DateTime.Now.ToString() + CRLF + ex.Message + CRLF + place.Substring(pos + 1);
                AppendErr(GetLogPath(ex), msg);
            }
            catch (Exception myEx)
            {
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + "  " + "Logger.LogError";
                Debug.WriteLine(errMsg);
                Console.WriteLine(errMsg);
            }
        }
        public static void LogError(Exception ex, string msg)
        {
            try
            {
                string place = ex.StackTrace.ToString();
                int pos = place.LastIndexOf("\n");

                string myMsg = CRLF + DateTime.Now.ToString() + CRLF + ex.Message + CRLF + msg + CRLF + place.Substring(pos + 1);
                AppendErr(GetLogPath(ex), myMsg);
            }
            catch (Exception myEx)
            {
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + "  " + "Logger.LogError";
                Debug.WriteLine(errMsg);
                Console.WriteLine(errMsg);
            }
        }
        public static void LogError(string msg)
        {
            try
            {
                string myMsg = CRLF + DateTime.Now.ToString() + CRLF + msg;
                AppendErr(GetLogPath(null), myMsg);
            }
            catch (Exception myEx)
            {
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + CRLF + "Logger.LogError";
                Debug.WriteLine(errMsg);
                Console.WriteLine(errMsg);
            }
        }

        public static string GetLogPath(Exception ex)
        {
            try
            {
                string logPath;



                logPath = AppDomain.CurrentDomain.BaseDirectory + errorFile;
                logPath = logPath.Replace("\\\\", "\\");
                return logPath;
            }
            catch (Exception myEx)
            {
                string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + CRLF + "Logger.GetLogPath";
                Debug.WriteLine(errMsg);
                Console.WriteLine(errMsg);
                return "err_log.log";
            }
        }

        public static void CheckAndCreateDir(string path)
        {
            try
            {
                string dir;
                int pos = path.LastIndexOf("\\");
                dir = path.Substring(0, pos);
                if (Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch (Exception ex)
            {
                string errMsg = CRLF + DateTime.Now.ToString() + "\\rn" + ex.ToString() + "\r\npath=" + path + CRLF + "Logger.CheckAndCreateDir";
                Debug.WriteLine(errMsg);
                Console.WriteLine(errMsg);
            }
        }

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
        public static void Log(Level level,string path, Exception e,  string msg)
        {
            try
            {
                if (path == null)
                    path = defaultLoggingPath;

                debugWriter = new StreamWriter(path+debugFile, true);
                if (level == Level.DEBUG)
                {
                    Debug.WriteLine(msg);
                    debugWriter.WriteLine(debuggingOut);
                    debugWriter.Flush();
                    debugWriter.Close();
                }
                if (level == Level.Error)
                {
                    LogError(e, msg);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

        }
    }
}
