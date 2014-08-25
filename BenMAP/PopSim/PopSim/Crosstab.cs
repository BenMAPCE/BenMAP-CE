// code example from http://weblogs.sqlteam.com/jeffs/archive/2005/05/11/5101.aspx

/*
 * from original article"

The arguments for the Pivot() function are as follows:

public static DataTable Pivot (IDataReader dataValues, string keyColumn, string pivotNameColumn, string pivotValueColumn)

    dataValues -- this is any open DataReader object, ready to be transformed and pivoted into a DataTable.  As mentioned, 
       it should be fully grouped, aggregated, sorted and ready to go. 
    keyColumn -- This is the column in the DataReader which serves to identify each row.  In the previous example, 
       this would be CustomerID.  Your DataReader's recordset should be grouped and sorted by this column as well.
    pivotNameColumn -- This is the column in the DataReader that contains the values you'd like to transform 
       from rows into columns.   In the example, this would be ProductName.
    pivotValueColumn -- This is the column that in the DataReader that contains the values to pivot into 
       the appropriate columns.  For our example, it would be Qty, which has been defined in the SELECT statement as SUM(Qty).
*/
using System.Data;

namespace PopSim {

    class CrosstabHelper
    {
        public static DataTable Pivot(IDataReader dataValues, string keyColumn, string pivotNameColumn, string pivotValueColumn)
        {

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
// Note: If you are using .NET 2.0 or above, you can use the change shown here to ensure that the new pivoted columns created by this function are sorted.
}