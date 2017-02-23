using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotSpatial.Data;

namespace BenMAP.Crosswalks
{
    public partial class frmCrosswalk : Form
    {
        //Define local variables
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly DAL _dal = new DAL(CommonClass.Connection);

        System.Data.DataSet _ds1, _ds2;
        private Boolean _HandsFree = false;
        private int _GridID1, _GridID2;

        public frmCrosswalk()
        {
            InitializeComponent();
        }

        public void RunCompact(int GridID1, int GridID2) 
        {
            //This mode is for running an individual crosswalk with the progress bar and no other user interface.
            string commandText = "";
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
            
            string GridName1 = _dal.GetGridDefinitionName(_GridID1);
            string GridName2 = _dal.GetGridDefinitionName(_GridID2);

            this.Text = string.Format("BenMAP - Crosswak Calculator - {0} & {1}",GridName1,GridName2) ;
            this.ShowDialog(); // When the form is activated it will check if we are in hands free mode and if so will automatically run the crosswalk and write the database.
            this.Close();//when done, close this window (unless it's already closed...)
        }

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

            var result = MessageBox.Show(
                    "This action will delete all current crosswalk definitions in the current setup requiring them to be rebuilt when they are requested.",
                    "Confirm Crosswalk Deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Cancel)
            {
                return;
            }

            _dal.DeleteAllCrosswalks(CommonClass.ManageSetup.SetupID);

            MessageBox.Show("All existing crosswalk definitions in the current setup deleted.","Complete", MessageBoxButtons.OK, MessageBoxIcon.Information); 

        }

        private void frmCrosswalk_Load(object sender, EventArgs e)
        {
            if (_HandsFree == false)
            {
                //in regular mode, prepopulate both list boxes with the available grids so we can select them

                var tb1 = _dal.GetGridDefinitions(CommonClass.ManageSetup.SetupID);
                var tb2 = tb1.Copy();

                lstCrosswalks1.DataSource = tb1;
                lstCrosswalks2.DataSource = tb2;

                lstCrosswalks1.DisplayMember = "GridDefinitionName";
                lstCrosswalks2.DisplayMember = "GridDefinitionName";
                lstCrosswalks1.ValueMember = "GridDefinitionID";
                lstCrosswalks2.ValueMember = "GridDefinitionID";
            }
            else
            {
                //in handsfree mode, we already have access to the gridID's we need so just run the crosswalks
                _dal.DeleteCrosswalk(_GridID1, _GridID2);
                PerformCrosswalk();
            }
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
                if (_dal.CrosswalkExists(_GridID1, _GridID2))
                {
                    var result = MessageBox.Show(
                            "The requested crosswalk already exists in the database." + Environment.NewLine +
                            "Do you want to replace it?", "Replace crosswalk", MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                //If we get here it means they want us to delete the crosswalk, or it doesn't exist, or only half exists.
                //so just to be safe, let's try to delete whatever is or may be there.
                _dal.DeleteCrosswalk(_GridID1, _GridID2);

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

            //Get the shapefile names based on the grid definition id
            string Shapefile1 = _dal.GetShapeFilenameForGrid(_GridID1);
            string Shapefile2 = _dal.GetShapeFilenameForGrid(_GridID2);

            string AppPath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\";

            //open the two shapefiles
            var fsInput1 = FeatureSet.Open(AppPath + "\\" + Shapefile1 + ".shp");
            var fsInput2 = FeatureSet.Open(AppPath + "\\" + Shapefile2 + ".shp");

            _cts = new CancellationTokenSource();
            var progress = new Progress(this);


            Task.Factory.StartNew(delegate
            {
                //perform the crosswalk calculation
                var results =  CrosswalksCalculator.Calculate(fsInput1, fsInput2, _cts.Token, progress);

                progress.OnProgressChanged("Writing results to database.", 0);

                // Insert results into database
                _dal.InsertCrosswalks(_GridID1, _GridID2, fsInput1, fsInput2, results,  _cts.Token, progress);

                progress.OnProgressChanged("Crosswalks written to database.", 100);
                
            }).ContinueWith(delegate (Task task)
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
                    OnCrosswalkFinish();
                }
            });
        }

        private void OnCrosswalkFinish()
        {
            if (_HandsFree)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Crosswalk computation completed.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            //Close the form.
            _cts.Cancel();
            this.Close();
        }

        private void frmCrosswalk_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Close the form.
            _cts.Cancel();
        }

    }
}
