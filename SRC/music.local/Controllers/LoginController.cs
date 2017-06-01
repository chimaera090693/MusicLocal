using System.Web.Configuration;
using System.Web.Mvc;
using music.local.Bussiness;
using music.local.Bussiness.DataAccess;
using music.local.Filter;

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
                var clientId = LoginsProcessing.GetRequestId(System.Web.HttpContext.Current);
                var tr = System.Web.HttpContext.Current.Request.Headers["User-Agent"];
                LoginsProcessing.Login(clientId, tr);
                Response.Redirect(string.IsNullOrEmpty(ru)?"/":ru);
                return null;
            }
            return View("~/Views/Login/Login.cshtml");
        }

        [CustomAuthFilter]
        [HttpGet]
        public ActionResult AccessLog()
        {
            var listAC = Logins.Logins_Get();
            ViewBag.ListAccessLog = listAC;
            return View("~/Views/Login/AccessLog.cshtml");
        }

        [CustomAuthFilter]
        [HttpPost]
        public ActionResult AccessLog(string xx="/")
        {
            var ip = Request.Form["ActionRemove"];
            //if (!string.IsNullOrEmpty(ip))
            {
                var currentID = LoginsProcessing.GetRequestId(System.Web.HttpContext.Current);
                Logins.Logins_DeleteLog(ip);
                if (ip.Equals(currentID))
                {
                    Response.Redirect("/Login?ru=/Login/AccessLog");
                    return null;
                }
            }
            var listAC = Logins.Logins_Get();
            ViewBag.ListAccessLog = listAC;
            return View("~/Views/Login/AccessLog.cshtml");
        }


    }
}