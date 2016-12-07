using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using log4net;

namespace music.local
{
    public class Common
    {
        public static string txtLogAccess = HostingEnvironment.ApplicationPhysicalPath + "\\Logs\\AccessLog.txt";

        private static readonly ILog Logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static void WriteLog(string title, string exception)
        {
            {
                Logger.Error("");
                Logger.Error("--------------------------------");
                Logger.Error(title + ": " + exception);
                Logger.Error("--------------------------------");
                Logger.Error("");
            }
        }

        public static void WriteDebug(string title, string exception)
        {
            {
                Logger.Debug("");
                Logger.Debug("--------------------------------");
                Logger.Debug(title + ": " + exception);
                Logger.Debug("--------------------------------");
                Logger.Debug("");
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái đăng nhập
        /// </summary>
        /// <param name="IsRedirect">redirect nếu chưa đăng nhập</param>
        /// <returns></returns>
        public static bool CheckLogin(bool IsRedirect = true)
        {
            var chkLogin = HttpContext.Current.Session["IsLogin"];
            if (chkLogin != null && (string) chkLogin == "ok")
            {
                return true;
            }
            if(IsRedirect) HttpContext.Current.Response.Redirect("~/Login");
            return false;
        }

        /// <summary>
        /// ghi log access ra file text
        /// </summary>
        public static void WriteLogAccess()
        {
            var ipAddress = HttpContext.Current.Request.UserHostAddress;
            var tr = HttpContext.Current.Request.Headers["User-Agent"];
            var msg = (DateTime.Now.ToString("s") + ": " + ipAddress + "    ||  User-Agent:" + tr);
            using (
                StreamWriter file =
                    new StreamWriter(txtLogAccess, true))
            {
                try
                {
                    file.WriteLine(msg);
                    file.WriteLine("=================================================================");
                    file.Flush();
                    file.Close();
                }
                catch (Exception ex)
                {
                    WriteLog(MethodBase.GetCurrentMethod().Name, ex + ex.StackTrace);
                }
            }
        }

        public static string GetFileName(string oldValue)
        {
            if (string.IsNullOrEmpty(oldValue)) return "";
            return oldValue.Replace(" ", "").Replace("(", "").Replace(")", "");
        }
    }
}