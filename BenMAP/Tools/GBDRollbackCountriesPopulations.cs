﻿using System;
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
                dtCountryPop.DefaultView.Sort = "COUNTRYNAME ASC";
                int iCountries=0;
                foreach (DataRow dr in dtCountryPop.Rows)
                {                       
                    DataGridViewRow row = new DataGridViewRow();
                    int i = dgvCountryPop.Rows.Add(row);
                    dgvCountryPop.Rows[i].Cells["colCountry"].Value = dr["COUNTRYNAME"].ToString();
                    dgvCountryPop.Rows[i].Cells["colPopulation"].Value = dr["POPULATION"].ToString();
                    iCountries++;
                }
                lblTotalCountries.Text = "Total Countries: " + iCountries.ToString();            
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
