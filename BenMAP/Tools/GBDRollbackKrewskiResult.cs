using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    class GBDRollbackKrewskiResult
    {
        /*
        public GBDRollbackKrewskiResult(double krewski, double krewski2_5, double krewski97_5, String countryid, int regionid)
        {
            this.krewski = krewski;
            this.krewski2_5 = krewski2_5;
            this.krewski97_5 = krewski97_5;
        }
        */
        public GBDRollbackKrewskiResult(double krewski, double krewski2_5, double krewski97_5)
        {
            this.krewski = krewski;
            this.krewski2_5 = krewski2_5;
            this.krewski97_5 = krewski97_5;
        }
        double krewski;

        public double Krewski
        {
            get { return krewski; }
            set { krewski = value; }
        }
        double krewski2_5;

        public double Krewski2_5
        {
            get { return krewski2_5; }
            set { krewski2_5 = value; }
        }
        double krewski97_5;

        public double Krewski97_5
        {
            get { return krewski97_5; }
            set { krewski97_5 = value; }
        }

        /*
        String countryid;

        public String Countryid
        {
            get { return countryid; }
            set { countryid = value; }
        }
        int regionid;

        public int Regionid
        {
            get { return regionid; }
            set { regionid = value; }
        }
        */

    }
}
