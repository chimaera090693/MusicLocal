using System;
using System.Globalization;
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
            var idClient = GetRequestId(HttpContext.Current);
            if (!string.IsNullOrEmpty(idClient))
            {
                var chkLogin = Logins.Logins_Get(idClient);
                if (chkLogin != null && chkLogin.Rows.Count > 0)
                {
                    var expired = chkLogin.Rows[0]["Expired"];
                    if (expired != DBNull.Value)
                    {
                        var dt = DateTime.ParseExact(expired.ToString(), Logins.SqliteDateTimeFormat, CultureInfo.InvariantCulture);
                        if (dt >= DateTime.Now)
                        {
                            
                            Logins.Logins_UpdateLastActive(idClient, DateTime.Now.ToString(Logins.SqliteDateTimeFormat));
                            return true;
                        }
                    }
                    else
                    {
                        var created = DateTime.ParseExact(chkLogin.Rows[0]["Created"].ToString(), Logins.SqliteDateTimeFormat, CultureInfo.InvariantCulture);
                        if (created.AddDays(2) >= DateTime.Now)
                        {
                            Logins.Logins_UpdateLastActive(idClient, DateTime.Now.ToString(Logins.SqliteDateTimeFormat));
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
            var ipad = GetRequestIP(HttpContext.Current);
            Logins.Logins_Update(ip, DateTime.Now, DateTime.Now.AddDays(2), other + "\r\n" + ipad);
        }

        public static string GetRequestId(HttpContext currentContext)
        {
            var idCookie = currentContext.Request.Cookies["ClientId"];
            if (idCookie != null)
            {

                return idCookie.Value;
            }
            else
            {
                var newID = Guid.NewGuid().ToString();
                var newck = new HttpCookie("ClientId");
                newck.Value = newID;
                newck.Expires = DateTime.MaxValue;
                currentContext.Response.Cookies.Add(newck);
                return newID;
            }
        }

        public static string GetRequestIP(HttpContext currentContext)
        {
            string ipAddress = currentContext.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"] ?? "";
            ipAddress = ipAddress != "" ? ipAddress : (currentContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? "");
            ipAddress = ipAddress != "" ? ipAddress : (currentContext.Request.ServerVariables["REMOTE_ADDR"] ?? "");
            return ipAddress;
        }

    }
}