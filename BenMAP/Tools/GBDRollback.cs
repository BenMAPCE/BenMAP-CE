using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;



namespace BenMAP
{
    public partial class GBDRollback : Form
    {

        private Dictionary<string, string> checkedCountries = new Dictionary<string, string>();
        private List<System.Drawing.Color> colorPalette = new List<System.Drawing.Color>();
        private List<GBDRollbackItem> rollbacks = new List<GBDRollbackItem>();
        private System.Data.DataTable dtCountries;
        private SpreadsheetDocument spreadsheetDocument;
        private uint styleIndexItalicsWithBorders;
        private uint styleIndexGrayFillWithBorders;
        private uint styleIndexNoFillWithBorders;
        private uint styleIndexNoFillIndentWithBorders;
        private uint styleIndexNoFillCenterWithBorders;
        private uint styleIndexNoFillNumber2DecimalPlacesWithBorders;
        private uint styleIndexNoFillNumber0DecimalPlacesWithBorders;
        private bool selectMapFeaturesOnNodeCheck = true;

        private const int POLLUTANT_ID = 1;
        private const double BACKGROUND = 5.8;
        private const int AQ_YEAR = 2015;
        private const int POP_YEAR = 2015;
        private const string FORMAT_DECIMAL_2_PLACES = "N";
        private const string FORMAT_DECIMAL_2_PLACES_CSV = "F";
        private const string FORMAT_DECIMAL_0_PLACES = "N0";
        private const string FORMAT_DECIMAL_0_PLACES_CSV = "F0";

        private const char MICROGRAMS = '\u00B5';
        private const char SUPER_3 = '\u00B3';

        //private System.Data.DataTable dtGBDDataByGridCell = null;
        private System.Data.DataTable dtConcEntireRollback = null; //Baseline and control concentration data for all selected countries
        private System.Data.DataTable dtConcEntirePopWeighted = null; //Population weighted AQ delta and incidence rates by Country-Age-Gender for all selected countries.
        //private DataTable dtGBDcountryConcDataByGroup = null;
        private Dictionary<string, GBDRollbackResult> rollbackResultsByCountry = null;


        Dictionary<String, IPolygonCategory> selectedButNotSavedIPCs = new Dictionary<String, IPolygonCategory>();

        private class CountryItem
        {
            string _id;
            string _name;

            public CountryItem(string Id, string Name)
            {
                _id = Id;
                _name = Name;
            }

            public string Id
            {
                get { return _id; }
                set { _id = value; }
            }

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public override string ToString()
            {
                return _name;
            }
        }


        public GBDRollback()
        {
            InitializeComponent();

            //set up locations,form size, visibility

            listCountries.Location = new System.Drawing.Point(tvRegions.Location.X, tvRegions.Location.Y);
            //increase height of list countries to better match that of tvRegions
            //to compensate for a rendering bug in the controls
            listCountries.Size = new Size(tvRegions.Size.Width, tvRegions.Size.Height + 2);

            gbCountrySelection.Location = new System.Drawing.Point(gbName.Location.X, gbName.Location.Y);
            gbParameterSelection.Location = new System.Drawing.Point(gbName.Location.X, gbName.Location.Y);
            gbMortalityValuationSetting.Location = new System.Drawing.Point(gbName.Location.X, gbName.Location.Y);
            SetActivePanel(0);
            Size = new Size(906, 777); //form size



            //parameter options in gbParameterSelection

            lblIncrement.Text = "Increment (" + MICROGRAMS.ToString() + "g/m" + SUPER_3.ToString() + "):";


            lblIncrementBackground.Text = "Background (" + MICROGRAMS.ToString() + "g/m" + SUPER_3.ToString() + "):";
            lblPercentageBackground.Text = lblIncrementBackground.Text;

            gbOptionsPercentage.Location = new System.Drawing.Point(gbOptionsIncremental.Location.X, gbOptionsIncremental.Location.Y);
            gbParameterSelection.Controls.Add(gbOptionsPercentage);
            gbOptionsStandard.Location = new System.Drawing.Point(gbOptionsIncremental.Location.X, gbOptionsIncremental.Location.Y);
            gbParameterSelection.Controls.Add(gbOptionsStandard);
            cboRollbackType.SelectedIndex = 0;
            SetActiveOptionsPanel(0);
            rbRegions.Checked = true;
            cboExportFormat.SelectedIndex = 0;

            txtFilePath.Text = CommonClass.ResultFilePath + @"\GBD";

            
            LoadCountries();
            LoadTreeView();
            LoadCountryList();
            LoadMap();
            LoadColorPalette();
            LoadStandards();
            LoadVSL();
            LoadFunctions();

        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }



        private void LoadColorPalette()
        {

            colorPalette.Add(System.Drawing.Color.FromArgb(165, 0, 38));
            colorPalette.Add(System.Drawing.Color.FromArgb(215, 48, 39));
            colorPalette.Add(System.Drawing.Color.FromArgb(244, 109, 67));
            colorPalette.Add(System.Drawing.Color.FromArgb(253, 174, 97));
            colorPalette.Add(System.Drawing.Color.FromArgb(254, 224, 144));
            colorPalette.Add(System.Drawing.Color.FromArgb(255, 255, 191));
            colorPalette.Add(System.Drawing.Color.FromArgb(224, 243, 248));
            colorPalette.Add(System.Drawing.Color.FromArgb(171, 217, 233));
            colorPalette.Add(System.Drawing.Color.FromArgb(116, 173, 209));
            colorPalette.Add(System.Drawing.Color.FromArgb(69, 117, 180));
            colorPalette.Add(System.Drawing.Color.FromArgb(49, 54, 149));

        }

        private void LoadMap()
        {
            //new map layer
            string mapFile = AppDomain.CurrentDomain.BaseDirectory + @"\Data\Shapefiles\GBDRollback\gadm_worldsimplify.shp";
            IMapPolygonLayer impl = null;
            if (File.Exists(mapFile))
            {
                IFeatureSet fs = (FeatureSet)FeatureSet.Open(mapFile);
                //mapGBD.Layers.Add(fs);
                //IMapFeatureLayer[] mfl = mapGBD.GetFeatureLayers();
                //IFeatureSet fs = (FeatureSet)FeatureSet.Open(mapFile);
                //mapGBD.Layers.Add(fs);
                //IMapFeatureLayer[] mfl = mapGBD.GetFeatureLayers();
                //mfl[0].Symbolizer = new PolygonSymbolizer(Color.Chocolate);
                //mfl[0].SelectionSymbolizer = new PolygonSymbolizer(Color.AliceBlue);
                impl = new MapPolygonLayer(FeatureSet.OpenFile(mapFile));
                //impl.Reproject(_mapArgs.Map.Projection);
                impl.LegendText = "Countries";
                impl.Symbolizer.SetFillColor(System.Drawing.Color.White);
                impl.Symbolizer.SetOutlineWidth(1);
                impl.Symbolizer.OutlineSymbolizer.SetFillColor(System.Drawing.Color.Black);
                mapGBD.Layers.Add(impl);
            }
        }

        private void LoadFunctions()
        {
            System.Data.DataTable dtFunctions = GBDRollbackDataSource.GetGBDFunctions();

            // load functions drop down
            cboFunction.DisplayMember = "FUNCTIONNAME";
            cboFunction.ValueMember = "FUNCTIONID";
            cboFunction.DataSource = dtFunctions;
        }

        private void LoadStandards()
        {
            System.Data.DataSet ds = GBDRollbackDataSource.GetStandardList();
            System.Data.DataTable dtStandards = ds.Tables[0].Copy();//new DataTable();

            //load standard drop down
            cboStandard.DisplayMember = "STANDARD_NAME";
            cboStandard.ValueMember = "STD_ID";
            cboStandard.DataSource = dtStandards;
        }

        private void LoadCountries()
        {
            System.Data.DataSet ds = GBDRollbackDataSource.GetRegionCountryList(POP_YEAR);
            dtCountries = ds.Tables[0].Copy();//new DataTable();
        }

        private void LoadTreeView()
        {
            if (dtCountries != null)
            {
                string region = String.Empty;
                string country = String.Empty;
                string countryid = String.Empty;
                tvRegions.BeginUpdate();
                foreach (DataRow dr in dtCountries.Rows)
                {
                    //new region?
                    if (!region.Equals(dr["REGIONNAME"].ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        region = dr["REGIONNAME"].ToString();
                        tvRegions.Nodes.Add(region, region);
                    }

                    countryid = dr["COUNTRYID"].ToString();
                    country = dr["COUNTRYNAME"].ToString();
                    tvRegions.Nodes[region].Nodes.Add(countryid, country);
                }
                tvRegions.EndUpdate();
            }

        }


        private void LoadCountryList()
        {
            if (dtCountries != null)
            {
                System.Data.DataTable dtTemp = dtCountries.DefaultView.ToTable(true, "COUNTRYID", "COUNTRYNAME");
                DataView dv = new DataView(dtTemp);
                dv.Sort = "COUNTRYNAME ASC";
                System.Data.DataTable dtAlph = dv.ToTable();

                string country = String.Empty;
                string countryid = String.Empty;
                foreach (DataRow dr in dtAlph.Rows)
                {
                    countryid = dr["COUNTRYID"].ToString();
                    country = dr["COUNTRYNAME"].ToString();

                    listCountries.Items.Add(new CountryItem(countryid, country));
                }
            }

        }


        private void LoadVSL()
        {
            System.Data.DataTable dtVSL = GBDRollbackDataSource.GetVSLlist();

            cboVSLStandard.DisplayMember = "VSLSTANDS";
            cboVSLStandard.ValueMember = "VSLID";
            cboVSLStandard.DataSource = dtVSL;
        }

        private void cboRollbackType_SelectedIndexChanged(object sender, EventArgs e)
        {

            SetActiveOptionsPanel(cboRollbackType.SelectedIndex);
            switch (cboRollbackType.SelectedIndex)
            {
                case 0:
                    gbOptionsIncremental.Visible = false;
                    gbOptionsPercentage.Visible = true;
                    gbOptionsStandard.Visible = false;

                    pb_incremental.Visible = false;
                    pb_percent.Visible = true;
                    pb_standard.Visible = false;
                    break;
                case 1:
                    gbOptionsIncremental.Visible = true;
                    gbOptionsPercentage.Visible = false;
                    gbOptionsStandard.Visible = false;

                    pb_incremental.Visible = true;
                    pb_percent.Visible = false;
                    pb_standard.Visible = false;

                    break;
                case 2:
                    gbOptionsIncremental.Visible = false;
                    gbOptionsPercentage.Visible = false;
                    gbOptionsStandard.Visible = true;

                    pb_incremental.Visible = false;
                    pb_percent.Visible = false;
                    pb_standard.Visible = true;

                    break;
                default:
                    gbOptionsIncremental.Visible = false;
                    gbOptionsPercentage.Visible = false;
                    gbOptionsStandard.Visible = false;

                    pb_incremental.Visible = false; ;
                    pb_percent.Visible = false;
                    pb_standard.Visible = false;

                    break;
            }

        }

        private void GBDRollback_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you wish to close?", "Confirm Close", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtName.Text.Trim()))
            {
                MessageBox.Show("Name is required.");
                txtName.Focus();
                return;
            }
            if (rollbacks.Exists(x => x.Name.Equals(txtName.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                DialogResult result = MessageBox.Show("A rollback with the name " + txtName.Text.Trim() + " already exists.  Do you wish to overwrite it?", "", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    txtName.Focus();
                    return;
                }
            }

            SetActivePanel(1);

        }

        private void btnNext2_Click(object sender, EventArgs e)
        {
            //check for country
            if (checkedCountries.Count == 0)
            {
                MessageBox.Show("You must select at least one country.");
                tvRegions.Focus();
                return;
            }

            SetActivePanel(2);
            //cboRollbackType.SelectedIndex = -1;     
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            SetActivePanel(0);
        }

        private void btnBack2_Click(object sender, EventArgs e)
        {
            SetActivePanel(1);
        }

        private void tvRegions_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                CheckChildNodes(e.Node);
                CheckParentNode(e.Node);
            }
            //Color.FromArgb(224, 243, 248)
            //if this is checked AND has no children)
            //then, it is country and we add to listd
            IMapFeatureLayer[] mfl = mapGBD.GetFeatureLayers();
            string filter = "[ID] = '" + e.Node.Name + "'";
            if ((e.Node.Checked) && (e.Node.Nodes.Count == 0))
            {
                if (!checkedCountries.ContainsKey(e.Node.Name))
                {
                    //add to country list
                    checkedCountries.Add(e.Node.Name, e.Node.Text);
                    //also select on map                
                    if (selectMapFeaturesOnNodeCheck)
                    {
                        //update map
                        IPolygonScheme ips = (IPolygonScheme)mfl[0].Symbology;
                        IPolygonCategory ipc = null;
                        ipc = new PolygonCategory(System.Drawing.Color.FromArgb(0, 255, 255), System.Drawing.Color.FromArgb(0, 225, 225), 1);
                        ipc.FilterExpression = "[ID]='" + e.Node.Name + "'";
                        selectedButNotSavedIPCs.Add(e.Node.Name, ipc);
                        mfl[0].Symbology.AddCategory(ipc);
                        mfl[0].ApplyScheme(mfl[0].Symbology);
                    }
                }
            }
            else
            {
                //remove from country list
                checkedCountries.Remove(e.Node.Name);
                //deselect from map
                if (selectedButNotSavedIPCs.ContainsKey(e.Node.Name))
                {
                    IPolygonCategory ipc = selectedButNotSavedIPCs[e.Node.Name];
                    mfl[0].Symbology.RemoveCategory(ipc);
                    selectedButNotSavedIPCs.Remove(e.Node.Name);
                    mfl[0].ApplyScheme(mfl[0].Symbology);
                }
            }

            //finally check/uncheck on country-only list box
            //but only if tvRegions is visible to avoid infinite loop
            //see listCountries_ItemCheck event
            if (tvRegions.Visible)
            {
                int index = listCountries.FindStringExact(e.Node.Text);
                if (index >= 0)
                {
                    listCountries.SetItemChecked(index, e.Node.Checked);
                }
            }

        }

        private void CheckChildNodes(TreeNode node)
        {

            //this will set child nodes, if any, to 
            //same status as parent, checked or unchecked
            tvRegions.BeginUpdate();
            foreach (TreeNode item in node.Nodes)
            {
                item.Checked = node.Checked;

                if (item.Nodes.Count > 0)
                {
                    this.CheckChildNodes(item);
                }
            }
            tvRegions.EndUpdate();
        }


        private void CheckParentNode(TreeNode node)
        {
            if (node.Parent == null)
            {
                return;
            }

            //this will set parent node, if any
            //to checked if all children are checked
            //otherwise parent will be unchecked
            tvRegions.BeginUpdate();

            bool allChecked = true;

            //loop siblings of current
            foreach (TreeNode item in node.Parent.Nodes)
            {
                if (!item.Checked)
                {
                    allChecked = false;
                    break;
                }
            }

            node.Parent.Checked = allChecked;

            tvRegions.EndUpdate();
        }

        private void btnSaveRollback_Click(object sender, EventArgs e)
        {
            GBDRollbackItem rollback = new GBDRollbackItem();
            rollback.Name = txtName.Text;
            rollback.Description = txtDescription.Text;
            rollback.Countries = new Dictionary<string,string>(checkedCountries);
            rollback.VSLID = (int)cboVSLStandard.SelectedValue;
            rollback.VSLStandard = cboVSLStandard.Text;
            double d;

            //clean text boxes for numerics
            txtPercentage.Text = txtPercentage.Text.Trim();
            txtPercentageBackground.Text = txtPercentageBackground.Text.Trim();
            txtIncrement.Text = txtIncrement.Text.Trim();
            txtIncrementBackground.Text = txtIncrementBackground.Text.Trim();

            switch (cboRollbackType.SelectedIndex)
            {
                case 0: //percentage
                    if (String.IsNullOrEmpty(txtPercentage.Text))
                    {
                        MessageBox.Show("Percentage is required.");
                        txtPercentage.Focus();
                        return;
                    }
                    if (!Double.TryParse(txtPercentage.Text, out d))
                    {
                        MessageBox.Show("Percentage must be numeric.");
                        txtPercentage.Focus();
                        return;
                    }

                    if (d > 100)
                    {
                        MessageBox.Show("Percentage can not be > 100");
                        txtPercentageBackground.Focus();
                        return;
                    }
                    if (!String.IsNullOrEmpty(txtPercentageBackground.Text))
                    {
                        if (!Double.TryParse(txtPercentageBackground.Text, out d))
                        {
                            MessageBox.Show("Background must be numeric.");
                            txtPercentageBackground.Focus();
                            return;
                        }

                    }
                    rollback.Type = GBDRollbackItem.RollbackType.Percentage;
                    rollback.Percentage = Double.Parse(txtPercentage.Text);
                    rollback.Background = BACKGROUND;
                    break;
                case 1: //incremental
                    if (String.IsNullOrEmpty(txtIncrement.Text))
                    {
                        MessageBox.Show("Increment is required.");
                        txtIncrement.Focus();
                        return;
                    }
                    if (!Double.TryParse(txtIncrement.Text, out d))
                    {
                        MessageBox.Show("Increment must be numeric.");
                        txtIncrement.Focus();
                        return;
                    }
                    if (!String.IsNullOrEmpty(txtIncrementBackground.Text))
                    {
                        if (!Double.TryParse(txtIncrementBackground.Text, out d))
                        {
                            MessageBox.Show("Background must be numeric.");
                            txtIncrementBackground.Focus();
                            return;
                        }
                    }
                    rollback.Type = GBDRollbackItem.RollbackType.Incremental;
                    rollback.Increment = Double.Parse(txtIncrement.Text);
                    rollback.Background = BACKGROUND;
                    break;
                case 2: //standard
                    if (cboStandard.SelectedIndex < 0)
                    {
                        MessageBox.Show("Standard is required.");
                        cboStandard.Focus();
                        return;
                    }
                    rollback.Type = GBDRollbackItem.RollbackType.Standard;
                    rollback.StandardName = cboStandard.GetItemText(cboStandard.SelectedItem);
                    rollback.StandardId = (int)cboStandard.SelectedValue;
                    rollback.Standard = GBDRollbackDataSource.GetStandardValue(rollback.StandardId);
                    rollback.IsNegativeRollbackToStandard = chkNegativeRollbackToStandard.Checked;
                    rollback.Background = BACKGROUND;
                    break;
            }

            rollback.Year = AQ_YEAR;
            rollback.Color = GetNextColor();

            //YY: update selected function id and name. rollback.Function data type changed from enum to string
            //switch (cboFunction.SelectedIndex)
            //{
            //    case 0: //Krewski
            //        rollback.Function = GBDRollbackItem.RollbackFunction.Krewski;
            //        rollback.FunctionID = Convert.ToInt32(cboFunction.SelectedValue.ToString());
            //        break;
            //}
            rollback.Function = cboFunction.Text;
            rollback.FunctionID = Convert.ToInt32(cboFunction.SelectedValue.ToString());

            //remove rollback if it already exists
            rollbacks.RemoveAll(x => x.Name.Equals(rollback.Name, StringComparison.OrdinalIgnoreCase));

            //add to rollbacks
            rollbacks.Add(rollback);

            //add to grid
            //dgvRollbacks.Rows.Clear();
            //foreach (GBDRollbackItem item in rollbacks)
            //{ 
            //    DataGridViewRow row = new DataGridViewRow();
            //    int i = dgvRollbacks.Rows.Add(row);
            //    dgvRollbacks.Rows[i].Cells["colName"].Value = item.Name;
            //    dgvRollbacks.Rows[i].Cells["colColor"].Style.BackColor = item.Color;
            //    dgvRollbacks.Rows[i].Cells["colTotalCountries"].Value = item.Countries.Count().ToString();
            //    dgvRollbacks.Rows[i].Cells["colTotalPopulation"].Value = GetRollbackTotalPopulation(item).ToString("#,###");
            //    dgvRollbacks.Rows[i].Cells["colRollbackType"].Value = GetRollbackTypeSummary(item);         
            //}

            RemoveGridRow(rollback.Name);
            DataGridViewRow row = new DataGridViewRow();
            int i = dgvRollbacks.Rows.Add(row);
            dgvRollbacks.Rows[i].Cells["colName"].Value = rollback.Name;
            dgvRollbacks.Rows[i].Cells["colColor"].Style.BackColor = rollback.Color;
            dgvRollbacks.Rows[i].Cells["colTotalCountries"].Value = rollback.Countries.Count().ToString();
            dgvRollbacks.Rows[i].Cells["colTotalPopulation"].Value = GetRollbackTotalPopulation(rollback).ToString("#,###");
            dgvRollbacks.Rows[i].Cells["colRollbackType"].Value = GetRollbackTypeSummary(rollback);
            dgvRollbacks.Rows[i].Cells["colFunction"].Value = rollback.Function.ToString();
            dgvRollbacks.Rows[i].Cells["colVSL"].Value = cboVSLStandard.Text.ToString();
            dgvRollbacks.Rows[i].Cells["colExecute"].Value = true;
            ToggleExecuteScenariosButton();


            //update map
            IMapFeatureLayer[] mfl = mapGBD.GetFeatureLayers();
            mfl[0].ClearSelection();
            foreach (IPolygonCategory ipcToRemove in selectedButNotSavedIPCs.Values)
            {
                mfl[0].Symbology.RemoveCategory(ipcToRemove);
            }
            selectedButNotSavedIPCs.Clear();
            IPolygonScheme ips = (IPolygonScheme)mfl[0].Symbology;
            IPolygonCategory ipc = null;
            //grab existing ips and add to it
            foreach (String s in rollback.Countries.Keys)
            {
                ipc = new PolygonCategory(rollback.Color, System.Drawing.Color.Black, 1);
                ipc.FilterExpression = "[ID]='" + s + "'";
                rollback.addIPC(ipc);
                ips.AddCategory(ipc);
                //set color of selected country features on map
                //string filter = "[ID] in (" + String.Join(",", rollback.Countries.Select(x => "'" + x.Key + "'")) + ")";
                //mfl[0].SelectByAttribute(filter, ModifySelectionMode.Subtract);
                //PolygonCategory category = new PolygonCategory(rollback.Color, Color.Black, 4);
                //category.FilterExpression = filter;
                //mfl[0].Symbology.AddCategory(ipc);        

            }
            mfl[0].ApplyScheme(ips);

            ClearFields();
            SetActivePanel(0);

        }

        private void RemoveGridRow(string name)
        {
            foreach (DataGridViewRow row in dgvRollbacks.Rows)
            {
                string s = row.Cells["colName"].Value.ToString();
                if (s.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    dgvRollbacks.Rows.Remove(row);
                    return;
                }
            }
        }

        private System.Drawing.Color GetNextColor()
        {
            foreach (System.Drawing.Color c in colorPalette)
            {
                GBDRollbackItem item = rollbacks.Find(x => x.Color.ToArgb() == c.ToArgb());
                if (item == null)
                {
                    return c;
                }

            }

            return GetRandomColor();
        }

        private System.Drawing.Color GetRandomColor()
        {
            Random random = new Random();
            return System.Drawing.Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
        }

        private long GetRollbackTotalPopulation(GBDRollbackItem rollback)
        {
            //build selected list of countries, pops
            string expression = "COUNTRYID in (" + String.Join(",", rollback.Countries.Select(x => "'" + x.Key + "'")) + ")";
            DataRow[] rows = dtCountries.Select(expression);
            System.Data.DataTable dt = rows.CopyToDataTable<DataRow>();

            long lPop = 0;

            // Declare an object variable. 
            object sumObject;
            sumObject = dt.Compute("Sum(POPULATION)", "");
            lPop = Int64.Parse(sumObject.ToString());

            return lPop;




        }


        private void ClearFields()
        {
            //clear fields
            txtName.Text = String.Empty;
            txtDescription.Text = String.Empty;
            selectMapFeaturesOnNodeCheck = false;
            //hide regions tree and countries list to avoid any feedback loop
            //see tvRegions_AfterCheck and listCountries_ItemCheck
            tvRegions.Visible = false;
            listCountries.Visible = false;
            //clear regions tree
            foreach (TreeNode node in tvRegions.Nodes)
            {
                node.Checked = false;
                foreach (TreeNode tn in node.Nodes)
                {
                    tn.Checked = false;
                }
            }
            //collapse tree nodes
            tvRegions.CollapseAll();
            //clear country list
            foreach (int checkedItemIndex in listCountries.CheckedIndices)
            {

                listCountries.SetItemChecked(checkedItemIndex, false);

            }
            //check regions radio button
            rbRegions.Checked = true; //this will show regions tree via rbRegions_CheckedChanged
            tvRegions.Visible = true; //the line above is not firing rbRegions_CheckedChanged, so set regions tree to visible
            //IMapFeatureLayer[] mfl = mapGBD.GetFeatureLayers();
            //mfl[0].UnSelectAll();
            selectMapFeaturesOnNodeCheck = true;
            cboRollbackType.SelectedIndex = (int)GBDRollbackItem.RollbackType.Percentage;
            txtPercentage.Text = String.Empty;
            txtPercentageBackground.Text = String.Empty;
            txtIncrement.Text = String.Empty;
            txtIncrementBackground.Text = String.Empty;
            cboStandard.SelectedIndex = -1;
            cboVSLStandard.SelectedValue = 1; //YY: reset VSL to default VSL
            cboFunction.SelectedValue = 1; //YY: reset function to the first one (Krewski)
        }

        private void LoadRollback(GBDRollbackItem item)
        {
            txtName.Text = item.Name;
            txtDescription.Text = item.Description;
            IMapFeatureLayer[] mfl = mapGBD.GetFeatureLayers();
            //IPCs can be hanging if scenario is edited then a country is deselected.
            foreach (IPolygonCategory ipcOld in item.IpcList)
            {
                mfl[0].Symbology.RemoveCategory(ipcOld);
            }
            item.IpcList.Clear();
            mfl[0].ApplyScheme(mfl[0].Symbology);

            //hide regions tree and countries list to avoid any feedback loop
            //see tvRegions_AfterCheck and listCountries_ItemCheck
            tvRegions.Visible = false;
            listCountries.Visible = false;
            foreach (KeyValuePair<string, string> kvp in item.Countries)
            {
                //check regions tree node
                string countryid = kvp.Key;
                TreeNode[] nodes = tvRegions.Nodes.Find(countryid, true);
                foreach (TreeNode node in nodes)
                {
                    node.Checked = true;
                    CheckParentNode(node);
                }
                //check country list item
                int index = listCountries.FindStringExact(kvp.Value);
                if (index >= 0)
                {
                    listCountries.SetItemChecked(index, true);
                }
            }
            rbRegions.Checked = true; //this will show regions tree via rbRegions_CheckedChanged
            tvRegions.Visible = true; //the line above is not firing rbRegions_CheckedChanged, so set regions tree to visible

            cboRollbackType.SelectedIndex = (int)item.Type;
            txtPercentage.Text = item.Percentage.ToString();
            txtPercentageBackground.Text = item.Background.ToString();
            txtIncrement.Text = item.Increment.ToString();
            txtIncrementBackground.Text = item.Background.ToString();
            cboStandard.SelectedIndex = (int)item.StandardId;

            cboFunction.SelectedValue = (int)item.FunctionID; //YY: changed function to FunctionID
            cboVSLStandard.SelectedValue = (int)item.VSLID; //YY: Load selected VSL.

        }

        private void SetActivePanel(int index)
        {
            switch (index)
            {
                case 0:
                    gbName.Visible = true;
                    gbCountrySelection.Visible = false;
                    gbParameterSelection.Visible = false;
                    gbMortalityValuationSetting.Visible = false;
                    break;
                case 1:
                    gbName.Visible = false;
                    gbCountrySelection.Visible = true;
                    gbParameterSelection.Visible = false;
                    gbMortalityValuationSetting.Visible = false;
                    break;
                case 2:
                    gbName.Visible = false;
                    gbCountrySelection.Visible = false;
                    gbParameterSelection.Visible = true;
                    gbMortalityValuationSetting.Visible = false;
                    break;
                case 3:
                    gbName.Visible = false;
                    gbCountrySelection.Visible = false;
                    gbParameterSelection.Visible = false;
                    gbMortalityValuationSetting.Visible = true;
                    break;
            }
        }

        private void SetActiveOptionsPanel(int index)
        {
            switch (index)
            {
                case 0:
                    gbOptionsPercentage.Visible = true;
                    gbOptionsIncremental.Visible = false;
                    gbOptionsStandard.Visible = false;
                    break;
                case 1:
                    gbOptionsPercentage.Visible = false;
                    gbOptionsIncremental.Visible = true;
                    gbOptionsStandard.Visible = false;
                    break;
                case 2:
                    gbOptionsPercentage.Visible = false;
                    gbOptionsIncremental.Visible = false;
                    gbOptionsStandard.Visible = true;
                    break;
            }
        }


        private void btnDeleteRollback_Click(object sender, EventArgs e)
        {
            if (dgvRollbacks.SelectedRows.Count > 0)
            {
                IMapFeatureLayer[] mfl = mapGBD.GetFeatureLayers();
                DialogResult result = MessageBox.Show("Are you sure you wish to delete the selected scenario?", "", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    DataGridViewRow row = dgvRollbacks.SelectedRows[0];
                    string name = row.Cells["colName"].Value.ToString();
                    GBDRollbackItem item = rollbacks.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    foreach (IPolygonCategory ipc in item.IpcList)
                    {
                        mfl[0].Symbology.RemoveCategory(ipc);
                    }
                    item.IpcList.Clear();
                    mfl[0].ApplyScheme(mfl[0].Symbology);
                    //delete rollback
                    rollbacks.RemoveAll(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    //delete row
                    dgvRollbacks.Rows.Remove(row);
                    ToggleExecuteScenariosButton();
                }

            }
        }

        private void btnEditRollback_Click(object sender, EventArgs e)
        {
            if (dgvRollbacks.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvRollbacks.SelectedRows[0];
                string name = row.Cells["colName"].Value.ToString();
                //get rollback
                GBDRollbackItem item = rollbacks.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                ClearFields();
                LoadRollback(item);
                SetActivePanel(0);
            }
        }

        private void dgvRollbacks_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvRollbacks.EndEdit();
            if ((e.RowIndex != -1) && (e.ColumnIndex != -1))
            {
                string columnName = dgvRollbacks.Columns[e.ColumnIndex].Name;

                if ((columnName.Equals("colTotalCountries", StringComparison.OrdinalIgnoreCase)) ||
                    (columnName.Equals("colTotalPopulation", StringComparison.OrdinalIgnoreCase)))
                {
                    string name = dgvRollbacks.Rows[e.RowIndex].Cells["colName"].Value.ToString();
                    //get rollback
                    GBDRollbackItem item = rollbacks.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    GBDRollbackCountriesPopulations frm = new GBDRollbackCountriesPopulations();

                    //build selected list of countries, pops
                    string expression = "COUNTRYID in (" + String.Join(",", item.Countries.Select(x => "'" + x.Key + "'")) + ")";
                    DataRow[] rows = dtCountries.Select(expression);
                    System.Data.DataTable dt = rows.CopyToDataTable<DataRow>();
                    frm.CountryPop = dt.Copy();
                    frm.ShowDialog();
                }
            }

        }



        private void btnExecuteRollbacks_Click(object sender, EventArgs e)
        {

            try
            {

                Cursor.Current = Cursors.WaitCursor;

                //double beta = 0;
                //double se = 0;

                //for each checked rollback...
                List<DataGridViewRow> list = dgvRollbacks.Rows.Cast<DataGridViewRow>().Where(k => Convert.ToBoolean(k.Cells["colExecute"].Value) == true).ToList();
                foreach (DataGridViewRow row in list)
                {
                    string name = row.Cells["colName"].Value.ToString();
                    //get rollback
                    GBDRollbackItem item = rollbacks.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                    //get pollutant beta, se 
                    //YY: We will have different beta per rollback setup. Beta and se will be added to table queries for calculation.
                    //GBDRollbackDataSource.GetPollutantBeta(item.FunctionID, out beta, out se);
                    //int retCode = ExecuteRollback(item, beta, se);
                    int retCode = ExecuteRollback(item);
                    if (retCode != 0)
                    {
                        Cursor.Current = Cursors.Default;
                        return;
                    }
                }
                //       throw new Exception("debug Test");
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Execute Scenarios successful!");

            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.ToString());
                String user = Environment.GetEnvironmentVariable("username");
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\" + user + @"\My Documents\My BenMAP-CE Files\error.txt"))
                {
                    file.Write(ex.ToString());
                }
            }



        }

        private int ExecuteRollback(GBDRollbackItem rollback)//YY: removed beta and se here
        {
            dtConcEntireRollback = null;
            dtConcEntirePopWeighted = null;

            List<string> countriesWithoutData = new List<string>();
            rollbackResultsByCountry = new Dictionary<string, GBDRollbackResult>();

            // for each country in rollback...
            foreach (string countryid in rollback.Countries.Keys)
            {
                Debug.WriteLine("GBD ExecuteRollback(" + rollback.Name + ", " + countryid + ") Starting");

                //Get region, country name for this round.
                int regionid = 0;
                string regionName = "";
                string countryName = "";
                double countryVsl = 0;
                GBDRollbackDataSource.GetRegionCountryName(countryid, ref regionid, ref regionName, ref countryName);
                GBDRollbackDataSource.GetRegionCountryVsl(countryid, rollback.VSLID, ref countryVsl);

                //Pollutant data for this country at grid cell level
                //Fields in dtGBDConcDataByGridCell: REGIONID, COUNTRYID, COORDID, YEARNUM, POLLUTANTID, CONCENTRATION
                DataTable dtGBDConcDataByGridCell = GBDRollbackDataSource.GetGBDConcPerGridCell(countryid, POLLUTANT_ID, AQ_YEAR);
                
                Debug.WriteLine("GBD ExecuteRollback(" + rollback.Name + ", " + countryid + ") DB Rollback Source Query Complete");

                if ((dtGBDConcDataByGridCell != null) && (dtGBDConcDataByGridCell.Rows.Count > 0))
                {
                    // run rollback, NOTE: this will add rollback columns
                    // Fields in dtGBDConcDataByGridCell: REGIONID, COUNTRYID, COORDID, YEARNUM, POLLUTANTID, CONCENTRATION
                    //                                   CONCENTRATION_ADJ, CONCENTRATION_ADJ_BACK, CONCENTRATION_FINAL, CONCENTRATION_DELTA
                    DoRollback(rollback, dtGBDConcDataByGridCell);

                    Debug.WriteLine("GBD ExecuteRollback(" + rollback.Name + ", " + countryid + ") DoRollback Complete");

                    GBDRollbackResult resultPerCountry = new GBDRollbackResult(0,0,0,0,0,0);
                    //YY: Change this class name to GBDRollbackResult

                    //Data tables used in Linq query to join incidence data and calculate population weighted concentration delta.
                    DataTable dtCountryPop = GBDRollbackDataSource.GetCountryPopulation(countryid, POP_YEAR, rollback.FunctionID);
                    DataTable dtCountryIncidence = GBDRollbackDataSource.GetCountryIncidence(countryid, rollback.FunctionID);
                    DataTable dtAgeRangeTable = GBDRollbackDataSource.GetAgeTable();
                    DataTable dtGenderTable = GBDRollbackDataSource.GetGenderTable();
                    //YY: create datatable for life table and function table
                    DataTable dtLifeTable = GBDRollbackDataSource.GetLifeTable();
                    DataTable dtFunctionTable = GBDRollbackDataSource.GetFunctionTable(rollback.FunctionID);

                    // some countries don't have data associated -- make sure this one does 
                    // Check both population and incidence data. If population or incidence datatable returns null instead of a datatable with 0 rows, there must be an issue.
                    if (((dtCountryPop != null) && (dtCountryPop.Rows.Count > 0)) && ((dtCountryIncidence != null) && (dtCountryIncidence.Rows.Count > 0)))
                    {
                        //Append concentration to entire concentration data table.
                        //This step may be moved out of this If Clause if we want to report rollback results even when the country does not have associated data.
                        if (dtConcEntireRollback == null)
                        {
                            dtConcEntireRollback = dtGBDConcDataByGridCell.Clone();
                        }
                        dtConcEntireRollback.Merge(dtGBDConcDataByGridCell, true, MissingSchemaAction.Ignore);

                        //Prepare concentration, incidence and population data at contry-age-gender level for mortality calculation.
                        //YY: if krewski function, use concDelta, others use concQ1 and concQ0
                        double[] concDelta;
                        double[] population;
                        double[] incRate;
                        //YY: add beta and se and others for yll
                        double[] q0;
                        double[] q1;
                        double[] betaMean;
                        double[] betaSe;
                        double[] paraA;
                        double[] paraB;
                        double[] paraC;
                        double[] probDeath;
                        double[] lifeExpect;

                        if (countryid != "CHN" && countryid != "IND")
                        {
                            //Join concentration data with population data and then group by Country, Age and Gender
                            var tmpJoin = from a in dtGBDConcDataByGridCell.AsEnumerable()
                                          join b in dtCountryPop.AsEnumerable()
                                          on a.Field<int>("COORDID") equals b.Field<int>("COORDID")
                                          select new
                                          {
                                              countryId = a.Field<string>("countryid"),
                                              year = a.Field<short>("YEARNUM"),
                                              age = b.Field<short>("AGERANGEID"),
                                              gender = b.Field<short>("GENDERID"),
                                              concDelta = Convert.ToDouble(a.Field<decimal>("CONCENTRATION_DELTA")),
                                              concBaseline = Convert.ToDouble(a.Field<decimal>("CONCENTRATION")),
                                              concControl = Convert.ToDouble(a.Field<decimal>("CONCENTRATION_FINAL")),
                                              popEstimate = b.Field<double>("POPESTIMATE")
                                          };
                            var tmpGroup = from row in tmpJoin
                                           group row by new { row.countryId, row.year, row.age, row.gender } into g
                                           select new
                                           {
                                               g.Key.countryId,
                                               g.Key.year,
                                               g.Key.age,
                                               g.Key.gender,
                                               sumConcDelta = g.Sum(y => y.popEstimate * y.concDelta) / g.Sum(z => z.popEstimate),
                                               sumConcBaseline = g.Sum(y => y.popEstimate * y.concBaseline) / g.Sum(z => z.popEstimate),
                                               sumConcControl = g.Sum(y => y.popEstimate * y.concControl) / g.Sum(z => z.popEstimate),
                                               sumPopulation = g.Sum(x => x.popEstimate)
                                           };

                            //YY var tmpGroupFunction...

                            //Join incidence Data
                            //YY: Add a step here to join function and life table to get beta, se, A, B, C, proDeath, lifeExp. 
                            //YY: remember to do this for CHN and IND part as well. 
                            var queryGBDDataByGroup = from a in tmpGroup
                                                      join b in dtCountryIncidence.AsEnumerable()
                                            on new { age = a.age, gender = a.gender }
                                            equals new { age = Convert.ToInt16(b.Field<int>("AGERANGEID")), gender = Convert.ToInt16(b.Field<int>("GENDERID")) }
                                                      join c in dtAgeRangeTable.AsEnumerable() on new { age = a.age } equals new { age = Convert.ToInt16(c.Field<int>("AGERANGEID")) }
                                                      join d in dtGenderTable.AsEnumerable() on new { gender = a.gender } equals new { gender = Convert.ToInt16(d.Field<short>("GENDERID")) }
                                                      join lt in dtLifeTable.AsEnumerable() on new { age = a.age} equals new { age = Convert.ToInt16(lt.Field<int>("AGERANGEID")) }
                                                      join ft in dtFunctionTable.AsEnumerable() 
                                            on new { age = a.age, gender=a.gender, endpoint = Convert.ToInt16(b.Field<int>("ENDPOINTID"))} 
                                            equals new { age = Convert.ToInt16(ft.Field<short>("AGERANGEID")), gender = Convert.ToInt16(ft.Field<short>("GENDERID")), endpoint = Convert.ToInt16(ft.Field<short>("ENDPOINTID")) }
                                                      select new
                                                      {
                                                          year = a.year,
                                                          age = a.age,
                                                          ageName = c.Field<string>("AGERANGENAME"),
                                                          gender = a.gender,
                                                          genderName = d.Field<string>("GENDERNAME"),
                                                          sumconcBaseline = a.sumConcBaseline,
                                                          sumconcControl = a.sumConcControl,
                                                          sumConcDelta = a.sumConcDelta,
                                                          sumPopulation = a.sumPopulation,
                                                          endpointId = b.Field<int>("ENDPOINTID"), 
                                                          incidenceRate = Convert.ToDouble(b.Field<decimal>("INCIDENCERATE")),
                                                          //YY: new added fields
                                                          betamean = Convert.ToDouble(ft.Field<double>("BETAMEAN")),
                                                          betase = Convert.ToDouble(ft.Field<double>("BETASE")),
                                                          paraA = Convert.ToDouble(ft.Field<double>("A")),
                                                          paraB = Convert.ToDouble(ft.Field<double>("B")),
                                                          paraC = Convert.ToDouble(ft.Field<double>("C")),
                                                          probDeath = Convert.ToDouble(lt.Field<double>("PROBOFDEATH")),
                                                          lifeExp = Convert.ToDouble(lt.Field<double>("LIFEEXPECT"))
                                                      };

                            // get concentration delta, population, and incidence arrays
                            //double[] concDelta = Array.ConvertAll<DataRow, double>(dtGBDConcDataByGridCell.Select(),
                            //    delegate (DataRow row) { return Convert.ToDouble(row["sumConcDelta"]); });
                            //double[] population = Array.ConvertAll<DataRow, double>(dtGBDConcDataByGridCell.Select(),
                            //    delegate (DataRow row) { return Convert.ToDouble(row["sumPopulation"]); });
                            //double[] incRate = Array.ConvertAll<DataRow, double>(dtGBDConcDataByGridCell.Select(),
                            //    delegate (DataRow row) { return Convert.ToDouble(row["INCIDENCERATE"]); });

                            //Use IEnumerables instead of datatable to get concentration delta, population, and incidence arrays

                            concDelta = queryGBDDataByGroup.Select(x => x.sumConcDelta).ToArray();
                            population = queryGBDDataByGroup.Select(x => x.sumPopulation).ToArray();
                            incRate = queryGBDDataByGroup.Select(x => x.incidenceRate).ToArray();
                            //YY: add betamean, betase, a, b and c, etc... here
                            q0 = queryGBDDataByGroup.Select(x => x.sumconcControl).ToArray();
                            q1 = queryGBDDataByGroup.Select(x => x.sumconcBaseline).ToArray();
                            betaMean = queryGBDDataByGroup.Select(x => x.betamean).ToArray();
                            betaSe = queryGBDDataByGroup.Select(x => x.betase).ToArray();
                            paraA = queryGBDDataByGroup.Select(x => x.paraA).ToArray();
                            paraB = queryGBDDataByGroup.Select(x => x.paraB).ToArray();
                            paraC = queryGBDDataByGroup.Select(x => x.paraC).ToArray();
                            probDeath = queryGBDDataByGroup.Select(x => x.probDeath).ToArray();
                            lifeExpect = queryGBDDataByGroup.Select(x => x.lifeExp).ToArray();

                            //Append this country's pop weighted data to all country pop weighted datatable.
                            //YY: the query here should include mortality, eco benefit and YLL.
                            if (dtConcEntirePopWeighted == null)
                            {
                                dtConcEntirePopWeighted = new DataTable();

                                DataColumn column;
                                column = new DataColumn("REGIONID", typeof(System.Int32));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("REGIONNAME", typeof(System.String));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("COUNTRYID", typeof(System.String));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("COUNTRYNAME", typeof(System.String));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("YEARNUM", typeof(System.Int32));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("AGERANGEID", typeof(System.Int32));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("AGERANGENAME", typeof(System.String));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("GENDERID", typeof(System.Int32));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("GENDERNAME", typeof(System.String));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("AIR_QUALITY_DELTA", typeof(System.Double));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                //column = new DataColumn("CONCENTRATION", typeof(System.Double));
                                //dtConcEntirePopWeighted.Columns.Add(column);
                                //column = new DataColumn("CONCENTRATION_FINAL", typeof(System.Double));
                                //dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("BASELINE_MORTALITY", typeof(System.Double));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("POPESTIMATE", typeof(System.Double));
                                dtConcEntirePopWeighted.Columns.Add(column);
                            }
                            foreach (var item in queryGBDDataByGroup)
                            {
                                //YY: non affected pop exluded when join function table
                                //if (!((item.age < 7) && (rollback.FunctionID == 1))) // AgeRangeId =7 --> 30 TO 34
                                //{
                                var row = dtConcEntirePopWeighted.NewRow();
                                row["REGIONID"] = regionid;
                                row["REGIONNAME"] = regionName;
                                row["COUNTRYID"] = countryid;
                                row["COUNTRYNAME"] = countryName;
                                row["YEARNUM"] = item.year;
                                row["AGERANGEID"] = item.age;
                                row["AGERANGENAME"] = item.ageName;
                                row["GENDERID"] = item.gender;
                                row["GENDERNAME"] = item.genderName;
                                row["AIR_QUALITY_DELTA"] = item.sumConcDelta; //Pop weighted AQ delta
                                                                              //row["CONCENTRATION"] = item.sumconcBaseline;
                                                                              //row["CONCENTRATION_FINAL"] = item.sumconcControl;
                                row["BASELINE_MORTALITY"] = item.incidenceRate * item.sumPopulation;
                                row["POPESTIMATE"] = item.sumPopulation;

                                dtConcEntirePopWeighted.Rows.Add(row);
                                //}
                            }
                        }
                        else // CHN or IND still calculate mortality at country-grid-gender-age level.
                        {
                            //Join dtGBDConcDataByGridCell with dtCountryPopulation
                            var tmpJoin = from a in dtGBDConcDataByGridCell.AsEnumerable()
                                          join b in dtCountryPop.AsEnumerable()
                                          on a.Field<int>("COORDID") equals b.Field<int>("COORDID")
                                          select new
                                          {
                                              countryId = a.Field<string>("countryid"),
                                              year = a.Field<short>("YEARNUM"),
                                              age = b.Field<short>("AGERANGEID"),
                                              gender = b.Field<short>("GENDERID"),
                                              concDelta = Convert.ToDouble(a.Field<decimal>("CONCENTRATION_DELTA")),
                                              concBaseline = Convert.ToDouble(a.Field<decimal>("CONCENTRATION")),
                                              concControl = Convert.ToDouble(a.Field<decimal>("CONCENTRATION_FINAL")),
                                              popEstimate = b.Field<double>("POPESTIMATE")
                                          };

                            //YY: Add a step here to join function beta, se, A, B, C. 
                            //YY: remember to do this for CHN and IND part as well. 
                            //YY var tmpGroupFunction...


                            //Join Incidence Data. This is not a group query. Word "group" is here to be consistent with other conditions.
                            var queryGBDDataByGroup = from a in tmpJoin
                                                      join b in dtCountryIncidence.AsEnumerable()
                                            on new { age = a.age, gender = a.gender }
                                            equals new { age = Convert.ToInt16(b.Field<int>("AGERANGEID")), gender = Convert.ToInt16(b.Field<int>("GENDERID")) }
                                                      join c in dtAgeRangeTable.AsEnumerable() on new { age = a.age } equals new { age = Convert.ToInt16(c.Field<int>("AGERANGEID")) }
                                                      join d in dtGenderTable.AsEnumerable() on new { gender = a.gender } equals new { gender = Convert.ToInt16(d.Field<short>("GENDERID")) }
                                                      join lt in dtLifeTable.AsEnumerable() on new { age = a.age } equals new { age = Convert.ToInt16(lt.Field<int>("AGERANGEID")) }
                                                      join ft in dtFunctionTable.AsEnumerable()
                                            on new { age = a.age, gender = a.gender, endpoint = Convert.ToInt16(b.Field<int>("ENDPOINTID")) }
                                            equals new { age = Convert.ToInt16(ft.Field<short>("AGERANGEID")), gender = Convert.ToInt16(ft.Field<short>("GENDERID")), endpoint = Convert.ToInt16(ft.Field<short>("ENDPOINTID")) }
                                                      select new
                                                      {
                                                          year = a.year,
                                                          age = a.age,
                                                          ageName = c.Field<string>("AGERANGENAME"),
                                                          gender = a.gender,
                                                          genderName = d.Field<string>("GENDERNAME"),
                                                          sumconcBaseline = a.concBaseline,
                                                          sumconcControl = a.concControl,
                                                          sumConcDelta = a.concDelta,
                                                          sumPopulation = a.popEstimate,
                                                          endpointId = b.Field<int>("ENDPOINTID"),
                                                          incidenceRate = Convert.ToDouble(b.Field<decimal>("INCIDENCERATE")),
                                                          //YY: new added fields
                                                          betamean = Convert.ToDouble(ft.Field<double>("BETAMEAN")),
                                                          betase = Convert.ToDouble(ft.Field<double>("BETASE")),
                                                          paraA = Convert.ToDouble(ft.Field<double>("A")),
                                                          paraB = Convert.ToDouble(ft.Field<double>("B")),
                                                          paraC = Convert.ToDouble(ft.Field<double>("C")),
                                                          probDeath = Convert.ToDouble(lt.Field<double>("PROBOFDEATH")),
                                                          lifeExp = Convert.ToDouble(lt.Field<double>("LIFEEXPECT"))
                                                      };

                            //Use IEnumerable instead of datatable to feed array
                            //YY: no need to use this step as pop<30 are excluded in first step.
                            concDelta = queryGBDDataByGroup.Select(x => x.sumConcDelta).ToArray();  
                                population = queryGBDDataByGroup.Select(x => x.sumPopulation).ToArray();
                                incRate = queryGBDDataByGroup.Select(x => x.incidenceRate).ToArray();
                                //YY; add beta se and ABC
                                //YY: add betamean, betase, a, b and c, etc... here
                                q0 = queryGBDDataByGroup.Select(x => x.sumconcControl).ToArray();
                                q1 = queryGBDDataByGroup.Select(x => x.sumconcBaseline).ToArray();
                                betaMean = queryGBDDataByGroup.Select(x => x.betamean).ToArray();
                                betaSe = queryGBDDataByGroup.Select(x => x.betase).ToArray();
                                paraA = queryGBDDataByGroup.Select(x => x.paraA).ToArray();
                                paraB = queryGBDDataByGroup.Select(x => x.paraB).ToArray();
                                paraC = queryGBDDataByGroup.Select(x => x.paraC).ToArray();
                                probDeath = queryGBDDataByGroup.Select(x => x.probDeath).ToArray();
                                lifeExpect = queryGBDDataByGroup.Select(x => x.lifeExp).ToArray();

                            // Group by country-age-gender and calculate pop weighted concentration delta. 
                            //This is for result output not for calculating mortality
                            var queryGBDDataByGroupFinal = from row in queryGBDDataByGroup
                                                           group row by new { row.year, row.age, row.ageName, row.gender, row.genderName } into g
                                                           select new
                                                           {
                                                               g.Key.year,
                                                               g.Key.age,
                                                               g.Key.ageName,
                                                               g.Key.gender,
                                                               g.Key.genderName,
                                                               sumConcDelta = g.Sum(y => y.sumPopulation * y.sumConcDelta) / g.Sum(z => z.sumPopulation),
                                                               //sumConcBaseline = g.Sum(y => y.sumPopulation * y.sumconcBaseline) / g.Sum(z => z.sumPopulation),
                                                               //sumConcControl = g.Sum(y => y.sumPopulation * y.sumconcControl) / g.Sum(z => z.sumPopulation),
                                                               sumPopulation = g.Sum(x => x.sumPopulation),
                                                               incidenceRate = g.Average(x => x.incidenceRate)
                                                           };

                            //Append this country's pop weighted data to all country pop weighted datatable.
                            //Note that the Linq query used here is queryGBDDataByGroupFinal but not queryGBDDataByGroup
                            if (dtConcEntirePopWeighted == null)
                            {
                                dtConcEntirePopWeighted = new DataTable();

                                DataColumn column;
                                column = new DataColumn("REGIONID", typeof(System.Int32));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("REGIONNAME", typeof(System.String));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("COUNTRYID", typeof(System.String));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("COUNTRYNAME", typeof(System.String));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("YEARNUM", typeof(System.Int32));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("AGERANGEID", typeof(System.Int32));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("AGERANGENAME", typeof(System.String));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("GENDERID", typeof(System.Int32));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("GENDERNAME", typeof(System.String));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("AIR_QUALITY_DELTA", typeof(System.Double));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                //column = new DataColumn("CONCENTRATION", typeof(System.Double));
                                //dtConcEntirePopWeighted.Columns.Add(column);
                                //column = new DataColumn("CONCENTRATION_FINAL", typeof(System.Double));
                                //dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("BASELINE_MORTALITY", typeof(System.Double));
                                dtConcEntirePopWeighted.Columns.Add(column);
                                column = new DataColumn("POPESTIMATE", typeof(System.Double));
                                dtConcEntirePopWeighted.Columns.Add(column);
                            }
                            foreach (var item in queryGBDDataByGroupFinal)
                            {
                                //YY: non affected pop exluded when join function table
                                //if (!((item.age < 7) && (rollback.FunctionID == 1))) // AgeRangeId =7 --> 30 TO 34
                                //{
                                var row = dtConcEntirePopWeighted.NewRow();
                                row["REGIONID"] = regionid;
                                row["REGIONNAME"] = regionName;
                                row["COUNTRYID"] = countryid;
                                row["COUNTRYNAME"] = countryName;
                                row["YEARNUM"] = item.year;
                                row["AGERANGEID"] = item.age;
                                row["AGERANGENAME"] = item.ageName;
                                row["GENDERID"] = item.gender;
                                row["GENDERNAME"] = item.genderName;
                                row["AIR_QUALITY_DELTA"] = item.sumConcDelta; //Pop weighted AQ delta;
                                                                              //row["CONCENTRATION"] = item.sumconcBaseline;
                                                                              //row["CONCENTRATION_FINAL"] = item.sumconcControl;
                                row["BASELINE_MORTALITY"] = item.incidenceRate * item.sumPopulation;
                                row["POPESTIMATE"] = item.sumPopulation;
                                dtConcEntirePopWeighted.Rows.Add(row);
                                //} 

                            }
                        }

                        // get results for country
                        //YY: allf unctions are now using the same KrewskiFunction                
                        GBDRollbackFunction func = new GBDRollbackFunction();
                        resultPerCountry = func.GBD_math(rollback.FunctionID, concDelta, population, incRate, betaMean, betaSe, q0, q1, paraA, paraB, paraC, probDeath, lifeExpect);
                        resultPerCountry.EcoBenefit = resultPerCountry.Result * countryVsl;
                        resultPerCountry.Population = Convert.ToDouble(dtCountryPop.Compute("SUM(POPESTIMATE)", ""));

                        Debug.WriteLine("GBD ExecuteRollback(" + rollback.Name + ", " + countryid + ") GBD_math Complete");

                        //ensure we have data for the country
                        //if ((dtGBDConcDataByGridCell != null) && (dtGBDConcDataByGridCell.Rows.Count > 0))
                        //{

                        //Add this coutnry's results to rollbackResultsByCountry and pop dtConcEntireRollback
                        // Capture totals for this country
                        rollbackResultsByCountry.Add("COUNTRYID = '" + countryid + "'", resultPerCountry);

                        // Capture totals for this region
                        String regionKey = "REGIONID = " + regionid;
                        if (rollbackResultsByCountry.ContainsKey(regionKey))
                        {
                            rollbackResultsByCountry[regionKey].Result += resultPerCountry.Result;
                            rollbackResultsByCountry[regionKey].Result2_5 += resultPerCountry.Result2_5;
                            rollbackResultsByCountry[regionKey].Result97_5 += resultPerCountry.Result97_5;
                            rollbackResultsByCountry[regionKey].Yll += resultPerCountry.Yll; //YY: new added YLL
                            rollbackResultsByCountry[regionKey].EcoBenefit += resultPerCountry.EcoBenefit; //YY: new added EcoBenefit
                            rollbackResultsByCountry[regionKey].Population += resultPerCountry.Population; //YY: new added Population

                        }
                        else
                        {
                            rollbackResultsByCountry.Add(regionKey, new GBDRollbackResult(resultPerCountry.Result, resultPerCountry.Result2_5, resultPerCountry.Result97_5, resultPerCountry.Yll, resultPerCountry.EcoBenefit, resultPerCountry.Population));
                        }

                        // Capture grand totals
                        String grandTotalKey = "1=1";
                        if (rollbackResultsByCountry.ContainsKey(grandTotalKey))
                        {
                            rollbackResultsByCountry[grandTotalKey].Result += resultPerCountry.Result;
                            rollbackResultsByCountry[grandTotalKey].Result2_5 += resultPerCountry.Result2_5;
                            rollbackResultsByCountry[grandTotalKey].Result97_5 += resultPerCountry.Result97_5;
                            rollbackResultsByCountry[grandTotalKey].Yll += resultPerCountry.Yll;//YY: new added YLL
                            rollbackResultsByCountry[grandTotalKey].EcoBenefit += resultPerCountry.EcoBenefit;//YY: new added EcoBenefit
                            rollbackResultsByCountry[grandTotalKey].Population += resultPerCountry.Population;//YY: new added Population
                        }
                        else
                        {
                            rollbackResultsByCountry.Add(grandTotalKey, new GBDRollbackResult(resultPerCountry.Result, resultPerCountry.Result2_5, resultPerCountry.Result97_5, resultPerCountry.Yll, resultPerCountry.EcoBenefit, resultPerCountry.Population));
                        }

                    }
                    else //add to list of countries with insufficient data
                    {
                        countriesWithoutData.Add(countryName);
                    }
                }
                else //add to list of countries with insufficient data
                {
                    countriesWithoutData.Add(countryName);
                }

            }
            //if we do not have result data for any of selected countries
            //inform user and abort
            if ((dtConcEntirePopWeighted == null) || (dtConcEntirePopWeighted.Rows.Count == 0))
            {
                MessageBox.Show("Scenario Name: " + rollback.Name + Environment.NewLine + Environment.NewLine + "Rollback failed to execute. Lack of sufficient data.");
                return 0; //rollback successfully processed (could not run but did not produce an error)
            }
            //show user countries that could not be run
            if (countriesWithoutData.Count > 0)
            {
                countriesWithoutData.Sort();
                string names = String.Join(Environment.NewLine, countriesWithoutData);
                MessageBox.Show("Scenario Name: " + rollback.Name + Environment.NewLine + Environment.NewLine + "The following countries lack sufficient data to run a rollback: " + Environment.NewLine + Environment.NewLine + names);
            }

            // save results as XLSX or CSV?
            Debug.WriteLine("GBD ExecuteRollback(" + rollback.Name + ") Starting Output");
            if (cboExportFormat.SelectedIndex == 0)
            {
                try
                {
                    SaveRollbackReport(rollback);
                }
                catch (Exception ex)
                {
                    string stacktrace = ex.StackTrace;
                    MessageBox.Show("An error occurred while exporting to XLSX format.  Please use CSV format to export GBD results.");
                    return 1;
                }
            }
            else
            {
                SaveRollbackReportCSV(rollback);
            }
            Debug.WriteLine("GBD ExecuteRollback(" + rollback.Name + ") Ending");

            return 0;
        }

        private void DoRollback(GBDRollbackItem rollback, DataTable dtGBDDataByGridCell)
        {
            switch (rollback.Type)
            {
                case GBDRollbackItem.RollbackType.Percentage:
                    DoPercentageRollback(rollback.Percentage, rollback.Background, dtGBDDataByGridCell);
                    break;
                case GBDRollbackItem.RollbackType.Incremental:
                    DoIncrementalRollback(rollback.Increment, rollback.Background, dtGBDDataByGridCell);
                    break;
                case GBDRollbackItem.RollbackType.Standard:
                    DoRollbackToStandard(rollback.Standard, rollback.IsNegativeRollbackToStandard, dtGBDDataByGridCell);
                    break;
            }

        }

        private void DoPercentageRollback(double percentage, double background, DataTable dtGBDDataByGridCell)
        {
            //rollback
            dtGBDDataByGridCell.Columns.Add("CONCENTRATION_ADJ", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "CONCENTRATION - (CONCENTRATION * " + (percentage / 100).ToString() + ")");

            //check against background
            dtGBDDataByGridCell.Columns.Add("CONCENTRATION_ADJ_BACK", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "IIF(CONCENTRATION_ADJ < " + background + ", " + background + ", CONCENTRATION_ADJ)");

            //get final, keep original values if <= background.
            dtGBDDataByGridCell.Columns.Add("CONCENTRATION_FINAL", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "IIF(CONCENTRATION <= " + background + ", CONCENTRATION, CONCENTRATION_ADJ_BACK)");

            //get delta (orig. conc - rolled back conc. (corrected for background)
            dtGBDDataByGridCell.Columns.Add("CONCENTRATION_DELTA", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "CONCENTRATION - CONCENTRATION_FINAL");

            //get air quality delta (conc delta * population)
            //moved AQ delta to the next step so that we don't need to involve population here
            //dtGBDDataByGridCell.Columns.Add("AIR_QUALITY_DELTA", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "CONCENTRATION_DELTA * POPESTIMATE");

        }

        private void DoIncrementalRollback(double increment, double background, DataTable dtGBDDataByGridCell)
        {
            //rollback
            dtGBDDataByGridCell.Columns.Add("CONCENTRATION_ADJ", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "CONCENTRATION - " + increment);

            //check against background
            dtGBDDataByGridCell.Columns.Add("CONCENTRATION_ADJ_BACK", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "IIF(CONCENTRATION_ADJ < " + background + ", " + background + ", CONCENTRATION_ADJ)");

            //get final, keep original values if <= background.
            dtGBDDataByGridCell.Columns.Add("CONCENTRATION_FINAL", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "IIF(CONCENTRATION <= " + background + ", CONCENTRATION, CONCENTRATION_ADJ_BACK)");

            //get delta (orig. conc - rolled back conc. (corrected for background)
            dtGBDDataByGridCell.Columns.Add("CONCENTRATION_DELTA", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "CONCENTRATION - CONCENTRATION_FINAL");

            //get air quality delta (conc delta * population)
            //moved AQ delta to the next step so that we don't need to involve population here
            //dtGBDDataByGridCell.Columns.Add("AIR_QUALITY_DELTA", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "CONCENTRATION_DELTA * POPESTIMATE");

        }


        private void DoRollbackToStandard(double standard, bool isNegativeRollback, DataTable dtGBDDataByGridCell)
        {
            //rollback to standard
            dtGBDDataByGridCell.Columns.Add("CONCENTRATION_ADJ", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, standard.ToString());


            if (isNegativeRollback)
            {
                //get final, keep original values if >= standard.
                dtGBDDataByGridCell.Columns.Add("CONCENTRATION_FINAL", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "IIF(CONCENTRATION >= " + standard + ", CONCENTRATION, CONCENTRATION_ADJ)");

            }
            else
            {
                //get final, keep original values if <= standard.
                dtGBDDataByGridCell.Columns.Add("CONCENTRATION_FINAL", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "IIF(CONCENTRATION <= " + standard + ", CONCENTRATION, CONCENTRATION_ADJ)");
            }

            //get delta (orig. conc - rolled back conc.)
            dtGBDDataByGridCell.Columns.Add("CONCENTRATION_DELTA", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "CONCENTRATION - CONCENTRATION_FINAL");

            //get air quality delta (conc delta * population)
            //moved AQ delta to the next step so that we don't need to involve population here
            //dtGBDDataByGridCell.Columns.Add("AIR_QUALITY_DELTA", dtGBDDataByGridCell.Columns["CONCENTRATION"].DataType, "CONCENTRATION_DELTA * POPESTIMATE");
        }

        private System.Data.DataTable GetRegionsCountriesTable()
        {

            System.Data.DataTable dtTemp = dtConcEntirePopWeighted.DefaultView.ToTable(true, "REGIONID", "REGIONNAME", "COUNTRYID", "COUNTRYNAME");
            DataView dv = new DataView(dtTemp);
            dv.Sort = "REGIONNAME ASC, COUNTRYNAME ASC";
            System.Data.DataTable dtRegionsCountries = dv.ToTable();

            return dtRegionsCountries;


        }


        public void CreateSpreadsheetWorkbook(string filepath)
        {
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());
            DateTime dtNow = DateTime.Now;

            // Add Sheets to the Workbook.
            DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>(new DocumentFormat.OpenXml.Spreadsheet.Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "mySheet"
            };
            sheets.Append(sheet);

            //write to sheet
            worksheetPart = GetWorksheetPartByName(spreadsheetDocument, "mySheet");
            UpdateCellSharedString(worksheetPart.Worksheet, "hello world", "A", 1);

            worksheetPart.Worksheet.Save();
            workbookPart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();
        }


        private WorksheetPart GetWorksheetPartByName(SpreadsheetDocument document, string sheetName)
        {
            IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>().Elements<Sheet>().Where(s => s.Name == sheetName);
            if (sheets.Count() == 0)
            {
                return null;
            }
            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;
        }

        private ChartsheetPart GetChartsheetPartByName(SpreadsheetDocument document, string sheetName)
        {
            IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>().Elements<Sheet>().Where(s => s.Name == sheetName);
            if (sheets.Count() == 0)
            {
                return null;
            }
            string relationshipId = sheets.First().Id.Value;
            ChartsheetPart chartsheetPart = (ChartsheetPart)document.WorkbookPart.GetPartById(relationshipId);
            return chartsheetPart;
        }

        public void UpdateCellSharedString(Worksheet worksheet, string text, string columnName, uint rowIndex)
        {

            // Get the SharedStringTablePart. If it does not exist, create a new one.
            SharedStringTablePart sharedStringPart;
            if (spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
            {
                sharedStringPart = spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
            }
            else
            {
                sharedStringPart = spreadsheetDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();
            }

            // Insert the text into the SharedStringTablePart.
            int index = InsertSharedStringItem(text, sharedStringPart);

            Cell cell = GetCell(worksheet, columnName, rowIndex);
            cell.CellValue = new CellValue(index.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        }

        public void UpdateCellNumber(Worksheet worksheet, string text, string columnName, uint rowIndex)
        {
            Cell cell = GetCell(worksheet, columnName, rowIndex);
            cell.CellValue = new CellValue(text.Replace(",", "")); //remove any commas from number string, number cell will not be formatted here           
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        public void UpdateCellDate(Worksheet worksheet, DateTime date, string columnName, uint rowIndex)
        {
            double doubleDate = date.ToOADate();

            Cell cell = GetCell(worksheet, columnName, rowIndex);
            cell.CellValue = new CellValue(doubleDate.ToString());
            cell.StyleIndex = (UInt32Value)2U;
            //cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        private Cell GetCell(Worksheet worksheet, string columnName, uint rowIndex)
        {
            Row row;
            string cellReference = columnName + rowIndex;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).FirstOrDefault();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            if (row == null)
            {
                return null;
            }

            if (row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (String.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell()
                {
                    CellReference = cellReference
                };
                row.InsertBefore(newCell, refCell);

                return newCell;
            }


        }


        private string GetCellValue(Worksheet worksheet, string columnName, uint rowIndex)
        {

            string value = String.Empty;
            Cell cell = GetCell(worksheet, columnName, rowIndex);

            value = cell.InnerText;

            if (cell.DataType != null)
            {
                if (cell.DataType.Value == CellValues.SharedString)
                {
                    // For shared strings, look up the value in the
                    // shared strings table.
                    SharedStringTablePart sharedStringPart = spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                    value = sharedStringPart.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                }
            }

            return value;
        }

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(string text, SharedStringTablePart sharedStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (sharedStringPart.SharedStringTable == null)
            {
                sharedStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in sharedStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            sharedStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            sharedStringPart.SharedStringTable.Save();

            return i;
        }

        private void ConfigureCellFormats()
        {

            WorkbookStylesPart stylesPart = spreadsheetDocument.WorkbookPart.GetPartsOfType<WorkbookStylesPart>().First();
            Stylesheet styleSheet = stylesPart.Stylesheet;

            //styleSheet.NumberingFormats = new NumberingFormats();
            //NumberingFormat nf2decimal = new NumberingFormat();
            //nf2decimal.NumberFormatId = UInt32Value.FromUInt32(3453);
            //nf2decimal.FormatCode = StringValue.FromString("0.0%");
            //styleSheet.NumberingFormat.Append(nf2decimal);            

            //italics
            DocumentFormat.OpenXml.Spreadsheet.Font font1 = new DocumentFormat.OpenXml.Spreadsheet.Font();
            Italic italic1 = new Italic();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            DocumentFormat.OpenXml.Spreadsheet.Color color1 = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = "00000000" };
            FontName fontName1 = new FontName() { Val = "Gill Sans MT" };

            font1.Append(italic1);
            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            styleSheet.Fonts.Append(font1);
            styleSheet.Fonts.Count++;

            CellFormat cellFormat1 = new CellFormat()
            {
                NumberFormatId = (UInt32Value)0U,
                FontId = (UInt32Value)styleSheet.Fonts.Count - 1,
                FillId = (UInt32Value)0U,
                BorderId = (UInt32Value)1U,
                FormatId = (UInt32Value)0U,
                ApplyNumberFormat = true,
                ApplyFont = true,
                ApplyFill = true,
                ApplyBorder = true,
                ApplyAlignment = true
            };
            DocumentFormat.OpenXml.Spreadsheet.Alignment alignment1 = new DocumentFormat.OpenXml.Spreadsheet.Alignment() { Horizontal = HorizontalAlignmentValues.Left };

            cellFormat1.Append(alignment1);
            styleSheet.CellFormats.Append(cellFormat1);
            styleSheet.CellFormats.Count++;
            styleIndexItalicsWithBorders = styleSheet.CellFormats.Count - 1;

            styleIndexGrayFillWithBorders = 1U;
            styleIndexNoFillWithBorders = 5U;

            //indent
            CellFormat cellFormat2 = new CellFormat()
            {
                NumberFormatId = (UInt32Value)0U,
                FontId = (UInt32Value)2U,
                FillId = (UInt32Value)0U,
                BorderId = (UInt32Value)1U,
                FormatId = (UInt32Value)0U,
                ApplyNumberFormat = true,
                ApplyFont = true,
                ApplyFill = true,
                ApplyBorder = true,
                ApplyAlignment = true
            };
            DocumentFormat.OpenXml.Spreadsheet.Alignment alignment2 = new DocumentFormat.OpenXml.Spreadsheet.Alignment() { Horizontal = HorizontalAlignmentValues.Left, Indent = 2 };
            cellFormat2.Append(alignment2);
            styleSheet.CellFormats.Append(cellFormat2);
            styleSheet.CellFormats.Count++;
            styleIndexNoFillIndentWithBorders = styleSheet.CellFormats.Count - 1;

            //center
            CellFormat cellFormat3 = new CellFormat()
            {
                NumberFormatId = (UInt32Value)0U,
                FontId = (UInt32Value)2U,
                FillId = (UInt32Value)0U,
                BorderId = (UInt32Value)1U,
                FormatId = (UInt32Value)0U,
                ApplyNumberFormat = true,
                ApplyFont = true,
                ApplyFill = true,
                ApplyBorder = true,
                ApplyAlignment = true
            };
            DocumentFormat.OpenXml.Spreadsheet.Alignment alignment3 = new DocumentFormat.OpenXml.Spreadsheet.Alignment() { Horizontal = HorizontalAlignmentValues.Center };
            cellFormat3.Append(alignment3);
            styleSheet.CellFormats.Append(cellFormat3);
            styleSheet.CellFormats.Count++;
            styleIndexNoFillCenterWithBorders = styleSheet.CellFormats.Count - 1;


            //number format 2 decimal places format 4U = #,##0.00
            CellFormat cellFormat4 = new CellFormat()
            {
                NumberFormatId = (UInt32Value)4U,
                FontId = (UInt32Value)2U,
                FillId = (UInt32Value)0U,
                BorderId = (UInt32Value)1U,
                FormatId = (UInt32Value)0U,
                ApplyNumberFormat = true,
                ApplyFont = true,
                ApplyFill = true,
                ApplyBorder = true,
                ApplyAlignment = true
            };
            //DocumentFormat.OpenXml.Spreadsheet.Alignment alignment4 = new DocumentFormat.OpenXml.Spreadsheet.Alignment() { Horizontal = HorizontalAlignmentValues.Right };
            //cellFormat4.Append(alignment4);
            styleSheet.CellFormats.Append(cellFormat4);
            styleSheet.CellFormats.Count++;
            styleIndexNoFillNumber2DecimalPlacesWithBorders = styleSheet.CellFormats.Count - 1;

            //number format 0 decimal places format 3U = #,##0
            CellFormat cellFormat5 = new CellFormat()
            {
                NumberFormatId = (UInt32Value)3U,
                FontId = (UInt32Value)2U,
                FillId = (UInt32Value)0U,
                BorderId = (UInt32Value)1U,
                FormatId = (UInt32Value)0U,
                ApplyNumberFormat = true,
                ApplyFont = true,
                ApplyFill = true,
                ApplyBorder = true,
                ApplyAlignment = true
            };
            //DocumentFormat.OpenXml.Spreadsheet.Alignment alignment5 = new DocumentFormat.OpenXml.Spreadsheet.Alignment() { Horizontal = HorizontalAlignmentValues.Right };
            //cellFormat5.Append(alignment5);
            styleSheet.CellFormats.Append(cellFormat5);
            styleSheet.CellFormats.Count++;
            styleIndexNoFillNumber0DecimalPlacesWithBorders = styleSheet.CellFormats.Count - 1;

        }




        private System.Data.DataTable GetDetailedResultsTable(System.Data.DataTable dtRegionsCountries, string format)
        {

            //build output table
            System.Data.DataTable dtDetailedResults = new System.Data.DataTable();
            dtDetailedResults.Columns.Add("NAME", Type.GetType("System.String"));
            dtDetailedResults.Columns.Add("IS_REGION", Type.GetType("System.Boolean"));
            dtDetailedResults.Columns.Add("POP_AFFECTED", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("AVOIDED_DEATHS", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("CONFIDENCE_INTERVAL", Type.GetType("System.String"));
            dtDetailedResults.Columns.Add("PERCENT_BASELINE_MORTALITY", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("DEATHS_PER_100_THOUSAND", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("AVOIDED_DEATHS_PERCENT_POP", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("BASELINE_MIN", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("BASELINE_MEDIAN", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("BASELINE_MAX", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("CONTROL_MIN", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("CONTROL_MEDIAN", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("CONTROL_MAX", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("AIR_QUALITY_CHANGE", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("ECO_BENEFIT", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("YLL", Type.GetType("System.Double"));

            string regionid = String.Empty;
            string countryid = String.Empty;
            foreach (DataRow dr in dtRegionsCountries.Rows)
            {
                //new region? get region data
                if (!regionid.Equals(dr["REGIONID"].ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    regionid = dr["REGIONID"].ToString();
                    GetResults(regionid, dr["REGIONNAME"].ToString(), true, dtDetailedResults, format);
                }

                //get country data
                countryid = dr["COUNTRYID"].ToString();
                GetResults(countryid, dr["COUNTRYNAME"].ToString(), false, dtDetailedResults, format);
            }

            return dtDetailedResults;


        }


        private System.Data.DataTable GetSummaryResultsTable(System.Data.DataTable dtDetailedResults, string format)
        {
            //get results for summary table
            System.Data.DataTable dtSummaryResults = dtDetailedResults.Clone();
            GetResults(null, "SUMMARY", false, dtSummaryResults, format);
            return dtSummaryResults;

        }

        private string GetBackgroundConcentrationText(GBDRollbackItem rollback)
        {

            return rollback.Background.ToString() + " " + MICROGRAMS.ToString() + "g/m" + SUPER_3.ToString();
        }


        private string GetRollbackTypeSummary(GBDRollbackItem rollback)
        {

            string summary = String.Empty;
            switch (rollback.Type)
            {
                case GBDRollbackItem.RollbackType.Percentage: //percentage
                    summary = rollback.Percentage.ToString() + "% Rollback";
                    break;
                case GBDRollbackItem.RollbackType.Incremental: //incremental

                    summary = rollback.Increment.ToString() + MICROGRAMS.ToString() + "g/m" + SUPER_3.ToString() + " Rollback";
                    break;
                case GBDRollbackItem.RollbackType.Standard:

                    if (rollback.IsNegativeRollbackToStandard)
                    {
                        summary = "Negative Rollback to " + rollback.StandardName + " Standard";
                    }
                    else
                    {
                        summary = "Rollback to " + rollback.StandardName + " Standard";
                    }
                    break;
            }

            return summary;
        }

        private void SaveRollbackReport(GBDRollbackItem rollback)
        {

            //get application path
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = appPath + @"Tools\GBDRollbackOutputTemplate.xlsx";

            //check save dir 
            string resultsDir = txtFilePath.Text.Trim();
            if (!Directory.Exists(resultsDir))
            {
                Directory.CreateDirectory(resultsDir);
            }

            //get timestamp
            DateTime dtNow = DateTime.Now;
            string timeStamp = dtNow.ToString("yyyyMMddHHmm");
            //get application path
            string filePathCopy = resultsDir + @"\GBDRollback_" + rollback.Name + "_" + timeStamp + ".xlsx";

            //copy template
            File.Copy(filePath, filePathCopy, true);

            //open copied report template            
            spreadsheetDocument = SpreadsheetDocument.Open(filePathCopy, true);

            ConfigureCellFormats();


            #region summary sheet
            //summary sheet          

            WorksheetPart worksheetPart = GetWorksheetPartByName(spreadsheetDocument, "Summary");

            //xlSheet.Name = "Summary";
            //xlSheet.Range["A2"].Value = "Date";
            UpdateCellDate(worksheetPart.Worksheet, dtNow, "B", 2);


            ////xlSheet.Range["A3"].Value = "Scenario Name";
            //xlSheet.Range["B3"].Value = rollback.Name;
            UpdateCellSharedString(worksheetPart.Worksheet, rollback.Name, "B", 3);
            ////xlSheet.Range["A4"].Value = "Scenario Description";
            //xlSheet.Range["B4"].Value = rollback.Description;
            UpdateCellSharedString(worksheetPart.Worksheet, rollback.Description, "B", 4);
            ////xlSheet.Range["A5"].Value = "GBD Year";
            //xlSheet.Range["B5"].Value = rollback.Year.ToString();
            UpdateCellNumber(worksheetPart.Worksheet, rollback.Year.ToString(), "B", 5);
            ////xlSheet.Range["A6"].Value = "Pollutant";
            //xlSheet.Range["B6"].Value = "PM 2.5";
            UpdateCellSharedString(worksheetPart.Worksheet, "PM 2.5", "B", 6);

            ////xlSheet.Range["A7"].Value = "Background Concentration";

            //xlSheet.Range["B7"].Value = rollback.Background.ToString() + " " + MICROGRAMS.ToString() + "g/m" + SUPER_3.ToString();
            UpdateCellSharedString(worksheetPart.Worksheet, GetBackgroundConcentrationText(rollback), "B", 7);


            ////xlSheet.Range["A8"].Value = "Rollback Type";       
            //xlSheet.Range["B8"].Value = summary;

            UpdateCellSharedString(worksheetPart.Worksheet, GetRollbackTypeSummary(rollback), "B", 8);

            //add function cells
            //UpdateCellSharedString(worksheetPart.Worksheet, "Function", "A", 9);
            string function = rollback.Function.ToString();
            UpdateCellSharedString(worksheetPart.Worksheet, function, "B", 9);

            //YY: add selected
            string selVSL = rollback.VSLStandard.ToString();
            UpdateCellSharedString(worksheetPart.Worksheet, selVSL, "B", 10);

            ////xlSheet.Range["A9"].Value = "Regions and Countries";
            //UpdateCellSharedString(worksheetPart.Worksheet, "Regions and Countries", "A", 10);


            //write regions and countries
            uint rowOffset = 0;
            uint nextRow = 0;

            System.Data.DataTable dtTemp = dtConcEntirePopWeighted.DefaultView.ToTable(true, "REGIONID", "REGIONNAME", "COUNTRYID", "COUNTRYNAME");
            DataView dv = new DataView(dtTemp);
            dv.Sort = "REGIONNAME ASC, COUNTRYNAME ASC";
            System.Data.DataTable dtRegionsCountries = dv.ToTable();
            string region = String.Empty;
            string country = String.Empty;
            foreach (DataRow dr in dtRegionsCountries.Rows)
            {
                //new region? write region
                if (!region.Equals(dr["REGIONNAME"].ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    region = dr["REGIONNAME"].ToString();

                    nextRow = 11 + rowOffset;
                    //xlSheet.Range["B" + nextRow.ToString()].Value = region;
                    UpdateCellSharedString(worksheetPart.Worksheet, region, "B", nextRow);
                    //xlSheet.Range["B" + nextRow.ToString()].Font.Italic = true;
                    GetCell(worksheetPart.Worksheet, "B", nextRow).StyleIndex = styleIndexItalicsWithBorders;
                    GetCell(worksheetPart.Worksheet, "A", nextRow).StyleIndex = styleIndexGrayFillWithBorders;
                    rowOffset++;
                }

                //write country
                country = dr["COUNTRYNAME"].ToString();

                nextRow = 11 + rowOffset;
                //xlSheet.Range["B" + nextRow.ToString()].Value = country;
                UpdateCellSharedString(worksheetPart.Worksheet, country, "B", nextRow);
                GetCell(worksheetPart.Worksheet, "B", nextRow).StyleIndex = styleIndexNoFillIndentWithBorders;
                GetCell(worksheetPart.Worksheet, "A", nextRow).StyleIndex = styleIndexGrayFillWithBorders;
                //xlSheet.Range["B" + nextRow.ToString()].ColumnWidth = 40;
                //xlSheet.Range["B" + nextRow.ToString()].WrapText = true;
                //xlSheet.Range["B" + nextRow.ToString()].InsertIndent(2);

                rowOffset++;
            }

            ////format
            //Microsoft.Office.Interop.Excel.Range xlRange;
            //xlRange = (Microsoft.Office.Interop.Excel.Range)(xlSheet.Columns[1]);
            //xlRange.AutoFit();
            ////add borders
            ////nextRow = 9 + rowOffset;
            //xlRange = xlSheet.Range["A2:B" + nextRow.ToString()];
            //xlRange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //xlRange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //xlRange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //xlRange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //xlRange.Borders.Color = Color.Black;
            ////bold, color label cells
            //xlRange = xlSheet.Range["A2:A" + nextRow.ToString()];
            //xlRange.Font.Bold = true;
            //xlRange.Interior.Color = xlSheet.Range["A2"].Interior.Color;


            //xlSheet.Range["J2"].Value = rollback.Year.ToString() + " " + xlSheet.Range["J2"].Value.ToString();
            //string value = GetCellValue(worksheetPart.Worksheet, "J", 2); YY: changed to column M
            //UpdateCellSharedString(worksheetPart.Worksheet, rollback.Year.ToString() + " " + value, "J", 2);
            string value = GetCellValue(worksheetPart.Worksheet, "M", 2);
            UpdateCellSharedString(worksheetPart.Worksheet, rollback.Year.ToString() + " " + value, "M", 2);
            #endregion

            //results sheet
            #region results sheet
            WorksheetPart worksheetPart2 = GetWorksheetPartByName(spreadsheetDocument, "Detailed Results");
            //xlSheet2.Name = "Results";
            //xlSheet2.Range["A3"].Value = "Country";
            //xlSheet2.Range["B3"].Value = "Population Affected";
            //xlSheet2.Range["C3"].Value = "Avoided Deaths (Total)";
            //xlSheet2.Range["D3"].Value = "Avoided Deaths (% Population)";
            //xlSheet2.Range["E3"].Value = "Min";
            //xlSheet2.Range["F3"].Value = "Median";
            //xlSheet2.Range["G3"].Value = "Max";
            //xlSheet2.Range["H2"].Value = rollback.Year.ToString() + " " + xlSheet2.Range["H2"].Value.ToString();

            //value = GetCellValue(worksheetPart2.Worksheet, "H", 2); YY: Changed to column K
            //UpdateCellSharedString(worksheetPart2.Worksheet, rollback.Year.ToString() + " " + value, "H", 2);
            value = GetCellValue(worksheetPart2.Worksheet, "K", 2);
            UpdateCellSharedString(worksheetPart2.Worksheet, rollback.Year.ToString() + " " + value, "K", 2);

            //xlSheet2.Range["E2:G2"].MergeCells = true;
            //xlSheet2.Range["H3"].Value = "Min";
            //xlSheet2.Range["I3"].Value = "Median";
            //xlSheet2.Range["J3"].Value = "Max";
            //xlSheet2.Range["H2"].Value = "Control";
            //xlSheet2.Range["H2:J2"].MergeCells = true;
            //xlSheet2.Range["K3"].Value = "Air Quality Change (Population Weighted)";

            //format
            //xlSheet2.Range["E2:J2"].Font.Bold = true;
            //xlSheet2.Range["E2:J2"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            //xlSheet2.Range["A3:K3"].Font.Bold = true;
            //xlSheet2.Range["A3:K3"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            //xlSheet2.Range["B3:D3"].ColumnWidth = 20;
            //xlSheet2.Range["E3:J3"].ColumnWidth = 10;
            //xlSheet2.Range["K3"].ColumnWidth = 20;
            //xlSheet2.Range["B3:K3"].WrapText = true;
            ////country column
            //xlRange = (Microsoft.Office.Interop.Excel.Range)(xlSheet2.Columns[1]);
            //xlRange.ColumnWidth = 40;
            //xlRange.WrapText = true;

            //build output table
            System.Data.DataTable dtDetailedResults = new System.Data.DataTable();
            dtDetailedResults.Columns.Add("NAME", Type.GetType("System.String"));
            dtDetailedResults.Columns.Add("IS_REGION", Type.GetType("System.Boolean"));
            dtDetailedResults.Columns.Add("POP_AFFECTED", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("AVOIDED_DEATHS", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("CONFIDENCE_INTERVAL", Type.GetType("System.String"));
            dtDetailedResults.Columns.Add("PERCENT_BASELINE_MORTALITY", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("DEATHS_PER_100_THOUSAND", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("AVOIDED_DEATHS_PERCENT_POP", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("BASELINE_MIN", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("BASELINE_MEDIAN", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("BASELINE_MAX", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("CONTROL_MIN", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("CONTROL_MEDIAN", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("CONTROL_MAX", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("AIR_QUALITY_CHANGE", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("ECONOMIC_BENEFITS", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("AVOIDED_YLL", Type.GetType("System.Double"));
            dtDetailedResults.Columns.Add("CHANGE_IN_LE", Type.GetType("System.Double"));


            string regionid = String.Empty;
            string countryid = String.Empty;
            foreach (DataRow dr in dtRegionsCountries.Rows)
            {
                //new region? get region data
                if (!regionid.Equals(dr["REGIONID"].ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    regionid = dr["REGIONID"].ToString();
                    GetResults(regionid, dr["REGIONNAME"].ToString(), true, dtDetailedResults, FORMAT_DECIMAL_0_PLACES);
                }

                //get country data
                countryid = dr["COUNTRYID"].ToString();
                GetResults(countryid, dr["COUNTRYNAME"].ToString(), false, dtDetailedResults, FORMAT_DECIMAL_0_PLACES);
            }


            //write results to spreadsheet
            nextRow = 4;
            foreach (DataRow dr in dtDetailedResults.Rows)
            {
                //xlSheet2.Range["A" + nextRow.ToString()].Value = dr["NAME"].ToString();
                UpdateCellSharedString(worksheetPart2.Worksheet, dr["NAME"].ToString(), "A", nextRow);
                if (Convert.ToBoolean(dr["IS_REGION"].ToString()))
                {
                    //xlSheet2.Range["A" + nextRow.ToString()].Font.Italic = true;
                    GetCell(worksheetPart2.Worksheet, "A", nextRow).StyleIndex = styleIndexItalicsWithBorders;
                }
                else
                {
                    //xlSheet2.Range["A" + nextRow.ToString()].ColumnWidth = 40;
                    //xlSheet2.Range["A" + nextRow.ToString()].WrapText = true;
                    //xlSheet2.Range["A" + nextRow.ToString()].InsertIndent(2);
                    GetCell(worksheetPart2.Worksheet, "A", nextRow).StyleIndex = styleIndexNoFillIndentWithBorders;
                }
                //xlSheet2.Range["B" + nextRow.ToString()].Value = FormatDoubleStringTwoSignificantFigures(FORMAT_DECIMAL_0_PLACES, dr["POP_AFFECTED"].ToString());
                UpdateCellNumber(worksheetPart2.Worksheet, FormatDoubleStringTwoSignificantFigures(FORMAT_DECIMAL_0_PLACES, dr["POP_AFFECTED"].ToString()), "B", nextRow);
                GetCell(worksheetPart2.Worksheet, "B", nextRow).StyleIndex = styleIndexNoFillNumber0DecimalPlacesWithBorders;
                //xlSheet2.Range["C" + nextRow.ToString()].Value = FormatDoubleStringTwoSignificantFigures(FORMAT_DECIMAL_0_PLACES, dr["AVOIDED_DEATHS"].ToString());
                UpdateCellNumber(worksheetPart2.Worksheet, FormatDoubleStringTwoSignificantFigures(FORMAT_DECIMAL_0_PLACES, dr["AVOIDED_DEATHS"].ToString()), "C", nextRow);
                GetCell(worksheetPart2.Worksheet, "C", nextRow).StyleIndex = styleIndexNoFillNumber0DecimalPlacesWithBorders;
                //xlSheet2.Range["D" + nextRow.ToString()].Value = "'" + dr["CONFIDENCE_INTERVAL"].ToString(); //prepend apostrophe so Excel treats this as text not date
                UpdateCellSharedString(worksheetPart2.Worksheet, dr["CONFIDENCE_INTERVAL"].ToString(), "D", nextRow);
                GetCell(worksheetPart2.Worksheet, "D", nextRow).StyleIndex = styleIndexNoFillCenterWithBorders;
                //xlSheet2.Range["E" + nextRow.ToString()].Value = dr["PERCENT_BASELINE_MORTALITY"].ToString();
                UpdateCellNumber(worksheetPart2.Worksheet, dr["PERCENT_BASELINE_MORTALITY"].ToString(), "E", nextRow);
                GetCell(worksheetPart2.Worksheet, "E", nextRow).StyleIndex = styleIndexNoFillWithBorders;
                //xlSheet2.Range["F" + nextRow.ToString()].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["DEATHS_PER_100_THOUSAND"].ToString());
                UpdateCellNumber(worksheetPart2.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["DEATHS_PER_100_THOUSAND"].ToString()), "F", nextRow);
                GetCell(worksheetPart2.Worksheet, "F", nextRow).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet2.Range["G" + nextRow.ToString()].Value = dr["AVOIDED_DEATHS_PERCENT_POP"].ToString();//FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["AVOIDED_DEATHS_PERCENT_POP"].ToString());
                UpdateCellNumber(worksheetPart2.Worksheet, dr["AVOIDED_DEATHS_PERCENT_POP"].ToString(), "G", nextRow);
                GetCell(worksheetPart2.Worksheet, "G", nextRow).StyleIndex = styleIndexNoFillWithBorders;

                //YY: the following fields are added in July 2017
                UpdateCellNumber(worksheetPart2.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["ECONOMIC_BENEFITS"].ToString()), "H", nextRow);
                GetCell(worksheetPart2.Worksheet, "H", nextRow).StyleIndex = styleIndexNoFillNumber0DecimalPlacesWithBorders;
                UpdateCellNumber(worksheetPart2.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["AVOIDED_YLL"].ToString()), "I", nextRow);
                GetCell(worksheetPart2.Worksheet, "I", nextRow).StyleIndex = styleIndexNoFillNumber0DecimalPlacesWithBorders;
                UpdateCellNumber(worksheetPart2.Worksheet, dr["CHANGE_IN_LE"].ToString(), "J", nextRow);
                GetCell(worksheetPart2.Worksheet, "J", nextRow).StyleIndex = styleIndexNoFillWithBorders;

                //YY: The following fields are moved 3 columns to the right in July 2017. For example H --> K
                //xlSheet2.Range["K" + nextRow.ToString()].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["BASELINE_MIN"].ToString());
                UpdateCellNumber(worksheetPart2.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["BASELINE_MIN"].ToString()), "K", nextRow);
                GetCell(worksheetPart2.Worksheet, "K", nextRow).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet2.Range["L" + nextRow.ToString()].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["BASELINE_MEDIAN"].ToString());
                UpdateCellNumber(worksheetPart2.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["BASELINE_MEDIAN"].ToString()), "L", nextRow);
                GetCell(worksheetPart2.Worksheet, "L", nextRow).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet2.Range["M" + nextRow.ToString()].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["BASELINE_MAX"].ToString());
                UpdateCellNumber(worksheetPart2.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["BASELINE_MAX"].ToString()), "M", nextRow);
                GetCell(worksheetPart2.Worksheet, "M", nextRow).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet2.Range["N" + nextRow.ToString()].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["CONTROL_MIN"].ToString());
                UpdateCellNumber(worksheetPart2.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["CONTROL_MIN"].ToString()), "N", nextRow);
                GetCell(worksheetPart2.Worksheet, "N", nextRow).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet2.Range["O" + nextRow.ToString()].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["CONTROL_MEDIAN"].ToString());
                UpdateCellNumber(worksheetPart2.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["CONTROL_MEDIAN"].ToString()), "O", nextRow);
                GetCell(worksheetPart2.Worksheet, "O", nextRow).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet2.Range["P" + nextRow.ToString()].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["CONTROL_MAX"].ToString());
                UpdateCellNumber(worksheetPart2.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["CONTROL_MAX"].ToString()), "P", nextRow);
                GetCell(worksheetPart2.Worksheet, "P", nextRow).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet2.Range["Q" + nextRow.ToString()].Value = dr["AIR_QUALITY_CHANGE"].ToString();// FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["AIR_QUALITY_CHANGE"].ToString());
                UpdateCellNumber(worksheetPart2.Worksheet, dr["AIR_QUALITY_CHANGE"].ToString(), "Q", nextRow);
                GetCell(worksheetPart2.Worksheet, "Q", nextRow).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                nextRow++;

            }

            ////center confidence interval
            //xlRange = xlSheet2.Range["D4:D" + (nextRow - 1).ToString()];
            //xlRange.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            ////add cell borders
            //xlRange = xlSheet2.Range["A4:N" + (nextRow - 1).ToString()];
            //xlRange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //xlRange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //xlRange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //xlRange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //xlRange.Borders.Color = Color.Black;

            #endregion

            #region back to summary sheet

            //get results for summary table
            System.Data.DataTable dtSummaryResults = dtDetailedResults.Clone();
            GetResults(null, "SUMMARY", false, dtSummaryResults, FORMAT_DECIMAL_0_PLACES);
            if (dtSummaryResults.Rows.Count > 0)
            {
                DataRow dr = dtSummaryResults.Rows[0];
                //xlSheet.Range["D4"].Value = FormatDoubleStringTwoSignificantFigures(FORMAT_DECIMAL_0_PLACES, dr["POP_AFFECTED"].ToString());
                UpdateCellNumber(worksheetPart.Worksheet, FormatDoubleStringTwoSignificantFigures(FORMAT_DECIMAL_0_PLACES, dr["POP_AFFECTED"].ToString()), "D", 4);
                GetCell(worksheetPart.Worksheet, "D", 4).StyleIndex = styleIndexNoFillNumber0DecimalPlacesWithBorders;
                //xlSheet.Range["E4"].Value = FormatDoubleStringTwoSignificantFigures(FORMAT_DECIMAL_0_PLACES, dr["AVOIDED_DEATHS"].ToString());
                UpdateCellNumber(worksheetPart.Worksheet, FormatDoubleStringTwoSignificantFigures(FORMAT_DECIMAL_0_PLACES, dr["AVOIDED_DEATHS"].ToString()), "E", 4);
                GetCell(worksheetPart.Worksheet, "E", 4).StyleIndex = styleIndexNoFillNumber0DecimalPlacesWithBorders;
                //xlSheet.Range["F4"].Value = "'" + dr["CONFIDENCE_INTERVAL"].ToString(); //prepend apostrophe so Excel treats this as text not date
                UpdateCellSharedString(worksheetPart.Worksheet, dr["CONFIDENCE_INTERVAL"].ToString(), "F", 4);
                GetCell(worksheetPart.Worksheet, "F", 4).StyleIndex = styleIndexNoFillCenterWithBorders;
                //xlSheet.Range["G4"].Value = dr["PERCENT_BASELINE_MORTALITY"].ToString();
                UpdateCellNumber(worksheetPart.Worksheet, dr["PERCENT_BASELINE_MORTALITY"].ToString(), "G", 4);
                GetCell(worksheetPart.Worksheet, "G", 4).StyleIndex = styleIndexNoFillWithBorders;
                //xlSheet.Range["H4"].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["DEATHS_PER_100_THOUSAND"].ToString());
                UpdateCellNumber(worksheetPart.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["DEATHS_PER_100_THOUSAND"].ToString()), "H", 4);
                GetCell(worksheetPart.Worksheet, "H", 4).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet.Range["I4"].Value = dr["AVOIDED_DEATHS_PERCENT_POP"].ToString();//FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["AVOIDED_DEATHS_PERCENT_POP"].ToString());
                UpdateCellNumber(worksheetPart.Worksheet, dr["AVOIDED_DEATHS_PERCENT_POP"].ToString(), "I", 4);
                GetCell(worksheetPart.Worksheet, "I", 4).StyleIndex = styleIndexNoFillWithBorders;

                //YY: the following fields are added in July 2017
                UpdateCellNumber(worksheetPart.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["ECONOMIC_BENEFITS"].ToString()), "J", 4);
                GetCell(worksheetPart.Worksheet, "J", 4).StyleIndex = styleIndexNoFillNumber0DecimalPlacesWithBorders;
                UpdateCellNumber(worksheetPart.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["AVOIDED_YLL"].ToString()), "K", 4);
                GetCell(worksheetPart.Worksheet, "K", 4).StyleIndex = styleIndexNoFillNumber0DecimalPlacesWithBorders;
                UpdateCellNumber(worksheetPart.Worksheet, dr["CHANGE_IN_LE"].ToString(), "L", 4);
                GetCell(worksheetPart.Worksheet, "L", 4).StyleIndex = styleIndexNoFillWithBorders;

                //YY: The following fields are moved 3 columns to the right in July 2017. For example J --> M
                //xlSheet.Range["M4"].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["BASELINE_MIN"].ToString());
                UpdateCellNumber(worksheetPart.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["BASELINE_MIN"].ToString()), "M", 4);
                GetCell(worksheetPart.Worksheet, "M", 4).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet.Range[N4"].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["BASELINE_MEDIAN"].ToString());
                UpdateCellNumber(worksheetPart.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["BASELINE_MEDIAN"].ToString()), "N", 4);
                GetCell(worksheetPart.Worksheet, "N", 4).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet.Range["O4"].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["BASELINE_MAX"].ToString());
                UpdateCellNumber(worksheetPart.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["BASELINE_MAX"].ToString()), "O", 4);
                GetCell(worksheetPart.Worksheet, "O", 4).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet.Range["P4"].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["CONTROL_MIN"].ToString());
                UpdateCellNumber(worksheetPart.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["CONTROL_MIN"].ToString()), "P", 4);
                GetCell(worksheetPart.Worksheet, "P", 4).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet.Range["Q4"].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["CONTROL_MEDIAN"].ToString());
                UpdateCellNumber(worksheetPart.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["CONTROL_MEDIAN"].ToString()), "Q", 4);
                GetCell(worksheetPart.Worksheet, "Q", 4).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet.Range["R4"].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["CONTROL_MAX"].ToString());
                UpdateCellNumber(worksheetPart.Worksheet, FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["CONTROL_MAX"].ToString()), "R", 4);
                GetCell(worksheetPart.Worksheet, "R", 4).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;
                //xlSheet.Range["S4"].Value = dr["AIR_QUALITY_CHANGE"].ToString();// FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["AIR_QUALITY_CHANGE"].ToString());
                UpdateCellNumber(worksheetPart.Worksheet, dr["AIR_QUALITY_CHANGE"].ToString(), "S", 4);
                GetCell(worksheetPart.Worksheet, "S", 4).StyleIndex = styleIndexNoFillNumber2DecimalPlacesWithBorders;

            }
            //xlRange = xlSheet.Range["D4:P4"];
            //xlRange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //xlRange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //xlRange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //xlRange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //xlRange.Borders.Color = Color.Black;

            #endregion

            #region charts

            //summary chart
            //write summary chart data to hidden sheet
            //sheet DataSource is hidden and is the 4th sheet (Metadata is the third sheet)
            //Microsoft.Office.Interop.Excel.Worksheet xlSheet4 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets[4];
            WorksheetPart worksheetPart3 = GetWorksheetPartByName(spreadsheetDocument, "DataSource");
            uint nextRowForSummary = 1;
            foreach (DataRow dr in dtDetailedResults.Rows)
            {
                //only write countries, skip regions
                if (!Convert.ToBoolean(dr["IS_REGION"].ToString()))
                {
                    //xlSheet4.Range["A" + nextRowForSummary.ToString()].Value = dr["NAME"].ToString();
                    UpdateCellSharedString(worksheetPart3.Worksheet, dr["NAME"].ToString(), "A", nextRowForSummary);
                    //xlSheet4.Range["B" + nextRowForSummary.ToString()].Value = FormatDoubleString(FORMAT_DECIMAL_2_PLACES, dr["AVOIDED_DEATHS"].ToString());
                    UpdateCellNumber(worksheetPart3.Worksheet, FormatDoubleStringTwoSignificantFigures(FORMAT_DECIMAL_0_PLACES, dr["AVOIDED_DEATHS"].ToString()), "B", nextRowForSummary);
                    GetCell(worksheetPart3.Worksheet, "B", nextRowForSummary).StyleIndex = styleIndexNoFillNumber0DecimalPlacesWithBorders;
                    nextRowForSummary++;
                }
            }
            //Microsoft.Office.Interop.Excel.ChartObject xlChartObject = (Microsoft.Office.Interop.Excel.ChartObject)xlSheet.ChartObjects(1);            
            //Microsoft.Office.Interop.Excel.Chart xlChart = (Microsoft.Office.Interop.Excel.Chart)xlChartObject.Chart;
            //Microsoft.Office.Interop.Excel.Series xlSeries = (Microsoft.Office.Interop.Excel.Series)xlChart.SeriesCollection(1);
            //xlSeries.Values = xlSheet4.Range["B1:B" + (nextRowForSummary - 1).ToString()];
            //xlSeries.XValues = xlSheet4.Range["A1:A" + (nextRowForSummary - 1).ToString()];
            DrawingsPart drawingsPart = worksheetPart.GetPartsOfType<DrawingsPart>().First();
            ChartPart chartPart = drawingsPart.GetPartsOfType<ChartPart>().First();
            DocumentFormat.OpenXml.Drawing.Charts.Chart chart = chartPart.ChartSpace.Elements<DocumentFormat.OpenXml.Drawing.Charts.Chart>().First();
            DocumentFormat.OpenXml.Drawing.Charts.PlotArea plotArea = chart.Elements<DocumentFormat.OpenXml.Drawing.Charts.PlotArea>().First();
            DocumentFormat.OpenXml.Drawing.Charts.PieChart pieChart = plotArea.Elements<DocumentFormat.OpenXml.Drawing.Charts.PieChart>().First();
            DocumentFormat.OpenXml.Drawing.Charts.PieChartSeries pieChartSeries = pieChart.Elements<DocumentFormat.OpenXml.Drawing.Charts.PieChartSeries>().First();
            DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData categoryAxisData;
            DocumentFormat.OpenXml.Drawing.Charts.MultiLevelStringReference multiLevelStringReference;
            DocumentFormat.OpenXml.Drawing.Charts.Formula formula;
            //if category axis data does not exist then add it
            if (pieChartSeries.Elements<DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData>().Count() > 0)
            {
                categoryAxisData = pieChartSeries.Elements<DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData>().First();
                multiLevelStringReference = categoryAxisData.Elements<DocumentFormat.OpenXml.Drawing.Charts.MultiLevelStringReference>().First();
                formula = multiLevelStringReference.Elements<DocumentFormat.OpenXml.Drawing.Charts.Formula>().First();
                formula.Text = "\'DataSource\'!$A$1:$A$" + (nextRowForSummary - 1).ToString();
            }
            else
            {
                categoryAxisData = new DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData();
                multiLevelStringReference = new DocumentFormat.OpenXml.Drawing.Charts.MultiLevelStringReference();
                formula = new DocumentFormat.OpenXml.Drawing.Charts.Formula();
                formula.Text = "\'DataSource\'!$A$1:$A$" + (nextRowForSummary - 1).ToString();
                multiLevelStringReference.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.Formula>(formula);
                categoryAxisData.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.MultiLevelStringReference>(multiLevelStringReference);
                pieChartSeries.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData>(categoryAxisData);
            }


            DocumentFormat.OpenXml.Drawing.Charts.Values values = pieChartSeries.Elements<DocumentFormat.OpenXml.Drawing.Charts.Values>().First();
            DocumentFormat.OpenXml.Drawing.Charts.NumberReference numberReference = values.Elements<DocumentFormat.OpenXml.Drawing.Charts.NumberReference>().First();
            DocumentFormat.OpenXml.Drawing.Charts.Formula formulaValues = numberReference.Elements<DocumentFormat.OpenXml.Drawing.Charts.Formula>().First();
            formulaValues.Text = "\'DataSource\'!$B$1:$B$" + (nextRowForSummary - 1).ToString();



            //write to total avoided deaths text box on chart
            //Microsoft.Office.Interop.Excel.Shape txtBox = (Microsoft.Office.Interop.Excel.Shape)xlSheet.Shapes.Item("TextBox 1");
            //txtBox.TextFrame.Characters().Text = txtBox.TextFrame.Characters().Text + " " + xlSheet.Range["E4"].Text; //use .Text rather than .Value on the range here, because it is formatted
            DocumentFormat.OpenXml.Drawing.Spreadsheet.WorksheetDrawing worksheetDrawing = drawingsPart.WorksheetDrawing;
            DocumentFormat.OpenXml.Drawing.Spreadsheet.TwoCellAnchor twoCellAnchor = worksheetDrawing.Elements<DocumentFormat.OpenXml.Drawing.Spreadsheet.TwoCellAnchor>().ElementAt(1);
            DocumentFormat.OpenXml.Drawing.Spreadsheet.Shape shape = twoCellAnchor.Elements<DocumentFormat.OpenXml.Drawing.Spreadsheet.Shape>().First();
            DocumentFormat.OpenXml.Drawing.Spreadsheet.TextBody textBody = shape.Elements<DocumentFormat.OpenXml.Drawing.Spreadsheet.TextBody>().First();
            DocumentFormat.OpenXml.Drawing.Paragraph paragraph = textBody.Elements<DocumentFormat.OpenXml.Drawing.Paragraph>().First();
            DocumentFormat.OpenXml.Drawing.Run run = paragraph.Elements<DocumentFormat.OpenXml.Drawing.Run>().First();
            DocumentFormat.OpenXml.Drawing.Text text = run.Elements<DocumentFormat.OpenXml.Drawing.Text>().First();
            text.Text = text.Text + " " + Double.Parse(GetCellValue(worksheetPart.Worksheet, "E", 4)).ToString(FORMAT_DECIMAL_0_PLACES);


            ////avoided deaths chart sheet
            //xlChart = (Microsoft.Office.Interop.Excel.Chart)xlBook.Charts[1];
            //xlSeries = (Microsoft.Office.Interop.Excel.Series)xlChart.SeriesCollection(1);
            //xlSeries.Values = xlSheet2.Range["C4:C" + (nextRow - 1).ToString()];
            //xlSeries.XValues = xlSheet2.Range["A4:A" + (nextRow - 1).ToString()];
            ChartsheetPart chartsheetPart = GetChartsheetPartByName(spreadsheetDocument, "Avoided Deaths By Country");
            drawingsPart = chartsheetPart.GetPartsOfType<DrawingsPart>().First();
            chartPart = drawingsPart.GetPartsOfType<ChartPart>().First();
            chart = chartPart.ChartSpace.Elements<DocumentFormat.OpenXml.Drawing.Charts.Chart>().First();
            plotArea = chart.Elements<DocumentFormat.OpenXml.Drawing.Charts.PlotArea>().First();
            DocumentFormat.OpenXml.Drawing.Charts.BarChart barChart = plotArea.Elements<DocumentFormat.OpenXml.Drawing.Charts.BarChart>().First();
            DocumentFormat.OpenXml.Drawing.Charts.BarChartSeries barChartSeries = barChart.Elements<DocumentFormat.OpenXml.Drawing.Charts.BarChartSeries>().First();
            DocumentFormat.OpenXml.Drawing.Charts.StringReference stringReference;
            //if category axis data does not exist then add it
            if (barChartSeries.Elements<DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData>().Count() > 0)
            {
                categoryAxisData = barChartSeries.Elements<DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData>().First();
                stringReference = categoryAxisData.Elements<DocumentFormat.OpenXml.Drawing.Charts.StringReference>().First();
                formula = stringReference.Elements<DocumentFormat.OpenXml.Drawing.Charts.Formula>().First();
                formula.Text = "\'Detailed Results\'!$A$4:$A$" + (nextRow - 1).ToString();
            }
            else
            {
                categoryAxisData = new DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData();
                stringReference = new DocumentFormat.OpenXml.Drawing.Charts.StringReference();
                formula = new DocumentFormat.OpenXml.Drawing.Charts.Formula();
                formula.Text = "\'Detailed Results\'!$A$4:$A$" + (nextRow - 1).ToString();
                stringReference.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.Formula>(formula);
                categoryAxisData.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.StringReference>(stringReference);
                barChartSeries.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData>(categoryAxisData);
            }

            values = barChartSeries.Elements<DocumentFormat.OpenXml.Drawing.Charts.Values>().First();
            numberReference = values.Elements<DocumentFormat.OpenXml.Drawing.Charts.NumberReference>().First();
            formulaValues = numberReference.Elements<DocumentFormat.OpenXml.Drawing.Charts.Formula>().First();
            formulaValues.Text = "\'Detailed Results\'!$C$4:$C$" + (nextRow - 1).ToString();

            ////deaths per 100,000
            //xlChart = (Microsoft.Office.Interop.Excel.Chart)xlBook.Charts[2];
            //xlSeries = (Microsoft.Office.Interop.Excel.Series)xlChart.SeriesCollection(1);
            //xlSeries.Values = xlSheet2.Range["F4:F" + (nextRow - 1).ToString()];
            //xlSeries.XValues = xlSheet2.Range["A4:A" + (nextRow - 1).ToString()];
            chartsheetPart = GetChartsheetPartByName(spreadsheetDocument, "Deaths Per 100,000");
            drawingsPart = chartsheetPart.GetPartsOfType<DrawingsPart>().First();
            chartPart = drawingsPart.GetPartsOfType<ChartPart>().First();
            chart = chartPart.ChartSpace.Elements<DocumentFormat.OpenXml.Drawing.Charts.Chart>().First();
            plotArea = chart.Elements<DocumentFormat.OpenXml.Drawing.Charts.PlotArea>().First();
            barChart = plotArea.Elements<DocumentFormat.OpenXml.Drawing.Charts.BarChart>().First();
            barChartSeries = barChart.Elements<DocumentFormat.OpenXml.Drawing.Charts.BarChartSeries>().First();
            //if category axis data does not exist then add it
            if (barChartSeries.Elements<DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData>().Count() > 0)
            {
                categoryAxisData = barChartSeries.Elements<DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData>().First();
                stringReference = categoryAxisData.Elements<DocumentFormat.OpenXml.Drawing.Charts.StringReference>().First();
                formula = stringReference.Elements<DocumentFormat.OpenXml.Drawing.Charts.Formula>().First();
                formula.Text = "\'Detailed Results\'!$A$4:$A$" + (nextRow - 1).ToString();
            }
            else
            {
                categoryAxisData = new DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData();
                stringReference = new DocumentFormat.OpenXml.Drawing.Charts.StringReference();
                formula = new DocumentFormat.OpenXml.Drawing.Charts.Formula();
                formula.Text = "\'Detailed Results\'!$A$4:$A$" + (nextRow - 1).ToString();
                stringReference.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.Formula>(formula);
                categoryAxisData.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.StringReference>(stringReference);
                barChartSeries.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData>(categoryAxisData);
            }

            values = barChartSeries.Elements<DocumentFormat.OpenXml.Drawing.Charts.Values>().First();
            numberReference = values.Elements<DocumentFormat.OpenXml.Drawing.Charts.NumberReference>().First();
            formulaValues = numberReference.Elements<DocumentFormat.OpenXml.Drawing.Charts.Formula>().First();
            formulaValues.Text = "\'Detailed Results\'!$F$4:$F$" + (nextRow - 1).ToString();

            //YY: economic benefits chart
            chartsheetPart = GetChartsheetPartByName(spreadsheetDocument, "Economic Benefits");
            drawingsPart = chartsheetPart.GetPartsOfType<DrawingsPart>().First();
            chartPart = drawingsPart.GetPartsOfType<ChartPart>().First();
            chart = chartPart.ChartSpace.Elements<DocumentFormat.OpenXml.Drawing.Charts.Chart>().First();
            plotArea = chart.Elements<DocumentFormat.OpenXml.Drawing.Charts.PlotArea>().First();
            barChart = plotArea.Elements<DocumentFormat.OpenXml.Drawing.Charts.BarChart>().First();
            barChartSeries = barChart.Elements<DocumentFormat.OpenXml.Drawing.Charts.BarChartSeries>().First();
            //if category axis data does not exist then add it
            if (barChartSeries.Elements<DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData>().Count() > 0)
            {
                categoryAxisData = barChartSeries.Elements<DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData>().First();
                stringReference = categoryAxisData.Elements<DocumentFormat.OpenXml.Drawing.Charts.StringReference>().First();
                formula = stringReference.Elements<DocumentFormat.OpenXml.Drawing.Charts.Formula>().First();
                formula.Text = "\'Detailed Results\'!$A$4:$A$" + (nextRow - 1).ToString();
            }
            else
            {
                categoryAxisData = new DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData();
                stringReference = new DocumentFormat.OpenXml.Drawing.Charts.StringReference();
                formula = new DocumentFormat.OpenXml.Drawing.Charts.Formula();
                formula.Text = "\'Detailed Results\'!$A$4:$A$" + (nextRow - 1).ToString();
                stringReference.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.Formula>(formula);
                categoryAxisData.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.StringReference>(stringReference);
                barChartSeries.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.CategoryAxisData>(categoryAxisData);
            }

            values = barChartSeries.Elements<DocumentFormat.OpenXml.Drawing.Charts.Values>().First();
            numberReference = values.Elements<DocumentFormat.OpenXml.Drawing.Charts.NumberReference>().First();
            formulaValues = numberReference.Elements<DocumentFormat.OpenXml.Drawing.Charts.Formula>().First();
            formulaValues.Text = "\'Detailed Results\'!$H$4:$H$" + (nextRow - 1).ToString();

            #endregion

            //Set Summary sheet (index = 0) as the active sheet
            //var sheet = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s=>s.Name =="Summary");
            //var sheetIndex = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>().ToList().IndexOf(sheet);
            //var workBookView = spreadsheetDocument.WorkbookPart.Workbook.Descendants<WorkbookView>().First();
            //workBookView.ActiveTab = Convert.ToUInt32(sheetIndex);
            var workBookView = spreadsheetDocument.WorkbookPart.Workbook.Descendants<WorkbookView>().First();
            workBookView.ActiveTab = 0;

            //save
            spreadsheetDocument.WorkbookPart.Workbook.Save();
            spreadsheetDocument.Close();

        }

        private void SaveRollbackReportCSV(GBDRollbackItem rollback)
        {
            //get application path
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = String.Empty;

            //check save dir 
            string resultsDir = txtFilePath.Text.Trim();
            if (!Directory.Exists(resultsDir))
            {
                Directory.CreateDirectory(resultsDir);
            }

            //get timestamp
            DateTime dtNow = DateTime.Now;
            string timeStamp = dtNow.ToString("yyyyMMddHHmm");

            //export details
            //get details path
            filePath = resultsDir + @"\GBDRollback_" + rollback.Name + "_Details_" + timeStamp + ".csv";
            System.Data.DataTable dtRegionsCountries = GetRegionsCountriesTable();
            //build output table
            System.Data.DataTable dtDetailedResults = GetDetailedResultsTable(dtRegionsCountries, FORMAT_DECIMAL_0_PLACES_CSV);

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                List<object> listOutputLine = null;
                string outputLine = "Region and Country,Is Region,Population Affected,Avoided Deaths (Total)," +
                    "95% CI,% of Baseline Mortality,Deaths per 100000,Avoided Deaths (% Population)," +
                    "2015 Air Quality Levels Min,2015 Air Quality Levels Median,2015 Air Quality Levels Max," +
                    "Policy Scenario Min,Policy Scenario Median,Policy Scenario Max,Air Quality Change (Population Weighted)";

                sw.WriteLine(outputLine);

                foreach (DataRow dr in dtDetailedResults.Rows)
                {
                    listOutputLine = dr.ItemArray.ToList<object>();
                    //write output line
                    outputLine = string.Join(",", listOutputLine);
                    sw.WriteLine(outputLine);
                }

            }

            //export summary
            //get results for summary table
            filePath = resultsDir + @"\GBDRollback_" + rollback.Name + "_Summary_" + timeStamp + ".csv";
            System.Data.DataTable dtSummaryResults = GetSummaryResultsTable(dtDetailedResults, FORMAT_DECIMAL_0_PLACES_CSV);
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                List<object> listOutputLine = null;

                string outputLine = "Pollutant,Background Concentration,Rollback Type,Function,Population Affected,Avoided Deaths (Total)," +
                    "95% CI,% of Baseline Mortality,Deaths per 100000,Avoided Deaths (% Population)," +
                    "2015 Air Quality Levels Min,2015 Air Quality Levels Median,2015 Air Quality Levels Max," +
                    "Policy Scenario Min,Policy Scenario Median,Policy Scenario Max,Air Quality Change (Population Weighted)";

                sw.WriteLine(outputLine);

                foreach (DataRow dr in dtSummaryResults.Rows)
                {
                    listOutputLine = dr.ItemArray.ToList<object>();
                    //remove name and is region columns 
                    listOutputLine[0] = "PM2.5";
                    listOutputLine[1] = GetBackgroundConcentrationText(rollback);

                    listOutputLine.Insert(2, GetRollbackTypeSummary(rollback));

                    listOutputLine.Insert(3, rollback.Function.ToString());

                    //write output line
                    outputLine = string.Join(",", listOutputLine);
                    sw.WriteLine(outputLine);
                }

            }



        }

        private string FormatDoubleString(string format, string str)
        {
            return Double.Parse(str).ToString(format);
        }

        private string FormatDoubleStringTwoSignificantFigures(string format, string str)
        {
            Double dbl = Double.Parse(str);

            if ((dbl > 100) || (dbl < -100))
            {
                //absolute value, log10, floor (round down to nearest int), round function
                //original Excel function: =ROUND(T19,2-1-INT(LOG10(ABS(T19))))

                long numDecimalPlaces = Math.Abs(Convert.ToInt64(1 - Math.Floor(Math.Log10(Math.Abs(dbl)))));

                double tenFactor = Math.Pow(Convert.ToDouble(10), Convert.ToDouble(numDecimalPlaces));

                dbl = dbl / tenFactor;

                dbl = Math.Round(dbl, 0, MidpointRounding.AwayFromZero);

                dbl = dbl * tenFactor;
            }

            return dbl.ToString(format);
        }

        private double Median(IEnumerable<double> list)
        {
            List<double> orderedList = list
                .OrderBy(numbers => numbers)
                .ToList();

            int listSize = orderedList.Count;
            double result;

            if (listSize % 2 == 0) // even
            {
                int midIndex = listSize / 2;
                result = ((orderedList.ElementAt(midIndex - 1) +
                           orderedList.ElementAt(midIndex)) / 2);
            }
            else // odd
            {
                double element = (double)listSize / 2;
                element = Math.Round(element, MidpointRounding.AwayFromZero);

                result = orderedList.ElementAt((int)(element - 1));
            }

            return result;
        }

        //region/country id, region/country name, isRegion, dtDetailedResults, format
        private void GetResults(string id, string name, bool isRegion, System.Data.DataTable dt, string format)
        {
            double popAffected;
            double avoidedDeaths;

            double result_2_5;
            double result_97_5;
            string confidenceInterval;
            double baselineMortality;
            double percentBaselineMortality;
            double deathsPer100Thousand;
            double avoidedDeathsPercentPop;
            double baselineMin;
            double baselineMedian;
            double baselineMax;
            double controlMin;
            double controlMedian;
            double controlMax;
            double airQualityChange;
            object result;
            //YY: need to add eco benifit and YLL
            double ecoBenefit=0;
            double yll=0;
            double changeLifeExp = 0;

            string filter = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                if (isRegion)
                {
                    filter = "REGIONID = " + id;
                }
                else
                {
                    filter = "COUNTRYID = '" + id + "'";
                }
            }
            else
            {
                filter = "1=1"; //no filter (i.e., all rows)
            }

            //population
            //get 1 population row per coordinate for each age range and gender
            //DataTable dtPopulation = dtConcEntirePopWeighted.DefaultView.ToTable(true, "REGIONID", "REGIONNAME", "COUNTRYID", "COUNTRYNAME", "AGERANGENAME", "GENDERNAME", "POPESTIMATE");
            //result = dtConcEntirePopWeighted.Compute("SUM(POPESTIMATE)", filter);
            //YY: population is now part of the result
            result = rollbackResultsByCountry[filter].Population;
            popAffected = Double.Parse(result.ToString());

            //baselineMortality
            result = dtConcEntirePopWeighted.Compute("SUM(BASELINE_MORTALITY)", filter);
            baselineMortality = Double.Parse(result.ToString());

            //System.Data.DataTable dtKrewski = dtConcEntireRollback.DefaultView.ToTable(true, "REGIONID", "REGIONNAME", "COUNTRYID", "COUNTRYNAME","RESULT", "RESULT_2_5", "RESULT_97_5");
            //dtKrewski.DefaultView.Sort = "REGIONNAME, COUNTRYNAME";

            //avoided deaths
            result = rollbackResultsByCountry[filter].Result; // dtKrewski.Compute("SUM(RESULT)", filter);
            avoidedDeaths = Double.Parse(result.ToString());

            //confidence interval
            result = rollbackResultsByCountry[filter].Result2_5; //dtKrewski.Compute("SUM(RESULT_2_5)", filter);
            result_2_5 = Double.Parse(result.ToString());
            result = rollbackResultsByCountry[filter].Result97_5; //dtKrewski.Compute("SUM(RESULT_97_5)", filter);
            result_97_5 = Double.Parse(result.ToString());
            confidenceInterval = FormatDoubleStringTwoSignificantFigures(format, result_2_5.ToString()) + " - " + FormatDoubleStringTwoSignificantFigures(format, result_97_5.ToString());

            //percent baseline mortality
            percentBaselineMortality = (avoidedDeaths / baselineMortality) * 100;

            //deaths per 100,000
            deathsPer100Thousand = avoidedDeaths / (popAffected / 100000);

            //avoided deaths percent pop
            avoidedDeathsPercentPop = (avoidedDeaths / popAffected) * 100;

            //YY: add eco bene
            result = rollbackResultsByCountry[filter].EcoBenefit;
            ecoBenefit = Double.Parse(result.ToString());

            //YY: YLL result
            result = rollbackResultsByCountry[filter].Yll;
            yll = Double.Parse(result.ToString());

            //YY: change in life exp
            result = yll / popAffected;
            changeLifeExp = Double.Parse(result.ToString());

            //concentration
            //get 1 concentration row per coordinate
            //DataTable dtConcentration = dtConcEntireRollback.DefaultView.ToTable(true, "REGIONID", "REGIONNAME", "COUNTRYID", "COUNTRYNAME", "COORDID", "CONCENTRATION", "CONCENTRATION_FINAL");
            //DataTable dtConcentration = dtConcEntireRollback.DefaultView.ToTable(true, "REGIONID", "COUNTRYID", "COORDID", "CONCENTRATION", "CONCENTRATION_FINAL");
            //baseline min, median, max
            result = dtConcEntireRollback.Compute("MIN(CONCENTRATION)", filter);
            baselineMin = Double.Parse(result.ToString());

            double[] concBase = Array.ConvertAll<DataRow, double>(dtConcEntireRollback.Select(filter),
                            delegate (DataRow row) { return Convert.ToDouble(row["CONCENTRATION"]); });
            baselineMedian = Median(concBase.ToList<double>());

            result = dtConcEntireRollback.Compute("MAX(CONCENTRATION)", filter);
            baselineMax = Double.Parse(result.ToString());

            //control min, median, max
            result = dtConcEntireRollback.Compute("MIN(CONCENTRATION_FINAL)", filter);
            controlMin = Double.Parse(result.ToString());

            double[] concControl = Array.ConvertAll<DataRow, double>(dtConcEntireRollback.Select(filter),
                             delegate (DataRow row) { return Convert.ToDouble(row["CONCENTRATION_FINAL"]); });
            controlMedian = Median(concControl.ToList<double>());

            result = dtConcEntireRollback.Compute("MAX(CONCENTRATION_FINAL)", filter);
            controlMax = Double.Parse(result.ToString());

            //air quality delta
            if (isRegion)
            {
                //AIR_QUALITY_DELTA is already pop weighted concentration delta at country level.
                double sumAQDelta = dtConcEntirePopWeighted.AsEnumerable().Where(x => x.Field<int>("REGIONID") == Convert.ToInt32(id))
                    .Sum(x => x.Field<double>("AIR_QUALITY_DELTA") * x.Field<double>("POPESTIMATE"));
                airQualityChange = sumAQDelta / popAffected;
                //DataTable dtAirQualityDelta = dtConcEntirePopWeighted.DefaultView.ToTable(true, "REGIONID", "COUNTRYID", "AGERANGENAME", "GENDERNAME", "POPESTIMATE", "AIR_QUALITY_DELTA");
                //result = dtAirQualityDelta.Compute("SUM(AIR_QUALITY_DELTA)", filter);
                //airQualityChange = Double.Parse(result.ToString());
                //airQualityChange = airQualityChange / popAffected;
            }
            else
            {
                DataTable dtAirQualityDelta = dtConcEntirePopWeighted.DefaultView.ToTable(true, "REGIONID", "REGIONNAME", "COUNTRYID", "COUNTRYNAME", "AIR_QUALITY_DELTA");
                result = dtAirQualityDelta.Compute("AVG(AIR_QUALITY_DELTA)", filter); //AIR_QUALITY_DELTA is already pop weighted concentration delta for each country.
                airQualityChange = Double.Parse(result.ToString());
            }

            DataRow dr = dt.NewRow();
            dr["NAME"] = name;
            dr["IS_REGION"] = isRegion;
            dr["POP_AFFECTED"] = popAffected;
            dr["AVOIDED_DEATHS"] = avoidedDeaths;
            dr["CONFIDENCE_INTERVAL"] = confidenceInterval;
            dr["PERCENT_BASELINE_MORTALITY"] = percentBaselineMortality;
            dr["DEATHS_PER_100_THOUSAND"] = deathsPer100Thousand;
            dr["AVOIDED_DEATHS_PERCENT_POP"] = avoidedDeathsPercentPop;
            dr["BASELINE_MIN"] = baselineMin;
            dr["BASELINE_MEDIAN"] = baselineMedian;
            dr["BASELINE_MAX"] = baselineMax;
            dr["CONTROL_MIN"] = controlMin;
            dr["CONTROL_MEDIAN"] = controlMedian;
            dr["CONTROL_MAX"] = controlMax;
            dr["AIR_QUALITY_CHANGE"] = airQualityChange;
            //YY: new fields
            dr["ECONOMIC_BENEFITS"] = ecoBenefit;
            dr["AVOIDED_YLL"] = yll;
            dr["CHANGE_IN_LE"] = changeLifeExp;

            dt.Rows.Add(dr);

        }


        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            mapGBD.FunctionMode = FunctionMode.ZoomIn;
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            mapGBD.FunctionMode = FunctionMode.ZoomOut;
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            mapGBD.FunctionMode = FunctionMode.Pan;
        }

        private void btnFullExtent_Click(object sender, EventArgs e)
        {
            mapGBD.ZoomToMaxExtent();
            mapGBD.FunctionMode = FunctionMode.None;
        }

        private void btnIdentify_Click(object sender, EventArgs e)
        {
            mapGBD.FunctionMode = FunctionMode.Info;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.RootFolder = Environment.SpecialFolder.txtFilePath.Text.Trim();
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtFilePath.Text = fbd.SelectedPath;

            }


        }

        private void ToggleExecuteScenariosButton()
        {
            List<DataGridViewRow> list = dgvRollbacks.Rows.Cast<DataGridViewRow>().Where(k => Convert.ToBoolean(k.Cells["colExecute"].Value) == true).ToList();
            btnExecuteRollbacks.Enabled = (list.Count > 0);
        }

        private void dgvRollbacks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvRollbacks.EndEdit();
            if ((e.RowIndex != -1) && (e.ColumnIndex != -1))
            {
                string columnName = dgvRollbacks.Columns[e.ColumnIndex].Name;

                if (columnName.Equals("colExecute", StringComparison.OrdinalIgnoreCase))
                {
                    ToggleExecuteScenariosButton();
                }
            }
        }


        private void ToggleRegionsCountries()
        {
            if (rbRegions.Checked)
            {
                tvRegions.Visible = true;
                listCountries.Visible = false;
            }
            else
            {
                tvRegions.Visible = false;
                listCountries.Visible = true;
            }

        }

        private void rbRegions_CheckedChanged(object sender, EventArgs e)
        {
            ToggleRegionsCountries();
        }

        private void rbCountries_CheckedChanged(object sender, EventArgs e)
        {
            ToggleRegionsCountries();
        }

        private void listCountries_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //if countries is visible then synchronize the check state
            //on the regions/countries tree view
            //otherwise ignore to avoid a feedback loop because the user is checking/unchecking on 
            //the regions/countries tree view and the check state will be sychronized from 
            //tvCountries_AfterCheck event. 
            if (listCountries.Visible)
            {
                //check country in regions tree view
                CountryItem item = (CountryItem)listCountries.Items[e.Index];
                TreeNode[] nodes = tvRegions.Nodes.Find(item.Id, true);
                bool IsChecked = (e.NewValue == CheckState.Checked);
                foreach (TreeNode node in nodes)
                {
                    node.Checked = IsChecked;
                    CheckParentNode(node);
                }
            }

        }

        private void btnBack3_Click(object sender, EventArgs e)
        {
            SetActivePanel(2);
        }


        private void btnNext3_Click(object sender, EventArgs e)
        {
            double d;

            //clean text boxes for numerics
            txtPercentage.Text = txtPercentage.Text.Trim();
            txtPercentageBackground.Text = txtPercentageBackground.Text.Trim();
            txtIncrement.Text = txtIncrement.Text.Trim();
            txtIncrementBackground.Text = txtIncrementBackground.Text.Trim();

            switch (cboRollbackType.SelectedIndex)
            {
                case 0: //percentage
                    if (String.IsNullOrEmpty(txtPercentage.Text))
                    {
                        MessageBox.Show("Percentage is required.");
                        txtPercentage.Focus();
                        return;
                    }
                    if (!Double.TryParse(txtPercentage.Text, out d))
                    {
                        MessageBox.Show("Percentage must be numeric.");
                        txtPercentage.Focus();
                        return;
                    }

                    if (d > 100)
                    {
                        MessageBox.Show("Percentage can not be > 100");
                        txtPercentageBackground.Focus();
                        return;
                    }
                    if (!String.IsNullOrEmpty(txtPercentageBackground.Text))
                    {
                        if (!Double.TryParse(txtPercentageBackground.Text, out d))
                        {
                            MessageBox.Show("Background must be numeric.");
                            txtPercentageBackground.Focus();
                            return;
                        }

                    }
                    break;
                case 1: //incremental
                    if (String.IsNullOrEmpty(txtIncrement.Text))
                    {
                        MessageBox.Show("Increment is required.");
                        txtIncrement.Focus();
                        return;
                    }
                    if (!Double.TryParse(txtIncrement.Text, out d))
                    {
                        MessageBox.Show("Increment must be numeric.");
                        txtIncrement.Focus();
                        return;
                    }
                    if (!String.IsNullOrEmpty(txtIncrementBackground.Text))
                    {
                        if (!Double.TryParse(txtIncrementBackground.Text, out d))
                        {
                            MessageBox.Show("Background must be numeric.");
                            txtIncrementBackground.Focus();
                            return;
                        }
                    }
                    break;
                case 2: //standard
                    if (cboStandard.SelectedIndex < 0)
                    {
                        MessageBox.Show("Standard is required.");
                        cboStandard.Focus();
                        return;
                    }
                    break;
            }
            SetActivePanel(3);
            //cboVSLStandard.SelectedIndex = 0;
        }


    }
}
