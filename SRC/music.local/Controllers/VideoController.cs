using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using music.local.Bussiness;

namespace music.local.Controllers
{
    public class VideoController : Controller
    {
        // GET: Video
        public ActionResult Index()
        {
            ViewBag.Data = TrackProcessing.GetVideoList();
            return View("/Views/Video/VideoIndex.cshtml");
        }

        public ActionResult Thumbnail(string p = "")
        {
            if (!string.IsNullOrEmpty(p))
            {
                var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
                var fPath = physPath+ p;
                if (System.IO.File.Exists(fPath))
                {
                    FileContentResult image;
                    using (var bmp = ThumbnailGen.GetThumbnail(fPath, 150, 86, ThumbnailOptions.None))
                    {
                        using (var ms = new MemoryStream())
                        {
                            bmp.Save(ms, ImageFormat.Png);
                            image = new FileContentResult(ms.ToArray(), "image/png");
                        }
                    }
                    return image;
                }
            }
            return null;
        }
    }
}