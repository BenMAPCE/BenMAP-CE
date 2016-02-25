using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class VarianceMulti : FormBase
    {
        private int betaIndex;
        private CRFVariable _poll;

        public CRFVariable Pollutant
        {
            get { return _poll; }
            set { _poll = value;  }
        }

        public VarianceMulti(HealthImpact hif, CRFVariable var, int betaInd)
        {
            InitializeComponent();
            tbModelSpec.Text = hif.ModelSpec;
            tbPollutant.Text = var.PollutantName;
            tbSeason.Text = var.PollBetas[betaInd].SeasonName;
            tbStart.Text = var.PollBetas[betaInd].StartDate;
            tbEnd.Text = var.PollBetas[betaInd].EndDate;

            _poll = var;
            betaIndex = betaInd;
        }

        private void VarianceMulti_Load(object sender, EventArgs e)
        {
            // Load variance data from db by crfbetaid
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText = string.Format("select pollutantname, varcov from crfvariables as crv left join crfbetas as crb on crb.crfvariableid = crv.crfvariableid left join crfvarcov as crvc on crvc.crfbetaID1 = crb.crfbetaid or crvc.crfbetaid2 = crb.crfbetaid where((crfbetaid2={0} and variablename!='{1}') or(crfbetaid1={0} and crfbetaid2={0})) order by variablename", Pollutant.PollBetas[betaIndex].BetaID, Pollutant.VariableName);
            DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

            olvVariance.DataSource = ds.Tables[0];
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
