using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.SQLite;
using System.IO;
using System.Runtime.InteropServices;
using System.Web.Configuration;
using System.Web.Hosting;

namespace music.local.CommonFunction
{
    public class SqliteHelper
    {
        public static string SqliteFile = HostingEnvironment.ApplicationPhysicalPath+   "App_Data\\MusicLocal.db";
        public static string SqliteConnectionstring = "Data Source=" + SqliteFile + ";Version=3;FailIfMissing=False;Pooling=True;Max Pool Size=15;";
        private SQLiteConnection Connection;
        public void InitSqlite()
        {
            if (!File.Exists(SqliteFile))
            {
                SQLiteConnection.CreateFile(SqliteFile);
                //SQLiteConnection.
            }
            
            Connection = new SQLiteConnection(SqliteConnectionstring);
            Connection.Open();
            SQLiteCommand command;
            if (!CheckExistsTable("Logins"))
            {
                string sql = "create table Logins (Identity nvarchar(50), Created nvarchar(50) ,Expired nvarchar(50), OtherInfor nvarchar(200))";
                command = new SQLiteCommand(sql,Connection);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            if (Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
                Connection.Dispose();
            }

        }


        public DataTable ExecuteGetDataTable(string commandstr)
        {
            DataTable dt = null;
            using (Connection = new SQLiteConnection(SqliteConnectionstring))
            {
                
                Connection.Open();
                string sql = commandstr;
                var command = new SQLiteCommand(sql, Connection);
                SQLiteDataAdapter dataAdt = new SQLiteDataAdapter(command);
                DataSet ds = new DataSet();
                dataAdt.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    dt= ds.Tables[0];
                }
                command.Dispose();
            }
            return dt;
        }


        public int ExecuteNonQuery(string commandstr)
        {
            var result = 0;
            using (Connection = new SQLiteConnection(SqliteConnectionstring))
            {
                Connection.Open();
                string sql = commandstr;
                var command = new SQLiteCommand(sql, Connection);
                result=command.ExecuteNonQuery(); 
                command.Dispose();
            }
            return result;
        }


        #region private function

        private bool CheckExistsTable(string name)
        {
            var chk = true;
            using (var subConnection = new SQLiteConnection(SqliteConnectionstring))
            {
                subConnection.Open();
                var str = "SELECT * FROM sqlite_master WHERE type='table' AND name='" + name + "' ";
                SQLiteCommand command = new SQLiteCommand(str, Connection);
                var result = command.ExecuteNonQuery();
                if (result < 1) chk = false;
                command.Dispose();
            }
            return chk;
        }

        #endregion

    }
}