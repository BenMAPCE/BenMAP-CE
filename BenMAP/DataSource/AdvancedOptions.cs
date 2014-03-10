using System;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class AdvancedOptions : FormBase
    {
        public BaseControlGroup bcg = new BaseControlGroup();
        public MonitorDataLine mDataLine = new MonitorDataLine();
        string _method = string.Empty;
        public AdvancedOptions(string Method, MonitorAdvance mAdvance)
        {
            InitializeComponent();
            _method = Method;
            if (mAdvance != null)
            {
                _myMonitorAdvance = mAdvance;
            }
        }

        private MonitorAdvance _myMonitorAdvance;

        public MonitorAdvance MyMonitorAdvance
        {
            get { return _myMonitorAdvance; }
            set { _myMonitorAdvance = value; }
        }

        private void AdvancedOptions_Load(object sender, EventArgs e)
        {
            try
            {
                if (_myMonitorAdvance != null)
                {
                    if (_myMonitorAdvance.MaxinumNeighborDistance != -1) txtMaximumNeighborDistance.Text = _myMonitorAdvance.MaxinumNeighborDistance.ToString();
                    if (_myMonitorAdvance.RelativeNeighborDistance != -1) txtMaximumRelativeNeighborDistance.Text = _myMonitorAdvance.RelativeNeighborDistance.ToString();
                }
                switch (_method)
                {
                    case "closestmonitor":
                        txtMaximumNeighborDistance.Enabled = true;
                        txtMaximumRelativeNeighborDistance.Text = "- No Maximum Relative Distance -";
                        txtMaximumRelativeNeighborDistance.Enabled = false;
                        rbtnInverseDistance.Enabled = false;
                        rbtnInverseDistanceSquared.Enabled = false;
                        chbGetClosest.Enabled = false;
                        btnCustomMonitor.Enabled = false;
                        break;
                    case "voronoineighborhoodaveraging":
                        txtMaximumNeighborDistance.Enabled = true;
                        txtMaximumRelativeNeighborDistance.Enabled = true;
                        if (_myMonitorAdvance.WeightingApproach == WeightingApproachEnum.InverseDistanceSquared)
                            rbtnInverseDistanceSquared.Checked = true;
                        chbGetClosest.Enabled = false;
                        btnCustomMonitor.Enabled = false;
                        break;
                    case "fixedradius":
                        txtMaximumNeighborDistance.Text = "- No Maximum Distance -";
                        txtMaximumNeighborDistance.Enabled = false;
                        txtMaximumRelativeNeighborDistance.Text = "- No Maximum Relative Distance -";
                        txtMaximumRelativeNeighborDistance.Enabled = false;
                        btnCustomMonitor.Enabled = false;
                        if (_myMonitorAdvance.WeightingApproach == WeightingApproachEnum.InverseDistanceSquared)
                            rbtnInverseDistanceSquared.Checked = true;
                        break;
                }
                if (CommonClass.MainSetup.SetupID == 1 && mDataLine.MonitorDirectType == 0)
                {
                    btnCustomMonitor.Enabled = true;
                }
                else
                {
                    btnCustomMonitor.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
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
                double value = 0.0;
                bool ok = false;
                switch (_method)
                {
                    case "closestmonitor":
                        ok = double.TryParse(txtMaximumNeighborDistance.Text, out value);
                        if (ok && Convert.ToDouble(txtMaximumNeighborDistance.Text) > 0)
                        {
                            _myMonitorAdvance.MaxinumNeighborDistance = value;
                            _myMonitorAdvance.RelativeNeighborDistance = -1;
                        }
                        break;
                    case "voronoineighborhoodaveraging":
                        ok = double.TryParse(txtMaximumNeighborDistance.Text, out value);
                        if (ok && Convert.ToDouble(txtMaximumNeighborDistance.Text) > 0)
                        {
                            _myMonitorAdvance.MaxinumNeighborDistance = value;
                        }
                        ok = double.TryParse(txtMaximumRelativeNeighborDistance.Text, out value);
                        if (ok && Convert.ToDouble(txtMaximumRelativeNeighborDistance.Text) >= 1) { _myMonitorAdvance.RelativeNeighborDistance = value; }
                        else if (ok && Convert.ToDouble(txtMaximumRelativeNeighborDistance.Text) < 1)
                        {
                            MessageBox.Show("The value of 'Maximum Relative Distance' must be greater than or equal to 1.");
                            return;
                        }
                        break;
                    case "fixedradius":
                        _myMonitorAdvance.MaxinumNeighborDistance = -1;
                        _myMonitorAdvance.RelativeNeighborDistance = -1;
                        if (chbGetClosest.Checked) { _myMonitorAdvance.GetClosedIfNoneWithinRadius = true; }
                        else { _myMonitorAdvance.GetClosedIfNoneWithinRadius = false; }
                        break;
                }
                WeightingApproachEnum wae = WeightingApproachEnum.InverseDistance;
                if (rbtnInverseDistance.Checked) { wae = WeightingApproachEnum.InverseDistance; }
                if (rbtnInverseDistanceSquared.Checked) { wae = WeightingApproachEnum.InverseDistanceSquared; }
                if (_myMonitorAdvance.IncludeMethods == null)
                {
                    if (CommonClass.MainSetup.SetupID == 1)
                    {
                        switch (bcg.Pollutant.PollutantName)
                        {
                            case "PM2.5":
                                _myMonitorAdvance.FilterMaximumPOC = 4;
                                _myMonitorAdvance.POCPreferenceOrder = "1,2,3,4";
                                break;
                            case "PM10":
                                _myMonitorAdvance.FilterMaximumPOC = 4;
                                _myMonitorAdvance.POCPreferenceOrder = "1,2,3,4";
                                break;
                            case "Ozone":
                                _myMonitorAdvance.FilterMaximumPOC = 4;
                                _myMonitorAdvance.POCPreferenceOrder = "1,2,3,4";
                                break;
                            case "NO2":
                                _myMonitorAdvance.FilterMaximumPOC = 9;
                                _myMonitorAdvance.POCPreferenceOrder = "1,2,3,4,5,6,7,8,9";
                                break;
                            case "SO2":
                                _myMonitorAdvance.FilterMaximumPOC = 9;
                                _myMonitorAdvance.POCPreferenceOrder = "1,2,3,4,5,6,7,8,9";
                                break;
                        }
                    }
                }
                _myMonitorAdvance.WeightingApproach = wae;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void btnCustomMonitor_Click(object sender, EventArgs e)
        {
            FilterMonitors frm = new FilterMonitors();
            frm.MonitorAdvanceFilter = MyMonitorAdvance;
            frm.bcg = bcg;
            frm.mDataLine = mDataLine;
            DialogResult rtn = frm.ShowDialog();
            if (rtn == DialogResult.OK) { MyMonitorAdvance = frm.MonitorAdvanceFilter; }
        }

        private void txtMaximumNeighborDistance_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (bool)txtMaximumNeighborDistance.Tag == true)
            {
                txtMaximumNeighborDistance.SelectAll();
            }
            txtMaximumNeighborDistance.Tag = false;
        }

        private void txtMaximumNeighborDistance_Enter(object sender, EventArgs e)
        {
            txtMaximumNeighborDistance.Tag = true; txtMaximumNeighborDistance.SelectAll();
        }

        private void txtMaximumRelativeNeighborDistance_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (bool)txtMaximumRelativeNeighborDistance.Tag == true)
            {
                txtMaximumRelativeNeighborDistance.SelectAll();
            }
            txtMaximumRelativeNeighborDistance.Tag = false;
        }

        private void txtMaximumRelativeNeighborDistance_Enter(object sender, EventArgs e)
        {
            txtMaximumRelativeNeighborDistance.Tag = true;
            txtMaximumRelativeNeighborDistance.SelectAll();
        }

        private void txtMaximumNeighborDistance_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)8)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == '.')
            {
                if (((TextBox)sender).Text.Trim().IndexOf('.') > -1)
                    e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtMaximumRelativeNeighborDistance_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)8)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == '.')
            {
                if (((TextBox)sender).Text.Trim().IndexOf('.') > -1)
                    e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}