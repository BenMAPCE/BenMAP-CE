using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;

/*
using System;
using System.Data;
using System.Xml;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Configuration;
using FirebirdSql.Data.FirebirdClient;
*/
/*
 * using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using DotSpatial.Data;
using System.Xml.Serialization;
using ProtoBuf;
using System.Collections;
using System.Data.Common;
using System.Data.OleDb;
using DotSpatial.Topology;
using System.Runtime.InteropServices;
using Excel;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Text.RegularExpressions;
*/


namespace PopSim
{
    public partial class frm_PopSim : Form
    {
        int currentpage = 0;
        const int MAXPAGE = 7;  // maximum tab number
        const int DEFAULTSCENARIOID = 1;    // load default scenario, this may be removed in the future if the user is permitted to save and load scenarios
        public FirebirdSql.Data.FirebirdClient.FbConnection dbConnection;
        DateTime StartTime;
        DateTime StopTime;
        string fileName;

        public frm_PopSim()
        {
            InitializeComponent();

            // create link to Firebird database
            dbConnection = getNewConnection();
            dbConnection.Open();
            // load boxes on form with stored scenario data
            fillFormWithScenarioData(DEFAULTSCENARIOID);
        }

        private static FbConnection getNewConnection()
        {

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            string str = settings.ConnectionString;
            //if (!str.Contains(":"))
            //    str = str.Substring(0, str.IndexOf("initial catalog=")) + "initial catalog=" + Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
            str = str.Replace("##USERDATA##", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

            FbConnection connection = new FirebirdSql.Data.FirebirdClient.FbConnection(str);

            return connection;
        }

        private void fillFormWithScenarioData(int Scenario_ID)
        {
            // load form data for scenario from database
            FbCommand dataCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
            dataCommand.Connection = dbConnection;
            dataCommand.CommandType = CommandType.Text;
            dataCommand.CommandText = "Select BEGIN_YEAR, END_YEAR, SCENARIO_NAME, DR_APPROACH_ID, "
                + "BETA_TYPE_ID, PM_THRESHOLD_CHOICE, PS_TRAJECTORY_ID, LAG_TYPE_ID, BIRTHS_DYNAMIC, "
                + "USER_SPECIFIED_BETA, PM_THRESHOLD_VALUE, BETA_ADJ_FACTOR, "
                + "LAG_K_SINGLE, LAG_K_MULTIPLE_CARDIO, LAG_K_MULTIPLE_LUNG, LAG_K_MULTIPLE_OTHER, "
                + "AGE_RANGE_START, AGE_RANGE_END, STUDY_ID, "
                + "PM_YEAR_1, PM_YEAR_2, PM_YEAR_3, PM_YEAR_4, PM_YEAR_5, PM_VAL_1, PM_VAL_2, "
                + "PM_VAL_3, PM_VAL_4, PM_VAL_5, SUB_POP_START_1, SUB_POP_START_2, SUB_POP_START_3, "
                + "SUB_POP_START_4, SUB_POP_START_5, SUB_POP_END_1, SUB_POP_END_2, SUB_POP_END_3, SUB_POP_END_4, "
                + "SUB_POP_END_5, SUB_POP_ADJUSTMENT_1, SUB_POP_ADJUSTMENT_2, SUB_POP_ADJUSTMENT_3, SUB_POP_ADJUSTMENT_4, "
                + "SUB_POP_ADJUSTMENT_5, LAG_FUNCT_TYPE_ID "
                + "from SCENARIOS where SCENARIO_ID = " + Scenario_ID.ToString();
            FbDataReader dataReader;
            dataReader = dataCommand.ExecuteReader();
            while (dataReader.Read())
            {
                nudStartYear.Value = decimal.Parse(dataReader[0].ToString());
                nudEndYear.Value = decimal.Parse(dataReader[1].ToString());
                txtScenarioName.Text = dataReader[2].ToString();

                // add new boxes - need to check indexes
                //  USER_SPECIFIED_BETA, PM_THRESHOLD_VALUE, BETA_ADJ_FACTOR, "
                txtUserSuppliedBeta.Text = dataReader[9].ToString();
                txtPMThreshold.Text = dataReader[10].ToString();
                txtBetaAdj.Text = dataReader[11].ToString();

                // + "LAG_K_SINGLE, LAG_K_MULTIPLE_CARDIO, LAG_K_MULTIPLE_LUNG, LAG_K_MULTIPLE_OTHER "
                txtLagSingle.Text = dataReader[12].ToString();
                txtLagCardio.Text = dataReader[13].ToString();
                txtLagLung.Text = dataReader[14].ToString();
                txtLagOther.Text = dataReader[15].ToString();

                // + "AGE_RANGE_START, AGE_RANGE_END "
                txtYoungest.Text = dataReader[16].ToString();
                txtOldest.Text = dataReader[17].ToString();

                // PM_YEAR_1, PM_YEAR_2, PM_YEAR_3, PM_YEAR_4, PM_YEAR_5, 
                txtPMYear_1.Text = dataReader[19].ToString();
                txtPMYear_2.Text = dataReader[20].ToString();
                txtPMYear_3.Text = dataReader[21].ToString();
                txtPMYear_4.Text = dataReader[22].ToString();
                txtPMYear_5.Text = dataReader[23].ToString();

                //PM_VAL_1, PM_VAL_2, PM_VAL_3, PM_VAL_4, PM_VAL_5, 
                txtPM_Val_1.Text = dataReader[24].ToString();
                txtPM_Val_2.Text = dataReader[25].ToString();
                txtPM_Val_3.Text = dataReader[26].ToString();
                txtPM_Val_4.Text = dataReader[27].ToString();
                txtPM_Val_5.Text = dataReader[28].ToString();

                // SUB_POP_START_1, SUB_POP_START_2, SUB_POP_START_3, SUB_POP_START_4, SUB_POP_START_5, 
                txtSUB_POP_START_1.Text = dataReader[29].ToString();
                txtSUB_POP_START_2.Text = dataReader[30].ToString();
                txtSUB_POP_START_3.Text = dataReader[31].ToString();
                txtSUB_POP_START_4.Text = dataReader[32].ToString();
                txtSUB_POP_START_5.Text = dataReader[33].ToString();

                // SUB_POP_END_1, SUB_POP_END_2, SUB_POP_END_3, SUB_POP_END_4, SUB_POP_END_5, 
                txtSUB_POP_END_1.Text = dataReader[34].ToString();
                txtSUB_POP_END_2.Text = dataReader[35].ToString();
                txtSUB_POP_END_3.Text = dataReader[36].ToString();
                txtSUB_POP_END_4.Text = dataReader[37].ToString();
                txtSUB_POP_END_5.Text = dataReader[38].ToString();

                // SUB_POP_ADJUSTMENT_1, SUB_POP_ADJUSTMENT_2, SUB_POP_ADJUSTMENT_3, SUB_POP_ADJUSTMENT_4, SUB_POP_ADJUSTMENT_5
                txtSUB_POP_ADJUSTMENT_1.Text = dataReader[39].ToString();
                txtSUB_POP_ADJUSTMENT_2.Text = dataReader[40].ToString();
                txtSUB_POP_ADJUSTMENT_3.Text = dataReader[41].ToString();
                txtSUB_POP_ADJUSTMENT_4.Text = dataReader[42].ToString();
                txtSUB_POP_ADJUSTMENT_5.Text = dataReader[43].ToString();


                // set the radio buttons on form
                // data type 
                setRB(gbDoseResponse, (int)dataReader[3]);
                // beta type
                setRB(gbBetaSource, (int)dataReader[4]);
                // pm threshold type
                setRB(gbPMTresholdType, (int)dataReader[5]);
                // pm trajectory type
                setRB(gbPMTrajectory, (int)dataReader[6]);
                // lag type
                setRB(gbLagType, (int)dataReader[7]);
                // dynamic birth type
                setRB(gbBirthsDynamic, (int)dataReader[8]);
                // lag function type
                setRB(gbLagFunction, (int)dataReader[44]);


                // load combo boxes
                // combo box load varies with aggregation/disaggregation
                string strSQL = "";
                if ((int)dataReader[3] > 0)
                { // disaggregated
                    strSQL = "SELECT SB.STUDY_ID, SB.STUDY_NAME FROM LK_STUDIES AS SB INNER JOIN "
                                   + "STUDY_BETAS AS S ON S.STUDY_ID = SB.STUDY_ID WHERE S.AGGREGATION = 'Disaggregated' "
                                   + "ORDER BY STUDY_NAME ";
                }
                else // aggregated
                {
                    strSQL = "SELECT SB.STUDY_ID, SB.STUDY_NAME FROM LK_STUDIES AS SB LEFT JOIN "
                        + "STUDY_BETAS AS S ON S.STUDY_ID = SB.STUDY_ID WHERE S.AGGREGATION = 'Aggregated' or S.AGGREGATION IS NULL "
                        + "ORDER BY STUDY_NAME ";
                }


                FbCommand cmdStudies = new FbCommand();
                cmdStudies.Connection = dbConnection;
                cmdStudies.CommandType = CommandType.Text;
                cmdStudies.CommandText = strSQL;
                FbDataReader drStudies = cmdStudies.ExecuteReader();
                DataTable dtStudies = new DataTable();
                dtStudies.Load(drStudies);
                cbStudy.DataSource = dtStudies;
                cbStudy.DisplayMember = "STUDY_NAME";
                cbStudy.ValueMember = "STUDY_ID";
                // preselect study from scenario table
                cbStudy.SelectedValue = (int)dataReader[18];
                

            }
        }

        private void updateScenarioDataFromForm(int Scenario_ID)
        {
            FbCommand cUpdate = new FbCommand();
            cUpdate.Connection = dbConnection;
            cUpdate.CommandType = CommandType.Text;
            // scenario name
            string strSQL = "UPDATE SCENARIOS SET SCENARIO_NAME ='" + txtScenarioName.Text + "' WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            // begin date
            strSQL = "UPDATE SCENARIOS SET BEGIN_YEAR =" + nudStartYear.Value.ToString() + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            // end date
            strSQL = "UPDATE SCENARIOS SET END_YEAR =" + nudEndYear.Value.ToString() + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            // add new boxes - - need to check indexes
            //  USER_SPECIFIED_BETA, PM_THRESHOLD_VALUE, BETA_ADJ_FACTOR, "
            strSQL = "UPDATE SCENARIOS SET USER_SPECIFIED_BETA=" + txtUserSuppliedBeta.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            strSQL = "UPDATE SCENARIOS SET PM_THRESHOLD_VALUE=" + txtPMThreshold.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            strSQL = "UPDATE SCENARIOS SET BETA_ADJ_FACTOR=" + txtBetaAdj.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            // + "LAG_K_SINGLE, LAG_K_MULTIPLE_CARDIO, LAG_K_MULTIPLE_LUNG, LAG_K_MULTIPLE_OTHER "
            strSQL = "UPDATE SCENARIOS SET LAG_K_SINGLE=" + txtLagSingle.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            strSQL = "UPDATE SCENARIOS SET LAG_K_MULTIPLE_CARDIO=" + txtLagCardio.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            strSQL = "UPDATE SCENARIOS SET LAG_K_MULTIPLE_LUNG=" + txtLagLung.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            strSQL = "UPDATE SCENARIOS SET LAG_K_MULTIPLE_OTHER=" + txtLagOther.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            // + "AGE_RANGE_START, AGE_RANGE_END "
            strSQL = "UPDATE SCENARIOS SET AGE_RANGE_START=" + txtYoungest.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            strSQL = "UPDATE SCENARIOS SET AGE_RANGE_END =" + txtOldest.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            // yearly factors not done yet!
            // PM_YEAR_1, PM_YEAR_2, PM_YEAR_3, PM_YEAR_4, PM_YEAR_5,
            strSQL = "UPDATE SCENARIOS SET PM_YEAR_1 =" + txtPMYear_1.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET PM_YEAR_2 =" + txtPMYear_2.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET PM_YEAR_3 =" + txtPMYear_3.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET PM_YEAR_4 =" + txtPMYear_4.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET PM_YEAR_5 =" + txtPMYear_5.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            // PM_VAL_1, PM_VAL_2, PM_VAL_3, PM_VAL_4, PM_VAL_5, 
            strSQL = "UPDATE SCENARIOS SET PM_VAL_1 =" + txtPM_Val_1.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET PM_VAL_2 =" + txtPM_Val_2.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET PM_VAL_3 =" + txtPM_Val_3.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET PM_VAL_4 =" + txtPM_Val_4.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET PM_VAL_5 =" + txtPM_Val_5.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            //SUB_POP_START_1, SUB_POP_START_2, SUB_POP_START_3, SUB_POP_START_4, SUB_POP_START_5, 
            strSQL = "UPDATE SCENARIOS SET SUB_POP_START_1 =" + txtSUB_POP_START_1.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET SUB_POP_START_2 =" + txtSUB_POP_START_2.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET SUB_POP_START_3 =" + txtSUB_POP_START_3.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET SUB_POP_START_4 =" + txtSUB_POP_START_4.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET SUB_POP_START_5 =" + txtSUB_POP_START_5.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            //SUB_POP_END_1, SUB_POP_END_2, SUB_POP_END_3, SUB_POP_END_4, SUB_POP_END_5,
            strSQL = "UPDATE SCENARIOS SET SUB_POP_END_1 =" + txtSUB_POP_END_1.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET SUB_POP_END_2 =" + txtSUB_POP_END_2.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET SUB_POP_END_3 =" + txtSUB_POP_END_3.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET SUB_POP_END_4 =" + txtSUB_POP_END_4.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET SUB_POP_END_5 =" + txtSUB_POP_END_5.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            // SUB_POP_ADJUSTMENT_1, SUB_POP_ADJUSTMENT_2, SUB_POP_ADJUSTMENT_3, SUB_POP_ADJUSTMENT_4, SUB_POP_ADJUSTMENT_5
            strSQL = "UPDATE SCENARIOS SET SUB_POP_ADJUSTMENT_1 =" + txtSUB_POP_ADJUSTMENT_1.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET SUB_POP_ADJUSTMENT_2 =" + txtSUB_POP_ADJUSTMENT_2.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET SUB_POP_ADJUSTMENT_3 =" + txtSUB_POP_ADJUSTMENT_3.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET SUB_POP_ADJUSTMENT_4 =" + txtSUB_POP_ADJUSTMENT_4.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            strSQL = "UPDATE SCENARIOS SET SUB_POP_ADJUSTMENT_5 =" + txtSUB_POP_ADJUSTMENT_5.Text + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();



            // radio boxes
            // dose response type
            strSQL = "UPDATE SCENARIOS SET DR_APPROACH_ID = " + getRB(gbDoseResponse).ToString() + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            // beta type
            strSQL = "UPDATE SCENARIOS SET BETA_TYPE_ID = " + getRB(gbBetaSource).ToString() + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            // pm threshold type
            strSQL = "UPDATE SCENARIOS SET PM_THRESHOLD_CHOICE = " + getRB(gbPMTresholdType).ToString() + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            // pm trajectory type
            strSQL = "UPDATE SCENARIOS SET PS_TRAJECTORY_ID = " + getRB(gbPMTrajectory).ToString() + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            // lag type
            strSQL = "UPDATE SCENARIOS SET LAG_TYPE_ID = " + getRB(gbLagType).ToString() + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
            // dynamic birth type
            strSQL = "UPDATE SCENARIOS SET BIRTHS_DYNAMIC = " + getRB(gbBirthsDynamic).ToString() + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            // lag function type
            strSQL = "UPDATE SCENARIOS SET LAG_FUNCT_TYPE_ID= " + getRB(gbLagFunction).ToString() + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();

            // Combo Boxes
            strSQL = "UPDATE SCENARIOS SET STUDY_ID = " + cbStudy.SelectedValue.ToString() + " WHERE SCENARIO_ID =" + Scenario_ID.ToString();
            cUpdate.CommandText = strSQL;
            cUpdate.ExecuteNonQuery();
        }

        private void setRB(GroupBox gbBox, int iToSet)
        {
            foreach (Control myControl in gbBox.Controls)
            {
                if ((myControl is RadioButton) && (myControl.TabIndex == iToSet))
                {
                    RadioButton tempBox = (RadioButton)myControl;
                    tempBox.Checked = true;
                }
            }

        }

        private int getRB(GroupBox gbBox)
        {
            // returns tab index of selected radio button in group box, 0 if nothing selected
            foreach (Control myControl in gbBox.Controls)
            {
                if (myControl is RadioButton)
                {
                    RadioButton tempBox = (RadioButton)myControl;
                    if (tempBox.Checked)
                    {
                        return tempBox.TabIndex;
                    }
                    else return 0;
                }
            }
            return 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void tabIntro_Click(object sender, EventArgs e)
        {

        }


        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void btnNext_Click(object sender, EventArgs e)
        { // select next page
            currentpage++;
            pageChange();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            currentpage--;
            pageChange();
        }

        /*if (currentpage == MAXPAGE)
            {
                btnNext.Visible = false;
                btnBack.Visible = true;
            }
            else if (currentpage > MAXPAGE)
            {    // don't go off end of tab control
                currentpage = MAXPAGE;
                btnNext.Visible = false;
                btnBack.Visible = true;
            }
            else
            {
                btnNext.Visible = true;
                if (currentpage > 0)
                {
                    btnBack.Visible = true;
                }
            };
            tabControl1.SelectTab(currentpage);
         */

        private void pageChange()   
        {
            // handle page change on tab or next button
            if (currentpage == 0)
            {
                btnBack.Visible = false;
                btnNext.Visible = true;
            }
            else if (currentpage < 0)
            {    // don't go off end of tab control
                currentpage = 0;
                btnBack.Visible = false;
                btnNext.Visible = true;
            }
            else if (currentpage == MAXPAGE)
            {
                btnNext.Visible = false;
                btnBack.Visible = true;
            }
            else if (currentpage > MAXPAGE)
            {    // don't go off end of tab control
                currentpage = MAXPAGE;
                btnNext.Visible = false;
                btnBack.Visible = true;

            } else
            {
                btnBack.Visible = true;
                if (currentpage < MAXPAGE - 1)
                {
                    btnNext.Visible = true;
                }
            };
            tabControl1.SelectTab(currentpage);
        }


        private void btnRunModel_Click(object sender, EventArgs e)
        {
            updateScenarioDataFromForm(DEFAULTSCENARIOID);
            // hide buttons to prevent changes to values or attempt to run more than once
            btnRunModel.Visible = false;
            btnBack.Visible = false;
            btnNext.Visible = false;
            progressBar1.Visible = true;
            lblRunStatus.Text = "PopSIM Model Started";
            lblRunProgress.Visible = true;
            lblRunStatus.Visible = true;

            // start model
            backgroundWorker1.RunWorkerAsync();
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            fileName = "PopsimResults";

            // open file
            // get user location for file save
            SaveFileDialog myDialog = new SaveFileDialog();
            myDialog.Filter = "Excel workbooks (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            // make the default save location "My Documents" - this is one of the few locations that is writable for all users
            myDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            myDialog.FileName = fileName;
            myDialog.OverwritePrompt = true;

            if (myDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = myDialog.FileName;
            }
            else
            {
                MessageBox.Show("Cancelled by User - file not saved.");
                return; // bail out
            }
            // show progress bar
            progressBar1.Value = 0;
            progressBar1.Visible = true;
            btnOutput.Visible = false;
            bwOutput.RunWorkerAsync();

        }
        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Help not implemented in this version");
        }

        private void label39_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            StartTime = DateTime.Now;
            PopSimModel psmModel = new PopSimModel(backgroundWorker1, e);
            psmModel.runPopSim();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            StopTime = DateTime.Now;
            btnOutput.Visible = true;
            progressBar1.Visible = true;
            lblRunStatus.Text = "Model Run Completed";
            MessageBox.Show("Run finished in " + (StopTime - StartTime).ToString(), "Run Completed");
            progressBar1.Visible = false;
            lblRunStatus.Visible = false;
            lblRunProgress.Visible = false;
            btnRunModel.Visible = false;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            // handle return status messages
            if (typeof(string[]) == e.UserState.GetType())
            {
                string[] StatusMsg = (string[])e.UserState;
                lblRunStatus.Text = StatusMsg[0];
            }
        }

        private void bwOutput_DoWork(object sender, DoWorkEventArgs e)
        {
            // output model results to workbook
            FirebirdOutput outputRoutine = new FirebirdOutput();

            outputRoutine.OpenWorkbook(fileName);
            if (outputRoutine.GetWBOpen())
            {    // output worksheets to workbook - modified to be in inverse order from original to get workbooks in correct output order
                bwOutput.ReportProgress(0);
                bwOutput.ReportProgress(0);

            
                // crosstab queries
                bwOutput.ReportProgress(10);
                outputRoutine.queryStringToXTabWB("Select Age, Year_Num, Sum(Increase_Male) as Increase_Female from  "
                    + " RPT_INC_PERIOD_COND_LIFE_EXP GROUP BY Age, Year_Num", "Report_Increase_PCLE_Male",
                    "Age", "Year_Num", "Increase_Female");

                bwOutput.ReportProgress(20);
                outputRoutine.queryStringToXTabWB("Select Age, Year_Num, Sum(Increase_Female) as Increase_Female from  "
                    + " RPT_INC_PERIOD_COND_LIFE_EXP GROUP BY Age, Year_Num", "Report_Increase_PCLE_Female",
                    "Age", "Year_Num", "Increase_Female");

                bwOutput.ReportProgress(30);
                outputRoutine.queryStringToXTabWB("Select Age, Year_Num, Sum(Increase_Male) as Increase_Male from  "
                                + " RPT_INC_COHORT_COND_LIFE_EXP GROUP BY Age, Year_Num", "Report_Increase_CCLE_Male",
                                "Age", "Year_Num", "Increase_Male");
                
                bwOutput.ReportProgress(40);
                outputRoutine.queryStringToXTabWB("Select Age, Year_Num, Sum(Increase_Female) as Increase_Female from  "
                    + " RPT_INC_COHORT_COND_LIFE_EXP GROUP BY Age, Year_Num", "Report_Increase_CCLE_Female",
                    "Age", "Year_Num", "Increase_Female");

                bwOutput.ReportProgress(60);
                outputRoutine.queryStringToXTabWB("Select Age, Year_Num, Sum(Life_Years_Gained) as Life_Years_Gained "
                    + "FROM Report_Life_Years_Gained Group By Age, Year_Num ", "Report_Life_Years_Gained_Xtab",
                    "Age", "Year_Num", "Life_Years_Gained");
                                
                bwOutput.ReportProgress(70);
                outputRoutine.queryStringToXTabWB("SELECT Age, Year_Num, Sum(Avoided_Deaths) AS Avoided_Deaths "
                    + " FROM Report_Avoided_Deaths GROUP BY Age, Year_Num ORDER BY Age, Year_Num", "Report_Avoided_Deaths_Xtab",
                    "Age", "Year_Num", "Avoided_Deaths");



                bwOutput.ReportProgress(80);
                outputRoutine.queryStringToWB("Select * from Report_Beta_Summary", "Report_Beta_Summary");
                bwOutput.ReportProgress(90);
                outputRoutine.queryStringToWB("Select * from Report_Input_Summary", "Report_Input_Summary");
                

                // close workbook
                outputRoutine.CloseWorkbook(fileName);
                bwOutput.ReportProgress(100);
                MessageBox.Show("Files Saved");

            }

        }

        private void bwOutput_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void bwOutput_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (getRB(gbBetaSource) == 0)
            {
                cbStudy.Visible = true;
                lblStudyBeta.Visible = true;
                txtUserSuppliedBeta.Visible = false;
                lblUserSuppliedBeta.Visible = false;
            }
            else
            {
                cbStudy.Visible = false;
                lblStudyBeta.Visible = false;
                txtUserSuppliedBeta.Visible = true;
                lblUserSuppliedBeta.Visible = true;
            }
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (getRB(gbBetaSource) == 1)
            {
                cbStudy.Visible = false;
                lblStudyBeta.Visible = false;
                txtUserSuppliedBeta.Visible = true;
                lblUserSuppliedBeta.Visible = true;
            }
            else
            {
                cbStudy.Visible = true;
                cbStudy.Visible = true;
                txtUserSuppliedBeta.Visible = false;
                lblUserSuppliedBeta.Visible = false;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (getRB(gbPMTresholdType) == 0)
            {
                lblPMThreshold.Visible = false;
                txtPMThreshold.Visible = false;
                lblBetaAdjAtThreshold.Visible = false;
                txtBetaAdj.Visible = false;

            }
            else
            {
                lblPMThreshold.Visible = true;
                txtPMThreshold.Visible = true;
                lblBetaAdjAtThreshold.Visible = true;
                txtBetaAdj.Visible = true;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (getRB(gbPMTresholdType) == 0)
            {
                lblPMThreshold.Visible = false;
                txtPMThreshold.Visible = false;
                lblBetaAdjAtThreshold.Visible = false;
                txtBetaAdj.Visible = false;
                
            }
            else
            {
                lblPMThreshold.Visible = true;
                txtPMThreshold.Visible = true;
                lblBetaAdjAtThreshold.Visible = true;
                txtBetaAdj.Visible = true;
            }
        }

        private void rbAggregated_CheckedChanged(object sender, EventArgs e)
        {
            // restrict study box to aggregated studies
            // load combo boxes
            string strSQL = "SELECT SB.STUDY_ID, SB.STUDY_NAME FROM LK_STUDIES AS SB LEFT JOIN "
                + "STUDY_BETAS AS S ON S.STUDY_ID = SB.STUDY_ID WHERE S.AGGREGATION = 'Aggregated' or S.AGGREGATION IS NULL "
                + "ORDER BY STUDY_NAME ";

            FbCommand cmdStudies = new FbCommand();
            cmdStudies.Connection = dbConnection;
            cmdStudies.CommandType = CommandType.Text;
            cmdStudies.CommandText = strSQL;
            FbDataReader drStudies = cmdStudies.ExecuteReader();
            DataTable dtStudies = new DataTable();
            dtStudies.Load(drStudies);
            cbStudy.DataSource = dtStudies;
            cbStudy.DisplayMember = "STUDY_NAME";
            cbStudy.ValueMember = "STUDY_ID";
            // preselect study from scenario table
            // cbStudy.SelectedIndex = (int)dataReader[18];


            if (getRB(gbDoseResponse) == 0) // can't have a cause-specific lag for an aggregated response
            {
                radioButton11.Visible = false;
                setRB(gbLagType, 0);
                makeLagTypeControlsVisible(0);
            }
            else
            {
                radioButton11.Visible = true;
                makeLagTypeControlsVisible(getRB(gbLagType));

            }
            makeLagTypeControlsVisible(getRB(gbLagType));
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            makeLagTypeControlsVisible(getRB(gbLagType));
        }

        private void makeLagTypeControlsVisible(int iLagType)
        {
            // toggle the boxes based on lag type (single or cause specific)
            if (iLagType == 0)
            {
                lblSingleLag.Visible = true;
                txtLagSingle.Visible = true;
                lblCauseSpecificLag.Visible = false;
                lblCause.Visible = false;
                lblK.Visible = false;
                lblCardio.Visible = false;
                txtLagCardio.Visible = false;
                lblLung.Visible = false;
                txtLagLung.Visible = false;
                lblOther.Visible = false;
                txtLagOther.Visible = false;
            }
            else
            {
                lblSingleLag.Visible = false;
                txtLagSingle.Visible = false;
                lblCauseSpecificLag.Visible = true;
                lblCause.Visible = true;
                lblK.Visible = true;
                lblCardio.Visible = true;
                txtLagCardio.Visible = true;
                lblLung.Visible = true;
                txtLagLung.Visible = true;
                lblOther.Visible = true;
                txtLagOther.Visible = true;
            }
        }

        private void rbDisaggregated_CheckedChanged(object sender, EventArgs e)
        {
            makeLagTypeControlsVisible(getRB(gbLagType));
            // load combo boxes
            string strSQL = "SELECT SB.STUDY_ID, SB.STUDY_NAME FROM LK_STUDIES AS SB INNER JOIN "
                + "STUDY_BETAS AS S ON S.STUDY_ID = SB.STUDY_ID WHERE S.AGGREGATION = 'Disaggregated' "
                + "ORDER BY STUDY_NAME ";
            FbCommand cmdStudies = new FbCommand();
            cmdStudies.Connection = dbConnection;
            cmdStudies.CommandType = CommandType.Text;
            cmdStudies.CommandText = strSQL;
            FbDataReader drStudies = cmdStudies.ExecuteReader();
            DataTable dtStudies = new DataTable();
            dtStudies.Load(drStudies);
            cbStudy.DataSource = dtStudies;
            cbStudy.DisplayMember = "STUDY_NAME";
            cbStudy.ValueMember = "STUDY_ID";
            txtUserSuppliedBeta.Text = "0"; // can't have user supplied beta for disaggregated model
        }

        private void txtUserSuppliedBeta_Leave(object sender, EventArgs e)
        {
            double dblTemp;
            const double MIN_BETA = -1.74E-02;
            const double MAX_BETA = 0.031;
                    
            try // is this numeric?
            {
                dblTemp = double.Parse( txtUserSuppliedBeta.Text);
                if (dblTemp < MIN_BETA)
                {   MessageBox.Show("Beta must be greater than or equal to "+MIN_BETA.ToString(),"Reset value to "+MIN_BETA.ToString());
                    txtUserSuppliedBeta.Text = MIN_BETA.ToString();
   
                }else if (dblTemp > MAX_BETA)
                {
                    MessageBox.Show("Beta must be less than or equal to " + MAX_BETA.ToString(), "Reset value to " + MAX_BETA.ToString());
                    txtUserSuppliedBeta.Text = MAX_BETA.ToString();
                }
            }catch {
                MessageBox.Show("Beta value must be a number");
                txtUserSuppliedBeta.Focus();    // return to box so user can fix value
            
            }
        }

        private void txtBetaAdj_Leave(object sender, EventArgs e)
        {
            double dblTemp;
            const double MIN_BETA = 0;
            const double MAX_BETA = 1;

            try // is this numeric?
            {
                dblTemp = double.Parse(txtBetaAdj.Text);
                if (dblTemp < MIN_BETA)
                {
                    MessageBox.Show("Beta adjustment must be greater than or equal to " + MIN_BETA.ToString(), "Reset value to " + MIN_BETA.ToString());
                    txtBetaAdj.Text = MIN_BETA.ToString();

                }
                else if (dblTemp > MAX_BETA)
                {
                    MessageBox.Show("Beta adjustment must be less than or equal to " + MAX_BETA.ToString(), "Reset value to " + MAX_BETA.ToString());
                    txtBetaAdj.Text = MAX_BETA.ToString();
                }
            }
            catch
            {
                MessageBox.Show("Beta adjustment value must be a number");
                txtBetaAdj.Focus();    // return to box so user can fix value

            }

        }

        private void txtPMThreshold_Leave(object sender, EventArgs e)
        {
            double dblTemp;
            const double MIN_PM = 4;

            try // is this numeric?
            {
                dblTemp = double.Parse(txtPMThreshold.Text);
                if (dblTemp < MIN_PM)
                {
                    MessageBox.Show("PM Threshold must be greater than or equal to " + MIN_PM.ToString(), "Reset value to " + MIN_PM.ToString());
                    txtPMThreshold.Text = MIN_PM.ToString();

                }
                
            }
            catch
            {
                MessageBox.Show("PM threshold must be a number");
                txtBetaAdj.Focus();    // return to box so user can fix value

            }

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // update current page counter to match tab clicked by user
            currentpage = tabControl1.SelectedIndex;
            // refresh next and back buttons based on current page
            pageChange();
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // from http://arsalantamiz.blogspot.com/2008/07/custom-tab-control-layout.html
            // translated from VB.Net
            Graphics g; 
            String sText;
            int iX;
            int iY;
            SizeF sizeText;
            TabControl ctlTab;

            ctlTab = (TabControl) sender;

            g = e.Graphics;

            sText = ctlTab.TabPages[e.Index].Text;
            sizeText = g.MeasureString(sText, ctlTab.Font);
            iX = e.Bounds.Left + 6;
            iY =(int) (e.Bounds.Top + (e.Bounds.Height - sizeText.Height) / 2);
            g.DrawString(sText, ctlTab.Font, Brushes.Black, iX, iY);
        }
    }
    }
