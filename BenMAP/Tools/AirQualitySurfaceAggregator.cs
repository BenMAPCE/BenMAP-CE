using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESIL.DBUtility;

namespace BenMAP
{
    public partial class AirQualitySurfaceAggregation : FormBase
    {
        public AirQualitySurfaceAggregation()
        {
            InitializeComponent();
        }

        private void AirQualitySurfaceAggregator_Load(object sender, EventArgs e)
        {
            try
            {
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = "Select GridDefinitionID,GridDefinitionName,TType from GridDefinitions where SetupID=" + CommonClass.ManageSetup.SetupID + "";
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                cboAggregationSurface.DataSource = ds.Tables[0];
                cboAggregationSurface.DisplayMember = "GridDefinitionName";
                cboAggregationSurface.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
                openFile.Filter = "Air Quality Surface(*.aqgx)|*.aqgx";
                openFile.FilterIndex = 1;
                openFile.RestoreDirectory = true;
                if (openFile.ShowDialog() != DialogResult.OK)
                { return; }
                txtAirQualitySurface.Text = openFile.FileName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                List<ModelResultAttribute> lstResult = new List<ModelResultAttribute>();
                BenMAPLine inputBenMAPLine = new BenMAPLine();
                List<ModelResultAttribute> outModelResultAttributes = new List<ModelResultAttribute>();
                List<Dictionary<string, float>> lstBig = new List<Dictionary<string, float>>();
                List<ModelResultAttribute> lstSmall = new List<ModelResultAttribute>();
                string err = "";
                inputBenMAPLine = DataSourceCommonClass.LoadAQGFile(txtAirQualitySurface.Text,ref err);
                if (inputBenMAPLine == null)
                {
                    MessageBox.Show(err);
                    return;
                }
                DataRowView drOutput = (cboAggregationSurface.SelectedItem) as DataRowView;
                 
                int outputGridDefinitionID = Convert.ToInt32(drOutput["GridDefinitionID"]);
                if (inputBenMAPLine.GridType.GridDefinitionID == outputGridDefinitionID)
                {
                    MessageBox.Show("The AQG file you want to aggregate is already at the spatial resolution of the selected aggregation surface.");
                    return;
                }

                //Save
                SaveFileDialog saveOutAQG = new SaveFileDialog();
                saveOutAQG.Filter = "aqgx file|*.aqgx";
                //saveOutAQG.FilterIndex = 2;
                saveOutAQG.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
                saveOutAQG.RestoreDirectory = true;
                if (saveOutAQG.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                WaitShow("Aggregating air quality surface...");
                string filePath = saveOutAQG.FileName.Substring(0, saveOutAQG.FileName.LastIndexOf(@"\") + 1);
                string fileName = saveOutAQG.FileName.Substring(saveOutAQG.FileName.LastIndexOf(@"\") + 1).Replace("aqgx", "shp");

                int GridFrom = inputBenMAPLine.GridType.GridDefinitionID;
                if (GridFrom == 28) GridFrom = 27;
                int GridTo = outputGridDefinitionID;
                if (GridTo == 28) GridTo = 27;
                if (GridFrom == GridTo)
                {
                    // out to --修正GridDefiniton即可
                    outModelResultAttributes = inputBenMAPLine.ModelResultAttributes;
                }
                else
                {
                    string str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", GridFrom, GridTo);
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    DataSet ds;//
                    int iCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select count(*) from (" + str + " ) a"));
                    if (iCount == 0)
                    {

                        Configuration.ConfigurationCommonClass.creatPercentageToDatabase(GridTo, GridFrom);
                        iCount = 1;

                    }
                    if (iCount != 0)
                    {
                        Dictionary<string, Dictionary<string, double>> dicRelationShip = new Dictionary<string, Dictionary<string, double>>();

                        ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);

                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            if (dicRelationShip.ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
                            {
                                if (!dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                                    dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                            }
                            else
                            {
                                dicRelationShip.Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), new Dictionary<string, double>());
                                dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                            }

                        }

                        ds.Dispose();
                        //--------Aggregation!--
                        Dictionary<string, ModelResultAttribute> dicModelFrom = new Dictionary<string, ModelResultAttribute>();
                        foreach (ModelResultAttribute mo in inputBenMAPLine.ModelResultAttributes)
                        {
                            dicModelFrom.Add(mo.Col + "," + mo.Row, mo);
                        }
                        foreach (KeyValuePair<string, Dictionary<string, double>> gra in dicRelationShip)
                        {
                            if (gra.Value == null || gra.Value.Count == 0) continue;
                            ModelResultAttribute anew = new ModelResultAttribute();
                            anew.Col = Convert.ToInt32(gra.Key.Split(new char[] { ',' }).ToArray()[0]);
                            anew.Row = Convert.ToInt32(gra.Key.Split(new char[] { ',' }).ToArray()[1]);
                            anew.Values = new Dictionary<string, float>();
                            Dictionary<string, float> dicValue = new Dictionary<string, float>();

                            //if (dicModelFrom.ContainsKey(gra.Value.First().Key))
                            //{
                            //    foreach (KeyValuePair<string, float> k in dicModelFrom[gra.Value.First().Key].Values)
                            //    {
                            //        anew.Values.Add(k.Key, Convert.ToSingle(k.Value * gra.Value.First().Value));

                            //    }
                            //}
                            for (int i = 0; i < gra.Value.Count; i++)
                            {
                                if (dicModelFrom.ContainsKey(gra.Value.ToList()[i].Key))
                                {
                                    foreach (KeyValuePair<string, float> k in dicModelFrom[gra.Value.ToList()[i].Key].Values)
                                    {
                                        if (!anew.Values.ContainsKey(k.Key))
                                            anew.Values.Add(k.Key, Convert.ToSingle(k.Value * gra.Value.ToList()[i].Value));
                                        else
                                            anew.Values[k.Key] += Convert.ToSingle(k.Value * gra.Value.ToList()[i].Value);

                                    }
                                }

                            }
                            
                            if (anew.Values != null && anew.Values.Count > 0)
                            {
                                List<string> lstKey = anew.Values.Keys.ToList();
                                foreach (string k in lstKey)
                                {
                                    anew.Values[k] = Convert.ToSingle(anew.Values[k] / gra.Value.Sum(p => p.Value));

                                }
                            }
                            outModelResultAttributes.Add(anew);

                        }
                    }

                }
                 
                
                //  inputBenMAPLine.GridType = Grid.GridCommon.getBenMAPGridFromID(inputGridDefinitionID);

                //if (inputBenMAPLine.GridType.GridDefinitionID == gRegionGridRelationship.bigGridID)//*.aqg 文件比较大
                //{
                //    foreach (GridRelationshipAttribute gra in gRegionGridRelationship.lstGridRelationshipAttribute)
                //    {

                //        foreach (RowCol rc in gra.smallGridRowCol)
                //        {
                //            outModelResultAttributes.Add(new ModelResultAttribute()
                //            {
                //                Col = rc.Col,
                //                Row = rc.Row
                //                //Values= inputBenMAPLine.ModelResultAttributes.Where(p=>p.Col==gra.bigGridRowCol.Col && p.Row==gra.bigGridRowCol.Row).First().Values;
                //            });
                //            lstBig = inputBenMAPLine.ModelResultAttributes.Where(p => p.Col == gra.bigGridRowCol.Col && p.Row == gra.bigGridRowCol.Row).Select(a => a.Values).ToList();
                //            if (lstBig.Count > 0)
                //            {
                //                outModelResultAttributes[outModelResultAttributes.Count - 1].Values = lstBig.First();

                //            }
                //            //先处理dictionary    
                //            //foreach (ModelResultAttribute mra in inputAQG.ModelResultAttributes)
                //            //{

                //            //}
                //            //lstResult.Add(new ModelResultAttribute()
                //            //{
                //            //    Col=rc.Col,Row=rc.Row,
                //            //    Values=
                //            //});                           
                //        }
                //    }
                //}
                //else if (inputBenMAPLine.GridType.GridDefinitionID == gRegionGridRelationship.smallGridID)
                //{
                //    foreach (GridRelationshipAttribute gra in gRegionGridRelationship.lstGridRelationshipAttribute)
                //    {
                //        lstSmall = inputBenMAPLine.ModelResultAttributes.Where(p => gra.smallGridRowCol.Contains(new RowCol() { Row = p.Row, Col = p.Col }, new RowColComparer())).ToList();
                //        if (lstSmall != null && lstSmall.Count > 0)
                //        {
                //            ModelResultAttribute m = new ModelResultAttribute();
                //            m.Row = gra.bigGridRowCol.Row;
                //            m.Col = gra.bigGridRowCol.Col;
                //            m.Values = new Dictionary<string, float>();
                //            for (int i = 0; i < lstSmall.First().Values.Count; i++)
                //            {
                //                m.Values.Add(lstSmall.First().Values.Keys.ToArray()[i], lstSmall.Average(p => p.Values.Where(c => c.Key == lstSmall.First().Values.Keys.ToArray()[i]).Select(a => a.Value).Average()));
                //                //m.Values.Add(lstSmall.First().Values.Keys.ToArray()[i], lstSmall.Where(t => t.Key.ToString() == lstSmall.First().Values.Keys.ToArray()[i]).Average(p=>p.k);

                //            }
                //            outModelResultAttributes.Add(m);
                //        }

                //    }

                //}
                inputBenMAPLine.ModelResultAttributes = outModelResultAttributes;
                inputBenMAPLine.ModelAttributes = null;
                //inputBenMAPLine.ShapeFile = "";
                inputBenMAPLine.GridType = Grid.GridCommon.getBenMAPGridFromID(outputGridDefinitionID);
                
                DataSourceCommonClass.CreateAQGFromBenMAPLine(inputBenMAPLine, saveOutAQG.FileName);
                WaitClose();
                MessageBox.Show("File saved.", "File saved");
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                WaitClose();
                Logger.LogError(ex.Message);
            }

        }

        #region 等待窗口
        TipFormGIF waitMess = new TipFormGIF();//等待窗体
        bool sFlog = true;
        //--显示等待窗体 
        private void ShowWaitMess()
        {
            try
            {
                if (!waitMess.IsDisposed)
                {
                    waitMess.ShowDialog();
                }

            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }

        //--新开辟一个线程调用 
        public void WaitShow(string msg)
        {
            try
            {
                if (sFlog == true)
                {
                    sFlog = false;
                    waitMess.Msg = msg;
                    System.Threading.Thread upgradeThread = null;
                    upgradeThread = new System.Threading.Thread(new System.Threading.ThreadStart(ShowWaitMess));
                    upgradeThread.Start();
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }
        private delegate void CloseFormDelegate();

        //--关闭等待窗体 
        public void WaitClose()
        {
            //同步到主线程上
            if (waitMess.InvokeRequired)
                waitMess.Invoke(new CloseFormDelegate(DoCloseJob));
            else
                DoCloseJob();
        }

        private void DoCloseJob()
        {
            try
            {
                if (!waitMess.IsDisposed)
                {
                    if (waitMess.Created)
                    {
                        sFlog = true;
                        waitMess.Close();
                    }
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }
        #endregion 等待窗口

    }
}
