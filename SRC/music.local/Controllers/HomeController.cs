using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Configuration;
using System.Web.Mvc;
using music.local.Bussiness;

namespace music.local.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Common.CheckLogin();
            var listAlbum = TrackProcessing.GetTree();
            ViewBag.Data = listAlbum;
            return View("/Views/Home.cshtml");
        }
        public ActionResult Demo(string p = "")
        {
            //Common.CheckLogin();
            return WaveFormProcessing.DemoDraw(p);
        }

        public ActionResult File(string p)
        {
            Common.CheckLogin();
            if (string.IsNullOrEmpty(p))
                return null;
            var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
            var filePath = physPath + p;
            if (System.IO.File.Exists(filePath))
            {
                FileInfo f = new FileInfo(filePath);
                var fileMine = "audio/mpeg3";
                if (!f.Extension.ToLower().Replace(".", "").Equals("mp3"))
                {
                    fileMine = "image/jpeg";
                }
                using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] data = new byte[str.Length];
                    int br = str.Read(data, 0, data.Length);
                    if (br != str.Length)
                        throw new System.IO.IOException(filePath);
                    return new FileContentResult(data, fileMine);
                }
            }
            return null;
        }

         /// <summary>
         /// renew session
         /// </summary>
         /// <returns></returns>
        public ActionResult CheckSession()
        {
            if (Common.CheckLogin(false))
            {
                return Content("1");
            }
            return Content("0");
        }
    }
}