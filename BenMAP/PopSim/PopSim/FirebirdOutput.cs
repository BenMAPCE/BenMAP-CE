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


    public void readerToTxtFile(string fileName, FbDataReader dataReader)
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
    
}
