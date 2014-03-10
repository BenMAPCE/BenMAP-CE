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
    }
}
