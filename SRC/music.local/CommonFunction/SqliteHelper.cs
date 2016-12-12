using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Web.Hosting;

namespace music.local.CommonFunction
{
    public class SqliteHelper
    {
        public static string SqliteFile = HostingEnvironment.ApplicationPhysicalPath + "App_Data\\MusicLocal.db";
        public static string SqliteConnectionstring = "Data Source=" + SqliteFile + ";Version=3;FailIfMissing=False;Pooling=True;Max Pool Size=15;";
        //private SQLiteConnection Connection;
        public SqliteHelper()
        {
            InitSqlite();
        }

        public void InitSqlite()
        {
            if (!File.Exists(SqliteFile))
            {
                SQLiteConnection.CreateFile(SqliteFile);
                //SQLiteConnection.
            }

            var Connection = new SQLiteConnection(SqliteConnectionstring);
            Connection.Open();
            if (!CheckExistsTable("Logins"))
            {
                string sql = "create table Logins (Identity nvarchar(50), Created nvarchar(50) ,Expired nvarchar(50), OtherInfor nvarchar(200))";
                var command = new SQLiteCommand(sql, Connection);
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
            using (var Connection = new SQLiteConnection(SqliteConnectionstring))
            {

                Connection.Open();
                string sql = commandstr;
                var command = new SQLiteCommand(sql, Connection);
                SQLiteDataAdapter dataAdt = new SQLiteDataAdapter(command);
                DataSet ds = new DataSet();
                dataAdt.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }
                command.Dispose();
            }
            return dt;
        }


        public int ExecuteNonQuery(string commandstr)
        {
            int result;
            using (var Connection = new SQLiteConnection(SqliteConnectionstring))
            {
                Connection.Open();
                string sql = commandstr;
                var command = new SQLiteCommand(sql, Connection);
                result = command.ExecuteNonQuery();
                command.Dispose();
            }
            return result;
        }


        #region private function

        private bool CheckExistsTable(string name)
        {
            var chk = true;
            var str = "SELECT * FROM sqlite_master WHERE type='table' AND name='" + name + "' ";
            var result = ExecuteGetDataTable(str);
            if (result == null || result.Rows.Count < 1)
            {
                chk = false;
            }
            return chk;
        }

        #endregion

    }
}