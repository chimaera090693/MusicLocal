using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace music.local.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            Common.WriteLogAccess();
            if (Common.CheckLogin(false))
            {
                Response.Redirect("/");
                return null;
            }
                return View("~/Views/Login.cshtml");
        }

        [HttpPost]
        public ActionResult Index(string pass)
        {
            var PassInWF = WebConfigurationManager.AppSettings["Password"];
            if (PassInWF.Equals(pass))
            {
                Session["IsLogin"] = "ok";
                Response.Redirect("/");
                return null;
            }
            return View("~/Views/Login.cshtml");
        }

    }
}