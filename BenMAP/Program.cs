using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Runtime.ExceptionServices;
using System.Threading;


namespace BenMAP
{
	static class Program
	{
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		private static extern bool AllocConsole();

		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		private static extern bool AttachConsole(int pid);

		[STAThread]
		static void Main(string[] arg)
		{
			if (arg.Length > 0)	//Adding after testing of BenMAP-229 (Command Line)--in dev environment, running as console app and using command line arguments in the "Debug" tab of properties 
			{
				if (!AttachConsole(-1))		//This ensures that the output is attached to the parent console (i.e. command prompt)
					AllocConsole();					//If unable to locate the parent console, it will allocate its own console for output
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);


				AppDomain currentDomain = AppDomain.CurrentDomain;
				// Add handler for UI thread exceptions
				Application.ThreadException += new ThreadExceptionEventHandler(ThreadExceptionHandler);
				// Force all WinForms errors to go through handler
				Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
				// This handler is for catching non-UI thread exceptions          
				currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

				// This handler is for catching all exceptions, handled or not     
				//currentDomain.FirstChanceException += FirstChanceExceptionHandler;
				
			}

				string strArg = "";
				foreach (string s in arg)
				{
					strArg = strArg + " " + s;
				}
				CommonClass.InputParams = new string[] { strArg };
			Application.Run(new Main());
		}

		//static void FirstChanceExceptionHandler(object source, FirstChanceExceptionEventArgs args)
		//{

		//    Exception ex = args.Exception;

		//    HandleException(ex);
		//}


		static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs args)
		{
			Exception ex = (Exception)args.Exception;

			HandleException(ex);
		}

		static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
		{
			Exception ex = (Exception)args.ExceptionObject;

			HandleException(ex);
		}

		static void HandleException(Exception ex)
		{

			Logger.LogError(ex);

			//check for Jira Connector needed by error reporting form
			//if no jira connector, then terminate application
			if (String.IsNullOrEmpty(CommonClass.JiraConnectorFilePath) || String.IsNullOrEmpty(CommonClass.JiraConnectorFilePathTXT))
			{
				MessageBox.Show("The application encountered the following fatal error and will now terminate." + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK);
				Environment.Exit(0);
			}


			//show error reporting form unless error is in error reporting form
			if (ex.StackTrace.IndexOf("ErrorReporting", StringComparison.OrdinalIgnoreCase) < 0)
			{
				try
				{
					DialogResult dialogResult = MessageBox.Show("The application encountered the following fatal error:" + Environment.NewLine + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine + "Would you like to report the error to the BenMAP-CE development team before the application terminates?", "Error", MessageBoxButtons.YesNo);

					if (dialogResult == DialogResult.Yes)
					{
						ErrorReporting frm = new ErrorReporting();
						string err = ex.StackTrace + Environment.NewLine + Environment.NewLine + "Please enter any additional information about the error that might prove useful:";
						frm.ErrorMessage = err;

						frm.ShowDialog();
					}

					Environment.Exit(0);

				}
				catch (Exception ex2)
				{
					//quit if error occurs opening or after opening error reporting form
					Logger.LogError(ex2);
					Environment.Exit(0);
				}
			}
			else //this error occurred in Error Reporting so alert user then quit
			{
				MessageBox.Show("The application encountered the following fatal error and will now terminate." + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK);
				Environment.Exit(0);
			}

		}




	}
}