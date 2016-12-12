using System;
using System.Data;
using System.Data.SqlClient;
using music.local.CommonFunction;

namespace music.local.Bussiness.DataAccess
{
    public class Logins
    {
        #region using sqlite

        public static string SqliteDateTimeFormat = "dd-MM-yyyy HH:mm:ss";

        /// <summary>
        /// cập nhật/ thêm mới
        /// </summary>
        /// <param name="ip">là identity cho web</param>
        /// <param name="created"></param>
        /// <param name="Expired"></param>
        /// <param name="other"></param>
        /// <param name="lastActive"></param>
        public static void Logins_Update(string ip, DateTime? created, DateTime? Expired, string other, DateTime? lastActive= null)
        {
            var strCreated = (created ?? DateTime.Now).ToString(SqliteDateTimeFormat);
            var strExp = (Expired ?? DateTime.Now.AddDays(2)).ToString(SqliteDateTimeFormat);
            var strlastActive = (lastActive ?? DateTime.Now).ToString(SqliteDateTimeFormat);
            SqliteHelper sqliteHelper = new SqliteHelper();
            string strCommandText = "select * from Logins where Identity='"+ip+"'";
            var data = sqliteHelper.ExecuteGetDataTable(strCommandText);
            if (data != null && data.Rows.Count > 0)
            {
                strCommandText = "update Logins set Identity='" + ip + "', Created= '" + strCreated + "', Expired='" + strExp
                    + "', OtherInfor='" + other + "', LastActive ='" + strlastActive + "'";
            }
            else
            {
                strCommandText = "insert into Logins values ('" + ip + "', '" + strCreated + "', '" + strExp + "', '" +
                                 other + "', '" + strlastActive + "')";
            }

            sqliteHelper.ExecuteNonQuery(strCommandText);
        }

        /// <summary>
        /// lấy danh sách/ 1 logins 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static DataTable Logins_Get(string ip="")
        {
            string strCommandText = "select * from Logins where Identity = '" + ip + "'";
            if (string.IsNullOrEmpty(ip)) strCommandText = "select * from Logins";
            var sqliteHelper = new SqliteHelper();
            return sqliteHelper.ExecuteGetDataTable(strCommandText);
        }

        public static int Logins_UpdateLastActive(string ip, string lastActive)
        {
            string strCommandText = "update Logins set LastActive = '" + lastActive + "' where Identity = '" + ip + "'";
            var sqliteHelper = new SqliteHelper();
            return sqliteHelper.ExecuteNonQuery(strCommandText);
        }
        #endregion


        #region using sql server

        /// <summary>
        /// cập nhật/ thêm mới
        /// </summary>
        /// <param name="ip">là identity cho web</param>
        /// <param name="created"></param>
        /// <param name="Expired"></param>
        /// <param name="other"></param>
        public static void Logins_Update1(string ip, DateTime? created, DateTime? Expired, string other)
        {
            string strCommandText = "Logins_Update";
            SqlParameter[] paraLocal = new SqlParameter[4];
            paraLocal[0] = new SqlParameter("@Identity", ip);
            paraLocal[1] = new SqlParameter("@Created", created);
            paraLocal[2] = new SqlParameter("@Expired", Expired);
            paraLocal[3] = new SqlParameter("@OtherInfor", other);
            SqlHelper.ExecuteNonQuery(CommandType.StoredProcedure, strCommandText, paraLocal);
        }

        /// <summary>
        /// lấy danh sách/ 1 logins 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static DataTable Logins_Get1(string ip = "")
        {
            string strCommandText = "Logins_Update";
            SqlParameter[] paraLocal = new SqlParameter[1];
            paraLocal[0] = new SqlParameter("@Identity", ip);
            return SqlHelper.ExecuteData(CommandType.StoredProcedure, strCommandText, paraLocal);
        }
        #endregion
    }
}