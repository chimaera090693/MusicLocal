using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using music.local.Bussiness.DataAccess;

namespace music.local.Bussiness
{
    public class LoginsProcessing
    {
        /// <summary>
        /// Kiểm tra trạng thái đăng nhập
        /// </summary>
        /// <param name="redirect">redirect url</param>
        /// <returns></returns>
        public static bool CheckLogin(bool redirect = false)
        {
            var ipAddress = HttpContext.Current.Request.UserHostAddress;
            if (!string.IsNullOrEmpty(ipAddress))
            {
                var chkLogin = DataAccess.Logins.Logins_Get(ipAddress);
                if (chkLogin != null && chkLogin.Rows.Count > 0)
                {
                    var expired = chkLogin.Rows[0]["Expired"];
                    if (expired != DBNull.Value)
                    {
                        var dt = DateTime.ParseExact(expired.ToString(), Logins.SqliteDateTimeFormat, CultureInfo.InvariantCulture);
                        if (dt >= DateTime.Now)
                        {
                            Logins.Logins_UpdateLastActive(ipAddress, DateTime.Now.ToString(Logins.SqliteDateTimeFormat));
                            return true;
                        }
                    }
                    else
                    {
                        var created = DateTime.ParseExact(chkLogin.Rows[0]["Created"].ToString(), Logins.SqliteDateTimeFormat, CultureInfo.InvariantCulture);
                        if (created.AddDays(2) >= DateTime.Now)
                        {
                            Logins.Logins_UpdateLastActive(ipAddress, DateTime.Now.ToString(Logins.SqliteDateTimeFormat));
                            return true;
                        }
                    }
                }
            }
            if (redirect)
            {
               var abpath = HttpContext.Current.Request.Url.AbsolutePath;
              HttpContext.Current.Response.Redirect("~/Login?ru=" + abpath);
            }
            return false;
        }

        /// <summary>
        /// lưu đăng nhập
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="other"></param>
        public static void Login(string ip, string other)
        {
            DataAccess.Logins.Logins_Update(ip, DateTime.Now, DateTime.Now.AddDays(2), other);
        }

    }
}