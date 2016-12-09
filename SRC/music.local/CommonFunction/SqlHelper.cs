using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace music.local
{
    public abstract class SqlHelper
    {
        private static readonly Hashtable ParasCache = Hashtable.Synchronized(new Hashtable());
        //private static readonly string Strconnection = ConfigurationManager.ConnectionStrings["JiraWrapperConnection"].ToString();
        private static readonly string Strconnection = @"AttachDbFilename=" + HostingEnvironment.ApplicationPhysicalPath + @"App_Data\MusicLocal.mdf;Integrated Security=SSPI;Trusted_Connection=false;";
        public static string ConnectionString
        {
            get { return Strconnection; }
        }
        public static DataTable ExecuteData(CommandType cmdCommandType, string cmdCommandString, params SqlParameter[] cmdParameters)
        {
            DataTable table2;
            SqlCommand selectCommand = new SqlCommand();
            SqlConnection connConnection = new SqlConnection(Strconnection);
            try
            {
                DataTable dataTable = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                PrepareCommand(selectCommand, connConnection, null, cmdCommandType, cmdCommandString, cmdParameters);
                selectCommand.ExecuteNonQuery();
                adapter.Fill(dataTable);
                selectCommand.Parameters.Clear();
                if (connConnection.State == ConnectionState.Open)
                {
                    connConnection.Close();
                }
                table2 = dataTable;
            }
            catch (SqlException exception)
            {
                Common.WriteLog(MethodBase.GetCurrentMethod().Name+": "+cmdCommandString, exception + exception.StackTrace );
                if (connConnection.State == ConnectionState.Open)
                {
                    connConnection.Close();
                    SqlConnection.ClearPool(connConnection);
                }
                throw;
            }
            return table2;
        }

        public static void ExecuteNonQuery(CommandType cmdCommandType, string cmdCommandString, params SqlParameter[] cmdParameters)
        {
            SqlCommand cmdCommand = new SqlCommand();
            SqlConnection connConnection = new SqlConnection(Strconnection);
            try
            {
                PrepareCommand(cmdCommand, connConnection, null, cmdCommandType, cmdCommandString, cmdParameters);
                cmdCommand.ExecuteNonQuery();
                cmdCommand.Parameters.Clear();
                if (connConnection.State == ConnectionState.Open)
                {
                    connConnection.Close();
                }
            }
            catch (SqlException exception)
            {
                Common.WriteLog(MethodBase.GetCurrentMethod().Name + ": " + cmdCommandString, exception + exception.StackTrace);
                if (connConnection.State == ConnectionState.Open)
                {
                    connConnection.Close();
                    SqlConnection.ClearPool(connConnection);
                }
                throw;
            }
        }
        public static void ExecuteNonQuery(CommandType cmdCommandType, SqlConnection connConnection, string cmdCommandString, params SqlParameter[] cmdParameters)
        {
            SqlCommand cmdCommand = new SqlCommand();
            try
            {
                PrepareCommand(cmdCommand, connConnection, null, cmdCommandType, cmdCommandString, cmdParameters);
                cmdCommand.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                Common.WriteLog(MethodBase.GetCurrentMethod().Name + ": " + cmdCommandString, exception + exception.StackTrace);
                if (connConnection.State == ConnectionState.Open)
                {
                    connConnection.Close();
                    SqlConnection.ClearPool(connConnection);
                }
                throw;
            }
        }
        public static void ExecuteNonQuery(CommandType cmdCommandType, SqlTransaction tran, string cmdCommandString, params SqlParameter[] cmdParameters)
        {
            SqlCommand cmdCommand = new SqlCommand();
            try
            {
                PrepareCommand(cmdCommand, tran.Connection, tran, cmdCommandType, cmdCommandString, cmdParameters);
                cmdCommand.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                Common.WriteLog(MethodBase.GetCurrentMethod().Name + ": " + cmdCommandString, exception + exception.StackTrace);
                throw ;
            }
        }
        public static SqlDataReader ExecuteReader(CommandType cmdCommandType, string cmdCommandString, params SqlParameter[] cmdParameters)
        {
            SqlDataReader reader2;
            SqlCommand cmdCommand = new SqlCommand();
            SqlConnection connConnection = new SqlConnection(Strconnection);
            try
            {
                PrepareCommand(cmdCommand, connConnection, null, cmdCommandType, cmdCommandString, cmdParameters);
                SqlDataReader reader = cmdCommand.ExecuteReader(CommandBehavior.CloseConnection);
                cmdCommand.Parameters.Clear();
                reader2 = reader;
            }
            catch (SqlException exception)
            {
                if (connConnection.State == ConnectionState.Open)
                {
                    connConnection.Close();
                    SqlConnection.ClearPool(connConnection);
                }
                Common.WriteLog(MethodBase.GetCurrentMethod().Name + ": " + cmdCommandString, exception + exception.StackTrace);
                throw;
            }
            return reader2;
        }

        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] parameterArray = (SqlParameter[])ParasCache[cacheKey];
            if (parameterArray == null)
            {
                return null;
            }
            SqlParameter[] parameterArray2 = new SqlParameter[parameterArray.Length];
            int index = 0;
            int length = parameterArray.Length;
            while (index < length)
            {
                parameterArray2[index] = (SqlParameter)((ICloneable)parameterArray[index]).Clone();
                index++;
            }
            return parameterArray2;
        }

        public static void PrepareCommand(SqlCommand cmdCommand, CommandType cmdCommandType, string cmdCommandString, SqlParameter[] cmdParameters)
        {
            cmdCommand.Parameters.Clear();
            cmdCommand.CommandType = cmdCommandType;
            cmdCommand.CommandText = cmdCommandString;
            if (cmdParameters != null)
            {
                foreach (SqlParameter parameter in cmdParameters)
                {
                    cmdCommand.Parameters.Add(parameter);
                }
            }
        }

        private static void PrepareCommand(SqlCommand cmdCommand, SqlConnection connConnection, SqlTransaction trasTransaction, CommandType cmdCommandType, string cmdCommandString, SqlParameter[] cmdParameters)
        {
            if (connConnection.State != ConnectionState.Open)
            {
                connConnection.Open();
            }
            cmdCommand.Connection = connConnection;
            cmdCommand.CommandText = cmdCommandString;
            if (trasTransaction != null)
            {
                cmdCommand.Transaction = trasTransaction;
            }
            cmdCommand.CommandType = cmdCommandType;
            if (cmdParameters != null)
            {
                foreach (SqlParameter parameter in cmdParameters)
                {
                    cmdCommand.Parameters.Add(parameter);
                }
            }
        }
        
    }
}
