using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace music.local.Bussiness.DataAccess
{
    public class Logins
    {
        #region using sqlite

        private static string SqliteDateTimeFormat = "dd-MM-yyyy HH:mm:ss";

        /// <summary>
        /// cập nhật/ thêm mới
        /// </summary>
        /// <param name="ip">là identity cho web</param>
        /// <param name="created"></param>
        /// <param name="Expired"></param>
        /// <param name="other"></param>
        public static void Logins_Update(string ip, DateTime? created, DateTime? Expired, string other)
        {
            var strCreated = (created ?? DateTime.Now).ToString(SqliteDateTimeFormat);
            var strExp = (Expired ?? DateTime.Now.AddDays(2)).ToString(SqliteDateTimeFormat);
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
        public static DataTable Logins_Get(string ip="")
        {
            string strCommandText = "Logins_Update";
            SqlParameter[] paraLocal = new SqlParameter[1];
            paraLocal[0] = new SqlParameter("@Identity", ip);
            return SqlHelper.ExecuteData(CommandType.StoredProcedure, strCommandText, paraLocal);
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