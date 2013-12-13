using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP
{
    
    public partial class BenMAPCalendar : UserControl
    {
        private int month;

        public int Month
        {
            get { return month; }
            set { 
                
                month = value; }
        }
        private int day=-1;

        public int Day
        {
            get { return day; }
            set { 
                
                day = value; }
        }
        private int yearDay=-1;
       // private int year = 2011;
        public int YearDay
        {
            get { return yearDay; }
            set { yearDay = value;
            DateTime dt = new DateTime(2011, 1, 1);
            dt = dt.AddDays(yearDay - 1);
            month = dt.Month;
            day = dt.Day;             
            
            }
        }

        public BenMAPCalendar()
        {
            InitializeComponent();
        }

        private void BenMAPCalendar_Load(object sender, EventArgs e)
        {
            try
            {
                string[] months = new string[] { "一月", "二月", "三月", "四月", "五月", "六月", "七", "八月", "九月", "十月", "十一月", "十二月" };
                dmMoths.Items.AddRange(months);
                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void cbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
           // //DateTime dt = new DateTime();
           // //dt=DateTime.Parse("2011/"+cbMonth.SelectedValue.ToString()+"/1");
           // DateTime dt = new DateTime(2011, cbMonth.SelectedIndex+1, 1);
           // int days= DateTime.DaysInMonth(dt.Year, dt.Month);

           // cbDay.Items.Clear();
           // int i = 1;
           // while (i <= days)
           // {
           //     cbDay.Items.Add(i);
           //     i++;
           // }
           // cbDay.SelectedIndex = -1;
           // cbDay.Text = "";
           // if(yearDay>-1)               
           //     {
           //         cbDay.SelectedIndex = day - 1;
           //     }

           //// DateTime dt = new DateTime();
           // dt = new DateTime(2011, cbMonth.SelectedIndex + 1, cbDay.SelectedIndex + 1);// DateTime.Parse("2011/" + cbMonth.SelectedText.ToString() + "/" + cbDay.SelectedText.ToString());
           // month = dt.Month;
           // day = dt.Day;
           // yearDay = dt.DayOfYear;
             

        }

        private void cbDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            //DateTime dt = new DateTime();
            //dt = new DateTime(2011, cbMonth.SelectedIndex + 1, cbDay.SelectedIndex + 1);// DateTime.Parse("2011/" + cbMonth.SelectedText.ToString() + "/" + cbDay.SelectedText.ToString());
            //month = dt.Month;
            //day = dt.Day;
            //yearDay = dt.DayOfYear;
        }
    }
}
