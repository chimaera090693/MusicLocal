using System.Reflection;
using System.Web;
using log4net;

namespace music.local
{
    public class Common
    {
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
    }
}