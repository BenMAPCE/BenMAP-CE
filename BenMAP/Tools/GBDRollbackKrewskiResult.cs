using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    class GBDRollbackResult
    {
        /*
        public GBDRollbackKrewskiResult(double krewski, double krewski2_5, double krewski97_5, String countryid, int regionid)
        {
            this.krewski = krewski;
            this.krewski2_5 = krewski2_5;
            this.krewski97_5 = krewski97_5;
        }
        */
        //YY: added 2 new result 
        public GBDRollbackResult(double result, double result2_5, double result97_5, double yll, double ecoBenefit, double population)
        {
            this.result = result;
            this.result2_5 = result2_5;
            this.result97_5 = result97_5;
            this.yll = yll;
            this.ecoBenefit = ecoBenefit;
            this.population = population;
        }
        double result;

        public double Result
        {
            get { return result; }
            set { result = value; }
        }
        double result2_5;

        public double Result2_5
        {
            get { return result2_5; }
            set { result2_5 = value; }
        }
        double result97_5;

        public double Result97_5
        {
            get { return result97_5; }
            set { result97_5 = value; }
        }
        double yll;
        public double Yll
        {
            get { return yll; }
            set { yll = value; }
        }
        double ecoBenefit;
        public double EcoBenefit
        {
            get { return ecoBenefit; }
            set { ecoBenefit = value; }
        }
        double population;
        public double Population
        {
            get { return population; }
            set { population = value; }
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
