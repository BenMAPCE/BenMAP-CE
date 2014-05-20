using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

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
        private double q;
        private double t;
       
        private string Illness_Type;
        private string Illness_Type_Specific;
        private int IllnessCount;
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
        
        PopSimInputData InputData = new PopSimInputData();
        // setup database - this probably should be moved to a class to minimize connection counts
        private FirebirdSql.Data.FirebirdClient.FbConnection dbConnection;

        public PopSimModel()    // class constructor
        {
            // open database
            // create link to Firebird database
            dbConnection = new FbConnection();
            string conStr = Properties.Settings.Default.PopSimConnectionString;
            // temporarly hard code password and file location
            conStr = "Database=C:\\eer\\active\\Projects\\0212979.002.026.001_Benmap\\popsimgui\\BenMAP\\PopSim\\PopSim\\bin\\Debug\\POPSIMDB.FDB;USER=SYSDBA;PASSWORD=masterkey";
            // conStr = "Database=|DataDirectory|\\POPSIMDB.FDB;USER=SYSDBA;PASSWORD=masterkey";
            dbConnection.ConnectionString = conStr;
            dbConnection.Open();
            
        }


        public void runPopSim()
        {
            Debug.Print("run started");
            setupInternalVariables();
            // STEP 1: DELETE RECORDS
            delete_Records();
            
            // STEP 2: CALCULATE ANNUAL PM VALUES
            run_Annual_PM_Values();
            
            //STEP 3: CALCULATE LAG
            //If user selected the single lag option, then calculate the lag only for the single lag option
            if (strLag_Type == "Single") {
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
            
            //STEP 4: CALCULATE THRESHOLD
            //Only run threshold calcs if user specified a threshold value
            if (InputData.getPM_Choice() == 1) { 
                run_threshold_calcs();
            }
            // STOPPED HERE
            //STEP 5: CALCULATE AGE-SPECIFIC ADJUSTMENT FACTORS
            run_age_specific_adjustment_factors();

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
        } // end run Pop Sim
        public void setupInternalVariables()
        {
            // setup up internal variables. This was done on in the Global code module of the original Access database program
            if (InputData.getPM_Choice() == 0) {
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

/*    

run_regulatory

wasRun = True
Command120.Visible = True
Command121.Visible = True
*/
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
            if (strMethod == "Step"){    
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
            // THIS ROUTINE ONLY APPEARS TO WORK WITH VERY CAREFUL MATCHING OF THE PM LEVELS
            // FREQUENTLY CRASHES WITH NO DATA IN EITHER THE SORTED_PM_TABLE
            
           //'Set variables to beginning values
            int year = InputData.getBegin_Year();
            double PM_val = 0;
            t = 0;
            j = 0;
            k = 0;
            m = 0;
            
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
                    sqltext = sqltext + " where Sorted_PM_Conc.PM_Conc = " + System.Math.Round((double)InputData.getPM_Threshold() - PM_val, 1);
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
        
            } // end for
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
                    j = (double)dataReader[1];
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
    } // end popsim model
