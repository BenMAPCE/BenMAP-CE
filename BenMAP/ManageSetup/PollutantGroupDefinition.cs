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
    public partial class PollutantGroupDefinition : FormBase
    {

        public string _pollutantGroupName = "";
        public string _pollutantGroupDesc = "";
        public object _pollutantGroupID;
        public List<int> _pollutantList = new List<int>();
        private bool _isNewPollutantGroup;

        private class PollutantItem
        {
            int _id;
            string _name;

            public PollutantItem(int Id, string Name)
            {
                _id = Id;
                _name = Name;
            }

            public int Id
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

        public PollutantGroupDefinition()
        {
            InitializeComponent();
        }

        private void PollutantGroupDefinition_Load(object sender, EventArgs e)
        {
            try
            {
                LoadPollutants();
                if(_pollutantGroupID != null)
                {
                    // We are editing a group
                    textGroupName.Text = _pollutantGroupName;
                    textGroupDescription.Text = _pollutantGroupDesc;
                    _isNewPollutantGroup = false;
                    if(PollutantGroupIsUsedByHIF())
                    {
                        chkPollutants.Enabled = false;
                    }
                } else
                {
                    // We are creating a new group
                    _isNewPollutantGroup = true;
                }


            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private bool PollutantGroupIsUsedByHIF()
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();

            string commandText = "select CRFunctionID from CRFunctions where PollutantGroupID=" + _pollutantGroupID + "";
            DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
            if (ds.Tables[0].Rows.Count != 0)
            {
                MessageBox.Show("This pollutant group is used in one or more Health Impact Functions so the pollutant list cannot be modified. You may edit the name and description if desired.");
                return true;
            }

            return false;
        }

        // Populate with chkPollutants with a list of all pollutants in the current setup
        // If the user is editing an existing group, select those that are currently part of the group
        private void LoadPollutants()
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string  commandText = string.Format("select pollutantname,pollutantID from pollutants where setupid={0} order by pollutantname asc", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    int id = Convert.ToInt32(dr["pollutantID"]);
                    string name = dr["pollutantname"].ToString();
                    chkPollutants.Items.Add(new PollutantItem(id, name), _pollutantList.Contains(id));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            string commandText = String.Empty;

            // Make sure we have a name entered
            if(String.IsNullOrWhiteSpace(textGroupName.Text))
            {
                MessageBox.Show("Please enter a group name.");
                return;

            }
            // Make sure we have a unique name
            commandText = string.Format("select count(*) from PollutantGroups where setupid={0} and pgname='{1}'", CommonClass.ManageSetup.SetupID, textGroupName.Text);

            // If we're editing an existing group, it's okay to keep the same name
            if(! _isNewPollutantGroup)
            {
                commandText = commandText + string.Format(" and pollutantgroupid <> {0}", _pollutantGroupID);
            }
            int testCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)); if (testCount > 0)
            {
                MessageBox.Show(string.Format("The name '{0}' has already been used in the {1} setup. Please enter a unique group name.", textGroupName.Text, CommonClass.ManageSetup.SetupName));
                return;
            }

            // Create or update the pollutant group
            if (_isNewPollutantGroup)
            {
                // This is a new group, get next group id
                commandText = "select max(PollutantGroupID) from PollutantGroups";
                int newId = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                _pollutantGroupID = newId; // We'll need this below to insert the pollutantgrouppollutants records
                commandText = string.Format("INSERT INTO POLLUTANTGROUPS (POLLUTANTGROUPID, SETUPID, PGNAME, PGDESCRIPTION) VALUES ({0}, {1}, '{2}', '{3}')", newId, CommonClass.ManageSetup.SetupID, textGroupName.Text, textGroupDescription.Text);
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

            }
            else if(_pollutantGroupName.Equals(textGroupName)==false || _pollutantGroupDesc.Equals(textGroupDescription)==false)
            {
                    commandText = string.Format("UPDATE POLLUTANTGROUPS SET PGNAME='{0}', PGDESCRIPTION='{1}' WHERE POLLUTANTGROUPID = {2}", textGroupName.Text, textGroupDescription.Text, _pollutantGroupID);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
            }


            // Update the group's pollutant list
            // Compare _pollutantList with the selected pollutants.
            //  If we have one that was selected, but no longer is, delete it from pollutantgrouppollutants
            //  If we have one that was not previously select, but is now, insert  pollutant membership in the database.
            List<int> selectedPollutantList = new List<int>();
            foreach (PollutantItem p in chkPollutants.CheckedItems)
            {
                selectedPollutantList.Add(p.Id);
            }

            foreach (int p in _pollutantList)
            {
                if(! selectedPollutantList.Contains(p))
                {
                    // DELETE p

                    commandText = string.Format("DELETE FROM POLLUTANTGROUPPOLLUTANTS WHERE POLLUTANTGROUPID={0} AND POLLUTANTID={1}", _pollutantGroupID, p);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }
            }

            foreach (int p in selectedPollutantList)
            {
                if (!_pollutantList.Contains(p))
                {
                    // INSERT p
                    commandText = string.Format("INSERT INTO POLLUTANTGROUPPOLLUTANTS (POLLUTANTGROUPID, POLLUTANTID) VALUES ({0}, {1})", _pollutantGroupID, p);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}
