using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;

namespace BenMAP
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] arg)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //string[] arg = new string[1];
            //arg[0] = @"D:\软件项目\BenMap\BenMap\trunk\Code\BenMAP\bin\Debug\Result\Project\t.projx";
            string strArg = "";
            foreach (string s in arg)
            {
                strArg = strArg + " " + s;
            }
            CommonClass.InputParams = new string[]{strArg};
            Application.Run(new Main());
        }
    }
}
