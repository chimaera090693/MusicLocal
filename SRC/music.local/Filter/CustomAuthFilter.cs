using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace music.local.Filter
{
    public class CustomAuthFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var str = "";
            str += "On author Action:\r\n";
            str+= 

            filterContext.Controller.ViewBag.AutherizationMessage = str;

        }
    }
}