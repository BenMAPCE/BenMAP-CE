using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using FirebirdSql.Data.FirebirdClient;


public class FirebirdOutput
{
    private FirebirdSql.Data.FirebirdClient.FbConnection dbConnection;

	public FirebirdOutput()
	{
        dbConnection = getNewConnection();
        dbConnection.Open();
        
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


    public void queryStringToFile(string strSQL, string filename)
    {
        FbCommand dataCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
        dataCommand.Connection = dbConnection;
        dataCommand.CommandType = CommandType.Text;
        dataCommand.CommandText = strSQL;
        FbDataReader dataReader;
        dataReader = dataCommand.ExecuteReader();

        readerToTxtFile(filename, dataReader);
            
    }

    public void queryStringToXTabFile(string strSQL, string filename, string keyColumn, string pivotNameColumn, string pivotValueColumn)
    {
        FbCommand dataCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
        dataCommand.Connection = dbConnection;
        dataCommand.CommandType = CommandType.Text;
        dataCommand.CommandText = strSQL;
        FbDataReader dataReader;
        dataReader = dataCommand.ExecuteReader();

        readerToXTabTxtFile(filename, dataReader, keyColumn, pivotNameColumn, pivotValueColumn);

    }
  


    public void readerToTxtFile(string fileName, IDataReader dataReader)
    {
        const string SEPARATOR = "\t";  // tab-delimited text 
        // open file
        // get user location for file save
        SaveFileDialog myDialog = new SaveFileDialog();
        myDialog.Filter = "tab-delimited text files (*.txt)|*.txt|All files (*.*)|*.*";
        // make the default save location "My Documents" - this is one of the few locations that is writable for all users
        myDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        myDialog.FileName = fileName;
        myDialog.OverwritePrompt = true;

       
        
        if (myDialog.ShowDialog() == DialogResult.OK)
        {
            fileName = myDialog.FileName;
        }
        else
        {
            MessageBox.Show("Cancelled by User - file not saved.");
            return; // bail out
        }

        // open file for overwrite
        StreamWriter myStream = new StreamWriter(fileName, false);

        // write out parameters

        int rowcount = 0;
        // check for data and output row headers
        while(dataReader.Read())
        {
            // write header on first row
            if (rowcount == 0)
            {
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    // output field names as header
                    string strFieldName = dataReader.GetName(i);
                    myStream.Write(strFieldName);
                    if (i < dataReader.FieldCount - 1)
                    {
                        myStream.Write(SEPARATOR);
                    }
                    // Debug.Print(strFieldName);
                }
                myStream.WriteLine();   // add return to header line
            }
            // output records
            // output fields in record
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                myStream.Write(dataReader[i].ToString());
                if (i < dataReader.FieldCount - 1)
                {
                    myStream.Write(SEPARATOR);
                }
            }
            myStream.WriteLine();   // return line
            rowcount++;

        }
        
        // flush and close output file
        myStream.Flush();
        myStream.Close();
        //MessageBox.Show("Save to: " + _outputFileName);
        //MessageBox.Show("All outputs saved to " + Path.GetDirectoryName(fileName));
        
    }

    public void readerToXTabTxtFile(string fileName, IDataReader dataReader, string keyColumn, string pivotNameColumn, string pivotValueColumn)
    {
        // create crosstab table from data reader using Pivot function
        DataTable dtOutput = Pivot(dataReader, keyColumn, pivotNameColumn, pivotValueColumn);

        // create a data reader from the new crosstab data table
        IDataReader drXTab = dtOutput.CreateDataReader();
       
        // use the datareader output function to dump to file
        readerToTxtFile(fileName, drXTab);

    }
    


    public static DataTable Pivot(IDataReader dataValues, string keyColumn, string pivotNameColumn, string pivotValueColumn)
    {
        // written by Jeff Smith 
        // and taken from http://weblogs.sqlteam.com/jeffs/archive/2005/05/11/5101.aspx
        /* The arguments for the Pivot() function are as follows (from original reference):
            dataValues -- this is any open DataReader object, ready to be transformed and pivoted into a DataTable.  
               As mentioned, it should be fully grouped, aggregated, sorted and ready to go. 
            keyColumn -- This is the column in the DataReader which serves to identify each row.  
               In the previous example, this would be CustomerID.  Your DataReader's recordset should be grouped and sorted by this column as well.
            pivotNameColumn -- This is the column in the DataReader that contains the values you'd like to transform from rows into columns.   In the example, this would be ProductName.
            pivotValueColumn -- This is the column that in the DataReader that contains the values to pivot into the appropriate columns.  For our example, it would be Qty, which has been defined in the SELECT statement as SUM(Qty).
        */

        DataTable tmp = new DataTable();
        DataRow r;
        string LastKey = "//dummy//";
        int i, pValIndex, pNameIndex;
        string s;
        bool FirstRow = true;

        // Add non-pivot columns to the data table:

        pValIndex = dataValues.GetOrdinal(pivotValueColumn);
        pNameIndex = dataValues.GetOrdinal(pivotNameColumn);

        for (i = 0; i <= dataValues.FieldCount - 1; i++)
            if (i != pValIndex && i != pNameIndex)
                tmp.Columns.Add(dataValues.GetName(i), dataValues.GetFieldType(i));

        r = tmp.NewRow();

        // now, fill up the table with the data:
        while (dataValues.Read())
        {
            // see if we need to start a new row
            if (dataValues[keyColumn].ToString() != LastKey)
            {
                // if this isn't the very first row, we need to add the last one to the table
                if (!FirstRow)
                    tmp.Rows.Add(r);
                r = tmp.NewRow();
                FirstRow = false;
                // Add all non-pivot column values to the new row:
                for (i = 0; i <= dataValues.FieldCount - 3; i++)
                    r[i] = dataValues[tmp.Columns[i].ColumnName];
                LastKey = dataValues[keyColumn].ToString();
            }
            // assign the pivot values to the proper column; add new columns if needed:
            s = dataValues[pNameIndex].ToString();
            if (!tmp.Columns.Contains(s))
                tmp.Columns.Add(s, dataValues.GetFieldType(pValIndex));
            r[s] = dataValues[pValIndex];
        }


        // add that final row to the datatable:
        tmp.Rows.Add(r);

        // Close the DataReader
        dataValues.Close();


        // and that's it!
        return tmp;
    }

}
