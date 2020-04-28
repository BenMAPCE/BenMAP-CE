using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Printing;

namespace BenMAP
{
	public partial class ReleaseNotes : FormBase
	{
		public ReleaseNotes()
		{
			InitializeComponent();
			this.printDocument1.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.printDocument1_BeginPrint);
			this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			this.Close();
		}
		private static string fileName = Application.StartupPath + @"\Data\BenMap-CE Release Notes.rtf";
		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog openRTFImplement = new OpenFileDialog();
				openRTFImplement.Filter = "RTF files(*.RTF)|*.RTF"; if (openRTFImplement.ShowDialog() == DialogResult.OK && openRTFImplement.FileName.Length > 0)
				{
					fileName = openRTFImplement.FileName; this.richTextBoxPrintCtrl1.LoadFile(fileName, RichTextBoxStreamType.RichText);
				}
			}
			catch (ArgumentException e1) { e1.ToString(); }
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveRTFImplement = new SaveFileDialog();
			saveRTFImplement.Title = "Save Release Notes...";
			saveRTFImplement.InitialDirectory = Application.StartupPath + @"\Data";
			saveRTFImplement.Filter = "RTF files(*.rtf)|*.rtf";
			saveRTFImplement.RestoreDirectory = true;

			if (saveRTFImplement.ShowDialog() == DialogResult.OK && saveRTFImplement.FileName.Length > 0)
			{
				richTextBoxPrintCtrl1.SaveFile(saveRTFImplement.FileName);
				MessageBox.Show("File saved.", "File saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
		}

		private void ReleaseNotes_Load(object sender, EventArgs e)
		{
			try
			{
				this.richTextBoxPrintCtrl1.LoadFile(Application.StartupPath + @"\Data\BenMap-CE Release Notes.rtf", RichTextBoxStreamType.RichText);
			}
			catch (ArgumentException e1)
			{
				e1.ToString();
				MessageBox.Show("IncorrectFile");
			}
		}

		private int checkPrint;
		private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			pageSetupDialog1.ShowDialog();
		}

		private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			printPreviewDialog1.StartPosition = FormStartPosition.CenterScreen;
			printPreviewDialog1.ShowDialog();
		}

		private void printToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (printDialog1.ShowDialog() == DialogResult.OK)
				printDocument1.Print();
		}

		private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			checkPrint = 0;
		}

		private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			checkPrint = richTextBoxPrintCtrl1.Print(checkPrint, richTextBoxPrintCtrl1.TextLength, e);

			if (checkPrint < richTextBoxPrintCtrl1.TextLength)
				e.HasMorePages = true;
			else
				e.HasMorePages = false;
		}

	}
}
