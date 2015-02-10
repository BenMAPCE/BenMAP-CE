using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP.Tools
{
    // generic Input Box 
    // based on https://social.msdn.microsoft.com/Forums/windows/en-US/191ddf61-3ae5-4845-b852-56bb9b77238a/input-message-box-in-c?forum=winforms
    // ====================================================================================
    // calling example (setting options separately)
    //       InputBox myBox = new InputBox();
    //       myBox.InputText = "MyInputText";
    //       myBox.InputLabel = "My Input Label";
    //       myBox.Title = "Copy Pollutant " + pollutantName;
    //       DialogResult inputResult = myBox.ShowDialog();
    //       if (inputResult == DialogResult.OK)
    //       {
    //           MessageBox.Show("User Pressed OK");
    //       }
    //       else if (inputResult == DialogResult.Cancel)
    //       {
    //           MessageBox.Show("Cancelled by user");
    //       }
    // ====================================================================================
    // calling example (one line)
    //  InputBox myBox = new InputBox("Form Title" + "Entry Label", "Default Value");
    // ====================================================================================
    public partial class InputBox : Form
    {
        public InputBox()
        {
            InitializeComponent();
        }
        public InputBox(string title="", string Label="", string text=""):this(){
            Title = title;
            lblLabel.Text = Label;
            txtInput.Text = text;
        }

        public string InputText
        {
            get{return txtInput.Text;}
            set { txtInput.Text = value;}
        }

        public string InputLabel
        {
            get { return lblLabel.Text;}
            set { lblLabel.Text = value;}
        }

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
