using System.Web.Mvc;
using music.local.Bussiness;

namespace music.local.Filter
{
    public class CustomAuthFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var str = "";
            bool isUnauthor = false;
            var httpCookie = filterContext.HttpContext.Request.Cookies.Get("ClientId");
            if (httpCookie != null) str = httpCookie.Value;

            if (string.IsNullOrEmpty(str))
            {
                isUnauthor = true;
            }
            else
            {
                isUnauthor = !LoginsProcessing.ValiadateLogin(str);
            }

            if (isUnauthor)
            {
                //redirect
                var abpath = "";
                if (filterContext.HttpContext.Request.Url != null)
                {
                    abpath = filterContext.HttpContext.Request.Url.AbsolutePath;
                }
                filterContext.Result = new RedirectResult("~/Login?ru=" + abpath);
            }
        }
    }
}