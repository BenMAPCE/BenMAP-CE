using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

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
        private int begin_year = 1980; // start of simulation run (Step 1)
        private int end_year = 2020;   // end of simulation run (Step 1)
        private int Age_Range_Start = 0;    // earliest age range affected 
        private int Age_Range_End = 100;      // oldest age range affected

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
        private int PM_year_3 = 2014;
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

        private int Birth_Type = 0;  // step 6 birth  dynamic if 0, static if 1
        private int Lag_Type = 0;    // LK_Lag_types, 0 = single, 1 = cause specific lag
        private int Lag_Function_Type;
        private double Lag_k_single;
        private double Lag_k_multiple_cardio;
        private double Lag_k_multiple_lung;
        private double Lag_k_multiple_other;
        private string Scenario_Name = "Blank";
        private int PM_Choice = 0;  // 1 = threshold
        private int PM_Trajectory = 0; // 0 = Linear, 1 = Step
        private double PM_Threshold = 30; // threshold value
        private double Beta_adj_factor;
        private FirebirdSql.Data.FirebirdClient.FbConnection dbConnection;

        public PopSimInputData()    { // class constructor
            // setup database - this probably should be moved to a class to minimize connection counts
            // open database
            // create link to Firebird database
            dbConnection = getNewConnection();
            dbConnection.Open();
    }

        private static FbConnection getNewConnection()
        {

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["PopSimConnectionString"];
            string str = settings.ConnectionString;
            //if (!str.Contains(":"))
            //    str = str.Substring(0, str.IndexOf("initial catalog=")) + "initial catalog=" + Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
            str = str.Replace("##USERDATA##", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

            FbConnection connection = new FirebirdSql.Data.FirebirdClient.FbConnection(str);

            return connection;
        }

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
        public string getScenario_Name()
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
        public int getPM_Trajectory()
        {
            return PM_Trajectory;
        }
        
        public double getBeta_adj_factor()
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
        // this function should be depricated and replaced by the study id in queries, etc.
        public string getUser_Study_Name()
        {
            FbCommand dataCommand = new  FirebirdSql.Data.FirebirdClient.FbCommand();
            dataCommand.Connection = dbConnection;
            dataCommand.CommandType = CommandType.Text;
            dataCommand.CommandText = "SELECT STUDY_NAME FROM LK_STUDIES WHERE STUDY_ID=" + this.getUser_Study().ToString() ;
            FbDataReader dataReader;
            dataReader = dataCommand.ExecuteReader();
            dataReader.Read();
            if (dataReader.HasRows)
            {
                return (string) dataReader[0];
            }
            else
            {
                return "";  // no study available - return empty string
            }
            

        }
                
        // set methods
        public void getDataFromScenario(int Scenario_ID){
            // get initial normalized values - will need to add others later
            // SELECT a.SCENARIO_ID, a.SCENARIO_NAME, a.BEGIN_YEAR, a.END_YEAR, a.DR_APPROACH_ID, a.BETA_TYPE_ID, a.STUDY_ID, a.USER_SPECIFIED_BETA, a.PM_THRESHOLD_CHOICE, a.PS_TRAJECTORY_ID, a.PM_THRESHOLD_VALUE, a.BETA_ADJ_FACTOR, a.LAG_TYPE_ID, a.LAG_FUNCT_TYPE_ID, a.LAG_K_SINGLE, a.LAG_K_MULTIPLE_CARDIO, a.LAG_K_MULTIPLE_LUNG, a.LAG_K_MULTIPLE_OTHER, a.BIRTHS_DYNAMIC, a.AGE_RANGE_START, a.AGE_RANGE_END FROM SCENARIOS a
            FbCommand dataCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
            dataCommand.Connection = dbConnection;
            dataCommand.CommandType = CommandType.Text;
            string strSQL = "SELECT a.SCENARIO_ID, a.SCENARIO_NAME, a.BEGIN_YEAR, a.END_YEAR, a.DR_APPROACH_ID, "
                + "a.BETA_TYPE_ID, a.STUDY_ID, a.USER_SPECIFIED_BETA, a.PM_THRESHOLD_CHOICE, a.PS_TRAJECTORY_ID, a.PM_THRESHOLD_VALUE, "
                + "a.BETA_ADJ_FACTOR, a.LAG_TYPE_ID, a.LAG_FUNCT_TYPE_ID, a.LAG_K_SINGLE, a.LAG_K_MULTIPLE_CARDIO, "
                + "a.LAG_K_MULTIPLE_LUNG, a.LAG_K_MULTIPLE_OTHER, a.BIRTHS_DYNAMIC, a.AGE_RANGE_START, a.AGE_RANGE_END, "
                + "a.PM_YEAR_1, a.PM_YEAR_2, a.PM_YEAR_3, a.PM_YEAR_4, a.PM_YEAR_5, a.PM_VAL_1, a.PM_VAL_2, a.PM_VAL_3, " 
                + "a.PM_VAL_4, a.PM_VAL_5, a.SUB_POP_START_1, a.SUB_POP_START_2, a.SUB_POP_START_3, a.SUB_POP_START_4, " 
                + "a.SUB_POP_START_5, a.SUB_POP_END_1, a.SUB_POP_END_2, a.SUB_POP_END_3, a.SUB_POP_END_4, a.SUB_POP_END_5, "
                + "a.SUB_POP_ADJUSTMENT_1, a.SUB_POP_ADJUSTMENT_2, a.SUB_POP_ADJUSTMENT_3, a.SUB_POP_ADJUSTMENT_4, " 
                + "a.SUB_POP_ADJUSTMENT_5 FROM SCENARIOS a "
                + " where Scenario_ID = " + Scenario_ID.ToString();
            dataCommand.CommandText = strSQL;
            FbDataReader dataReader;
            dataReader = dataCommand.ExecuteReader();
            dataReader.Read();
            if (dataReader.HasRows) // fill scenario values - ignore invalid scenario
            {
                // fill scenario values
                // a.SCENARIO_ID, 
                Scenario_ID = (int)dataReader[0];
                // a.SCENARIO_NAME, 
                Scenario_Name = (string)dataReader[1];
                //a.BEGIN_YEAR, 
                begin_year = (int)dataReader[2];
                //a.END_YEAR, 
                end_year = (int)dataReader[3];
                // a.DR_APPROACH_ID, 
                approach = (int)dataReader[4];
                // a.BETA_TYPE_ID, 
                Beta_Type = (int)dataReader[5];
                //a.STUDY_ID, 
                User_Study = (int)dataReader[6];
                //a.USER_SPECIFIED_BETA, 
                User_Beta = (double)dataReader[7];
                //a.PM_THRESHOLD_CHOICE, 
                PM_Choice = (int)dataReader[8];
                //a.PS_TRAJECTORY_ID, 
                PM_Trajectory = (int)dataReader[9];
                //a.PM_THRESHOLD_VALUE, 
                PM_Threshold = (double)dataReader[10];
                //a.BETA_ADJ_FACTOR, 
                Beta_adj_factor = (double)dataReader[11];
                //a.LAG_TYPE_ID
                Lag_Type = (int)dataReader[12];
                //a.LAG_FUNCT_TYPE_ID, 
                Lag_Function_Type = (int)dataReader[13];
                //a.LAG_K_SINGLE, 
                Lag_k_single = (double)dataReader[14];
                //a.LAG_K_MULTIPLE_CARDIO, 
                Lag_k_multiple_cardio = (double)dataReader[15];
                //a.LAG_K_MULTIPLE_LUNG, 
                Lag_k_multiple_lung = (double)dataReader[16];
                //a.LAG_K_MULTIPLE_OTHER, 
                Lag_k_multiple_other = (double)dataReader[17];
                //a.BIRTHS_DYNAMIC, 
                Birth_Type = (int)dataReader[18];
                //a.AGE_RANGE_START, 
                Age_Range_Start = (int)dataReader[19];
                //a.AGE_RANGE_END 
                Age_Range_End = (int)dataReader[20];
                // a.PM_YEAR_1, 
                PM_year_1 = (int)dataReader[21];
                // a.PM_YEAR_2, 
                PM_year_2 = (int)dataReader[22];
                //a.PM_YEAR_3, 
                PM_year_3 = (int)dataReader[23];
                // a.PM_YEAR_4, 
                PM_year_4 = (int)dataReader[24];
                // a.PM_YEAR_5, 
                PM_year_5 = (int)dataReader[25];
                // a.PM_VAL_1, 
                PM_val_1 = (double)dataReader[26];
                // a.PM_VAL_2, 
                PM_val_2 = (double)dataReader[27];
                // a.PM_VAL_3,
                PM_val_3 = (double)dataReader[28];
                // a.PM_VAL_4, 
                PM_val_4 = (double)dataReader[29];
                // a.PM_VAL_5, 
                PM_val_5 = (double)dataReader[30];
                // a.SUB_POP_START_1, 
                Sub_Pop_Start_1 = (int)dataReader[31];
                // a.SUB_POP_START_2, 
                Sub_Pop_Start_2 = (int)dataReader[32];
                // a.SUB_POP_START_3, 
                Sub_Pop_Start_3 = (int)dataReader[33];
                // a.SUB_POP_START_4, 
                Sub_Pop_Start_4 = (int)dataReader[34];
                // a.SUB_POP_START_5, 
                Sub_Pop_Start_5 = (int)dataReader[35];
                // a.SUB_POP_END_1, 
                Sub_Pop_End_1 = (int)dataReader[36];
                // a.SUB_POP_END_2, 
                Sub_Pop_End_2 = (int)dataReader[37];
                // a.SUB_POP_END_3, 
                Sub_Pop_End_3 = (int)dataReader[38];
                // a.SUB_POP_END_4, 
                Sub_Pop_End_4 = (int)dataReader[39];
                // a.SUB_POP_END_5, 
                Sub_Pop_End_5 = (int)dataReader[40];
                
                // a.SUB_POP_ADJUSTMENT_1, 
                Sub_Pop_Adjustment_1 = (double)dataReader[41];
                // a.SUB_POP_ADJUSTMENT_2, 
                Sub_Pop_Adjustment_2 = (double)dataReader[42];
                // a.SUB_POP_ADJUSTMENT_3, 
                Sub_Pop_Adjustment_3 = (double)dataReader[43];
                // a.SUB_POP_ADJUSTMENT_4, 
                Sub_Pop_Adjustment_4 = (double)dataReader[44];
                // a.SUB_POP_ADJUSTMENT_5
                Sub_Pop_Adjustment_5 = (double)dataReader[45];
            
            
            }
            
        }

    }
 }