using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotSpatial.Data;

namespace BenMAP.Crosswalks
{
    public partial class CrosswalksConfiguration : Form
    {
        #region Fields

        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly DAL _dal = new DAL(CommonClass.Connection);

        private bool _handsFree;
        private int _gridId1, _gridId2;
        private string _setupNameOverride=null;

        #endregion

        public CrosswalksConfiguration()
        {
            InitializeComponent();
        }

        public void RunCompact(int gridId1, int gridId2, string setupNameOverride) 
        {

            //This mode is for running an individual crosswalk with the progress bar and no other user interface.
            // setupName is optional and is used by the ManageSetups dialog to ensure the correct setup is used. If setupName is null, the current benmap setup will be used
            label1.Visible = false;
            lstCrosswalks1.Visible = false;
            lstCrosswalks2.Visible = false;
            btnCancel.Visible = false;
            btnClearCrosswalks.Visible = false;
            btnClose.Visible = false;
            btnCompute.Visible = false;
            tbProgress.Top = 4;
            progressBar1.Top = 27;
            Height = 84;
            Width = 420;
            tbProgress.Width = 380;
            progressBar1.Width = 380;
            ControlBox = true;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            _handsFree = true;
            _gridId1 = gridId1;
            _gridId2 = gridId2;
            _setupNameOverride = setupNameOverride;
            
            var gridName1 = _dal.GetGridDefinitionName(_gridId1);
            var gridName2 = _dal.GetGridDefinitionName(_gridId2);

            Text = string.Format("BenMAP - Crosswalk Calculator - {0} & {1}", gridName1, gridName2);

            ShowDialog();    // When the form is activated it will check if we are in hands free mode and if so will automatically run the crosswalk and write the database.
            Close();        //when done, close this window (unless it's already closed...)
        }

        private class Progress : IProgress
        {
            private readonly CrosswalksConfiguration _form1;

            public Progress(CrosswalksConfiguration form1)
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
                        if (message.Substring(0,5) == "Error")
                        {
                            _form1.lblErrorMsg.Visible= true;
                            _form1.lblErrorMsg.Text = message;
                        }
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
                this,
                "This action will delete all current crosswalk definitions in the selected setup requiring them to be rebuilt when they are requested.",
                "Confirm Crosswalk Deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Cancel)
            {
                return;
            }

            _dal.DeleteAllCrosswalks(SelectedSetup.SetupID);

            MessageBox.Show(this, "All existing crosswalk definitions in the current setup deleted.","Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void frmCrosswalk_Load(object sender, EventArgs e)
        {
            //jk 2/27/2017 populate combobox for the current setup
            cboSetupName.DataSource = _dal.GetSetups();
            cboSetupName.DisplayMember = "SetupName";
            cboSetupName.Text = CommonClass.MainSetup.SetupName;

            if (_handsFree == false)
            {
                //in regular mode, prepopulate both list boxes with the available grids so we can select them
                LoadGridDefinitions();
            }
            else
            {
                //in handsfree mode, we already have access to the gridID's we need so just run the crosswalks
                if(_setupNameOverride != null)
                {
                    cboSetupName.Text = _setupNameOverride;
                }
                _dal.DeleteCrosswalk(_gridId1, _gridId2);
                PerformCrosswalk();
            }
        }

        private BenMAPSetup SelectedSetup
        {
            get
            {
                var dgv = (DataRowView) cboSetupName.SelectedItem;
                return new BenMAPSetup
                {
                    SetupID = Convert.ToInt32(dgv["SetupID"]),
                    SetupName = Convert.ToString(dgv["SetupName"])
                };
            }
        }

        private void LoadGridDefinitions()
        {
            var tb1 = _dal.GetGridDefinitions(SelectedSetup.SetupID);
            var tb2 = tb1.Copy();

            lstCrosswalks1.DataSource = tb1;
            lstCrosswalks2.DataSource = tb2;

            lstCrosswalks1.DisplayMember = "GridDefinitionName";
            lstCrosswalks2.DisplayMember = "GridDefinitionName";
            lstCrosswalks1.ValueMember = "GridDefinitionID";
            lstCrosswalks2.ValueMember = "GridDefinitionID";
        }

        //jk 2/27/2017 reload the grid definitions according to the current setup
        private void cboSetupName_SelectedValueChanged(object sender, EventArgs e)
        {
            LoadGridDefinitions();
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            //dpa 1/28/2017 compute the crosswalk between the selected grids in the list box.
            lblErrorMsg.Visible = false; //make sure the error label is invisible.
            _gridId1 = Convert.ToInt32(lstCrosswalks1.SelectedValue);
            _gridId2 = Convert.ToInt32(lstCrosswalks2.SelectedValue);

            if (_gridId1 == _gridId2)
            {
                MessageBox.Show(this, "Please select two different grids.", "Information", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Check if they already have an entry for this crosswalk in GridDefinitionPercentages table
            if (_dal.CrosswalkExists(_gridId1, _gridId2))
            {
                // Are they locked?
                if (Convert.ToChar(((DataRowView) lstCrosswalks1.SelectedItem)["Locked"]) == 'T' &&
                    Convert.ToChar(((DataRowView) lstCrosswalks2.SelectedItem)["Locked"]) == 'T')
                {
                    var grid1Name = Convert.ToString(((DataRowView) lstCrosswalks1.SelectedItem)["GridDefinitionName"]);
                    var grid2Name = Convert.ToString(((DataRowView) lstCrosswalks2.SelectedItem)["GridDefinitionName"]);

                    MessageBox.Show(this,
                        string.Format(
                            "The crosswalk between the [{0}] and [{1}] grid definitions is provided with the BenMAP CE application and should not be deleted and recreated.",
                            grid1Name, grid2Name),
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MessageBox.Show(this,
                        "The requested crosswalk already exists in the database." + Environment.NewLine +
                        "Do you want to replace it?", "Replace crosswalk", MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                {
                    return;
                }

                _dal.DeleteCrosswalk(_gridId1, _gridId2);
            }

            // Call a separate function to actually perform the crosswalk
            PerformCrosswalk();
        }

        private IFeatureSet OpenShapeFile(string path)
        {
            try
            {
                return FeatureSet.Open(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(this, e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void PerformCrosswalk()
        {
            //Get the shapefile names based on the grid definition id
            var shapefile1 = _dal.GetShapeFilenameForGrid(_gridId1);
            var shapefile2 = _dal.GetShapeFilenameForGrid(_gridId2);

            var appPath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + SelectedSetup.SetupName + "\\";
         
            var fsInput1 = OpenShapeFile(appPath + "\\" + shapefile1 + ".shp");
            if (fsInput1 == null) return;

            var fsInput2 = OpenShapeFile(appPath + "\\" + shapefile2 + ".shp");
            if (fsInput2 == null) return;

            _cts = new CancellationTokenSource();
            var progress = new Progress(this);

            Task.Factory.StartNew(delegate
            {
                //perform the crosswalk calculation
                var results =  CrosswalksCalculator.Calculate(fsInput1, fsInput2, _cts.Token, progress);

                progress.OnProgressChanged("Writing results to database.", 0);

                // Insert results into database
                _dal.InsertCrosswalks(_gridId1, _gridId2, fsInput1, fsInput2, results,  _cts.Token, progress);

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
            if (_handsFree)
            {
                Close();
            }
            else
            {
                MessageBox.Show(this, "Crosswalk computation completed.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            //Close the form.
            Close();
        }

        private void frmCrosswalk_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Close the form.
            _cts.Cancel();

            _dal.Dispose();
        }
    }
}
