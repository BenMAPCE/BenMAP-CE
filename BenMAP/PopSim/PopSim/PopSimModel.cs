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


             
        }
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
                
                // STOPPED HERE

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
    } // end popsim model
} // end namespace
