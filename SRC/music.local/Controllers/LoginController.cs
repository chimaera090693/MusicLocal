using System.Web.Configuration;
using System.Web.Mvc;
using music.local.Bussiness;
using music.local.Bussiness.DataAccess;

namespace music.local.Controllers
{
    public class LoginController : Controller
    {
        /// <summary>
        /// Login home
        /// </summary>
        /// <param name="ru"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(string ru = "/")
        {
            Common.WriteLogAccess();
            if (LoginsProcessing.CheckLogin())
            {
                Response.Redirect(string.IsNullOrEmpty(ru)?"/":ru);
                return null;
            }
            ViewBag.RediectURL = ru;
            return View("~/Views/Login/Login.cshtml");
        }
        [HttpPost]
        public ActionResult Login(string ru = "/", string pass = "")
        {
            var PassInWF = WebConfigurationManager.AppSettings["Password"];
            if (PassInWF.Equals(pass))
            {
                var ipAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
                var tr = System.Web.HttpContext.Current.Request.Headers["User-Agent"];
                LoginsProcessing.Login(ipAddress, tr);
                Response.Redirect(string.IsNullOrEmpty(ru)?"/":ru);
                return null;
            }
            return View("~/Views/Login/Login.cshtml");
        }

        [HttpGet]
        public ActionResult AccessLog()
        {
            if (!LoginsProcessing.CheckLogin(true)) return null;
            var listAC = Bussiness.DataAccess.Logins.Logins_Get();
            ViewBag.ListAccessLog = listAC;
            return View("~/Views/Login/AccessLog.cshtml");
        }

        [HttpPost]
        public ActionResult ActionLog()
        {
            if (!LoginsProcessing.CheckLogin(true)) return null;

            var ip = Request.Form["ActionRemove"];
            if (!string.IsNullOrEmpty(ip))
            {
                var currentIp = System.Web.HttpContext.Current.Request.UserHostAddress;
                Logins.Logins_DeleteLog(ip);
                if (ip.Equals(currentIp))
                {
                    Response.Redirect("/Login?ru=/Login/AccessLog");
                    return null;
                }
            }
            var listAC = Bussiness.DataAccess.Logins.Logins_Get();
            ViewBag.ListAccessLog = listAC;
            return View("~/Views/Login/AccessLog.cshtml");
        }


    }
}