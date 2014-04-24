using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;

namespace BenMAP
{
    static class Program
    {
        [STAThread]
        static void Main(string[] arg)
        {
            try
            {
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
            catch (Exception ex)
            {
                Logger.LogError(ex);

                //show error reporting form unless error is in error reporting form
                if (ex.StackTrace.IndexOf("ErrorReporting", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    try
                    {
                        DialogResult dialogResult = MessageBox.Show("The following error occurred:\n\n" + ex.Message + "\n\nWould you like to report this to the BenMAP-CE development team?", "Error", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.Yes)
                        {
                            ErrorReporting frm = new ErrorReporting();
                            frm.ShowDialog();
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger.LogError(ex2);
                    }
                }
               
            }
        }
    }
}
