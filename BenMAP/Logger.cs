using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BenMAP
{
	public static class Logger
	{
		public enum Level : int { Error = 1, DEBUG = 2, INFO = 3 };
		public static StringBuilder debuggingOut = new StringBuilder();
		/*
		 * the default location the log files are written to if Logger is instantiated with this field null
		 **/
		private static string defaultLoggingPath = CommonClass.ResultFilePath;
		private static Boolean outputToConsole = true;
		private static string debugFile = "\\debug.csv";
		private static string logFile = "\\BenMAP.log";
		private static string CRLF = "\r\n";

		public static void LogError(Exception ex)
		{
			try
			{
				string place = ex.StackTrace.ToString();
				int pos = place.LastIndexOf("\n");
				string msg = CRLF + "[ERROR " + DateTime.Now.ToString() + "]: " + ex.Message + CRLF + place.Substring(pos + 1);
				Append(GetLogPath(ex), msg);
				//CommonClass.CurrentMainFormStat = "Errors have occurred. Details are available in 'My BenMAP-CE Files\\BenMAP.log'.";
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
				Append(GetLogPath(ex), myMsg);
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
				Append(GetLogPath(null), myMsg);
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
			string logPath;
			logPath = defaultLoggingPath + logFile;
			logPath = logPath.Replace("\\\\", "\\");
			return logPath;
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

		private static void Append(string path, string msg)
		{
			try
			{
				StreamWriter writer = new StreamWriter(path, true);
				writer.WriteLine(msg);
				writer.Close();

			}
			catch (Exception myEx)
			{
				string errMsg = CRLF + DateTime.Now.ToString() + CRLF + myEx.Message + CRLF + "Logger.AppendErr";
				Debug.WriteLine(errMsg);
				Console.WriteLine(errMsg);
			}
		}
		public static void Log(Level level, string path, Exception e, string msg)
		{
			try
			{
				string newMsg = "";
				if (path == null)
					path = defaultLoggingPath;

				if (level == Level.INFO)
				{
					newMsg = "[INFO " + DateTime.Now.ToString() + "]: " + msg;
					Append(path + logFile, newMsg);

				}
				if (level == Level.DEBUG)
				{
					newMsg = "[DEBUG " + DateTime.Now.ToString() + "]: " + msg;
					Append(path + logFile, newMsg);
					Append(path + debugFile, debuggingOut.ToString());
				}
				if (level == Level.Error)
				{
					string place = e.StackTrace.ToString();
					int pos = place.LastIndexOf("\n");
					newMsg = CRLF + "[ERROR " + DateTime.Now.ToString() + "]: " + e.Message + CRLF + place.Substring(pos + 1);
					Append(path + logFile, newMsg);
				}
				if (outputToConsole)
				{
					Console.WriteLine(newMsg);
				}
			}
			catch (Exception ex)
			{
				LogError(ex, ex.Message);
			}

		}
	}
}
