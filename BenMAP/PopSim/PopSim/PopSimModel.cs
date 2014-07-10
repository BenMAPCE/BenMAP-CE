using System;
using System.Configuration;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
// next namespace includes the background worker object
using System.ComponentModel;

// this is the module that runs the simulation
namespace PopSim
{
    class PopSimModel
    {
         
        //'Define global variables calculated in the model but not in the InputData class - these need to be refactored and cleaned up later
        private double j;
        private double k;
        private double m;
        private double p;
        private double t;
       
        private string Illness_Type;
        private string Illness_Type_Specific;
        //private int IllnessCount;
        private int GenderCount;
        private string gender;
        private string source;
        private string destination;
        private string strMethod;
        private string strApproach;
        private string strLag_Type;
        private string strLag_Type_Specific;
        private string strLag_Function_Type;
        private string strBirth_Type;
        private string strBeta_Type;
        private string strscenarioText;
        private string strcensus_table;
        private string strmortal_table;
        private string strmigration_table;
        private string strbirth_table;
        private string strinfant_migration_table;
        private string strgenderText;
        private BackgroundWorker psWorker;
        

        PopSimInputData InputData = new PopSimInputData();
        // setup database - this probably should be moved to a class to minimize connection counts
        private FirebirdSql.Data.FirebirdClient.FbConnection dbConnection;

        public PopSimModel(BackgroundWorker worker, DoWorkEventArgs e)    // class constructor
        {
            psWorker = worker;
            // open database
            // create link to Firebird database
            dbConnection = getNewConnection();
            dbConnection.Open();
            InputData.getDataFromScenario(1);   // only one scenario currently permitted
        }

        private static FbConnection getNewConnection()
        {

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            string str = settings.ConnectionString;
            //if (!str.Contains(":"))
            //    str = str.Substring(0, str.IndexOf("initial catalog=")) + "initial catalog=" + Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
            str = str.Replace("##USERDATA##", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

            FbConnection connection = new FirebirdSql.Data.FirebirdClient.FbConnection(str);

            return connection;
        }


        public void runPopSim()
        {
            string[] CurrentStatus = new string[1];
            Debug.Print("run started");
            setupInternalVariables();
            // STEP 1: DELETE RECORDS
            delete_Records();
            //psE.Result
            CurrentStatus[0] = "Calculating Annual PM Values";
            psWorker.ReportProgress(10, CurrentStatus);
            
            // STEP 2: CALCULATE ANNUAL PM VALUES
            run_Annual_PM_Values();
            CurrentStatus[0] = "Calculating Lag";
            psWorker.ReportProgress(20, CurrentStatus);

            //STEP 3: CALCULATE LAG
            //If user selected the single lag option, then calculate the lag only for the single lag option
            // if (strLag_Type == "Single")
            if ( InputData.getLag_Type() == 0) {
                strLag_Type = "Single";
                strLag_Type_Specific = "Single";
                run_Lag_Calcs();
            //Otherwise, calculate the lag for all the multiple causes
            } else {
                strLag_Type_Specific = "Multiple-Cardiopulmonary";
                run_Lag_Calcs();
                strLag_Type_Specific = "Multiple-LungCancer";
                run_Lag_Calcs();
                strLag_Type_Specific = "Multiple-AllOtherCauses";
                run_Lag_Calcs();
            }
            //If user selected the user-defined lag option, bring up a table to allow the user to edit the values.
            // USER INPUT FORM DOES NOT EXIST YET
            /*
            If Lag_Function_Type = "User-defined" Then
                MsgBox ("You may now edit the default lag values. Close the form when you are done editing.")

                DoCmd.OpenForm "frmLagUpdate", acNormal, , , , acDialog

            End If
            */
            CurrentStatus[0] = "Calculating Threshold";
            psWorker.ReportProgress(30,CurrentStatus);
            
            //STEP 4: CALCULATE THRESHOLD
            //Only run threshold calcs if user specified a threshold value
            if (InputData.getPM_Choice() == 1) { 
                run_threshold_calcs();
            }
            CurrentStatus[0] = "Calculating Age-Specific Adjustment Factors";
            psWorker.ReportProgress(40, CurrentStatus);

            //STEP 5: CALCULATE AGE-SPECIFIC ADJUSTMENT FACTORS
            run_age_specific_adjustment_factors();
            CurrentStatus[0] = "Calculating Illness Factors";
            psWorker.ReportProgress(50, CurrentStatus);

            //STEP 6: RUN ILLNESS CALCULATIONS
            // all the references should be replaced with the study id to clean up
            string User_Study_Name = InputData.getUser_Study_Name();
            if (strApproach == "Aggregated") {
                Illness_Type = "All-Cause";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
            }
            else if ((User_Study_Name == "Dockery et al., 1993") || (User_Study_Name == "Krewski (Six Cities) et al., 2000"))
            { 
                Illness_Type = "Cardiopulmonary";
                Illness_Type_Specific = "Cardiopulmonary-2";
                run_illness_calcs();
        
                Illness_Type = "Lung Cancer";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
        
                Illness_Type = "All Other Causes";
                Illness_Type_Specific = "All Other Causes-2";
                run_illness_calcs();
            }
            else if ((User_Study_Name == "Krewski (ACS median) et al., 2000") || (User_Study_Name == "Pope et al., 1995")
                    || (User_Study_Name == "Pope et al., 2002"))
            {
                Illness_Type = "Cardiopulmonary";
                Illness_Type_Specific = "Cardiopulmonary-1";
                run_illness_calcs();
        
                Illness_Type = "Lung Cancer";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
        
                Illness_Type = "All Other Causes";
                Illness_Type_Specific = "All Other Causes-1";
                run_illness_calcs();
            }
            else if (User_Study_Name == "Pope et al., 2004 - 4 causes of death")
            {
                Illness_Type = "All Cardiovascular Disease Plus Diabetes";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
                
                Illness_Type = "Diseases of the Respiratory System";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
                
                Illness_Type = "Lung Cancer";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
                
                Illness_Type = "All Other Causes";
                Illness_Type_Specific = "All Other Causes-3";
                run_illness_calcs();
            }
            else if (User_Study_Name == "Pope et al., 2004 - 12 causes of death")
            {
                Illness_Type = "Ischemic Heart Disease";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
                
                Illness_Type = "Dysrhythmias, Heart Failure, Cardiac Arrest";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
                
                Illness_Type = "Hypertensive Disease";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
                
                Illness_Type = "Other Atherosclerosis and Aortic Aneurysms";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
                
                Illness_Type = "Cerebrovascular Disease";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
                
                Illness_Type = "Diabetes";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
                
                Illness_Type = "All Other Cardiovascular Diseases";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
                
                Illness_Type = "COPD and Allied Conditions";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();

                Illness_Type = "Pneumonia and Influenza";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();
                
                Illness_Type = "All Other Respiratory Diseases";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();

                Illness_Type = "Lung Cancer";
                Illness_Type_Specific = Illness_Type;
                run_illness_calcs();

                Illness_Type = "All Other Causes";
                Illness_Type_Specific = "All Other Causes-3";
                run_illness_calcs();
            } // endif
            CurrentStatus[0] = "Calculating Death Probabilities";
            psWorker.ReportProgress(60, CurrentStatus);

            // STEP 7: CALCULATE THE PROBABILITY OF DEATH
            run_calculate_pdeath();
            CurrentStatus[0] = "Calculating Regulatory Population, Number of Deaths, and Life Expectancy";
            psWorker.ReportProgress(70,CurrentStatus);

            //STEP 8: CALCULATE REGULATORY POPULATION, NUMBER OF DEATHS, AND LIFE EXPECTANCY
            strscenarioText = "Regulatory";

            for (GenderCount = 1; GenderCount <= 2; GenderCount++){ 
    
                //Set table source names
                if (GenderCount == 1) { //girls
                    strcensus_table = "Population_F_Table_Census";
                    strmortal_table = "Probability_of_Death_RF";
                    strmigration_table = "net_female_migration";
                    strbirth_table = "Rate_of_Baby_Girls";
                    strinfant_migration_table = "Infant_Migration_Female";
                    strgenderText = "female";
                } else { //boys
                    strcensus_table = "Population_M_Table_Census";
                    strmortal_table = "Probability_of_Death_RM";
                    strmigration_table = "net_male_migration";
                    strbirth_table = "Rate_of_Baby_Boys";
                    strinfant_migration_table = "Infant_Migration_Male";
                    strgenderText = "male";
                } // End If
                run_calculate_pop_and_death();
                // next routine has minor numerical problems and needs review
                run_calculate_life_expectancy();
            } // Next GenderCount
            CurrentStatus[0] = "Generating Input Summary";
            psWorker.ReportProgress(90, CurrentStatus);
            //STEP 9: GENERATE SUMMARY OF INPUTS
            run_summarize_results();

            CurrentStatus[0] = "PopSim Model Run Finished";
            psWorker.ReportProgress(100, CurrentStatus);
          

        } // end run Pop Sim
        public void setupInternalVariables()
        {
            // setup up internal variables. This was done on in the Global code module of the original Access database program
            if (InputData.getPM_Choice() == 0)
            {
                strMethod = "Linear";
            }else{
                strMethod = "Step";
            }
            if (InputData.getApproach() == 0){ 
                strApproach = "Aggregated";
            }else{
                strApproach = "Disaggregated";
             }
            if (InputData.getLag_Function_Type() == 0 ){
                strLag_Type = "Single";
            }else{
                strLag_Type = "Cause-specific";
            }
            if (InputData.getLag_Function_Type() == 0 ){
                strLag_Function_Type = "HES_Default";
            }else if (InputData.getLag_Function_Type() == 1){
                strLag_Function_Type = "Smooth";
            }else {
                strLag_Function_Type = "User-defined";
            }
            if (InputData.getBirth_Type() == 0){
                strBirth_Type = "Dynamic";
            } else {
                strBirth_Type = "Static";
            }
           

            if (InputData.getBeta_Type() == 0 ){
                strBeta_Type = "Study Beta";
            } else 
            {
                strBeta_Type = "User-Entered Beta";
            }

        }
        private void delete_Records(){

            FbCommand dataCommand = new  FirebirdSql.Data.FirebirdClient.FbCommand();
            dataCommand.Connection = dbConnection;
            dataCommand.CommandType = CommandType.Text;
            // empty previous run's results
            
            //Delete all baseline records from PM_Changes table
            dataCommand.CommandText  = "DELETE FROM PM_Changes";
            dataCommand.ExecuteNonQuery();
            
            //Delete all records from the lag table
            dataCommand.CommandText  = "DELETE FROM Lag";
            dataCommand.ExecuteNonQuery();

            //Delete all records from the threshold table
            dataCommand.CommandText  = "DELETE FROM Thresholds";
            dataCommand.ExecuteNonQuery();

            //Delete all baseline records from ASAF table
            dataCommand.CommandText  = "DELETE FROM ASAF";
            dataCommand.ExecuteNonQuery();

            //Delete all baseline records from MHAF_Calcs table
            dataCommand.CommandText  = "DELETE FROM MHAF_Calcs";
            dataCommand.ExecuteNonQuery();

            //Delete all records from PM_Impact_Vectors table
            dataCommand.CommandText  = "DELETE FROM PM_Impact_Vectors";
            dataCommand.ExecuteNonQuery();

            //Delete all records from PM_Impact_Vectors_lag table
            dataCommand.CommandText  = "DELETE FROM PM_Impact_Vectors_lag";
            dataCommand.ExecuteNonQuery();

            //Delete all records from Hazards_adj table
            dataCommand.CommandText  = "DELETE FROM Hazards_adj";
            dataCommand.ExecuteNonQuery();

            //Delete all records from Disease_Rates_Combined table
            dataCommand.CommandText  = "DELETE FROM Disease_Rates_Combined";
            dataCommand.ExecuteNonQuery();

            //Delete all records from Probability_of_Death_RF table
            dataCommand.CommandText  = "DELETE FROM Probability_of_Death_RF";
            dataCommand.ExecuteNonQuery();

            //Delete all records from Probability_of_Death_RM table
            dataCommand.CommandText  = "DELETE FROM Probability_of_Death_RM";
            dataCommand.ExecuteNonQuery();

            //Delete all regulatory records from "Final_Table_Pop"
            dataCommand.CommandText  = "DELETE FROM Final_Table_Pop WHERE Scenario = 'Regulatory'";
            dataCommand.ExecuteNonQuery();

            //Delete all regulatory records from "Final_Table_Deaths"
            dataCommand.CommandText  = "DELETE FROM Final_Table_Deaths WHERE Scenario = 'Regulatory'";
            dataCommand.ExecuteNonQuery();

            //Delete all regulatory records from "Final_Table_Cohort_CLE"
            dataCommand.CommandText  = "DELETE FROM Final_Table_Cohort_CLE WHERE Scenario = 'Regulatory'";
            dataCommand.ExecuteNonQuery();

            //Delete all regulatory records from "Final_Table_Cohort_ELY"
            dataCommand.CommandText  = "DELETE FROM Final_Table_Cohort_ELY WHERE Scenario = 'Regulatory'";
            dataCommand.ExecuteNonQuery();

            //Delete all regulatory records from "Final_Table_Cohort_LY"
            dataCommand.CommandText  = "DELETE FROM Final_Table_Cohort_LY WHERE Scenario = 'Regulatory'";
            dataCommand.ExecuteNonQuery();

            //Delete all regulatory records from "Final_Table_Period_CLE"
            dataCommand.CommandText  = "DELETE FROM Final_Table_Period_CLE WHERE Scenario = 'Regulatory'";
            dataCommand.ExecuteNonQuery();

            //Delete all regulatory records from "Final_Table_Period_ELY"
            dataCommand.CommandText  = "DELETE FROM Final_Table_Period_ELY WHERE Scenario = 'Regulatory'";
            dataCommand.ExecuteNonQuery();

            //Delete all regulatory records from "Final_Table_Period_LY"
            dataCommand.CommandText  = "DELETE FROM Final_Table_Period_LY WHERE Scenario = 'Regulatory'";
            dataCommand.ExecuteNonQuery();

            //Delete all records from "Report_Beta_Summary"
            dataCommand.CommandText  = "DELETE FROM Report_Beta_Summary";
            dataCommand.ExecuteNonQuery();

            //Delete all records from "Report_Avoided_Deaths"
            dataCommand.CommandText  = "DELETE FROM Report_Avoided_Deaths";
            dataCommand.ExecuteNonQuery();

            //Delete all records from "Report_Life_Years_Gained"
            dataCommand.CommandText  = "DELETE FROM Report_Life_Years_Gained";
            dataCommand.ExecuteNonQuery();

            //Delete all records from "Report_Increase_Cohort_Conditional_Life_Expectancy"
            dataCommand.CommandText  = "DELETE FROM RPT_Inc_Cohort_Cond_Life_Exp";
            dataCommand.ExecuteNonQuery();

            //Delete all records from "Report_Increase_Period_Conditional_Life_Expectancy"
            dataCommand.CommandText  = "DELETE FROM RPT_Inc_Prd_Cond_Life_Exp";
            dataCommand.ExecuteNonQuery();
        }

        private void run_Annual_PM_Values()
        {

            // create a data command to use throughout procedure
            FbCommand dataCommand = new  FirebirdSql.Data.FirebirdClient.FbCommand();
            dataCommand.Connection = dbConnection;
            dataCommand.CommandType = CommandType.Text;
            
           
            //Populate table with annual PM values either linearly or step-wise

            //Set variables to beginning values
            
            int PM_year = InputData.getBegin_Year();
            double PM_val = 0;

            //Step interpolation between PM years
            if (InputData.getPM_Trajectory() ==1) { // strMethod == "Step")
                while (PM_year <=  InputData.getEnd_Year()) {
                    // set PM value for year step
                    if (PM_year == InputData.getPM_year_1()){
                        PM_val = InputData.getPM_val_1();
                    }else if (PM_year == InputData.getPM_year_2()) {
                        PM_val = InputData.getPM_val_2();
                    }else if (PM_year == InputData.getPM_year_3()) {
                        PM_val = InputData.getPM_val_3();
                    }else if (PM_year == InputData.getPM_year_4()){
                        PM_val = InputData.getPM_val_4();
                    }else if (PM_year == InputData.getPM_year_5()) {
                        PM_val = InputData.getPM_val_5();
                    }else {
                        PM_val = 0;
                    }
                    
                    //Write values to PM_Changes table
                    string strSQL = "INSERT INTO PM_Changes (Year_Num, PM_Change) VALUES( " + PM_year.ToString() + " , " + PM_val.ToString() + " )";
                    dataCommand.CommandText = strSQL;          
                    dataCommand.ExecuteNonQuery();
                    
                    //Proceed to next year
                    PM_year = PM_year + 1;

                }   // wend
        
            //Linear interpolation between PM years
            }else{
                while (PM_year <= InputData.getEnd_Year()) {
        
                    if (PM_year == InputData.getBegin_Year()){
                        PM_val = 0;
                    }else if (PM_year <= InputData.getPM_year_1()) {
                        PM_val = InputData.getPM_val_1() / (InputData.getPM_year_1() - InputData.getBegin_Year());
                    }else if (PM_year <= InputData.getPM_year_2()) { 
                        PM_val = InputData.getPM_val_2() / (InputData.getPM_year_2() - InputData.getPM_year_1());
                    }else if (PM_year <= InputData.getPM_year_3()) {
                        PM_val = InputData.getPM_val_3() / (InputData.getPM_year_3() - InputData.getPM_year_2());
                    }else if (PM_year <= InputData.getPM_year_4()){
                        PM_val = InputData.getPM_val_4() / (InputData.getPM_year_4() - InputData.getPM_year_3());
                    }else if (PM_year <= InputData.getPM_year_5()){
                        PM_val = InputData.getPM_val_5() / (InputData.getPM_year_5() - InputData.getPM_year_4());
                    } else {
                        PM_val = 0;
                    }
                    
                    //Write values to PM_Changes table
                    string strSQL = "INSERT INTO PM_Changes (Year_Num, PM_Change) VALUES( " + PM_year.ToString() + " , " + PM_val.ToString() + " )";
                    dataCommand.CommandText = strSQL;          
                    dataCommand.ExecuteNonQuery();

                    //Proceed to next year
                    PM_year = PM_year + 1;
                } // wend
            }

        }
        public void run_Lag_Calcs()
        {
            // create a data command to use throughout procedure
            FbCommand dataCommand = new  FirebirdSql.Data.FirebirdClient.FbCommand();
            dataCommand.Connection = dbConnection;
            dataCommand.CommandType = CommandType.Text;
            
            if ((strLag_Function_Type == "HES_Default") || (strLag_Function_Type == "User-defined")) {
    
                double m = 0;
                int j = 1;
    
                while (j <= 61) {
    
                    if (j == 1) {
                        m = 0.3;
                    }else if (j <= 5) {
                        m = m + 0.5 / 4;
                    }else if (j <= 20) {
                        m = m + 0.2 / 15;
                    }else {
                        m = 1;
                    }
    
                    // Write values to table
                    string strSQL = "INSERT INTO Lag(Year_after_PM_Change, Perc_Impacts, Type) Values(" +  j.ToString()  + ", "  + m.ToString() + " , '" + strLag_Type_Specific + "' ) ";
                    dataCommand.CommandText = strSQL;          
                    dataCommand.ExecuteNonQuery();

    
                    j = j + 1;
    
                } // wend 
            }else if (strLag_Function_Type == "Smooth") {
    
                if (strLag_Type == "Single") {
                    k = InputData.getLag_k_single();
                }else if (strLag_Type_Specific == "Multiple-Cardiopulmonary") {
                    k = InputData.getLag_k_multiple_cardio();
                }else if (strLag_Type_Specific == "Multiple-LungCancer") {
                    k = InputData.getLag_k_multiple_lung();
                }else if (strLag_Type_Specific == "Multiple-AllOtherCauses") {
                    k = InputData.getLag_k_multiple_other();
                }
    
                m = 0;
                j = 1;
    
                while (j <= 61) {
                    m = 1 - System.Math.Exp(-k * j);

                    //Write values to table
                    string strSQL = "INSERT INTO Lag(Year_after_PM_Change, Perc_Impacts, Type) Values(" +  j.ToString()  + ", "  + m.ToString() + " , '" + strLag_Type_Specific + "' ) ";
                    dataCommand.CommandText = strSQL;          
                    dataCommand.ExecuteNonQuery();
                    
                    j = j + 1;
    
                } //wend 
    
            } // endif
        } // end run lag calcs

        private void run_threshold_calcs()
        {
           //'Set variables to beginning values
            int year;
            double PM_val = 0;
            t = 0;
            j = 0;
            k = 0;
            m = 0;

            year = InputData.getBegin_Year();
            while (year <= InputData.getEnd_Year()){

                // Calculate threshold value
    
                //Look up year-specific PM value from PM_Changes table
                string sqltext = "SELECT PM_Changes.Year_Num, PM_Changes.PM_Change FROM PM_Changes";
                sqltext = sqltext + " where PM_Changes.Year_Num = " + year.ToString();
                Debug.Print(sqltext);
                FbCommand dataCommand = new  FirebirdSql.Data.FirebirdClient.FbCommand();
                dataCommand.Connection = dbConnection;
                dataCommand.CommandType = CommandType.Text;
                dataCommand.CommandText = sqltext;

                FbDataReader dataReader;
                dataReader = dataCommand.ExecuteReader();
                dataReader.Read();
                
                PM_val = (double) dataReader[1];
                dataReader.Close();
    
                if (PM_val <= 0) {
        
                    //Find threshold value in sorted PM Concentration table
                    sqltext = "SELECT Sorted_PM_Conc.PM_Conc, Sorted_PM_Conc.Perc1, Sorted_PM_Conc.Perc2 FROM Sorted_PM_Conc";
                    sqltext = sqltext + " where Sorted_PM_Conc.PM_Conc = " + System.Math.Round((double)InputData.getPM_Threshold(), 1);
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    j = (double) dataReader[1];
                    dataReader.Close();

                    //Find adjusted threshold value in sorted PM Concentration table
                    sqltext = "SELECT Sorted_PM_Conc.PM_Conc, Sorted_PM_Conc.Perc1, Sorted_PM_Conc.Perc2 FROM Sorted_PM_Conc";
                    sqltext = sqltext + " where Sorted_PM_Conc.PM_Conc = " + System.Math.Round(((double)InputData.getPM_Threshold() - PM_val), 1);
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();                  
                    k = (double) dataReader[2];
                    dataReader.Close();
        
                } else {
        
                    // Find adjusted threshold value in sorted PM Concentration table
                    sqltext = "SELECT Sorted_PM_Conc.PM_Conc, Sorted_PM_Conc.Perc1, Sorted_PM_Conc.Perc2 FROM Sorted_PM_Conc";
                    sqltext = sqltext + " where Sorted_PM_Conc.PM_Conc = " + System.Math.Round(((double)InputData.getPM_Threshold() - PM_val), 1);
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    j = (double)dataReader[1];
                    dataReader.Close();
        
                    // Find threshold value in sorted PM Concentration table
                    sqltext = "SELECT Sorted_PM_Conc.PM_Conc, Sorted_PM_Conc.Perc1, Sorted_PM_Conc.Perc2 FROM Sorted_PM_Conc";
                    sqltext = sqltext + " where Sorted_PM_Conc.PM_Conc = " + System.Math.Round((double)InputData.getPM_Threshold(), 1);
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    k = (double) dataReader[2]; 
                    dataReader.Close();
        
                } // End If
    
                m = 1 - j - k;
        
                t = 0 * j + 1 * k + 0.5 * m;
    
                //Write values to Thresholds table
                sqltext = "INSERT INTO Thresholds(Year_Num, Threshold) VALUES( " + year.ToString() + " , " + t.ToString() + " )";
                dataCommand.CommandText = sqltext;
                dataCommand.ExecuteNonQuery();
                
                // Proceed to next year
                year = year + 1;
            } // wend 
        } // end run thresholds
        private void run_age_specific_adjustment_factors()
        {
            int maxRunAge = 100; // 100 is max
            int age = 0;
            FbCommand dataCommand = new  FirebirdSql.Data.FirebirdClient.FbCommand();
            dataCommand.Connection = dbConnection;

            // Repeat this loop for ages 0-100
            while (age <= maxRunAge){
           
                // Reset variables to zero
                j = 0;
                k = 0;
           
                // Determine whether age is within age range or not (indicator of 1 or 0)
                if ((age >= InputData.getAge_Range_Start()) & (age <= InputData.getAge_Range_End())) {
                    j = 1;
                }else {
                    j = 0;
                }
    
                // Determine whether age is within an age-specific adjustment range and apply adjustment factor
                if ((age >= InputData.getSub_Pop_Start_1()) & (age <= InputData.getSub_Pop_End_1())) { 
                    k = InputData.getSub_Pop_Adjustment_1();
                }else if ((age >= InputData.getSub_Pop_Start_2()) & (age <= InputData.getSub_Pop_End_2())) {
                    k = InputData.getSub_Pop_Adjustment_2();
                }else if ((age >= InputData.getSub_Pop_Start_3()) & (age <= InputData.getSub_Pop_End_3())) {
                    k = InputData.getSub_Pop_Adjustment_3();
                }else if ((age >= InputData.getSub_Pop_Start_4()) & (age <= InputData.getSub_Pop_End_4())) {
                    k = InputData.getSub_Pop_Adjustment_4();
                }else if ((age >= InputData.getSub_Pop_Start_5()) & (age <= InputData.getSub_Pop_End_5())) {
                    k = InputData.getSub_Pop_Adjustment_5();
                }else {
                    k = 1;
                } // endif
                
    
            // Write values to ASAF table
            dataCommand.CommandType = CommandType.Text;
            string sqltext = "INSERT INTO ASAF (Age, ASAF) Values( " +  age.ToString() + " , " + (j * k).ToString() + " )";
            dataCommand.CommandText = sqltext;
            dataCommand.ExecuteNonQuery();

            // Proceed to next age
            age = age + 1;

            } // wend 
        } //  end run age specific adjustment factors
    
        private void run_illness_calcs(){
            double Beta, PM_val, PM_Impact, cum_impact;
            string sqltext;
            int year, age;
            FbCommand dataCommand = new  FirebirdSql.Data.FirebirdClient.FbCommand();
            FbDataReader dataReader;
            dataCommand.Connection = dbConnection;        
            //GET BETA VALUE FOR USE IN MODEL
            
            //If User specified a Beta value (only an option when Aggregated is selected), use user Beta in model
            if (InputData.getUser_Beta() != 0) {
                Beta = InputData.getUser_Beta();
            //Otherwise if user did not specify a Beta value, then look up the relevant Beta in the study table
            } else {
                sqltext = "SELECT Studies.Study, Studies.Aggregation, Studies.Cause, Studies.Beta FROM Studies";
                sqltext = sqltext + " where Studies.Study = '" + InputData.getUser_Study_Name() + 
                        "' AND Studies.Aggregation = '" + strApproach + "' AND Studies.Cause = '" + Illness_Type + "'";
                dataCommand.CommandText = sqltext;
                dataReader = dataCommand.ExecuteReader();
                dataReader.Read();
                Beta = (double)dataReader[3];
                dataReader.Close();
            } 
        
            //Write illness type and beta value to beta summary table
            sqltext = "INSERT INTO Report_Beta_Summary (Illness_Type, Beta_Value) VALUES('" + Illness_Type + "', " + Beta.ToString() + " ) ";
            dataCommand.CommandText = sqltext;
            dataCommand.ExecuteNonQuery();
                
            //Apply adjustment factor if user input a threshold and beta adjustment
            if (InputData.getPM_Choice() == 1) {
                Beta = InputData.getBeta_adj_factor() * Beta + Beta;

            } 
            //Set variables to beginning values
            year = InputData.getBegin_Year();
            PM_val = 0;
    
            while (year <= InputData.getEnd_Year()) {

                //Look up year-specific PM value from PM_Changes table
                sqltext = "SELECT PM_Changes.Year_Num, PM_Changes.PM_Change FROM PM_Changes";
                sqltext = sqltext + " where PM_Changes.Year_Num = " + year.ToString();
                dataCommand.CommandText = sqltext;
                dataReader = dataCommand.ExecuteReader();
                dataReader.Read();
                
                PM_val = (double)dataReader[1];
                dataReader.Close();
                
                //If user has specified a threshold value, look up t
                if (InputData.getPM_Choice() == 1) {
    
                    //Look up threshold value and store as variable t
                    sqltext = "SELECT Thresholds.Year_Num, Thresholds.Threshold FROM Thresholds";
                    sqltext = sqltext + " where Thresholds.Year_Num = " + year.ToString();
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                
                    t = (double)dataReader[1];
                    dataReader.Close();
                } else {
                    t = 1;
                } 
    
                //Calculate PM_Impact
                PM_Impact = ((System.Math.Exp(Beta * PM_val)) - 1) * t;
    
                //Write values to MHAF_Calcs table
                sqltext = "INSERT INTO MHAF_Calcs (Year_Num, Beta, PM_Impact, Type) Values(" + year.ToString() 
                    + " , " + Beta.ToString() + " , " + PM_Impact.ToString()  + " , '" + Illness_Type + "' )";
                dataCommand.CommandText = sqltext;
                dataCommand.ExecuteNonQuery();
            
                //Proceed to next year
                year = year + 1;
    
                } // wend 


            //Reset counters
            j = 1;
    
            while (j <= 61) {

                //Reset counters
                year = 1990;
    
                while (year <= 2050) {
                    k = year - (j - 1);
                    if ((k > InputData.getEnd_Year()) || (k < InputData.getBegin_Year())) {
                        PM_Impact = 0;
                    } else {
                        //Look up year-specific value from MHAF_Calcs table
                        sqltext = "SELECT MHAF_Calcs.Year_Num, MHAF_Calcs.PM_Impact, MHAF_Calcs.Type FROM MHAF_Calcs";
                        sqltext = sqltext + " where MHAF_Calcs.Year_Num = " + k.ToString() + " AND MHAF_Calcs.Type = '" + Illness_Type + "' ";
                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        PM_Impact = (double)dataReader[1];
                        dataReader.Close();
                    } 
        
                    //Write values to PM_Impact_Vectors table
                    sqltext = "INSERT INTO PM_Impact_Vectors (Year_Num, Year_after_PM_Change, PM_Impact, Type) Values( " 
                        + year.ToString() + " , " + j + " , " + PM_Impact.ToString() + " , '" + Illness_Type + "' )";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
                    year = year + 1;
                } // wend 
                j = j + 1;
            } // wend 
            //Reset counters
            year = 1990;
            //If user selected the single lag option, use the single lag type
            if (InputData.getLag_Type() == 0) { // Single Lag Type
                strLag_Type = "Single";
            //Otherwise, choose the relevant multi-cause lag given the current illness type
            } else {
                if (Illness_Type == "All-Cause") {
                    strLag_Type = "Single";
                }else if (Illness_Type == "Lung Cancer") {
                    strLag_Type = "Multiple-LungCancer";
                }else if (Illness_Type == "All Other Causes") {
                    strLag_Type = "Multiple-AllOtherCauses";
                }else {
                    strLag_Type = "Multiple-Cardiopulmonary";
                } 
            } 
            while (year <= 2050) {
                //Reset counters
                j = 1;
                k = 0;
                cum_impact = 0;
    
                while (j <= 61){
        
                    //Look up year-specific value from lag table
                    sqltext = "SELECT lag.Year_after_PM_Change, lag.Perc_Impacts, lag.Type FROM lag";
                    sqltext = sqltext + " where lag.Year_after_PM_Change = " + j.ToString() + " AND lag.Type = '" + strLag_Type + "'";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    k = (double)dataReader[1];
                    dataReader.Close();

                    //Look up year-specific value from PM_Impact_Vectors table
                    sqltext = "SELECT PM_Impact_Vectors.Year_Num, PM_Impact_Vectors.Year_after_PM_Change, PM_Impact_Vectors.PM_Impact, PM_Impact_Vectors.Type FROM PM_Impact_Vectors";
                    sqltext = sqltext + " where (PM_Impact_Vectors.Year_Num = " + year.ToString() + " AND PM_Impact_Vectors.Year_after_PM_Change = " 
                            + j.ToString() + " AND PM_Impact_Vectors.Type = '" + Illness_Type + "')";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    PM_Impact = (double)dataReader[2];
                    dataReader.Close();
                    
                    cum_impact = cum_impact + (k * PM_Impact);

                    //Advance to next year
                    j = j + 1;
    
                } // wend Loop
    
                //Write final value to table
                sqltext = "INSERT INTO PM_Impact_Vectors_lag (Year_Num, PM_Impact, Type) Values(" + year +
                    " , " + cum_impact + " , '" + Illness_Type + "') ";
                dataCommand.CommandText = sqltext;
                dataCommand.ExecuteNonQuery();
                    
                year = year + 1;

            } // wend Loop

            //Reset year counter
            year = 1990;

            while (year <= 2050) {

                //If year <= 2050 Then
    
                    //m = year
    
                //Years 2051 through 2150 are copies of year 2050
                //Else
    
                    //m = 2050
    
                //End If


                //Look up year-specific value from PM_Impact_Vectors_lag table
                sqltext = "SELECT PM_Impact_Vectors_lag.Year_Num, PM_Impact_Vectors_lag.PM_Impact, PM_Impact_Vectors_lag.Type FROM PM_Impact_Vectors_lag";
                sqltext = sqltext + " where PM_Impact_Vectors_lag.Year_Num = " + year + " AND PM_Impact_Vectors_lag.Type = '" + Illness_Type + "' ";
                dataCommand.CommandText = sqltext;
                dataReader = dataCommand.ExecuteReader();
                dataReader.Read();
                // double check the index here 
                k = (double)dataReader[1];
                dataReader.Close();
    
                //Reset age counter
                age = 0;
    
                while (age <= 100) {
       
                    //Look up age-specific value from ASAF table
                    sqltext = "SELECT ASAF.Age, ASAF.ASAF FROM ASAF";
                    sqltext = sqltext + " where ASAF.Age = " + age.ToString();
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    j = (double)dataReader[1];
                    dataReader.Close();
        
                    //Write values to Hazards table
                    sqltext = "INSERT INTO Hazards_Adj (Age, Year_Num, Type, Hazards_Adj) VALUES(" + age.ToString() + 
                            " , " + year.ToString() + " , '" + Illness_Type + "' , " + (1 + j * k).ToString() + " )";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
                
                    //Proceed to next age
                    age = age + 1;
    
                } // wend Loop
    
            //Proceed to next year
            year = year + 1;
    
            } // wend Loop


            for (GenderCount = 1; GenderCount <= 2; GenderCount++){

                if (GenderCount == 1) {
                    source = "Disease_Rates_RF";
                    gender = "female";
                }else {
                    source = "Disease_Rates_RM";
                    gender = "male";
                }
      
            //Reset year counter
            year = 1990;

            while (year <= 2050) {
    
                //Reset age counter
                age = 0;
    
                while (age <= 100) {
                    //Look up age/year-specific value from source table
                    sqltext = "SELECT " + source + ".Age, " + source + ".Year_Num, " + source + ".Type, " + source + ".Rate FROM " + source;
                    sqltext = sqltext + " where " + source + ".Age = " + age.ToString() + " AND " + source + ".Year_Num = " + year + 
                        " AND " + source + ".Type = '" + Illness_Type_Specific + "'";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    j = (double)dataReader[3];
                    dataReader.Close();
                    
                    //Look up age/year-specific value from hazards table
                    sqltext = "SELECT Hazards_Adj.Age, Hazards_Adj.Year_Num, Hazards_Adj.Type, Hazards_Adj.Hazards_Adj FROM Hazards_Adj";
                    sqltext = sqltext + " where Hazards_Adj.Age = " + age.ToString() + " AND Hazards_Adj.Year_Num = " 
                        + year.ToString() + " AND Hazards_Adj.Type = '" + Illness_Type + "'";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    // double check the index here 
                    k = (double)dataReader[3];
                    dataReader.Close();
           
                    //Write values to Disease Rates Combined table
                    sqltext = "INSERT INTO Disease_Rates_Combined (Age, Year_Num, Gender, Type, Rate) VALUES(" + age.ToString() + 
                        " , " + year.ToString() + " , '" + gender + "', '" + Illness_Type + "' , " + (j * k).ToString() + " ) ";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
                    
                    //Proceed to next age
                    age = age + 1;
    
                    } //wend 
    
            //Proceed to next year
            year = year + 1;

            } //wend 
    
        } // Next GenderCount

        } // end run illness calcs
        private void run_calculate_pdeath()
        {
            // CALCULATE P(DEATH) BY SUMMING PREVIOUS PIECES
            FbCommand dataCommand = new  FirebirdSql.Data.FirebirdClient.FbCommand();
            FbDataReader dataReader;
            dataCommand.Connection = dbConnection;        
            int age, year;
            string sqltext;

            for (GenderCount = 1; GenderCount <=2; GenderCount++){
                if (GenderCount == 1) {
                    destination = "Probability_of_Death_RF";
                    gender = "female";
                }else {
                    destination = "Probability_of_Death_RM";
                    gender = "male";
                } // End If
       
                //Reset age counter
                age = 0;
        
                while (age <= 100) {
           
                    // Reset year counter
                    year = 1990;
    
                    while (year <= 2150) {
    
                        if (year <= 2050) {
        
                            //Look up year-specific value from Disease Rates Combined table
                            sqltext = "SELECT Disease_Rates_Combined.Age, Disease_Rates_Combined.Year_Num, Disease_Rates_Combined.Gender, "
                                + " Sum(Disease_Rates_Combined.Rate) AS SumOfRate FROM Disease_Rates_Combined";
                            sqltext = sqltext + " GROUP BY Disease_Rates_Combined.Age, Disease_Rates_Combined.Year_Num, Disease_Rates_Combined.Gender";
                            sqltext = sqltext + " HAVING (((Disease_Rates_Combined.Age) = " + age.ToString() + " ) AND ((Disease_Rates_Combined.Year_Num) = " 
                                + year.ToString() + " ) AND ((Disease_Rates_Combined.Gender) = '" + gender + "' ))";
                            dataCommand.CommandText = sqltext;
                            dataReader = dataCommand.ExecuteReader();
                            dataReader.Read();
                            k = (double)dataReader[3];
                            dataReader.Close();
                            
                        } // End If
        
                        //Write values to destination table
                        sqltext = "INSERT INTO " + destination + " (Age, Year_Num, Rate) VALUES( " + age.ToString() + " , "
                               + year.ToString() + " , " + k + " )";
                        dataCommand.CommandText = sqltext;
                        dataCommand.ExecuteNonQuery();
                        
                        // Proceed to next year
                        year = year + 1;
                    } // wend Loop
        
                    //Proceed to next age
                    age = age + 1;
        
                } // wend Loop
    
            } //Next GenderCount

        } // end run calculate pdeath
        private void run_calculate_pop_and_death(){
            FbCommand dataCommand = new  FirebirdSql.Data.FirebirdClient.FbCommand();
            FbDataReader dataReader;
            dataCommand.Connection = dbConnection;        
            
            //Set initial values
            int year = 1990;
            double pop = 0;
            double rate = 0;
            double migration = 0;
            double deaths = 0;
            double births = 0;
           

            int maxRunAge = 100; //100 is max
            int maxRunYear = 2050; //2050 is max
            int age;
            int age_mother;
            string sqltext;


            //Repeat this loop for years 1990-2050
            while (year <= maxRunYear)
            {

                //Reset age counter to zero for each year loop
                age = 0;

                //Repeat this loop for ages 0-100
                while (age <= maxRunAge)
                {

                    if (year == 1990)
                    { //Get the census pop info for that age/year pair

                        //Select records from relevant table for that age/year pair
                        sqltext = "SELECT " + strcensus_table + ".Age, " + strcensus_table + ".Year_Num, " +
                            strcensus_table + ".Population FROM " + strcensus_table;
                        sqltext = sqltext + " where Age = " + age.ToString() + " AND Year_Num = " + year.ToString();
                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        pop = (double)dataReader[2];
                        dataReader.Close();
                    }
                    else if (age == 0)
                    { //Calculate births
                        if (strBirth_Type == "Static")
                        {//Just pull Census data as for year 1990
                            //Select records from relevant table for that age/year pair
                            sqltext = "SELECT " + strcensus_table + ".Age, " + strcensus_table + ".Year_Num, "
                                + strcensus_table + ".Population FROM " + strcensus_table;
                            sqltext = sqltext + " where Age = " + age.ToString() + " AND Year_Num = " + year.ToString();
                            dataCommand.CommandText = sqltext;
                            dataReader = dataCommand.ExecuteReader();
                            dataReader.Read();
                            pop = (double)dataReader[2];
                            dataReader.Close();
                        }
                        else
                        { //If Birth_Type = "Dynamic" then calculate expected births
                            //Reset counter to youngest age of childbearing women and reset birth counter to zero
                            age_mother = 12;
                            births = 0;
                            //Loop through all women of childbearing age
                            while (age_mother <= 49)
                            {

                                //Pull mother//s calculated pop value for the previous year, for each age
                                sqltext = "SELECT Final_Table_Pop.Age, Final_Table_Pop.Proj_Year, Final_Table_Pop.Pop, "
                                    + " Final_Table_Pop.Gender, Final_Table_Pop.Scenario FROM Final_Table_Pop";
                                sqltext = sqltext + " where ((Age = (" + age_mother.ToString() + ")) AND (proj_Year = "
                                    + (year-1).ToString() + " ) AND (Gender = 'female') AND (Scenario = '" + strscenarioText + "'))";
                                dataCommand.CommandText = sqltext;
                                dataReader = dataCommand.ExecuteReader();
                                dataReader.Read();
                                pop = (double)dataReader[2];
                                dataReader.Close();

                                if (year <= 2005)
                                {//Use separate male/female birth rates

                                    //Pull the birth rate for the previous year, for each age
                                    sqltext = "SELECT " + strbirth_table + ".Age, " + strbirth_table + ".Year_Num, "
                                        + strbirth_table + ".Rate FROM " + strbirth_table;
                                    sqltext = sqltext + " where Age = " + (age_mother).ToString() + " AND Year_Num = "
                                        + (year - 1).ToString();
                                    dataCommand.CommandText = sqltext;
                                    dataReader = dataCommand.ExecuteReader();
                                    dataReader.Read();
                                    rate = (double)dataReader[2];
                                    dataReader.Close();

                                }
                                else
                                { //Use combined birth rates

                                    //Pull the birth rate for the previous year, for each age
                                    sqltext = "SELECT Age, Year_Num, "
                                        + " Rate FROM RT_BABIES_COMBINED_PST_2005 ";
                                    sqltext = sqltext + " where Age = " + age_mother.ToString() + " AND Year_Num = " + (year - 1).ToString();
                                    dataCommand.CommandText = sqltext;
                                    dataReader = dataCommand.ExecuteReader();
                                    dataReader.Read();
                                    rate = (double)dataReader[2];
                                    dataReader.Close();
                                } //End If

                                //Keep a running count of total births
                                births = births + pop * rate;

                                //Proceed to the next mother age group
                                age_mother = age_mother + 1;

                            } // wend Loop

                            //Pull the F census data for that age/year
                            sqltext = "SELECT Population_F_Table_Census.Age, Population_F_Table_Census.Year_Num, "
                                + " Population_F_Table_Census.Population FROM Population_F_Table_Census";
                            sqltext = sqltext + " where Age = " + age.ToString() + " AND Year_Num = " + year.ToString();
                            dataCommand.CommandText = sqltext;
                            dataReader = dataCommand.ExecuteReader();
                            dataReader.Read();
                            j = (double)dataReader[2];
                            dataReader.Close();

                            //Pull the M census data for that age/year
                            sqltext = "SELECT Population_M_Table_Census.Age, Population_M_Table_Census.Year_Num, "
                                + " Population_M_Table_Census.Population FROM Population_M_Table_Census";
                            sqltext = sqltext + " where Age = " + age.ToString() + " AND Year_Num = " + year.ToString();
                            dataCommand.CommandText = sqltext;
                            dataReader = dataCommand.ExecuteReader();
                            dataReader.Read();
                            k = (double)dataReader[2];
                            dataReader.Close();

                            if ((year > 2005) && (strgenderText == "female"))
                            {
                                births = births * (j / (j + k));
                            }
                            else if ((year > 2005) && (strgenderText == "male"))
                            {
                                births = births * (k / (j + k));
                            }

                            //Look up infant migration for the previous year
                            sqltext = "SELECT " + strinfant_migration_table + ".Year_Num, " + strinfant_migration_table
                                    + ".Pop FROM " + strinfant_migration_table;
                            sqltext = sqltext + " where Year_Num = " + (year - 1).ToString();
                            dataCommand.CommandText = sqltext;
                            dataReader = dataCommand.ExecuteReader();
                            dataReader.Read();
                            m = (double)dataReader[1];
                            dataReader.Close();

                            //Calculate births net of migrations (added because migrations are stored as negative), to get estimate for age 0 pop
                            pop = births + m;

                        } //End If

                    }
                    else
                    { //Use pop/migration/death rate from previous age/year pair to calculate current age/year pop

                        //Select records from final population table for the previous age/year pair
                        sqltext = "SELECT Final_Table_Pop.Age, Final_Table_Pop.Proj_Year, Final_Table_Pop.Pop, "
                            + " Final_Table_Pop.Gender, Final_Table_Pop.Scenario FROM Final_Table_Pop";
                        sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 1)) AND (proj_Year = ("
                            + year.ToString() + " - 1)) AND (Gender = '" + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";
                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        pop = (double)dataReader[2];
                        dataReader.Close();



                        //Select records from death rate table for the previous age/year pair
                        sqltext = "SELECT " + strmortal_table + ".Age, " + strmortal_table + ".Year_Num, " + strmortal_table
                            + ".Rate FROM " + strmortal_table;
                        sqltext = sqltext + " where Age = " + (age - 1).ToString() + " AND Year_Num = " + (year - 1).ToString();
                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        rate = (double)dataReader[2];
                        dataReader.Close();


                        //Select records from migration table for the previous age/year pair
                        sqltext = "SELECT " + strmigration_table + ".Age, " + strmigration_table + ".Year_Num, "
                            + strmigration_table + ".Migration FROM " + strmigration_table;
                        sqltext = sqltext + " where Age = " + (age - 1).ToString() + " AND Year_Num = " + (year - 1).ToString();
                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        migration = (double)dataReader[2];
                        dataReader.Close();

                        pop = pop * (1 - rate) + migration;

                        if (age == 100)
                        {

                            //Select records from death rate table for the same age/previous year pair
                            sqltext = "SELECT " + strmortal_table + ".Age, " + strmortal_table + ".Year_Num, "
                                + strmortal_table + ".Rate FROM " + strmortal_table;
                            sqltext = sqltext + " where Age = " + (age - 0).ToString() + " AND Year_Num = " + (year - 1).ToString();
                            dataCommand.CommandText = sqltext;
                            dataReader = dataCommand.ExecuteReader();
                            dataReader.Read();
                            rate = (double)dataReader[2];
                            dataReader.Close();

                            //Select records from final population table for the same age/previous year pair
                            sqltext = "SELECT Final_Table_Pop.Age, Final_Table_Pop.proj_Year, Final_Table_Pop.Pop, "
                                + "Final_Table_Pop.Gender, Final_Table_Pop.Scenario FROM Final_Table_Pop";
                            sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = ("
                                + year.ToString() + " - 1)) AND (Gender = '" + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";
                            dataCommand.CommandText = sqltext;
                            dataReader = dataCommand.ExecuteReader();
                            dataReader.Read();
                            pop = pop + (double)dataReader[2] * (1 - rate);
                            dataReader.Close();

                        } //End If

                    } // End If

                    //Use Age/Year/Pop to write value in pop table
                    sqltext = "INSERT INTO Final_Table_Pop (Age, Proj_Year, Pop, Gender, Scenario) VALUES( "
                            + age.ToString() + " , " + year.ToString() + " , " + pop.ToString()
                            + " , '" + strgenderText + "' , '" + strscenarioText + "' ) ";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();

                    //CALCULATE TOTAL DEATHS
                    //Select records from death rate table for the same age/same year pair
                    sqltext = "SELECT " + strmortal_table + ".Age, " + strmortal_table + ".Year_Num, " + strmortal_table
                        + ".Rate FROM " + strmortal_table;
                    sqltext = sqltext + " where Age = " + (age - 0).ToString() + " AND Year_Num = " + (year - 0).ToString();
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    rate = (double)dataReader[2];
                    dataReader.Close();

                    deaths = pop * rate;

                    //Use Age/Year/Pop to write value in deaths table
                    sqltext = "INSERT INTO Final_Table_Deaths (Age, Proj_Year, Deaths, Gender, Scenario) VALUES( "
                        + age.ToString() + " , " + year.ToString() + " , " + deaths.ToString()
                        + " , '" + strgenderText + "' , '" + strscenarioText + "' ) ";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();

                    //Proceed to next age group in given year
                    age = age + 1;

                } // wend Loop

                //Proceed to next year
                year = year + 1;
            } // wend loop
		} // end run calculate pop and death
        private void run_calculate_life_expectancy()
        {
            FbCommand dataCommand = new  FirebirdSql.Data.FirebirdClient.FbCommand();
            FbDataReader dataReader;
            dataCommand.Connection = dbConnection;        
            
            
            //CALCULATE COHORT LY
            //Set initial values
            int year = 1990;
            double pop, rate, m;
            int age;
            string sqltext;
            
            year = 1990;
            pop = 0;
            rate = 0;
            m = 0;
            
            //Repeat this loop for years 1990-2150
            while (year <= 2150) {

                //Reset age counter to zero for each year loop
                age = 0;
    
                //Repeat this loop for ages 0-100
                while (age <= 100) {
               
                    if ((year == 1990) || ((age == 0) &&  (year <= 2050))) {
                        //Select records from pop table for that age/year pair
                        sqltext = "SELECT Final_Table_Pop.Age, Final_Table_Pop.Proj_Year, Final_Table_Pop.Pop, Final_Table_Pop.gender, " 
                        + " Final_Table_Pop.scenario FROM Final_Table_Pop";
                        sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" 
                            + year.ToString() + " - 0)) AND (Gender = '" + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";
                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        pop = (double)dataReader[2];
                        dataReader.Close();
                        m = pop;
            
                    }else if ((age == 0) && (year > 2050)) {
                        m = 0;
                    }else {
        
                        //Select records from cohort LY table for the previous age/year pair
                        sqltext = "SELECT Final_Table_Cohort_LY.Age, Final_Table_Cohort_LY.Proj_Year, Final_Table_Cohort_LY.Val, "
                            + "Final_Table_Cohort_LY.Gender, Final_Table_Cohort_LY.Scenario FROM Final_Table_Cohort_LY";
                        sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 1)) AND (proj_Year = (" + year.ToString() 
                            + " - 1)) AND (Gender = '" + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";
                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        pop = (double)dataReader[2];
                        dataReader.Close();

                        //Select records from death rate table for the previous age/year pair
                        sqltext = "SELECT " + strmortal_table + ".Age, " + strmortal_table + ".Year_Num, " + strmortal_table 
                            + ".Rate FROM " + strmortal_table;
                        sqltext = sqltext + " where Age = " + (age - 1).ToString() + " AND Year_Num = " + (year - 1).ToString();
                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        rate  = (double)dataReader[2];
                        dataReader.Close();

                        m = pop * (1.0 - rate);
            
                    } //End If

                    //Write value in LY table
                    sqltext = "INSERT INTO Final_Table_Cohort_LY (Age, Proj_Year, Val, Gender, Scenario) VALUES( " 
                        + age.ToString() + " , " + year.ToString() + " , " + m.ToString() + " , '" + strgenderText 
                        + "' , '" + strscenarioText + "' )";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
        
                    //Proceed to next age group in given year
                    age = age + 1;
    
                } //wend Loop
    
                //Proceed to next year
                year = year + 1;
    
            } // wend Loop
            Debug.Print("End Cohort_LY loop");

            //CALCULATE COHORT ELY
            //Set initial values
            year = 2150;
            j = 0;
            m = 0;

            //Repeat this loop for years 1990-2150, backwards
            for (year = 2150; year >= 1990; year--) {

                //Reset age counter to one hundred for each year loop
                age = 100;
    
                //Repeat this loop for ages 0-100, backwards
                for (age = 100; age >= 0; age--) {
               
                    if ((year == 2150) || (age == 100) ){
                        m = 0;
                    } else {
        
                        //Select records from ELY table for the future age/year pair
                        sqltext = "SELECT Final_Table_Cohort_ELY.Age, Final_Table_Cohort_ELY.Proj_Year, Final_Table_Cohort_ELY.Val, " 
                            + "Final_Table_Cohort_ELY.Gender, Final_Table_Cohort_ELY.Scenario FROM Final_Table_Cohort_ELY";
                        sqltext = sqltext + " where ((Age = (" + age.ToString() + " + 1)) AND (proj_Year = (" 
                            + year.ToString() + " + 1)) AND (Gender = '" + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";
                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        m = (double)dataReader[2];
                        dataReader.Close();

                    } //End If

                    //Select records from LY table for the same age/year pair
                    sqltext = "SELECT Final_Table_Cohort_LY.Age, Final_Table_Cohort_LY.Proj_Year, Final_Table_Cohort_LY.Val, " 
                        + "Final_Table_Cohort_LY.Gender, Final_Table_Cohort_LY.Scenario FROM Final_Table_Cohort_LY";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " + 0)) AND (proj_Year = (" 
                        + year.ToString() + " + 0)) AND (Gender = '"  + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    j = (double)dataReader[2];
                    dataReader.Close();

                    //Write value in ELY table
                    sqltext = "INSERT INTO Final_Table_Cohort_ELY (Age, Proj_Year, Val, Gender, Scenario) VALUES( " 
                        + age.ToString() + " , " + year.ToString() + " , " + (j + m).ToString() 
                        + " , '" + strgenderText + "' , '" + strscenarioText + "' )";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
        
                //Proceed to next age group in given year
                } // Next age
    
            //Proceed to next year
            } // Next year
            Debug.Print("End Cohort_ELY loop");

            //CALCULATE COHORT CLE
            //Set initial values
            year = 1990;
            j = 0;
            m = 0;

            //Repeat this loop for years 1990-2050
            while (year <= 2050) {

                //Reset age counter to zero for each year loop
                age = 0;
    
                //Repeat this loop for ages 0-100
                while (age <= 100) {
        
                    //Select records from cohort LY table for the same age/year pair
                    sqltext = "SELECT Final_Table_Cohort_LY.Age, Final_Table_Cohort_LY.Proj_Year, Final_Table_Cohort_LY.Val, "
                    + "Final_Table_Cohort_LY.Gender, Final_Table_Cohort_LY.Scenario FROM Final_Table_Cohort_LY";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" 
                        + year.ToString() + " - 0)) AND (Gender = '" + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    j = (double)dataReader[2];
                    dataReader.Close();

                    //Select records from cohort ELY table for the same age/year pair
                    sqltext = "SELECT Final_Table_Cohort_ELY.Age, Final_Table_Cohort_ELY.Proj_Year, Final_Table_Cohort_ELY.Val, "
                        + "Final_Table_Cohort_ELY.Gender, Final_Table_Cohort_ELY.Scenario FROM Final_Table_Cohort_ELY";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString()
                        + " - 0)) AND (Gender = '" + strgenderText + "') AND (Scenario = '" +  strscenarioText + "'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    m = (double)dataReader[2];
                    dataReader.Close();
                   
                    //Write value in CLE table
                    sqltext = "INSERT INTO Final_Table_Cohort_CLE (Age, Proj_Year, Val, Gender, Scenario) VALUES(" 
                        + age.ToString() + " , " + year.ToString() + " , " + (m / j).ToString() + " , '" + strgenderText 
                        + "' , '" + strscenarioText + "' )";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
        
                    //Proceed to next age group in given year
                    age = age + 1;
    
                } // wend Loop
    
                //Proceed to next year
                year = year + 1;
    
            } // wend Loop


            //CALCULATE PERIOD LY
            //Set initial values
            year = 1990;
            pop = 0;
            rate = 0;
            m = 0;

            //Repeat this loop for years 1990-2050
            while (year <= 2050) {
                //Reset age counter to zero for each year loop
                age = 0;
    
                //Repeat this loop for ages 0-100
                while (age <= 100) {
               
                    if (age == 0) {
        
                        //Select records from pop table for that age/year pair
                        sqltext = "SELECT Final_Table_Pop.Age, Final_Table_Pop.Proj_Year, Final_Table_Pop.Pop, Final_Table_Pop.gender,"
                            + "Final_Table_Pop.scenario FROM Final_Table_Pop";
                        sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" 
                            + year.ToString()  + " - 0)) AND (Gender = '" + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";
                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        pop = (double)dataReader[2];
                        dataReader.Close();
                   
                        m = pop;
            
                    } else {
        
                        //Select records from period LY table for the previous age/same year pair
                        sqltext = "SELECT Final_Table_Period_LY.Age, Final_Table_Period_LY.Proj_Year, Final_Table_Period_LY.Val, " 
                            + "Final_Table_Period_LY.Gender, Final_Table_Period_LY.Scenario FROM Final_Table_Period_LY";
                        sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 1)) AND (proj_Year = (" 
                            + year.ToString() + " - 0)) AND (Gender = '"  + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";
                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        pop = (double)dataReader[2];
                        dataReader.Close();
                   
                        //Select records from death rate table for the previous age/same year pair
                        sqltext = "SELECT " + strmortal_table + ".Age, " + strmortal_table + ".Year_Num, " + strmortal_table + ".Rate FROM " + strmortal_table;
                        sqltext = sqltext + " where Age = " + (age - 1).ToString() + " AND Year_Num = " + (year - 0).ToString();
                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        rate = (double)dataReader[2];
                        dataReader.Close();
                        
                        m = pop * (1 - rate);
            
                    } //End If

                    //Write value in LY table
                    sqltext = "INSERT INTO Final_Table_Period_LY (Age, Proj_Year, Val, Gender, Scenario) VALUES(" 
                        + age.ToString() + " , " + year.ToString() + " , " + m.ToString() + " , '" 
                        + strgenderText + "' , '" + strscenarioText + "') ";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
        
                    //Proceed to next age group in given year
                    age = age + 1;
    
                } //Loop
    
                //Proceed to next year
                year = year + 1;
    
            } // Loop


            //CALCULATE PERIOD ELY
            //Set initial values
            year = 2050;
            j = 0;
            m = 0;

            //Repeat this loop for years 1990-2150, backwards
            for (year = 2050; year >= 1990; year--) {

                //Reset age counter to one hundred for each year loop
                age = 100;
    
                //Repeat this loop for ages 0-100, backwards
                for (age = 100; age >= 0; age--) {
               
                    if (age == 100) {
    
                        m = 0;
        
                    } else {
        
                        //Select records from ELY table for the future age/same year pair
                        sqltext = "SELECT Final_Table_Period_ELY.Age, Final_Table_Period_ELY.Proj_Year, Final_Table_Period_ELY.Val, " 
                            + "Final_Table_Period_ELY.Gender, Final_Table_Period_ELY.Scenario FROM Final_Table_Period_ELY";
                        sqltext = sqltext + " where ((Age = (" + age.ToString() + " + 1)) AND (proj_Year = (" 
                            + year.ToString() + " + 0)) AND (Gender = '" + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";

                        dataCommand.CommandText = sqltext;
                        dataReader = dataCommand.ExecuteReader();
                        dataReader.Read();
                        m = (double)dataReader[2];
                        dataReader.Close();
                    } // End If

                    //Select records from LY table for the same age/year pair
                    sqltext = "SELECT Final_Table_Period_LY.Age, Final_Table_Period_LY.Proj_Year, Final_Table_Period_LY.Val, "
                        + "Final_Table_Period_LY.Gender, Final_Table_Period_LY.Scenario FROM Final_Table_Period_LY";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " + 0)) AND (proj_Year = (" + year.ToString() 
                        + " + 0)) AND (Gender = '" + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    j = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Write value in ELY table
                    sqltext = "INSERT INTO Final_Table_Period_ELY (Age, Proj_Year, Val, Gender, Scenario) VALUES(" 
                        + age.ToString() + " , " + year.ToString() + " , " + (j + m).ToString() + " , '" 
                        + strgenderText + "' , '" + strscenarioText + "' )";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
        
                //Proceed to next age group in given year
                } //Next age
    
            //Proceed to next year
            } //Next year


            //CALCULATE PERIOD CLE
            //Set initial values
            year = 1990;
            j = 0;
            m = 0;

            //Repeat this loop for years 1990-2050
            while (year <= 2050) {

                //Reset age counter to zero for each year loop
                age = 0;
    
                //Repeat this loop for ages 0-100
                while (age <= 100) {
        
                    //Select records from period LY table for the same age/year pair
                    sqltext = "SELECT Final_Table_Period_LY.Age, Final_Table_Period_LY.Proj_Year, Final_Table_Period_LY.Val, "
                        + "Final_Table_Period_LY.Gender, Final_Table_Period_LY.Scenario FROM Final_Table_Period_LY";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() 
                        + " - 0)) AND (Gender = '" + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    j = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Select records from period ELY table for the same age/year pair
                    sqltext = "SELECT Final_Table_Period_ELY.Age, Final_Table_Period_ELY.Proj_Year, Final_Table_Period_ELY.Val, "
                        + "Final_Table_Period_ELY.Gender, Final_Table_Period_ELY.Scenario FROM Final_Table_Period_ELY";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() 
                        + " - 0)) AND (Gender = '" + strgenderText + "') AND (Scenario = '" + strscenarioText + "'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    m = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Write value in CLE table
                    sqltext = "INSERT INTO Final_Table_Period_CLE (Age, Proj_Year, Val, Gender, Scenario) VALUES(" 
                        + age.ToString() + " , " + year.ToString() + " , " + (m / j).ToString() + " , '" + strgenderText 
                        + "' , '" + strscenarioText + "' )";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
        
                    //Proceed to next age group in given year
                    age = age + 1;
    
                    } //wend Loop
    
                //Proceed to next year
                year = year + 1;
    
                } //wend Loop
        } // end run calculate life expectancy
        private void run_summarize_results()
        {
            //CREATE INPUTS REPORT
            //create loop for all variables

            string tempValue = "";
            string tempCategory= "";
            string sqltext;
            int I;

            FbCommand dataCommand = new  FirebirdSql.Data.FirebirdClient.FbCommand();
            FbDataReader dataReader;
            dataCommand.Connection = dbConnection;        
            

            //approach = "Disaggregated"
            //begin_year = 1990
            //end_year = 2020

            for (I = 1; I <= 44; I++) {

                switch(I) {
                    case 1:
                        tempValue = InputData.getBegin_Year().ToString();
                        tempCategory = "Begin Year";
                        break;
                    case 2:
                        tempValue = InputData.getEnd_Year().ToString();
                        tempCategory = "End Year";
                        break;
                    case 3:
                        if (InputData.getApproach() == 0)
                        {
                            tempValue = "Aggregated";
                        }
                        else
                        {
                            tempValue = "Disaggregated";
                        }
                        tempCategory = "Dose-Response Technique";
                        break;
                    case 4:
                        if (InputData.getBeta_Type() == 0)
                        {
                            tempValue = "Study beta";
                        }
                        else
                        {
                            tempValue = "User-defined beta";
                        }
                        tempCategory = "Beta Type";
                        break;
                    case 5:
                        tempValue = InputData.getUser_Study_Name();
                        tempCategory = "Study";
                        break;
                    case 6:
                        tempValue = InputData.getUser_Beta().ToString();
                        tempCategory = "User Beta";
                        break;
                    case 7:
                        tempValue = InputData.getPM_year_1().ToString();
                        tempCategory = "PM Change: Year 1";
                        break;
                    case 8:
                        tempValue = InputData.getPM_year_2().ToString();
                        tempCategory = "PM Change: Year 2";
                        break;
                    case 9:
                        tempValue = InputData.getPM_year_3().ToString();
                        tempCategory = "PM Change: Year 3";
                        break;
                    case 10:
                        tempValue = InputData.getPM_year_4().ToString();
                        tempCategory = "PM Change: Year 4";
                        break;
                    case 11:
                        tempValue = InputData.getPM_year_5().ToString();
                        tempCategory = "PM Change: Year 5";
                        break;
                    case 12:
                        tempValue = InputData.getPM_val_1().ToString();
                        tempCategory = "PM Change: Value 1";
                        break;
                    case 13:
                        tempValue = InputData.getPM_val_2().ToString();
                        tempCategory = "PM Change: Value 2";
                        break;
                    case 14:
                        tempValue = InputData.getPM_val_3().ToString();
                        tempCategory = "PM Change: Value 3";
                        break;
                    case 15:
                        tempValue = InputData.getPM_val_4().ToString();
                        tempCategory = "PM Change: Value 4";
                        break;
                    case 16:
                        tempValue = InputData.getPM_val_5().ToString();
                        tempCategory = "PM Change: Value 5";
                        break;
                    case 17:
                        if (InputData.getPM_Trajectory() == 0)
                        {
                            tempValue = "Linear";
                        }
                        else
                        {
                            tempValue = "Step";
                        }
                        tempCategory = "PM Trajectory";
                        break;
                    case 18:
                        if (InputData.getPM_Choice() == 0)
                        {
                            tempValue = "No PM Threshold";
                        }
                        else
                        {
                            tempValue = "PM Threshold";
                        }
                        tempCategory = "PM Threshold";
                        break;
                    case 19:
                        tempValue = InputData.getPM_Threshold().ToString();
                        tempCategory = "PM Threshold";
                        break;
                    case 20:
                        tempValue = InputData.getBeta_adj_factor().ToString();
                        tempCategory = "Beta Adjustment Factor";
                        break;
                    case 21:
                        tempValue = InputData.getAge_Range_Start().ToString();
                        tempCategory = "Age Range Affected: Start Age";
                        break;
                    case 22:
                        tempValue = InputData.getAge_Range_End().ToString();
                        tempCategory = "Age Range Affected: End Age";
                        break;
                    case 23:
                        tempValue = InputData.getSub_Pop_Start_1().ToString();
                        tempCategory = "Age-Specific Adjustment: Start Age 1";
                        break;
                    case 24:
                        tempValue = InputData.getSub_Pop_Start_2().ToString();
                        tempCategory = "Age-Specific Adjustment: Start Age 2";
                        break;
                    case 25:
                        tempValue = InputData.getSub_Pop_Start_3().ToString();
                        tempCategory = "Age-Specific Adjustment: Start Age 3";
                        break;
                    case 26:
                        tempValue = InputData.getSub_Pop_Start_4().ToString();
                        tempCategory = "Age-Specific Adjustment: Start Age 4";
                        break;
                    case 27:
                        tempValue = InputData.getSub_Pop_Start_5().ToString();
                        tempCategory = "Age-Specific Adjustment: Start Age 5";
                        break;
                    case 28:
                        tempValue = InputData.getSub_Pop_End_1().ToString();
                        tempCategory = "Age-Specific Adjustment: End Age 1";
                        break;
                    case 29:
                        tempValue = InputData.getSub_Pop_End_2().ToString();
                        tempCategory = "Age-Specific Adjustment: End Age 2";
                        break;
                    case 30:
                        tempValue = InputData.getSub_Pop_End_3().ToString();
                        tempCategory = "Age-Specific Adjustment: End Age 3";
                        break;
                    case 31:
                        tempValue = InputData.getSub_Pop_End_4().ToString();
                        tempCategory = "Age-Specific Adjustment: End Age 4";
                        break;
                    case 32:
                        tempValue = InputData.getSub_Pop_End_5().ToString();
                        tempCategory = "Age-Specific Adjustment: End Age 5";
                        break;
                    case 33:
                        tempValue = InputData.getSub_Pop_Adjustment_1().ToString();
                        tempCategory = "Age-Specific Adjustment: Factor 1";
                        break;
                    case 34:
                        tempValue = InputData.getSub_Pop_Adjustment_2().ToString();
                        tempCategory = "Age-Specific Adjustment: Factor 2";
                        break;
                    case 35:
                        tempValue = InputData.getSub_Pop_Adjustment_3().ToString();
                        tempCategory = "Age-Specific Adjustment: Factor 3";
                        break;
                    case 36:
                        tempValue = InputData.getSub_Pop_Adjustment_4().ToString();
                        tempCategory = "Age-Specific Adjustment: Factor 4";
                        break;
                    case 37:
                        tempValue = InputData.getSub_Pop_Adjustment_5().ToString();
                        tempCategory = "Age-Specific Adjustment: Factor 5";
                        break;
                    case 38:
                        if (InputData.getLag_Type() == 0)
                        {
                            tempValue = "Single Lag";
                        }
                        else
                        {
                            tempValue = "Cause-Specific Lag";
                        }
                        tempCategory = "Lag Type";
                        break;
                    case 39:
                        if (InputData.getLag_Function_Type() == 0) 
                        { 
                            tempValue = "HES_Default";
                        }else if (InputData.getLag_Function_Type() == 1)
                        {
                            tempValue= "Smooth";
                        } else 
                        {
                            tempValue = "User-defined";
                        }
                        tempCategory = "Lag Function Type";
                        break;
                    case 40:
                        tempValue = InputData.getLag_k_single().ToString();
                        tempCategory = "Single-lag k";
                        break;
                    case 41:
                        tempValue = InputData.getLag_k_multiple_cardio().ToString();
                        tempCategory = "Cause-specific lag cardio-k";
                        break;
                    case 42:
                        tempValue = InputData.getLag_k_multiple_lung().ToString();
                        tempCategory = "Cause-specific lag lung-k";
                        break;
                    case 43:
                        tempValue = InputData.getLag_k_multiple_other().ToString();
                        tempCategory = "Cause-specific lag all other-k";
                        break;
                    case 44:
                        if(InputData.getBirth_Type() == 0) {
                            tempValue = "Dynamic";
                        } else {
                            tempValue = "Static";
                        }
                        tempCategory = "Birth Type";
                        break;
                    default:
                        tempValue = "Unknown";
                        tempCategory = "Unknown";
                        break;
                } //End Select
    
                sqltext = "UPDATE Report_Input_Summary SET Report_Input_Summary.User_Selection = '" + tempValue + "' WHERE ((category='" + tempCategory + "'))";
                dataCommand.CommandText = sqltext;
                dataCommand.ExecuteNonQuery();
        
            } // Next I


            //CALCULATE AVOIDED DEATHS
            //Set initial values
            int year = 1990;
            int age;
            j = 0;
            k = 0;
            m = 0;
            p = 0;

            //Repeat this loop for years 1990-2050
            while (year <= 2050) {

                //Reset age counter to zero for each year loop
                age = 0;
    
                //Repeat this loop for ages 0-100
                while (age <= 100) {
               
                    //Select BASELINE FEMALE deaths from the final deaths table
                    sqltext = "SELECT Final_Table_Deaths.Age, Final_Table_Deaths.Proj_Year, Final_Table_Deaths.Deaths, " 
                        + " Final_Table_Deaths.Gender, Final_Table_Deaths.Scenario FROM Final_Table_Deaths";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() 
                        + " - 0)) AND (Gender = 'female') AND (Scenario = 'Baseline'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    j = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Select REGULATORY FEMALE deaths from the final deaths table
                    sqltext = "SELECT Final_Table_Deaths.Age, Final_Table_Deaths.Proj_Year, Final_Table_Deaths.Deaths, "
                        + " Final_Table_Deaths.Gender, Final_Table_Deaths.Scenario FROM Final_Table_Deaths";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() + " - 0)) AND (Gender = 'female') AND (Scenario = 'Regulatory'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    k = (double)dataReader[2];
                    dataReader.Close();
        
                    //Select BASELINE MALE deaths from the final deaths table
                    sqltext = "SELECT Final_Table_Deaths.Age, Final_Table_Deaths.Proj_Year, Final_Table_Deaths.Deaths, "
                        + " Final_Table_Deaths.Gender, Final_Table_Deaths.Scenario FROM Final_Table_Deaths";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() + " - 0)) AND (Gender = 'male') AND (Scenario = 'Baseline'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    m = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Select REGULATORY MALE deaths from the final deaths table
                    sqltext = "SELECT Final_Table_Deaths.Age, Final_Table_Deaths.Proj_Year, Final_Table_Deaths.Deaths, " 
                        + " Final_Table_Deaths.Gender, Final_Table_Deaths.Scenario FROM Final_Table_Deaths";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() + " - 0)) AND (Gender = 'male') AND (Scenario = 'Regulatory'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    p = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Write value into report table
                    sqltext = "INSERT INTO Report_Avoided_Deaths (Age, Year_Num, Avoided_Deaths) VALUES( " + age.ToString() 
                        + " , " + year.ToString() + " , " + System.Math.Round(((j - k) + (m - p)), 0).ToString() + " )";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
        
        
                    //Proceed to next age group in given year
                    age = age + 1;
    
                } // for age Loop
    
                //Proceed to next year
                year = year + 1;
    
            } // for year Loop


            //CALCULATE LIFE YEARS GAINED
            //Set initial values
            year = 1990;
            j = 0;
            k = 0;
            m = 0;
            p = 0;

            //Repeat this loop for years 1990-2050
            while (year <= 2050) {

                //Reset age counter to zero for each year loop
                age = 0;
    
                //Repeat this loop for ages 0-100
                while (age <= 100) {
               
                    //Select REGULATORY FEMALE pop from the final pop table
                    sqltext = "SELECT Final_Table_Pop.Age, Final_Table_Pop.Proj_Year, Final_Table_Pop.Pop, Final_Table_Pop.Gender, "
                        + " Final_Table_Pop.Scenario FROM Final_Table_Pop";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() 
                        + " - 0)) AND (Gender = 'female') AND (Scenario = 'Regulatory'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    j = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Select BASELINE FEMALE pop from the final pop table
                    sqltext = "SELECT Final_Table_Pop.Age, Final_Table_Pop.Proj_Year, Final_Table_Pop.Pop, Final_Table_Pop.Gender, "
                        + " Final_Table_Pop.Scenario FROM Final_Table_Pop";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" 
                        + year.ToString() + " - 0)) AND (Gender = 'female') AND (Scenario = 'Baseline'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    k = (double)dataReader[2];
                    dataReader.Close();
                                        
                    //Select REGULATORY MALE pop from the final pop table
                    sqltext = "SELECT Final_Table_Pop.Age, Final_Table_Pop.Proj_Year, Final_Table_Pop.Pop, Final_Table_Pop.Gender, " 
                        + "Final_Table_Pop.Scenario FROM Final_Table_Pop";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" 
                        + year.ToString() + " - 0)) AND (Gender = 'male') AND (Scenario = 'Regulatory'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    m = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Select BASELINE MALE pop from the final pop table
                    sqltext = "SELECT Final_Table_Pop.Age, Final_Table_Pop.Proj_Year, Final_Table_Pop.Pop, Final_Table_Pop.Gender, "
                        + " Final_Table_Pop.Scenario FROM Final_Table_Pop";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" 
                        + year.ToString() + " - 0)) AND (Gender = 'male') AND (Scenario = 'Baseline'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    p = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Write value into report table
                    sqltext = "INSERT INTO Report_Life_Years_Gained (Age, Year_Num, Life_Years_Gained) VALUES( " 
                        + age.ToString() + " , " + year.ToString() + " , " + System.Math.Round(((j - k) + (m - p)), 0).ToString() + " )";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
        
                    //Proceed to next age group in given year
                    age = age + 1;
    
                } // age Loop
    
                //Proceed to next year
                year = year + 1;
    
            } // year Loop


            //CALCULATE INCREASE IN COHORT CONDITIONAL LIFE EXPECTANCY
            //Set initial values
            year = 1990;
            j = 0;
            k = 0;
            m = 0;
            p = 0;

            //Repeat this loop for years 1990-2050
            while (year <= 2050) {

                //Reset age counter to zero for each year loop
                age = 0;
    
                //Repeat this loop for ages 0-100
                while (age <= 100) {
               
                    //Select REGULATORY FEMALE CLE
                    sqltext = "SELECT Final_Table_Cohort_CLE.Age, Final_Table_Cohort_CLE.Proj_Year, Final_Table_Cohort_CLE.Val, "
                        + " Final_Table_Cohort_CLE.Gender, Final_Table_Cohort_CLE.Scenario FROM Final_Table_Cohort_CLE";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" 
                        + year.ToString() + " - 0)) AND (Gender = 'female') AND (Scenario = 'Regulatory'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    j = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Select BASELINE FEMALE CLE
                    sqltext = "SELECT Final_Table_Cohort_CLE.Age, Final_Table_Cohort_CLE.Proj_Year, Final_Table_Cohort_CLE.Val, "
                        + " Final_Table_Cohort_CLE.Gender, Final_Table_Cohort_CLE.Scenario FROM Final_Table_Cohort_CLE";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() 
                        + " - 0)) AND (Gender = 'female') AND (Scenario = 'Baseline'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    k = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Select REGULATORY MALE CLE
                    sqltext = "SELECT Final_Table_Cohort_CLE.Age, Final_Table_Cohort_CLE.Proj_Year, Final_Table_Cohort_CLE.Val, "
                        + " Final_Table_Cohort_CLE.Gender, Final_Table_Cohort_CLE.Scenario FROM Final_Table_Cohort_CLE";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() 
                        + " - 0)) AND (Gender = 'male') AND (Scenario = 'Regulatory'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    m = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Select BASELINE MALE CLE
                    sqltext = "SELECT Final_Table_Cohort_CLE.Age, Final_Table_Cohort_CLE.Proj_Year, Final_Table_Cohort_CLE.Val, "
                        + " Final_Table_Cohort_CLE.Gender, Final_Table_Cohort_CLE.Scenario FROM Final_Table_Cohort_CLE";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() 
                        + " - 0)) AND (Gender = 'male') AND (Scenario = 'Baseline'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    p = (double)dataReader[2];
                    dataReader.Close();
                    

                    //Write value into report table
                    sqltext  = "INSERT INTO RPT_INC_Cohort_Cond_Life_Exp (Age, Year_Num, Increase_Female, Increase_Male) VALUES( " 
                        + age.ToString() + " , " + year.ToString() + " , " + System.Math.Round((j - k), 2).ToString() + " , " 
                        + System.Math.Round((m - p), 2) + " )";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
        
                    //Proceed to next age group in given year
                    age = age + 1;
    
                } // ageLoop
    
                //Proceed to next year
                year = year + 1;
    
        } // year Loop


            //CALCULATE INCREASE IN PERIOD CONDITIONAL LIFE EXPECTANCY
            //Set initial values
            year = 1990;
            j = 0;
            k = 0;
            m = 0;
            p = 0;

            //Repeat this loop for years 1990-2050
            while (year <= 2050) {

                //Reset age counter to zero for each year loop
                age = 0;
    
                //Repeat this loop for ages 0-100
                while (age <= 100){
               
                    //Select REGULATORY FEMALE CLE
                    sqltext = "SELECT Final_Table_Period_CLE.Age, Final_Table_Period_CLE.Proj_Year, Final_Table_Period_CLE.Val, "
                        + " Final_Table_Period_CLE.Gender, Final_Table_Period_CLE.Scenario FROM Final_Table_Period_CLE";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() 
                        + " - 0)) AND (Gender = 'female') AND (Scenario = 'Regulatory'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    j = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Select BASELINE FEMALE CLE
                    sqltext = "SELECT Final_Table_Period_CLE.Age, Final_Table_Period_CLE.Proj_Year, Final_Table_Period_CLE.Val, "
                        + " Final_Table_Period_CLE.Gender, Final_Table_Period_CLE.Scenario FROM Final_Table_Period_CLE";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() 
                        + " - 0)) AND (Gender = 'female') AND (Scenario = 'Baseline'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    k = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Select REGULATORY MALE CLE
                    sqltext = "SELECT Final_Table_Period_CLE.Age, Final_Table_Period_CLE.Proj_Year, Final_Table_Period_CLE.Val, "
                        + " Final_Table_Period_CLE.Gender, Final_Table_Period_CLE.Scenario FROM Final_Table_Period_CLE";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() 
                        + " - 0)) AND (Gender = 'male') AND (Scenario = 'Regulatory'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    m = (double)dataReader[2];
                    dataReader.Close();
                    

                    //Select BASELINE MALE CLE
                    sqltext = "SELECT Final_Table_Period_CLE.Age, Final_Table_Period_CLE.Proj_Year, Final_Table_Period_CLE.Val, "
                        + " Final_Table_Period_CLE.Gender, Final_Table_Period_CLE.Scenario FROM Final_Table_Period_CLE";
                    sqltext = sqltext + " where ((Age = (" + age.ToString() + " - 0)) AND (proj_Year = (" + year.ToString() 
                        + " - 0)) AND (Gender = 'male') AND (Scenario = 'Baseline'))";
                    dataCommand.CommandText = sqltext;
                    dataReader = dataCommand.ExecuteReader();
                    dataReader.Read();
                    p = (double)dataReader[2];
                    dataReader.Close();
                    
                    //Write value into report table
                    sqltext = "INSERT INTO RPT_Inc_PRD_Cond_Life_Exp (Age, Year_Num, Increase_Female, Increase_Male) VALUES( " 
                        + age + " , " + year + " , " + System.Math.Round((j - k), 2) + " , " 
                        + System.Math.Round((m - p), 2) + " )";
                    dataCommand.CommandText = sqltext;
                    dataCommand.ExecuteNonQuery();
        
                    //Proceed to next age group in given year
                    age = age + 1;
    
                } //wend Loop
    
                //Proceed to next year
                year = year + 1;
    
        } // wend Loop
        } // end run summarize results
    } // end popsim model
} // end namespace