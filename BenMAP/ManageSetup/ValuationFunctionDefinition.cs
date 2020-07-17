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
    public partial class ValuationFunctionDefinition : FormBase
    {
        private string _dataName;
        public ValuationFunctionDefinition()
        {
            InitializeComponent();
            _dataName = string.Empty;
        }
        public ValuationFunctionDefinition(string EndpointGroup, string Endpoint, string Qualifier, string Reference, int StartAge, int EndAge, string Function, float A, string NameA, string ADistribution, float P1A, float P2A, float B, string NameB, float C, string NameC, float D, string NameD, List<double> listValue)
        {
            InitializeComponent();
            _dataName = EndpointGroup;
            _enpoint = Endpoint;
            _qualifier = Qualifier;
            _reference = Reference;
            _startAge = StartAge;
            _endAge = EndAge;
            _function = Function;
            _a = A;
            _aDescription = NameA;
            _aDistribution = ADistribution;
            _aParameter1 = P1A;
            _aParameter2 = P2A;
            _b = B;
            _bName = NameB;
            _c = C;
            _cName = NameC;
            _d = D;
            _dName = NameD;
            _listCustom = listValue;
        }
        private string _endpointGroup;
        public string EndpointGroup
        {
            get { return _endpointGroup; }
            set { _endpointGroup = value; }
        }

        private string _enpoint;
        public string Endpoint
        {
            get { return _enpoint; }
            set { _enpoint = value; }
        }

        private string _qualifier;
        public string Qualifier
        {
            get { return _qualifier; }
            set { _qualifier = value; }
        }

        private string _reference;
        public string Reference
        {
            get { return _reference; }
            set { _reference = value; }
        }

        private int _startAge;
        public int StartAge
        {
            get { return _startAge; }
            set { _startAge = value; }
        }

        private int _endAge;
        public int EndAge
        {
            get { return _endAge; }
            set { _endAge = value; }
        }

        private string _function;
        public string Function
        {
            get { return _function; }
            set { _function = value; }
        }

        private string _aDescription;
        public string ADescription
        {
            get { return _aDescription; }
            set { _aDescription = value; }
        }

        private float _a;
        public float A
        {
            get { return _a; }
            set { _a = value; }
        }

        private string _aDistribution;
        public string ADistribution
        {
            get { return _aDistribution; }
            set { _aDistribution = value; }
        }

        private float _aParameter1;
        public float AParameter1
        {
            get { return _aParameter1; }
            set { _aParameter1 = value; }
        }

        private float _aParameter2;
        public float AParameter2
        {
            get { return _aParameter2; }
            set { _aParameter2 = value; }
        }

        private float _b;
        public float B
        {
            get { return _b; }
            set { _b = value; }
        }

        private string _bName;
        public string BName
        {
            get { return _bName; }
            set { _bName = value; }
        }

        private float _c;
        public float C
        {
            get { return _c; }
            set { _c = value; }
        }

        private string _cName;
        public string CName
        {
            get { return _cName; }
            set { _cName = value; }
        }

        private float _d;
        public float D
        {
            get { return _d; }
            set { _d = value; }
        }

        private string _dName;
        public string DName
        {
            get { return _dName; }
            set { _dName = value; }
        }

        private List<double> _listCustom;
        public List<double> listCustom
        {
            get { return _listCustom; }
            set { _listCustom = value; }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet dsValueFunctions = new DataSet();
                string commandText = "select * from VALUATIONFUNCTIONALFORMS";
                dsValueFunctions = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                List<string> listValueFunctions = new List<string>();
                int rowFunctionsCount = dsValueFunctions.Tables[0].Rows.Count;
                for (int i = 0; i < rowFunctionsCount; i++)
                {
                    listValueFunctions.Add(dsValueFunctions.Tables[0].Rows[i][1].ToString());
                }
                List<string> lstSystemVariableName = Configuration.ConfigurationCommonClass.getAllSystemVariableNameList();
                Dictionary<string, double> dicVariable = new Dictionary<string, double>();
                foreach (string s in lstSystemVariableName)
                {
                    dicVariable.Add(s, 1);
                }
                string valueFunctionText = APVX.APVCommonClass.getFunctionStringFromDatabaseFunction(txtFunction.Text);
                double valueFunctionResult = APVX.APVCommonClass.getValueFromValuationFunctionString(valueFunctionText, 1, 1, 1, 1, 1, 1, 1, 1, dicVariable);
                if (txtFunction.Text == string.Empty || valueFunctionResult == -999999999.0)
                {
                    MessageBox.Show("Please input a valid value for 'Function'");
                    return;
                }
                else
                {
                    if (!listValueFunctions.Contains(txtFunction.Text))
                    {
                        commandText = "select coalesce(max(FUNCTIONALFORMID),1) from VALUATIONFUNCTIONALFORMS";
                        object objFunction = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        int valueFunctionsID = int.Parse(objFunction.ToString()) + 1;
                        commandText = string.Format("insert into ValuationFunctionalForms values ({0},'{1}')", valueFunctionsID, txtFunction.Text);
                        int rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }
                }
                if (txtA.Text == string.Empty)
                {
                    MessageBox.Show("'A' can not be null. Please input a valid value.");
                    return;
                }
                if (txtAParameter1.Text == string.Empty)
                {
                    MessageBox.Show("'A Parameter 1' can not be null. Please input a valid value.");
                    return;
                }
                if (txtAParameter2.Text == string.Empty)
                {
                    MessageBox.Show("'A Parameter 2' can not be null. Please input a valid value.");
                    return;
                }
                if (txtBValue.Text == string.Empty)
                {
                    MessageBox.Show("'B' can not be null. Please input a valid value.");
                    return;
                }
                if (txtCValue.Text == string.Empty)
                {
                    MessageBox.Show("'C' can not be null. Please input a valid value.");
                    return;
                }
                if (txtDValue.Text == string.Empty)
                {
                    MessageBox.Show("'D' can not be null. Please input a valid value.");
                    return;
                }
                _endpointGroup = cboEndpointGroup.Text;
                _enpoint = cboEndpoint.Text;
                _qualifier = txtQualifier.Text;
                _reference = txtReference.Text;
                if (nudownStartAge.Value > nudownEndAge.Value)
                {
                    MessageBox.Show("End age must be larger than start age.");
                    return;
                }
                _startAge = int.Parse(nudownStartAge.Value.ToString());
                _endAge = int.Parse(nudownEndAge.Value.ToString());
                _function = txtFunction.Text;
                _aDescription = txtADescription.Text;
                _a = float.Parse(txtA.Text);
                _aDistribution = cboADistribution.SelectedItem.ToString();
                _aParameter1 = float.Parse(txtAParameter1.Text);
                _aParameter2 = float.Parse(txtAParameter2.Text);
                _bName = txtBDescription.Text;
                _b = float.Parse(txtBValue.Text);
                _cName = txtCDescription.Text;
                _c = float.Parse(txtCValue.Text);
                _dName = txtDDescription.Text;
                _d = float.Parse(txtDValue.Text);
                _listCustom = list;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }
        private void ValuationFunctionDefinition_Load(object sender, EventArgs e)
        {
            try
            {
                if (_dataName != string.Empty)
                {
                    BindItems();
                    cboADistribution.Items.Add("None");
                    cboADistribution.Items.Add("Normal");
                    cboADistribution.Items.Add("Triangular");
                    cboADistribution.Items.Add("Poisson");
                    cboADistribution.Items.Add("Binomial");
                    cboADistribution.Items.Add("LogNormal");
                    cboADistribution.Items.Add("Uniform");
                    cboADistribution.Items.Add("Exponential");
                    cboADistribution.Items.Add("Geometric");
                    cboADistribution.Items.Add("Weibull");
                    cboADistribution.Items.Add("Gamma");
                    cboADistribution.Items.Add("Logistic");
                    cboADistribution.Items.Add("Beta");
                    cboADistribution.Items.Add("Pareto");
                    cboADistribution.Items.Add("Cauchy");
                    cboADistribution.Items.Add("Custom");
                    cboADistribution.SelectedIndex = 0;
                    cboEndpointGroup.Text = _dataName;
                    cboEndpoint.Text = Endpoint;
                    txtQualifier.Text = Qualifier;
                    txtReference.Text = Reference;
                    txtFunction.Text = Function;
                    txtA.Text = A.ToString();
                    txtADescription.Text = ADescription;
                    txtBValue.Text = B.ToString();
                    txtBDescription.Text = BName;
                    txtCValue.Text = C.ToString();
                    txtCDescription.Text = CName;
                    txtDValue.Text = D.ToString();
                    txtDDescription.Text = DName;
                    txtAParameter1.Text = AParameter1.ToString();
                    txtAParameter2.Text = AParameter2.ToString();
                    cboADistribution.Text = ADistribution;
                    nudownStartAge.Value = StartAge;
                    nudownEndAge.Value = EndAge;
                    list = listCustom;
                }
                else
                {
                    BindItems();
                    cboADistribution.Items.Add("None");
                    cboADistribution.Items.Add("Normal");
                    cboADistribution.Items.Add("Triangular");
                    cboADistribution.Items.Add("Poisson");
                    cboADistribution.Items.Add("Binomial");
                    cboADistribution.Items.Add("LogNormal");
                    cboADistribution.Items.Add("Uniform");
                    cboADistribution.Items.Add("Exponential");
                    cboADistribution.Items.Add("Geometric");
                    cboADistribution.Items.Add("Weibull");
                    cboADistribution.Items.Add("Gamma");
                    cboADistribution.Items.Add("Logistic");
                    cboADistribution.Items.Add("Beta");
                    cboADistribution.Items.Add("Pareto");
                    cboADistribution.Items.Add("Cauchy");
                    cboADistribution.Items.Add("Custom");
                    cboADistribution.SelectedIndex = 0;
                }
                cboADistribution.SelectedValueChanged -= cboADistribution_SelectedValueChanged;
                cboADistribution.SelectedValueChanged += cboADistribution_SelectedValueChanged;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public void BindItems()
        {
            string commandText = "select ENDPOINTGROUPNAME from ENDPOINTGROUPS ";
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
            cboEndpointGroup.DataSource = ds.Tables[0];
            cboEndpointGroup.DisplayMember = "ENDPOINTGROUPNAME";
            cboEndpointGroup.SelectedIndex = 0;
            int maxEndpointGroupWidth = 189;
            int EndpointGroupWidth = 189;
            for (int i = 0; i < cboEndpointGroup.Items.Count; i++)
            {
                using (Graphics g = this.CreateGraphics())
                {
                    SizeF string_size = g.MeasureString(cboEndpointGroup.GetItemText(cboEndpointGroup.Items[i]), this.Font);
                    EndpointGroupWidth = Convert.ToInt16(string_size.Width) + 50;
                }
                maxEndpointGroupWidth = Math.Max(maxEndpointGroupWidth, EndpointGroupWidth);
            }
            cboEndpointGroup.DropDownWidth = maxEndpointGroupWidth;

            string[] composeFunction = new string[] { "ABS(x)", "EXP(x)", "LOG(x)", "POW(x,y)", "SQR(x)", "ACOS(x)", "ASIN(x)", "ATAN(x)", "ATAN2(x,y)", "BIGMUL(x,y)", "CEILING(x)", "COS(x)", "COSH(x)", "DIVREM(x,y,z)", "FLOOR(x)", "IEEEREMAINDER(x,y)", "LOG10(x)", "MAX(x,y)", "MIN(x,y)", "ROUND(x,y)", "SIGN(x)", "SIN(x)", "SINH(x)", "TAN(x)", "TANH(x)", "TRUNCATE(x)" };
            lstComposeF.Items.AddRange(composeFunction);
            lstComposeF.SelectedIndex = -1;
            string[] availableVariables = new string[] { "A", "B", "C", "D", "AllGoodsIndex", "MedicalCostIndex", "WageIndex", "LagAdjustment" };
            lstComposeVariables.Items.AddRange(availableVariables);
            lstComposeVariables.SelectedIndex = -1;
            commandText = string.Format("select distinct lower(SetupVariableName) as SetupVariableName from SetupVariables where setupvariabledatasetid in (select setupvariabledatasetid from setupvariabledatasets where setupid={0})", CommonClass.ManageSetup.SetupID);
            ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
            lstComposeSetupVariables.DataSource = ds.Tables[0];
            lstComposeSetupVariables.DisplayMember = "SetupVariableName";
            lstComposeSetupVariables.SelectedIndex = -1;
            commandText = string.Format("select FUNCTIONALFORMTEXT from VALUATIONFUNCTIONALFORMS");
            ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
            lstExistingF.DataSource = ds.Tables[0];
            lstExistingF.DisplayMember = "FUNCTIONALFORMTEXT";
            lstExistingF.SelectedIndex = -1;
            string[] availableCompiledFunction = new string[] { "AllGoodsIndex", "A*AllGoodsIndex", "A*MedicalCostIndex", "A*B*AllGoodsIndex", "A*B*C*AllGoodsIndex", "A*MedicalCostIndex+B*WageIndex", "A*EXP(-B*(13-C))*AllGoodsIndex", "A*EXP(-B*12)*AllGoodsIndex", "A*MedicalCostIndex+B*(median_income/(52*5))*WageIndex", "A*WageIndex", "median_income*WageIndex" };
            lstExistingComposeF.Items.AddRange(availableCompiledFunction);
            lstExistingComposeF.SelectedIndex = -1;
        }
        private void cboEndpointGroup_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                string str = cboEndpointGroup.Text;
                string commandText = string.Format("select * from ENDPOINTS where ENDPOINTGROUPID=(select ENDPOINTGROUPID from ENDPOINTGROUPS where ENDPOINTGROUPNAME='{0}' )", str);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboEndpoint.DataSource = ds.Tables[0];
                if (cboEndpoint.Items.Count != 0)
                {
                    cboEndpoint.DisplayMember = "ENDPOINTNAME";
                    cboEndpoint.SelectedItem = cboEndpoint.Items[0];
                }
                int maxEndpointWidth = 195;
                int EndpointWidth = 195;
                for (int i = 0; i < cboEndpoint.Items.Count; i++)
                {
                    using (Graphics g = this.CreateGraphics())
                    {
                        SizeF string_size = g.MeasureString(cboEndpoint.GetItemText(cboEndpoint.Items[i]), this.Font);
                        EndpointWidth = Convert.ToInt16(string_size.Width) + 50;
                    }
                    maxEndpointWidth = Math.Max(maxEndpointWidth, EndpointWidth);
                }
                cboEndpoint.DropDownWidth = maxEndpointWidth;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void txtAParameter1_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtAParameter2_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtBValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtCValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtDValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtA_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void lstExistingF_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = txtFunction.SelectionStart;
                txtFunction.Text = txtFunction.Text.Insert(index, lstExistingF.Text);
                txtFunction.SelectionStart = txtFunction.Text.Length;
                txtFunction.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstExistingComposeF_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = txtFunction.SelectionStart;
                txtFunction.Text = txtFunction.Text.Insert(index, lstExistingComposeF.Text);
                txtFunction.SelectionStart = txtFunction.Text.Length;
                txtFunction.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstComposeVariables_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = txtFunction.SelectionStart;
                txtFunction.Text = txtFunction.Text.Insert(index, lstComposeVariables.Text);
                txtFunction.SelectionStart = txtFunction.Text.Length;
                txtFunction.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstComposeSetupVariables_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = txtFunction.SelectionStart;
                txtFunction.Text = txtFunction.Text.Insert(index, lstComposeSetupVariables.Text);
                txtFunction.SelectionStart = txtFunction.Text.Length;
                txtFunction.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstComposeF_DoubleClick(object sender, EventArgs e)
        {
            Dictionary<string, string> dicComposeF = new Dictionary<string, string>();
            dicComposeF.Add("ABS(x)", "ABS()");
            dicComposeF.Add("EXP(x)", "EXP()");
            dicComposeF.Add("LOG(x)", "LOG()");
            dicComposeF.Add("POW(x,y)", "POW(,)");
            dicComposeF.Add("SQR(x)", "SQR()");
            dicComposeF.Add("ACOS(x)", "ACOS()");
            dicComposeF.Add("ASIN(x)", "ASIN()");
            dicComposeF.Add("ATAN(x)", "ATAN()");
            dicComposeF.Add("ATAN2(x,y)", "ATAN2(,)");
            dicComposeF.Add("BIGMUL(x,y)", "BIGMUL(,)");
            dicComposeF.Add("CEILING(x)", "CEILING()");
            dicComposeF.Add("COS(x)", "COS()");
            dicComposeF.Add("COSH(x)", "COSH()");
            dicComposeF.Add("DIVREM(x,y,z)", "DIVREM(,,)");
            dicComposeF.Add("FLOOR(x)", "FLOOR()");
            dicComposeF.Add("IEEEREMAINDER(x,y)", "IEEEREMAINDER(,)");
            dicComposeF.Add("LOG10(x)", "LOG10()");
            dicComposeF.Add("MAX(x,y)", "MAX(,)");
            dicComposeF.Add("MIN(x,y)", "MIN(,)");
            dicComposeF.Add("ROUND(x,y)", "ROUND(,)");
            dicComposeF.Add("SIGN(x)", "SIGN()");
            dicComposeF.Add("SIN(x)", "SIN()");
            dicComposeF.Add("SINH(x)", "SINH()");
            dicComposeF.Add("TAN(x)", "TAN()");
            dicComposeF.Add("TANH(x)", "TANH()");
            dicComposeF.Add("TRUNCATE(x)", "TRUNCATE()");
            string insert = lstComposeF.SelectedItem.ToString();
            string insertFunction = dicComposeF[insert];
            int index = txtFunction.SelectionStart;
            txtFunction.Text = txtFunction.Text.Insert(index, insertFunction);
            txtFunction.SelectionStart = txtFunction.Text.Length;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        List<double> list = new List<double>();
        private void cboADistribution_SelectedValueChanged(object sender, EventArgs e)
        {
            HealthImpact healthImpactValues = new HealthImpact();
            healthImpactValues.BetaDistribution = cboADistribution.SelectedItem.ToString();
            healthImpactValues.Beta = txtA.Text;
            healthImpactValues.BetaParameter1 = txtAParameter1.Text;
            healthImpactValues.BetaParameter2 = txtAParameter2.Text;
            if (cboADistribution.SelectedItem == "None") { return; }
            if (cboADistribution.SelectedItem == "Custom")
            {
                if (list.Count == 0)
                {
                    CustomDistributionEntries frm = new CustomDistributionEntries();
                    DialogResult rtn = frm.ShowDialog();
                    if (rtn != DialogResult.OK) { return; }
                    list = frm.list;
                }
                else
                {
                    CustomDistributionEntries frmCustom = new CustomDistributionEntries(list);
                    DialogResult rtnCustom = frmCustom.ShowDialog();
                    if (rtnCustom != DialogResult.OK) { return; }
                    list = frmCustom.list;
                }
            }
            else
            {
                EditDistributionValues frm = new EditDistributionValues(cboADistribution.SelectedItem.ToString(), healthImpactValues);
                DialogResult rtn = frm.ShowDialog();
                if (rtn != DialogResult.OK) { return; }
                txtA.Text = frm.MeanValue;
                txtAParameter1.Text = frm.Parameter1;
                txtAParameter2.Text = frm.Parameter2;
                if (cboADistribution.SelectedItem == "Normal" || cboADistribution.SelectedItem == "Poisson" || cboADistribution.SelectedItem == "Exponential" || cboADistribution.SelectedItem == "Geometric")
                {
                    txtAParameter2.Text = healthImpactValues.BetaParameter2;
                }
            }
        }
    }
}
