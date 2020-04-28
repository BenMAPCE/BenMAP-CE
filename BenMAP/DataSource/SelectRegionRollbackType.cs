using System;
using System.Windows.Forms;


namespace BenMAP
{
	public partial class SelectRegionRollbackType : FormBase
	{
		public SelectRegionRollbackType()
		{
			InitializeComponent();
		}

		private void SelectRegionRollbackType_Load(object sender, EventArgs e)
		{
		}

		private string _controlName = string.Empty;

		public string ControlName
		{
			get { return _controlName; }
			set { _controlName = value; }
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (rbtnPercentage.Checked)
			{
				DataSource.PercentageControl pcfrm = new DataSource.PercentageControl();
				_controlName = "Percentage";
			}
			if (rbtnIncremental.Checked)
			{
				DataSource.IncrementalControl icfrm = new DataSource.IncrementalControl();
				_controlName = "Incremental";
			}
			if (rbtnToStandard.Checked)
			{
				_controlName = "ToStandard";
			}
			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}
	}
}