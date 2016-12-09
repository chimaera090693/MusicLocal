using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using music.local.Bussiness;

namespace music.local.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index(string ru = "")
        {
            Common.WriteLogAccess();
            if (LoginsProcessing.CheckLogin())
            {
                Response.Redirect(ru);
                return null;
            }
            return View("~/Views/Login.cshtml");
        }

        [HttpPost]
        public ActionResult Login(string ru = "", string pass = "")
        {
            var PassInWF = WebConfigurationManager.AppSettings["Password"];
            if (PassInWF.Equals(pass))
            {
                var ipAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
                var tr = System.Web.HttpContext.Current.Request.Headers["User-Agent"];
                LoginsProcessing.Login(ipAddress, tr);
                Response.Redirect(ru??"/");
                return null;
            }
            return View("~/Views/Login.cshtml");
        }

    }
}