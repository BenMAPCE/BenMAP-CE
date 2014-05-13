using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Runtime.ExceptionServices;


namespace BenMAP
{
    static class Program
    {
        [STAThread]
        static void Main(string[] arg)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;      
            currentDomain.FirstChanceException += FirstChanceExceptionHandler;
   
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string strArg = "";
            foreach (string s in arg)
            {
                strArg = strArg + " " + s;
            }
            CommonClass.InputParams = new string[] { strArg };
            Application.Run(new Main());
            
        }

        static void FirstChanceExceptionHandler(object source, FirstChanceExceptionEventArgs args)
        {
           
            Exception ex = args.Exception;
            Logger.LogError(ex);

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
            else //this error occurred in Error Reporting so just quit
            {
                Environment.Exit(0);
            }

        }
        
    }
}
