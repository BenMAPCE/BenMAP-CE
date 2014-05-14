using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FirebirdSql.Data.FirebirdClient;

// this is the PopSim data object used by the model to store its input data
namespace PopSim
{
    class PopSimInputData
    {
        // Step numbers refer to labels on the original Access GUI form "Main_Screen"
        // defaults are set to permit code development before we get the scenario code working

        // dates 
        // these should be datetime, but are kept integers as in the original code
        private int begin_year = 2000; // start of simulation run (Step 1)
        private int end_year = 2016;   // end of simulation run (Step 1)
        private int Age_Range_Start = 1;    // earliest age range affected 
        private int Age_Range_End = 65;      // oldest age range affected

        // boolean values - these are integers because Firebird version 2 has no boolean data type
        private int wasRun = 0; // set to true (1) after model has been run

        // index values - used to get values from table
        // private int method; // unclear what this is used for - could be DR approach?
        private int approach = 0; // from LK_DR_APPROACHES - 0 is aggregated, 1 is disaggregated
        private int Beta_Type = 1; // from LK_BETA_TYPES 0 0 is study beta, 1 is user defined beta
        private double User_Beta = 0.015; // user supplied beta to be used if study is not selected
        private int User_Study = 1; // Study ID, from LK_Study_Betas


        // repeating years - these should be handled with a child table
        // the child tables have been created but are not used yet
        // step 3
        private int PM_year_1 =1999; // first year PM Step
        private int PM_year_2 = 2000;
        private int PM_year_3 = 2005;
        private int PM_year_4 = 2010;
        private int PM_year_5 = 2020; 
        private double PM_val_1 = 10; // first year PM step value
        private double PM_val_2 = 20; 
        private double PM_val_3 = 30; 
        private double PM_val_4 = 40;
        private double PM_val_5 = 50; 
        // step 4 - ages affected        
        private int Sub_Pop_Start_1 = 0;    // earliest age of range 1
        private int Sub_Pop_Start_2 = 0;
        private int Sub_Pop_Start_3 = 0;
        private int Sub_Pop_Start_4 = 0;
        private int Sub_Pop_Start_5 = 0;
        private int Sub_Pop_End_1 = 0;  // oldest range of range 1
        private int Sub_Pop_End_2 = 0;
        private int Sub_Pop_End_3 = 0;
        private int Sub_Pop_End_4 = 0;
        private int Sub_Pop_End_5 = 0;
        private double Sub_Pop_Adjustment_1 = 0;    // adjustment for age 1
        private double Sub_Pop_Adjustment_2 = 0;
        private double Sub_Pop_Adjustment_3 = 0;
        private double Sub_Pop_Adjustment_4 = 0;
        private double Sub_Pop_Adjustment_5 = 0;

        private int Birth_Type = 0;  // step 6 birth not dynamic if 0, dynamic if 1
        private int Lag_Type = 0;    // LK_Lag_types, 0 = single, 1 = cause specific lag
        // private int Lag_Type_Specific; // appears to be unused 
        private int Lag_Function_Type;
        private double Lag_k_single;
        private double Lag_k_multiple_cardio;
        private double Lag_k_multiple_lung;
        private double Lag_k_multiple_other;
        private string strFolderName;
        private int Scenario_Name;
        private int PM_Choice = 0;  // 1 = threshold
        private double PM_Threshold = 30; // threshold value
        private int Beta_adj_factor;




        // get methods
        public int getBegin_Year()
        {
            return begin_year;
        }
        public int getEnd_Year()
        {
            return end_year;
        }
        public int getAge_Range_Start(){
            return Age_Range_Start;
        }
        public int getAge_Range_End(){
            return Age_Range_End;
        }
        private int getWasRun(){
            return wasRun;
        }

        public int getApproach()
        {
            return approach;
        }
        public int getBeta_Type()
        {
            return Beta_Type;
        }
        public double getUser_Beta()
        {
            return User_Beta;
        }
        public int getUser_Study()
        {
            return User_Study;
        }
        public int getBirth_Type()
        {
            return Birth_Type;
        }
        public int getLag_Type()
        {
            return Lag_Type;
        }
        // public int Lag_Type_Specific; // appears to be unused 
        public int getLag_Function_Type()
        {
            return Lag_Function_Type;
        }
        public double getLag_k_single()
        {
            return Lag_k_single;
        }
        public double getLag_k_multiple_cardio()
        {
            return Lag_k_multiple_cardio;
        }
        public double getLag_k_multiple_lung()
        {
            return Lag_k_multiple_lung;
        }
        public double getLag_k_multiple_other()
        {
            return Lag_k_multiple_other;
        }
        public string getstrFolderName()
        {
            return strFolderName;
        }
        public int getScenario_Name()
        {
            return Scenario_Name;
        }
        public int getPM_Choice()
        {
            return PM_Choice;
        }
        public double getPM_Threshold()
        {
            return PM_Threshold;
        }
        public int getBeta_adj_factor()
        {
            return Beta_adj_factor;
        }

        // sets of 5 categories kept as in original Access application 
        // these should all be changed to child relationships - the tables have been done, but the code needs to be modified in the future
        public int getPM_year_1()
        {
            return PM_year_1;
        }
        public int getPM_year_2()
        {
            return PM_year_2;
        }
        public int getPM_year_3()
        {
            return PM_year_3;
        }
        public int getPM_year_4()
        {
            return PM_year_4;
        }
        public int getPM_year_5()
        {
            return PM_year_5;
        }
        public double getPM_val_1()
        {
            return PM_val_1;
        }
        public double getPM_val_2()
        {
            return PM_val_2;
        }
        public double getPM_val_3()
        {
            return PM_val_3;
        }
        public double getPM_val_4()
        {
            return PM_val_4;
        }
        public double getPM_val_5()
        {
            return PM_val_5;
        }
        public int getSub_Pop_Start_1()
        {
            return Sub_Pop_Start_1;
        }
        public int getSub_Pop_Start_2()
        {
            return Sub_Pop_Start_2;
        }
        public int getSub_Pop_Start_3()
        {
            return Sub_Pop_Start_3;
        }
        public int getSub_Pop_Start_4()
        {
            return Sub_Pop_Start_4;
        }
        public int getSub_Pop_Start_5()
        {
            return Sub_Pop_Start_5;
        }
        public int getSub_Pop_End_1()
        {
            return Sub_Pop_End_1;
        }
        public int getSub_Pop_End_2()
        {
            return Sub_Pop_End_2;
        }
        public int getSub_Pop_End_3()
        {
            return Sub_Pop_End_3;
        }
        public int getSub_Pop_End_4()
        {
            return Sub_Pop_End_4;
        }
        public int getSub_Pop_End_5()
        {
            return Sub_Pop_End_5;
        }
        public double getSub_Pop_Adjustment_1()
        {
            return Sub_Pop_Adjustment_1;
        }
        public double getSub_Pop_Adjustment_2()
        {
            return Sub_Pop_Adjustment_2;
        }
        public double getSub_Pop_Adjustment_3()
        {
            return Sub_Pop_Adjustment_3;
        }
        public double getSub_Pop_Adjustment_4()
        {
            return Sub_Pop_Adjustment_4;
        }
        public double getSub_Pop_Adjustment_5()
        {
            return Sub_Pop_Adjustment_5;
        }
                
        // set methods
        private void getDataFromScenario(int Scenario_ID){
        }

    }
 }