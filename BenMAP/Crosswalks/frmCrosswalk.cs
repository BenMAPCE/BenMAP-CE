using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotSpatial.Data;
using System.Data;                      //dpa provides access to CommandType

namespace BenMAP
{
    public partial class frmCrosswalk : Form
    {
        public frmCrosswalk()
        {
            InitializeComponent();
        }

        public void RunCompact(int GridID1, int GridID2) 
        {
            //This mode is for running an individual crosswalk with the progress bar and no other user interface.
            this.label1.Visible = false;
            this.lstCrosswalks1.Visible = false;
            this.lstCrosswalks2.Visible = false;
            this.btnCancel.Visible = false;
            this.btnClearCrosswalks.Visible = false;
            this.btnClose.Visible = false;
            this.btnCompute.Visible = false;
            this.tbProgress.Top = 4;
            this.progressBar1.Top = 27;
            this.Height = 84;
            this.Width = 420;
            this.tbProgress.Width = 380;
            this.progressBar1.Width = 380;
            this.ControlBox = true;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            _HandsFree = true;
            _GridID1 = GridID1;
            _GridID2 = GridID2;
            this.ShowDialog(); // When the form is activated it will check if we are in hands free mode and if so will automatically run the crosswalk and write the database.
        }

        //Define local variables
        private CancellationTokenSource _cts = new CancellationTokenSource();
        System.Data.DataSet _ds1, _ds2;
        private Boolean _HandsFree = false;
        private int _GridID1, _GridID2;
        ESIL.DBUtility.FireBirdHelperBase _fb = new ESIL.DBUtility.ESILFireBirdHelper();

        private class Progress : IProgress
        {
            private readonly frmCrosswalk _form1;

            public Progress(frmCrosswalk form1)
            {
                _form1 = form1;
            }

            public void OnProgressChanged(string message, float progress)
            {
                try
                {
                    _form1.tbProgress.Invoke((Action)delegate
                    {
                        _form1.progressBar1.Value = Convert.ToInt32(Math.Round(progress));
                        _form1.progressBar1.Refresh();
                        _form1.tbProgress.Text = message;
                    });
                }
                catch { }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _cts.Cancel();
        }
    
        private void btnClearCrosswalks_Click(object sender, EventArgs e)
        {
            /* dpa 1/28/2017 this function is used to clear the contents of the crosswalk tables so that they can be rebuilt
             * In the future we could remove this button and just run this function when the user clicks "regenerate crosswalks".
             * The two tables of interest are: GRIDDEFINITIONPERCENTAGES and GRIDDEFINITIONPERCENTAGEENTRIES.
             */

            DialogResult result = MessageBox.Show("This action will delete all current crosswalk definitions requiring them to be rebuilt when they are requested.", "Confirm Crosswalk Deletion", MessageBoxButtons.OKCancel,MessageBoxIcon.Information);
            if (result== DialogResult.Cancel) {
                return;
            }
            int intResult = 0;
            string commandText = "";

            commandText = "DELETE from GRIDDEFINITIONPERCENTAGES";
            intResult = _fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);                    

            commandText = "DELETE from GRIDDEFINITIONPERCENTAGEENTRIES";
            intResult = _fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);   
            
            result = MessageBox.Show("All existing crosswalk definitions deleted.","Complete", MessageBoxButtons.OK, MessageBoxIcon.Information); 

        }

        private void DeleteSelectedCrosswalk()
        {
            /* dpa 1/28/2017 this function is used to clear the contents of the crosswalk tables just for the two selected entries.
             * The two tables of interest are: GRIDDEFINITIONPERCENTAGES and GRIDDEFINITIONPERCENTAGEENTRIES.
             */
            string commandText = "";
            int iResult = 0;

            //find the correct percentageid entry for the forward direction crosswalk
            commandText = string.Format("SELECT PERCENTAGEID from GRIDDEFINITIONPERCENTAGES where SOURCEGRIDDEFINITIONID={0} and TARGETGRIDDEFINITIONID={1}", _GridID1, _GridID2);
            try
            {
                iResult = Convert.ToInt32(_fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
            }
            catch { }

            //remove this percentageid entry
            commandText = string.Format("DELETE from GRIDDEFINITIONPERCENTAGEENTRIES where PERCENTAGEID={0}", iResult);
            _fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
            
            //remove all data entries for this percentageid
            commandText = string.Format("DELETE from GRIDDEFINITIONPERCENTAGES where PERCENTAGEID={0}", iResult);
            _fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

            //find the correct percentageid entry for the backward direction crosswalk
            commandText = string.Format("SELECT PERCENTAGEID from GRIDDEFINITIONPERCENTAGES where SOURCEGRIDDEFINITIONID={0} and TARGETGRIDDEFINITIONID={1}", _GridID1, _GridID2);
            try
            {
                iResult = Convert.ToInt32(_fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
            }
            catch { }
            //remove this percentageid entry
            commandText = string.Format("DELETE from GRIDDEFINITIONPERCENTAGEENTRIES where PERCENTAGEID={0}", iResult);
            _fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

            //remove all data entries for this percentageid
            commandText = string.Format("DELETE from GRIDDEFINITIONPERCENTAGES where PERCENTAGEID={0}", iResult);
            _fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

        }

        private void frmCrosswalk_Load(object sender, EventArgs e)
        {

        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            //dpa 1/28/2017 compute the crosswalk between the selected grids in the list box.
            string commandText = "";
            Boolean ForwardExists=false , BackwardExists=false;
            int iResult = 0;
            _GridID1 = Convert.ToInt32(lstCrosswalks1.SelectedValue);
            _GridID2 = Convert.ToInt32(lstCrosswalks2.SelectedValue);
            try
            {
                //Check if they already have an entry for this crosswalk in GridDefinitionPercentages table
                commandText = string.Format("select Count(*) from GridDefinitionPercentages where SOURCEGRIDDEFINITIONID={0} AND TARGETGRIDDEFINITIONID={1}", _GridID1, _GridID2);
                iResult = Convert.ToInt32(_fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (iResult > 0)
                {
                    ForwardExists = true;                
                }

                commandText = string.Format("select Count(*) from GridDefinitionPercentages where SOURCEGRIDDEFINITIONID={0} AND TARGETGRIDDEFINITIONID={1}", _GridID2, _GridID1);
                iResult = Convert.ToInt32(_fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (iResult > 0)
                {
                    BackwardExists = true;
                }

                if (ForwardExists & BackwardExists)
                {
                    DialogResult result = MessageBox.Show("The requested crosswalk already exists in the database." + Environment.NewLine + "Do you want to replace it?", "Replace crosswalk", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                //If we get here it means they want us to delete the crosswalk, or it doesn't exist, or only half exists.
                //so just to be safe, let's try to delete whatever is or may be there.
                DeleteSelectedCrosswalk();

                //Call a separate function to actually perform the crosswalk
                PerformCrosswalk();
            }
            catch
            {
            }
        }

        private void PerformCrosswalk()
        {
            //This function uses multithreading. Be careful with it.

            string commandText = "";

            //Get the shapefile names based on the grid definition id
            commandText = string.Format("select ShapeFileName from ShapeFileGridDefinitionDetails where GridDefinitionID={0}", _GridID1);
            string Shapefile1 = _fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText).ToString();
            commandText = string.Format("select ShapeFileName from ShapeFileGridDefinitionDetails where GridDefinitionID={0}", _GridID2);
            string Shapefile2 = _fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText).ToString();

            string AppPath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\";

            //open the two shapefiles
            var fsInput1 = FeatureSet.Open(AppPath + "\\" + Shapefile1 + ".shp");
            var fsInput2 = FeatureSet.Open(AppPath + "\\" + Shapefile2 + ".shp");

            _cts = new CancellationTokenSource();
            var progress = new Progress(this);


            Task.Factory.StartNew(delegate
            {
                //perform the crosswalk calculation
                return CrosswalksCalculator.Calculate(fsInput1, fsInput2, _cts.Token, progress);
            }).ContinueWith(delegate(Task<Dictionary<CrosswalkIndex, CrosswalkRatios>> task)
            {

                if (task.IsCanceled)
                {
                    progress.OnProgressChanged("Cancelled", 100);
                }
                else if (task.IsFaulted)
                {
                    var baseException = task.Exception.GetBaseException();
                    if (baseException is OperationCanceledException)
                    {
                        progress.OnProgressChanged("Cancelled", 100);
                    }
                    else
                    {
                        progress.OnProgressChanged("Faulted: " + baseException, 100);
                    }
                }
                else
                {
                    WriteResults(fsInput1, fsInput2, task.Result, progress);
                }
            });
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            //Close the form.
            _cts.Cancel();
            this.Close();
        }

        private void WriteResults(IFeatureSet fsInput1, IFeatureSet fsInput2, Dictionary<CrosswalkIndex, CrosswalkRatios> results, Progress progress)
        {
            /* dpa 1/28/2017 Algorithm completed successfully and we now have the results.
            * Here we will write the results to the database and optionally to a text file for testing purposes.
            */
            try
            {
                string commandText = "";
                tbProgress.Text = "Writing results to database.";

                int PercentageID1, PercentageID2;

                /* first we need to add entries to the griddefinitionpercentages table
                 * get the highest index already in the griddefinitionpercentages table or use 1 for the next percentageID
                 */
                commandText = "select count(*) from GridDefinitionPercentages";
                int iResult = Convert.ToInt32(_fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (iResult > 0)
                {
                    commandText = "select max(PercentageID) from GridDefinitionPercentages";
                    PercentageID1 = Convert.ToInt32(_fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                }
                else
                {
                    PercentageID1 = 1;
                }
                PercentageID2 = PercentageID1 + 1;

                commandText = string.Format("insert into GridDefinitionPercentages(PERCENTAGEID,SOURCEGRIDDEFINITIONID, TARGETGRIDDEFINITIONID) "
                    + "values({0},{1},{2})", PercentageID1, _GridID1, _GridID2);
                _fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                commandText = string.Format("insert into GridDefinitionPercentages(PERCENTAGEID,SOURCEGRIDDEFINITIONID, TARGETGRIDDEFINITIONID) "
                    + "values({0},{1},{2})", PercentageID2, _GridID2, _GridID1);
                _fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                DataTable attributes1;
                DataTable attributes2;
                int intResult = 0;
                double forward = 0.0;
                double backward = 0.0;
                int i = 0, j = 1;
                float prog = 0;
                var fieldNames = new[] { "COL", "ROW" };
                float step = results.Count / 100;
                foreach (var entry in results)
                {
                    //update the progress bar to show progress writing output to database.
                    i += 1;
                    if (i > step * j)
                    {
                        j += 1;
                        prog = Convert.ToSingle(100 * i / results.Count);
                        progress.OnProgressChanged(string.Format("{0} of {1} written to database.", i, results.Count), prog);
                    }
                    //get the column and row attributes and forward backward results
                    attributes1 = fsInput1.GetAttributes(entry.Key.FeatureId1, 1, fieldNames);
                    attributes2 = fsInput2.GetAttributes(entry.Key.FeatureId2, 1, fieldNames);
                    forward = entry.Value.ForwardRatio;
                    backward = entry.Value.BackwardRatio;

                    //write the entries to the firebird database

                    if (forward > 0.00001)
                    {
                        forward = Math.Round(forward, 4); //note - rounding doesn't seem to help. The fb database still adds noise.
                        commandText = string.Format("insert into GridDefinitionPercentageEntries(PERCENTAGEID, SOURCECOLUMN, SOURCEROW, TARGETCOLUMN, TARGETROW, PERCENTAGE,NORMALIZATIONSTATE) values({0},{1},{2},{3},{4},{5},{6})",
                            PercentageID1, attributes1.Rows[0]["COL"], attributes1.Rows[0]["ROW"], attributes2.Rows[0]["COL"], attributes2.Rows[0]["ROW"], forward, 0);
                        intResult = _fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }

                    if (entry.Value.BackwardRatio > 0.00001)
                    {
                        backward = Math.Round(backward, 4);
                        commandText = string.Format("insert into GridDefinitionPercentageEntries(PERCENTAGEID, SOURCECOLUMN, SOURCEROW, TARGETCOLUMN, TARGETROW, PERCENTAGE,NORMALIZATIONSTATE) values({0},{1},{2},{3},{4},{5},{6})",
                            PercentageID2, attributes2.Rows[0]["COL"], attributes2.Rows[0]["ROW"], attributes1.Rows[0]["COL"], attributes1.Rows[0]["ROW"], backward, 0);
                        intResult = _fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
                progress.OnProgressChanged("Crosswalks written to database.",100);
                if (_HandsFree)
                {
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Crosswalk computation completed.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            { }
        }

        private void frmCrosswalk_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Close the form.
            _cts.Cancel();
        }

        private void frmCrosswalk_Activated(object sender, EventArgs e)       
        {
            if (_HandsFree == false)
            {
                //in regular mode, prepopulate both list boxes with the available grids so we can select them
                string commandText = string.Format("select GridDefinitionName, GridDefinitionID from GridDefinitions where setupID={0}", CommonClass.ManageSetup.SetupID);
                _ds1 = _fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                _ds2 = _ds1.Copy();
                lstCrosswalks1.DataSource = _ds1.Tables[0];
                lstCrosswalks2.DataSource = _ds2.Tables[0];
                lstCrosswalks1.DisplayMember = "GridDefinitionName";
                lstCrosswalks2.DisplayMember = "GridDefinitionName";
                lstCrosswalks1.ValueMember = "GridDefinitionID";
                lstCrosswalks2.ValueMember = "GridDefinitionID";
            }
            else
            {
                //in handsfree mode, we already have access to the gridID's we need so just run the crosswalks
                DeleteSelectedCrosswalk();
                PerformCrosswalk();
            }

        }
    }
}
