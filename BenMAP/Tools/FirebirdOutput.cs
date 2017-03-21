using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using FirebirdSql.Data.FirebirdClient;
using System.Threading;

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

        ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["PopSimConnectionString"];
        string str = settings.ConnectionString;
        str = str.Replace("##USERDATA##", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

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
        //2016 03 08 changed delimiter to comma from tab
        const string SEPARATOR = ",";  // comma-delimited text 

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
    }

    public void readerToXTabTxtFile(string fileName, IDataReader dataReader, string keyColumn, string pivotNameColumn, string pivotValueColumn)
    {
        // create crosstab table from data reader using Pivot function
        System.Data.DataTable dtOutput = Pivot(dataReader, keyColumn, pivotNameColumn, pivotValueColumn);

        // create a data reader from the new crosstab data table
        IDataReader drXTab = dtOutput.CreateDataReader();
       
        // use the datareader output function to dump to file
        readerToTxtFile(fileName, drXTab);

    }

    public static System.Data.DataTable Pivot(IDataReader dataValues, string keyColumn, string pivotNameColumn, string pivotValueColumn)
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

        System.Data.DataTable tmp = new System.Data.DataTable();
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
