using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;


namespace BenMAP
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            RegistrationServices regSrv = new RegistrationServices();
            regSrv.RegisterAssembly(base.GetType().Assembly,
            AssemblyRegistrationFlags.SetCodeBase);
        }
        public void ExecuteDelete()
        {
            String[] cmds = new String[4];
            cmds[0] =@"del /F /S /Q %LocalAppData%\BenMap-CE\Data\*.* ";
            cmds[1] =@"del /F /S /Q %LocalAppData%\BenMap-CE\Database\*.*";
            cmds[2] =@"del /F /S /Q %LocalAppData%\BenMap-CE\Tmp\*.*";
            cmds[3] =@"rmdir /S /Q  %LocalAppData%\BenMap-CE\";
            for (int i = 0; i < cmds.Length; i++)
            {

                int ExitCode;
                ProcessStartInfo ProcessInfo;
                Process Process;

                ProcessInfo = new ProcessStartInfo("cmd.exe", "/c"+ cmds[i]);
            
                ProcessInfo.CreateNoWindow = true; ;
                ProcessInfo.UseShellExecute = false;

                Process = Process.Start(ProcessInfo);
                Process.WaitForExit();

                ExitCode = Process.ExitCode;
                Process.Close();
            }  
          //  MessageBox.Show("ExitCode: " + ExitCode.ToString(), "ExecuteCommand");
        }
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);
            RegistrationServices regSrv = new RegistrationServices();
            regSrv.UnregisterAssembly(base.GetType().Assembly);
            ExecuteDelete();

          
        }
    }
}
