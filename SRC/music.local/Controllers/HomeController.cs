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
            var listAlbum = TrackProcessing.GetTree();
            ViewBag.Data = listAlbum;
            return View("/Views/Home.cshtml");
        }
        public ActionResult Demo(string p="")
        {
            return WaveFormProcessing.DemoDraw(p);
            //return Content("null");
        }

        public ActionResult File(string p)
        {
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
                    //var ms = str.
                    //return new FileContentResult(str, "image/png");

                    return new FileContentResult(data, fileMine);
                }
                //return new FilePathResult(filePath, fileMine)
                //{
                //    FileDownloadName = f.Name
                //};
            }
            return null;
        }

        //public HttpResponseMessage Stream(string p)
        //{
        //    HttpResponseMessage response = new HttpResponseMessage();
            
        //}
    }
}