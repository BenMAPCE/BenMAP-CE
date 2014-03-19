using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class ViewEditMetadata : Form
    {
        public ViewEditMetadata()
        {
            InitializeComponent();
        }
        public ViewEditMetadata(string fileName): this()
        {

        }
    }
}
