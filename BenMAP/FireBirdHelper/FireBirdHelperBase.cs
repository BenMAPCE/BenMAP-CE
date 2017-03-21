/*--------------------------------------------------------------------------------------
 * Author: Rafey
 * 
 * Comments: FireBird SQLHelper with Embedded and Ini Query support.
 *			 You could download MSDAB V2 from http://www.microsoft.com/downloads/details.aspx?familyid=F63D1F0A-9877-4A7B-88EC-0426B48DF275&displaylang=en
 * 
 *			 There are great deal of sample available for MSDAB V2 which you 
 *			 could also use for FireBird SQLHelper
 *				
 * 
 * Email: syedrafey@gmail.com
 * 
 -------------------------------------------------------------------------------------*/

using System;
using System.Data;
using System.Xml;
using System.Collections;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Configuration;
using FirebirdSql.Data.FirebirdClient;

namespace ESIL.DBUtility
{
	/// <summary>
	/// The SqlHelper class is intended to encapsulate high performance, scalable best practices for 
	/// common uses of SqlClient
	/// </summary>
	public abstract  class FireBirdHelperBase:IFireBirdHelper
	{
		#region private utility methods & constructors

		// Since this class provides only  methods, make the default constructor private to prevent 
		// instances from being created with "new SqlHelper()"
        public FireBirdHelperBase() { }

		/// <summary>
		/// This method is used to attach array of FbParameters to a FbCommand.
		/// 
		/// This method will assign a value of DbNull to any parameter with a direction of
		/// InputOutput and a value of null.  
		/// 
		/// This behavior will prevent default values from being used, but
		/// this will be the less common case than an intended pure output parameter (derived as InputOutput)
		/// where the user provided no input value.
		/// </summary>
		/// <param name="command">The command to which the parameters will be added</param>
		/// <param name="commandParameters">An array of FbParameters to be added to command</param>
		private void AttachParameters(FbCommand command, FbParameter[] commandParameters)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (commandParameters != null)
			{
				foreach (FbParameter p in commandParameters)
				{
					if (p != null)
					{
						// Check for derived output value with no value assigned
						if ((p.Direction == ParameterDirection.InputOutput ||
							p.Direction == ParameterDirection.Input) &&
							(p.Value == null))
						{
							p.Value = DBNull.Value;
						}
						command.Parameters.Add(p);
					}
				}
			}
		}

		/// <summary>
		/// This method assigns dataRow column values to an array of FbParameters
		/// </summary>
		/// <param name="commandParameters">Array of FbParameters to be assigned values</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values</param>
		private void AssignParameterValues(FbParameter[] commandParameters, DataRow dataRow)
		{
			if ((commandParameters == null) || (dataRow == null))
			{
				// Do nothing if we get no data
				return;
			}

			int i = 0;
			// Set the parameters values
			foreach (FbParameter commandParameter in commandParameters)
			{
				// Check the parameter name
				if (commandParameter.ParameterName == null ||
					commandParameter.ParameterName.Length <= 1)
					throw new Exception(
						string.Format(
							"Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.",
							i, commandParameter.ParameterName));
				if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
					commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
				i++;
			}
		}

		/// <summary>
		/// This method assigns an array of values to an array of FbParameters
		/// </summary>
		/// <param name="commandParameters">Array of FbParameters to be assigned values</param>
		/// <param name="parameterValues">Array of objects holding the values to be assigned</param>
		private void AssignParameterValues(FbParameter[] commandParameters, object[] parameterValues)
		{
			if ((commandParameters == null) || (parameterValues == null))
			{
				// Do nothing if we get no data
				return;
			}

			// We must have the same number of values as we pave parameters to put them in
			if (commandParameters.Length != parameterValues.Length)
			{
				throw new ArgumentException("Parameter count does not match Parameter Value count.");
			}

			// Iterate through the FbParameters, assigning the values from the corresponding position in the 
			// value array
			for (int i = 0, j = commandParameters.Length; i < j; i++)
			{
				// If the current array value derives from IDbDataParameter, then assign its Value property
				if (parameterValues[i] is IDbDataParameter)
				{
					IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
					if (paramInstance.Value == null)
					{
						commandParameters[i].Value = DBNull.Value;
					}
					else
					{
						commandParameters[i].Value = paramInstance.Value;
					}
				}
				else if (parameterValues[i] == null)
				{
					commandParameters[i].Value = DBNull.Value;
				}
				else
				{
					commandParameters[i].Value = parameterValues[i];
				}
			}
		}

		/// <summary>
		/// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
		/// to the provided command
		/// </summary>
		/// <param name="command">The FbCommand to be prepared</param>
		/// <param name="connection">A valid FbConnection, on which to execute this command</param>
		/// <param name="transaction">A valid FbTransaction, or 'null'</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of FbParameters to be associated with the command or 'null' if no parameters are required</param>
		/// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
		private void PrepareCommand(FbCommand command, FbConnection connection, FbTransaction transaction, CommandType commandType, string commandText, FbParameter[] commandParameters, out bool mustCloseConnection)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

			// If the provided connection is not open, we will open it
			if (connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				connection.Open();
			}
			else
			{
				mustCloseConnection = false;
			}

			// Associate the connection with the command
			command.Connection = connection;

			// Set the command text (stored procedure name or SQL statement)
			command.CommandText = commandText;

			// If we were provided a transaction, assign it
			if (transaction != null)
			{
				if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
				command.Transaction = transaction;
			}

			// Set the command type
			command.CommandType = commandType;

			// Attach the command parameters if they are provided
			if (commandParameters != null)
			{
				AttachParameters(command, commandParameters);
			}
			return;
		}

		#endregion private utility methods & constructors

		#region ExecuteNonQuery

		/// <summary>
		/// Execute a FbCommand (that returns no resultset and takes no parameters) against the database specified in 
		/// the connection string
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders");
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public   int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteNonQuery(connectionString, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns no resultset) against the database specified in the connection string 
		/// using the provided parameters
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public   int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

			// Create & open a FbConnection, and dispose of it after we are done
			using (FbConnection connection = new FbConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns no resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="spName">The name of the stored prcedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public   int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns no resultset and takes no parameters) against the provided FbConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public   int ExecuteNonQuery(FbConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteNonQuery(connection, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns no resultset) against the specified FbConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public   int ExecuteNonQuery(FbConnection connection, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// Create a command and prepare it for execution
		    using (var cmd = new FbCommand())
		    {
		        bool mustCloseConnection = false;
		        PrepareCommand(cmd, connection, (FbTransaction) null, commandType, commandText, commandParameters,
		            out mustCloseConnection);

		        // Finally, execute the command
		        int retval = cmd.ExecuteNonQuery();

		        // Detach the FbParameters from the command object, so they can be used again
		        cmd.Parameters.Clear();
		        if (mustCloseConnection)
		            connection.Close();
		        return retval;
		    }
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns no resultset) against the specified FbConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public   int ExecuteNonQuery(FbConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns no resultset and takes no parameters) against the provided FbTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders");
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public   int ExecuteNonQuery(FbTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteNonQuery(transaction, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns no resultset) against the specified FbTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public   int ExecuteNonQuery(FbTransaction transaction, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Create a command and prepare it for execution
		    using (var cmd = new FbCommand())
		    {
		        bool mustCloseConnection = false;
		        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters,
		            out mustCloseConnection);

		        // Finally, execute the command
		        int retval = cmd.ExecuteNonQuery();

		        // Detach the FbParameters from the command object, so they can be used again
		        cmd.Parameters.Clear();
		        return retval;
		    }
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns no resultset) against the specified 
		/// FbTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public   int ExecuteNonQuery(FbTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteNonQuery

		#region ExecuteDataset

		/// <summary>
		/// Execute a FbCommand (that returns a resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteDataset(connectionString, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
           
			// Create & open a FbConnection, and dispose of it after we are done
			using (FbConnection connection = new FbConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				return ExecuteDataset(connection, commandType, commandText, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset and takes no parameters) against the provided FbConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  DataSet ExecuteDataset(FbConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteDataset(connection, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset) against the specified FbConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  DataSet ExecuteDataset(FbConnection connection, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// Create a command and prepare it for execution
			FbCommand cmd = new FbCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, connection, (FbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

			// Create the DataAdapter & DataSet
			using (FbDataAdapter da = new FbDataAdapter(cmd))
			{
				DataSet ds = new DataSet();

				// Fill the DataSet using default values for DataTable names, etc
				da.Fill(ds);

				// Detach the FbParameters from the command object, so they can be used again
				cmd.Parameters.Clear();

				if (mustCloseConnection)
					connection.Close();

				// Return the dataset
				return ds;
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified FbConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  DataSet ExecuteDataset(FbConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset and takes no parameters) against the provided FbTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  DataSet ExecuteDataset(FbTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteDataset(transaction, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset) against the specified FbTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  DataSet ExecuteDataset(FbTransaction transaction, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Create a command and prepare it for execution
			FbCommand cmd = new FbCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

			// Create the DataAdapter & DataSet
			using (FbDataAdapter da = new FbDataAdapter(cmd))
			{
				DataSet ds = new DataSet();

				// Fill the DataSet using default values for DataTable names, etc
				da.Fill(ds);

				// Detach the FbParameters from the command object, so they can be used again
				cmd.Parameters.Clear();

				// Return the dataset
				return ds;
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified 
		/// FbTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  DataSet ExecuteDataset(FbTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteDataset

		#region ExecuteReader

		/// <summary>
		/// This enum is used to indicate whether the connection was provided by the caller, or created by SqlHelper, so that
		/// we can set the appropriate CommandBehavior when calling ExecuteReader()
		/// </summary>
		public enum FbConnectionOwnership
		{
			/// <summary>Connection is owned and managed by SqlHelper</summary>
			Internal,
			/// <summary>Connection is owned and managed by the caller</summary>
			External
		}

		/// <summary>
		/// Create and prepare a FbCommand, and call ExecuteReader with the appropriate CommandBehavior.
		/// </summary>
		/// <remarks>
		/// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
		/// 
		/// If the caller provided the connection, we want to leave it to them to manage.
		/// </remarks>
		/// <param name="connection">A valid FbConnection, on which to execute this command</param>
		/// <param name="transaction">A valid FbTransaction, or 'null'</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of FbParameters to be associated with the command or 'null' if no parameters are required</param>
		/// <param name="connectionOwnership">Indicates whether the connection parameter was provided by the caller, or created by SqlHelper</param>
		/// <returns>FbDataReader containing the results of the command</returns>
		private FbDataReader ExecuteReader(FbConnection connection, FbTransaction transaction, CommandType commandType, string commandText, FbParameter[] commandParameters, FbConnectionOwnership connectionOwnership)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			bool mustCloseConnection = false;
			// Create a command and prepare it for execution
			FbCommand cmd = new FbCommand();
			try
			{
				PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

				// Create a reader
				FbDataReader dataReader;

				// Call ExecuteReader with the appropriate CommandBehavior
				if (connectionOwnership == FbConnectionOwnership.External)
				{
					dataReader = cmd.ExecuteReader();
				}
				else
				{
					dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				}

				// Detach the FbParameters from the command object, so they can be used again.
				// HACK: There is a problem here, the output parameter values are fletched 
				// when the reader is closed, so if the parameters are detached from the command
				// then the SqlReader can磘 set its values. 
				// When this happen, the parameters can磘 be used again in other command.
				bool canClear = true;
				foreach (FbParameter commandParameter in cmd.Parameters)
				{
					if (commandParameter.Direction != ParameterDirection.Input)
						canClear = false;
				}

				if (canClear)
				{
					cmd.Parameters.Clear();
				}

				return dataReader;
			}
			catch
			{
				if (mustCloseConnection)
					connection.Close();
				throw;
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FbDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A FbDataReader containing the resultset generated by the command</returns>
		public  FbDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteReader(connectionString, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FbDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>A FbDataReader containing the resultset generated by the command</returns>
		public  FbDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			FbConnection connection = null;
			try
			{
				connection = new FbConnection(connectionString);
				connection.Open();

				// Call the private overload that takes an internally owned connection in place of the connection string
				return ExecuteReader(connection, null, commandType, commandText, commandParameters, FbConnectionOwnership.Internal);
			}
			catch
			{
				// If we fail to return the SqlDatReader, we need to close the connection ourselves
				if (connection != null) connection.Close();
				throw;
			}

		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  FbDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A FbDataReader containing the resultset generated by the command</returns>
		public  FbDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
                FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connectionString, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset and takes no parameters) against the provided FbConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FbDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A FbDataReader containing the resultset generated by the command</returns>
		public  FbDataReader ExecuteReader(FbConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteReader(connection, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset) against the specified FbConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FbDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>A FbDataReader containing the resultset generated by the command</returns>
		public  FbDataReader ExecuteReader(FbConnection connection, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			// Pass through the call to the private overload using a null transaction value and an externally owned connection
			return ExecuteReader(connection, (FbTransaction)null, commandType, commandText, commandParameters, FbConnectionOwnership.Internal);
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified FbConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  FbDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A FbDataReader containing the resultset generated by the command</returns>
		public  FbDataReader ExecuteReader(FbConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connection, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteReader(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset and takes no parameters) against the provided FbTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FbDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A FbDataReader containing the resultset generated by the command</returns>
		public  FbDataReader ExecuteReader(FbTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteReader(transaction, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset) against the specified FbTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///   FbDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>A FbDataReader containing the resultset generated by the command</returns>
		public  FbDataReader ExecuteReader(FbTransaction transaction, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Pass through to private overload, indicating that the connection is owned by the caller
			return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, FbConnectionOwnership.External);
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified
		/// FbTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  FbDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A FbDataReader containing the resultset generated by the command</returns>
		public  FbDataReader ExecuteReader(FbTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteReader

		#region ExecuteScalar

		/// <summary>
		/// Execute a FbCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public  object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteScalar(connectionString, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a 1x1 resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public  object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			// Create & open a FbConnection, and dispose of it after we are done
			using (FbConnection connection = new FbConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				return ExecuteScalar(connection, commandType, commandText, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a 1x1 resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public  object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns a 1x1 resultset and takes no parameters) against the provided FbConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public  object ExecuteScalar(FbConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteScalar(connection, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a 1x1 resultset) against the specified FbConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public  object ExecuteScalar(FbConnection connection, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// Create a command and prepare it for execution
		    using (var cmd = new FbCommand())
		    {
		        bool mustCloseConnection = false;
		        PrepareCommand(cmd, connection, (FbTransaction) null, commandType, commandText, commandParameters,
		            out mustCloseConnection);

		        // Execute the command & return the results
		        object retval = cmd.ExecuteScalar();

		        // Detach the FbParameters from the command object, so they can be used again
		        cmd.Parameters.Clear();

		        if (mustCloseConnection)
		            connection.Close();

		        return retval;
		    }
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a 1x1 resultset) against the specified FbConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public  object ExecuteScalar(FbConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns a 1x1 resultset and takes no parameters) against the provided FbTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount");
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public  object ExecuteScalar(FbTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteScalar(transaction, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a 1x1 resultset) against the specified FbTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public  object ExecuteScalar(FbTransaction transaction, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Create a command and prepare it for execution
		    using (var cmd = new FbCommand())
		    {
		        bool mustCloseConnection = false;
		        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters,
		            out mustCloseConnection);

		        // Execute the command & return the results
		        object retval = cmd.ExecuteScalar();

		        // Detach the FbParameters from the command object, so they can be used again
		        cmd.Parameters.Clear();
		        return retval;
		    }
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a 1x1 resultset) against the specified
		/// FbTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public  object ExecuteScalar(FbTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// PPull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteScalar

		#region ExecuteXmlReader
		/// <summary>
		/// Execute a FbCommand (that returns a resultset and takes no parameters) against the provided FbConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public  XmlReader ExecuteXmlReader(FbConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteXmlReader(connection, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset) against the specified FbConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public  XmlReader ExecuteXmlReader(FbConnection connection, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			bool mustCloseConnection = false;
			// Create a command and prepare it for execution
			FbCommand cmd = new FbCommand();
			try
			{
				PrepareCommand(cmd, connection, (FbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

				// Create the DataAdapter & DataSet
				XmlReader retval = null;// cmd.ExecuteXmlReader(); NOT available in FireBird

				// Detach the FbParameters from the command object, so they can be used again
				cmd.Parameters.Clear();

				return retval;
			}
			catch
			{
				if (mustCloseConnection)
					connection.Close();
				throw;
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified FbConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="spName">The name of the stored procedure using "FOR XML AUTO"</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public  XmlReader ExecuteXmlReader(FbConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset and takes no parameters) against the provided FbTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public  XmlReader ExecuteXmlReader(FbTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of FbParameters
			return ExecuteXmlReader(transaction, commandType, commandText, (FbParameter[])null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset) against the specified FbTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders", new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public  XmlReader ExecuteXmlReader(FbTransaction transaction, CommandType commandType, string commandText, params FbParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Create a command and prepare it for execution
			FbCommand cmd = new FbCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

			// Create the DataAdapter & DataSet
			XmlReader retval = null; // cmd.ExecuteXmlReader();

			// Detach the FbParameters from the command object, so they can be used again
			cmd.Parameters.Clear();
			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified 
		/// FbTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  XmlReader ExecuteXmlReader(FbTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteXmlReader

		#region FillDataset
		/// <summary>
		/// Execute a FbCommand (that returns a resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)</param>
		public  void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (dataSet == null) throw new ArgumentNullException("dataSet");

			// Create & open a FbConnection, and dispose of it after we are done
			using (FbConnection connection = new FbConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				FillDataset(connection, commandType, commandText, dataSet, tableNames);
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		public  void FillDataset(string connectionString, CommandType commandType,
			string commandText, DataSet dataSet, string[] tableNames,
			params FbParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (dataSet == null) throw new ArgumentNullException("dataSet");
			// Create & open a FbConnection, and dispose of it after we are done
			using (FbConnection connection = new FbConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, 24);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>    
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		public  void FillDataset(string connectionString, string spName,
			DataSet dataSet, string[] tableNames,
			params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (dataSet == null) throw new ArgumentNullException("dataSet");
			// Create & open a FbConnection, and dispose of it after we are done
			using (FbConnection connection = new FbConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				FillDataset(connection, spName, dataSet, tableNames, parameterValues);
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset and takes no parameters) against the provided FbConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>    
		public  void FillDataset(FbConnection connection, CommandType commandType,
			string commandText, DataSet dataSet, string[] tableNames)
		{
			FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset) against the specified FbConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		public  void FillDataset(FbConnection connection, CommandType commandType,
			string commandText, DataSet dataSet, string[] tableNames,
			params FbParameter[] commandParameters)
		{
			FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified FbConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  FillDataset(conn, "GetOrders", ds, new string[] {"orders"}, 24, 36);
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		public  void FillDataset(FbConnection connection, string spName,
			DataSet dataSet, string[] tableNames,
			params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (dataSet == null) throw new ArgumentNullException("dataSet");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
			}
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset and takes no parameters) against the provided FbTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		public  void FillDataset(FbTransaction transaction, CommandType commandType,
			string commandText,
			DataSet dataSet, string[] tableNames)
		{
			FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
		}

		/// <summary>
		/// Execute a FbCommand (that returns a resultset) against the specified FbTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		public  void FillDataset(FbTransaction transaction, CommandType commandType,
			string commandText, DataSet dataSet, string[] tableNames,
			params FbParameter[] commandParameters)
		{
			FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified 
		/// FbTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  FillDataset(trans, "GetOrders", ds, new string[]{"orders"}, 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		public  void FillDataset(FbTransaction transaction, string spName,
			DataSet dataSet, string[] tableNames,
			params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (dataSet == null) throw new ArgumentNullException("dataSet");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of FbParameters
				FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
			}
		}

		/// <summary>
		/// Private helper method that execute a FbCommand (that returns a resultset) against the specified FbTransaction and FbConnection
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FillDataset(conn, trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new FbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid FbConnection</param>
		/// <param name="transaction">A valid FbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
		/// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
		/// by a user defined name (probably the actual table name)
		/// </param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		private void FillDataset(FbConnection connection, FbTransaction transaction, CommandType commandType,
			string commandText, DataSet dataSet, string[] tableNames,
			params FbParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (dataSet == null) throw new ArgumentNullException("dataSet");

			// Create a command and prepare it for execution
			FbCommand command = new FbCommand();
			bool mustCloseConnection = false;
			PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

			// Create the DataAdapter & DataSet
			using (FbDataAdapter dataAdapter = new FbDataAdapter(command))
			{

				// Add the table mappings specified by the user
				if (tableNames != null && tableNames.Length > 0)
				{
					string tableName = "Table";
					for (int index = 0; index < tableNames.Length; index++)
					{
						if (tableNames[index] == null || tableNames[index].Length == 0) throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
						dataAdapter.TableMappings.Add(tableName, tableNames[index]);
						tableName += (index + 1).ToString();
					}
				}

				// Fill the DataSet using default values for DataTable names, etc
				dataAdapter.Fill(dataSet);

				// Detach the FbParameters from the command object, so they can be used again
				command.Parameters.Clear();
			}

			if (mustCloseConnection)
				connection.Close();
		}
		#endregion

		#region UpdateDataset
		/// <summary>
		/// Executes the respective command for each inserted, updated, or deleted row in the DataSet.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  UpdateDataset(conn, insertCommand, deleteCommand, updateCommand, dataSet, "Order");
		/// </remarks>
		/// <param name="insertCommand">A valid transact-SQL statement or stored procedure to insert new records into the data source</param>
		/// <param name="deleteCommand">A valid transact-SQL statement or stored procedure to delete records from the data source</param>
		/// <param name="updateCommand">A valid transact-SQL statement or stored procedure used to update records in the data source</param>
		/// <param name="dataSet">The DataSet used to update the data source</param>
		/// <param name="tableName">The DataTable used to update the data source.</param>
		public  void UpdateDataset(FbCommand insertCommand, FbCommand deleteCommand, FbCommand updateCommand, DataSet dataSet, string tableName)
		{
			if (insertCommand == null) throw new ArgumentNullException("insertCommand");
			if (deleteCommand == null) throw new ArgumentNullException("deleteCommand");
			if (updateCommand == null) throw new ArgumentNullException("updateCommand");
			if (tableName == null || tableName.Length == 0) throw new ArgumentNullException("tableName");

			// Create a FbDataAdapter, and dispose of it after we are done
			using (FbDataAdapter dataAdapter = new FbDataAdapter())
			{
				// Set the data adapter commands
				dataAdapter.UpdateCommand = updateCommand;
				dataAdapter.InsertCommand = insertCommand;
				dataAdapter.DeleteCommand = deleteCommand;

				// Update the dataset changes in the data source
				dataAdapter.Update(dataSet, tableName);

				// Commit all the changes made to the DataSet
				dataSet.AcceptChanges();
			}
		}
		#endregion

		#region CreateCommand
		/// <summary>
		/// Simplify the creation of a Sql command object by allowing
		/// a stored procedure and optional parameters to be provided
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  FbCommand command = CreateCommand(conn, "AddCustomer", "CustomerID", "CustomerName");
		/// </remarks>
		/// <param name="connection">A valid FbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="sourceColumns">An array of string to be assigned as the source columns of the stored procedure parameters</param>
		/// <returns>A valid FbCommand object</returns>
		public  FbCommand CreateCommand(FbConnection connection, string spName, params string[] sourceColumns)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// Create a FbCommand
			FbCommand cmd = new FbCommand(spName, connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// If we receive parameter values, we need to figure out where they go
			if ((sourceColumns != null) && (sourceColumns.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided source columns to these parameters based on parameter order
				for (int index = 0; index < sourceColumns.Length; index++)
					commandParameters[index].SourceColumn = sourceColumns[index];

				// Attach the discovered parameters to the FbCommand object
				AttachParameters(cmd, commandParameters);
			}

			return cmd;
		}
		#endregion

		#region ExecuteNonQueryTypedParams
		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns no resultset) against the database specified in 
		/// the connection string using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
		/// </summary>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public  int ExecuteNonQueryTypedParams(String connectionString, String spName, DataRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns no resultset) against the specified FbConnection 
		/// using the dataRow column values as the stored procedure's parameters values.  
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
		/// </summary>
		/// <param name="connection">A valid FbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public  int ExecuteNonQueryTypedParams(FbConnection connection, String spName, DataRow dataRow)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connection, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns no resultset) against the specified
		/// FbTransaction using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
		/// </summary>
		/// <param name="transaction">A valid FbTransaction object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public  int ExecuteNonQueryTypedParams(FbTransaction transaction, String spName, DataRow dataRow)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// Sf the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
			}
		}
		#endregion

		#region ExecuteDatasetTypedParams
		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the database specified in 
		/// the connection string using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
		/// </summary>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  DataSet ExecuteDatasetTypedParams(string connectionString, String spName, DataRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			//If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified FbConnection 
		/// using the dataRow column values as the store procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
		/// </summary>
		/// <param name="connection">A valid FbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  DataSet ExecuteDatasetTypedParams(FbConnection connection, String spName, DataRow dataRow)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connection, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified FbTransaction 
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
		/// </summary>
		/// <param name="transaction">A valid FbTransaction object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public  DataSet ExecuteDatasetTypedParams(FbTransaction transaction, String spName, DataRow dataRow)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion

		#region ExecuteReaderTypedParams
		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the database specified in 
		/// the connection string using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>A FbDataReader containing the resultset generated by the command</returns>
		public  FbDataReader ExecuteReaderTypedParams(String connectionString, String spName, DataRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
			}
		}


		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified FbConnection 
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <param name="connection">A valid FbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>A FbDataReader containing the resultset generated by the command</returns>
		public  FbDataReader ExecuteReaderTypedParams(FbConnection connection, String spName, DataRow dataRow)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connection, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified FbTransaction 
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <param name="transaction">A valid FbTransaction object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>A FbDataReader containing the resultset generated by the command</returns>
		public  FbDataReader ExecuteReaderTypedParams(FbTransaction transaction, String spName, DataRow dataRow)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
			}
		}
		#endregion

		#region ExecuteScalarTypedParams
		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a 1x1 resultset) against the database specified in 
		/// the connection string using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <param name="connectionString">A valid connection string for a FbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public  object ExecuteScalarTypedParams(String connectionString, String spName, DataRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a 1x1 resultset) against the specified FbConnection 
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <param name="connection">A valid FbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public  object ExecuteScalarTypedParams(FbConnection connection, String spName, DataRow dataRow)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connection, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a 1x1 resultset) against the specified FbTransaction
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <param name="transaction">A valid FbTransaction object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public  object ExecuteScalarTypedParams(FbTransaction transaction, String spName, DataRow dataRow)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
			}
		}
		#endregion

		#region ExecuteXmlReaderTypedParams
		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified FbConnection 
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <param name="connection">A valid FbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public  XmlReader ExecuteXmlReaderTypedParams(FbConnection connection, String spName, DataRow dataRow)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(connection, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a FbCommand (that returns a resultset) against the specified FbTransaction 
		/// using the dataRow column values as the stored procedure's parameters values.
		/// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <param name="transaction">A valid FbTransaction object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
		/// <returns>An XmlReader containing the resultset generated by the command</returns>
		public  XmlReader ExecuteXmlReaderTypedParams(FbTransaction transaction, String spName, DataRow dataRow)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If the row has values, the store procedure parameters must be initialized
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				FbParameter[] commandParameters = FireBirdHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Set the parameters values
				AssignParameterValues(commandParameters, dataRow);

                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
			}
		}
		#endregion

	}

    /// <summary>
    /// FireBirdHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
    /// ability to discover parameters for stored procedures at run-time.
    /// </summary>
    public sealed class FireBirdHelperParameterCache
    {
        #region private methods, variables, and constructors

        //Since this class provides only static methods, make the default constructor private to prevent 
        //instances from being created with "new FireBirdHelperParameterCache()"
        private FireBirdHelperParameterCache() { }

        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Resolve at run time the appropriate set of FbParameters for a stored procedure
        /// </summary>
        /// <param name="connection">A valid FbConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
        /// <returns>The parameter array discovered.</returns>
        private static FbParameter[] DiscoverSpParameterSet(FbConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            FbCommand cmd = new FbCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            FbCommandBuilder.DeriveParameters(cmd);
            connection.Close();

            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }

            FbParameter[] discoveredParameters = new FbParameter[cmd.Parameters.Count];

            cmd.Parameters.CopyTo(discoveredParameters, 0);

            // Init the parameters with a DBNull value
            foreach (FbParameter discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }

        /// <summary>
        /// Deep copy of cached FbParameter array
        /// </summary>
        /// <param name="originalParameters"></param>
        /// <returns></returns>
        private static FbParameter[] CloneParameters(FbParameter[] originalParameters)
        {
            FbParameter[] clonedParameters = new FbParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (FbParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion private methods, variables, and constructors

        #region caching functions

        /// <summary>
        /// Add parameter array to the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a FbConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters to be cached</param>
        public static void CacheParameterSet(string connectionString, string commandText, params FbParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        /// <summary>
        /// Retrieve a parameter array from the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a FbConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An array of SqlParamters</returns>
        public static FbParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            FbParameter[] cachedParameters = paramCache[hashKey] as FbParameter[];
            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        #endregion caching functions

        #region Parameter Discovery Functions

        /// <summary>
        /// Retrieves the set of FbParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a FbConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of FbParameters</returns>
        public static FbParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, true);
        }

        /// <summary>
        /// Retrieves the set of FbParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a FbConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of FbParameters</returns>
        public static FbParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            using (FbConnection connection = new FbConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// Retrieves the set of FbParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid FbConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of FbParameters</returns>
        internal static FbParameter[] GetSpParameterSet(FbConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, true);//modify by xiejp 2011-7-28 解决存储过程参数问题
        }

        /// <summary>
        /// Retrieves the set of FbParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid FbConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of FbParameters</returns>
        internal static FbParameter[] GetSpParameterSet(FbConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            using (FbConnection clonedConnection = (FbConnection)((ICloneable)connection).Clone())
            {
                return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// Retrieves the set of FbParameters appropriate for the stored procedure
        /// </summary>
        /// <param name="connection">A valid FbConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of FbParameters</returns>
        private static FbParameter[] GetSpParameterSetInternal(FbConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            FbParameter[] cachedParameters;

            cachedParameters = paramCache[hashKey] as FbParameter[];
            if (cachedParameters == null)
            {
                FbParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }

            return CloneParameters(cachedParameters);
        }

        #endregion Parameter Discovery Functions

    }

    #region IniFile Class

    public class IniFile
    {
        public string Path;

        #region Win32
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);
        #endregion

        #region Constructors
        public IniFile(string path)
        {
            Path = path;
        }
        #endregion

        #region Set Key
        public void SetKey(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this.Path);
        }
        #endregion

        #region Get Key
        public string GetKey(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);

            int i = GetPrivateProfileString(section, key, "", temp, 255, this.Path);

            return temp.ToString();
        }
        #endregion
    }

    #endregion

    #region SQLHelp Class
    public class SQLHelp
    {
        #region Helper Methods

        #region Common Methods

        public static void SetKey(string key, string value)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings[key].Value = value;

            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }

        public static string GetKey(string key, string defaultValue)
        {
            try
            {
                string val = ConfigurationManager.AppSettings[key];

                if (val == null)
                {
                    val = defaultValue;
                }

                return val;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
        #endregion

        #region Application Properties

        #region Folder Path

        //e.g C:\App.WinUI\bin\Debug
        public static string FolderApp
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        //e.g C:\App.WinUI
        public static string FolderRoot
        {
            get { return Path.GetFullPath(@"..\..\"); }
        }

        public static string FolderBin
        {
            get { return Path.GetFullPath(@".\"); }
        }

        public static string GetAppFolder(string name)
        {
            string path = SQLHelp.FolderApp + name + @"\";

            if (!Directory.Exists(path) || Directory.GetFiles(path).Length < 1)
            {
                path = SQLHelp.FolderRoot + name + @"\";
            }

            return path;
        }

        public static string FolderData
        {
            get { return SQLHelp.GetAppFolder("Data"); }
        }

        public static string FolderQuery
        {
            get { return SQLHelp.GetAppFolder("Query"); }
        }

        public static string FileFdb
        {
            get { return SQLHelp.FolderData + @"BenMAP.fdb"; }
        }

        public static string FileQueryIni
        {
            get { return SQLHelp.FolderQuery + @"Query.ini"; }
        }
        #endregion

        #region Connectionstring

        public static string Connectionstring
        {
            get
            {
                FbConnectionStringBuilder _fbsc = new FbConnectionStringBuilder();
                _fbsc.Pooling = true;
                _fbsc.MinPoolSize = 5;
                _fbsc.MaxPoolSize = 20;
                _fbsc.ConnectionTimeout = 15;
                _fbsc.ConnectionLifeTime = 15;
                _fbsc.Database = @"C:\Program Files\BenMAP 4.0\Database\BenMAP40.fdb";// SQLHelp.FileFdb;
                _fbsc.UserID = "SYSDBA";
                _fbsc.Password = "masterkey";
                _fbsc.Charset = "NONE";
                //_fbsc.ClientLibrary = @"C:\Windows\System32\GDS32.DLL";
                _fbsc.ClientLibrary = "GDS32.DLL";

                _fbsc.ServerType = FbServerType.Default;
                //FbConnection.CreateDatabase(_fbsc.ToString(), ok);
                //if (!ok) { return ok; }

                return _fbsc.ToString();// "ServerType=1;User=SYSDBA;Password=masterkey;Database=" + SQLHelp.FileFdb;
            }
        }

        public static string GetConnectionstring(string userid, string password, string database)
        {
            FbConnectionStringBuilder _fbsc = new FbConnectionStringBuilder();
            _fbsc.Pooling = true;
            _fbsc.MinPoolSize = 5;
            _fbsc.MaxPoolSize = 20;
            _fbsc.ConnectionTimeout = 15;
            _fbsc.ConnectionLifeTime = 15;
            _fbsc.Database = database;// SQLHelp.FileFdb;
            _fbsc.UserID = userid;// "SYSDBA";
            _fbsc.Password = password;// "masterkey";
            _fbsc.Charset = "NONE";
            //_fbsc.ClientLibrary = @"C:\Windows\System32\GDS32.DLL";
            _fbsc.ClientLibrary = "GDS32.DLL";

            _fbsc.ServerType = FbServerType.Default;
            //FbConnection.CreateDatabase(_fbsc.ToString(), ok);
            //if (!ok) { return ok; }

            return _fbsc.ToString();

        }
        #endregion

        #endregion

        #region String Helpers
        public static string Quote(string s)
        {
            return "\'" + s + "\'";
        }

        public static string SQuote(string s)
        {
            return " \'" + s + "\' ";
        }

        public static string DQuote(string s)
        {
            return "\"" + s + "\"";
        }

        public static string DSQuote(string s)
        {
            return " \"" + s + "\" ";
        }

        public static string Bracket(string s)
        {
            return "[" + s + "]";
        }

        public static string Parenthesis(string s)
        {
            return "(" + s + ")";
        }

        public static string Space(string s)
        {
            return " " + s + " ";
        }

        public static string Timestamp()
        {
            return DateTime.Now.ToString();
        }

        public static string BracketNonBlank(string s)
        {
            if (s.Trim() != "")
            {
                return SQLHelp.Bracket(s);
            }

            return s;
        }

        public static bool IsEmpty(string s)
        {
            return s == "";
        }

        public static bool IsAnyEmpty(params object[] vals)
        {
            for (int i = 0; i <= vals.Length; i++)
            {
                if (vals.GetValue(i).ToString() == "")
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsNonEmpty(string s)
        {
            return s != "";
        }

        public static string LogStr(string s)
        {
            return "[" + s + "]";
        }

        public static string RemoveLast(string s)
        {
            return SQLHelp.RemoveLast(s, 1);
        }

        public static string RemoveLast(string s, int count)
        {
            return s.Remove(s.Length - count, count);
        }
        #endregion

        #endregion
    }
    #endregion
}

