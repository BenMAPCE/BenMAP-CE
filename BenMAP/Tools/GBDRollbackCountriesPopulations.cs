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
    public partial class GBDRollbackCountriesPopulations : Form
    {
        private DataTable dtCountryPop;

        public GBDRollbackCountriesPopulations()
        {
            InitializeComponent();            
        }

        public void LoadGrid()
        {
            if (dtCountryPop != null)
            {
                dtCountryPop.Columns.Add("POPULATION_STRING", Type.GetType("System.String"));
                dtCountryPop.DefaultView.Sort = "COUNTRYNAME ASC";
                
                foreach (DataRow dr in dtCountryPop.Rows)
                {
                    dr["POPULATION_STRING"] = Int32.Parse(dr["POPULATION"].ToString()).ToString("#,###");
                    DataGridViewRow row = new DataGridViewRow();
                    int i = dgvCountryPop.Rows.Add(row);
                    dgvCountryPop.Rows[i].Cells["colCountry"].Value = dr["COUNTRYNAME"].ToString();
                    dgvCountryPop.Rows[i].Cells["colPopulation"].Value = dr["POPULATION_STRING"].ToString();
                }

                lblTotalCountries.Text = "Total Countries: " + dtCountryPop.Rows.Count.ToString();

                int iPop = 0;
                object sumObject;
                sumObject = dtCountryPop.Compute("Sum(POPULATION)", "");
                iPop = Int32.Parse(sumObject.ToString());
                lblTotalPopulation.Text = "Total Popluation: " + iPop.ToString("#,###");
            }        
        }

        public DataTable CountryPop
        {
            get
            {
                return dtCountryPop;
            }
            set
            {
                dtCountryPop = value;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GBDRollbackCountriesPopulations_Shown(object sender, EventArgs e)
        {
            LoadGrid();
        }
    }
}
