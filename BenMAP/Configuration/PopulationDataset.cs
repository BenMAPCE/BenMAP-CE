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
    public partial class PopulationDataset : FormBase
    {
        public PopulationDataset()
        {
            InitializeComponent();
        }

        private BenMAPPopulation benMAPPopulation;

        public BenMAPPopulation BenMAPPopulation
        {
            get { return benMAPPopulation; }
            set { benMAPPopulation = value; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
        string commandText = string.Empty;
        private void PopulationDataset_Load(object sender, EventArgs e)
        {
            try
            {
                int iSelectIndex = 0;
                commandText = string.Format("select * from PopulationDataSets where PopulationDataSetID<>37 and  SetupID={0} order by PopulationDataSetID", CommonClass.MainSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboPopulationDataSet.DataSource = ds.Tables[0];
                cboPopulationDataSet.DisplayMember = "PopulationDataSetName";
                cboPopulationDataSet.SelectedIndex = 0;
                if (benMAPPopulation != null)
                {
                    int i = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (Convert.ToInt32(dr["PopulationDataSetID"]) == benMAPPopulation.DataSetID)
                        {
                            cboPopulationDataSet.SelectedIndex = i;
                            break;
                        }
                        i++;
                    }
                }
                else if (CommonClass.GBenMAPGrid != null && CommonClass.MainSetup.SetupID == 1)
                {
                    if (CommonClass.GBenMAPGrid.GridDefinitionID == 4 || CommonClass.GBenMAPGrid.GridDefinitionID == 27 || CommonClass.GBenMAPGrid.GridDefinitionID == 28)
                    {
                        cboPopulationDataSet.SelectedIndex = 1;
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboPopulationDataSet_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                List<string> lstYear = new List<string>();
                DataRowView drv = cboPopulationDataSet.SelectedItem as DataRowView;
                int PopulationDataSetID = Convert.ToInt32(drv["PopulationDataSetID"]);
                if (CommonClass.MainSetup.SetupID == 1 && Convert.ToInt16(drv["APPLYGROWTH"]) == 1) commandText = string.Format("select distinct Yyear from t_PopulationDataSetIDYear where PopulationDataSetID in(select PopulationDataSetID from PopulationDataSets where    SetupID={0})", CommonClass.MainSetup.SetupID);
                else
                    commandText = string.Format("select distinct Yyear from t_PopulationDataSetIDYear  where PopulationDataSetID={0}", PopulationDataSetID);
                DataSet dsYear = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboPopulationYear.DataSource = dsYear.Tables[0];
                cboPopulationYear.DisplayMember = "Yyear";
                if (benMAPPopulation != null)
                {
                    int i = 0;
                    foreach (DataRow dr in dsYear.Tables[0].Rows)
                    {
                        if (Convert.ToInt32(dr["Yyear"]) == benMAPPopulation.Year)
                        {
                            cboPopulationYear.SelectedIndex = i;
                            break;
                        }
                        i++;
                    }
                }
                else
                    cboPopulationYear.Text = "2010";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DataRowView drv = cboPopulationDataSet.SelectedItem as DataRowView;
            int PopulationDataSetID = Convert.ToInt32(drv["PopulationDataSetID"]);
            benMAPPopulation = new global::BenMAP.BenMAPPopulation();
            benMAPPopulation.DataSetID = PopulationDataSetID;
            benMAPPopulation.DataSetName = drv["PopulationDataSetName"].ToString();
            benMAPPopulation.GridType = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"]));
            benMAPPopulation.PopulationConfiguration = Convert.ToInt32(drv["PopulationConfigurationID"]);
            benMAPPopulation.Year = Convert.ToInt32(cboPopulationYear.Text);
            this.DialogResult = DialogResult.OK;
            //YY: temp code to pull weight table
            //BaseControlGroup bcg = CommonClass.LstBaseControlGroup[0];
            //if (bcg.Base is MonitorDataLine)
            //{
            //    MonitorDataLine mdl = (MonitorDataLine)bcg.Base;
            //    foreach (MonitorNeighborAttribute mna in mdl.MonitorNeighbors)
            //    {
            //        string path = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\My BenMAP-CE Files\";
            //        System.IO.StreamWriter baseWriter = new System.IO.StreamWriter(path + "debug_weightBase.csv", true);
            //        string baseMsg = string.Format("{0:yyyyMMddhhmmss}.bin", DateTime.Now) + "," + mna.Col + "," + mna.Row + "," + mna.Distance + "," + mna.MonitorName + "," + mna.Weight;
            //        baseWriter.WriteLine(baseMsg);
            //        baseWriter.Close();
            //    }
            //}
           
            
        }

        private void btnPopMAP_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboPopulationDataSet.Text) && !string.IsNullOrEmpty(cboPopulationYear.Text))
            {
                PopulationMap frm = new PopulationMap();
                frm.PopDataset = cboPopulationDataSet.Text;
                frm.PopYear = cboPopulationYear.Text;
                frm.ShowDialog();
            }
        }
    }
}
