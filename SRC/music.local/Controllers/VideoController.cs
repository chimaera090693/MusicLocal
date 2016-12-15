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
            if (!LoginsProcessing.CheckLogin(true)) return null;
            ViewBag.Data = TrackProcessing.GetVideoList();
            return View("/Views/Video/VideoIndex.cshtml");
        }

        public ActionResult Thumbnail(string p = "")
        {
            if (!LoginsProcessing.CheckLogin()) return null;
            if (!string.IsNullOrEmpty(p))
            {
                var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
                var thumbName = WaveFormProcessing.GetMd5Hash(p);
                var thumbSavePath = physPath + "\\_thumb\\" + thumbName + ".png";

                var fPath = physPath+ p;
                if (!System.IO.File.Exists(thumbSavePath))
                {
                    //không có => generate thumb
                    if (System.IO.File.Exists(fPath))
                    {
                        FileContentResult image;
                        using (var bmp = ThumbnailGen.GetThumbnail(fPath, 150, 86, ThumbnailOptions.None))
                        {
                            using (var ms = new MemoryStream())
                            {
                                bmp.Save(ms, ImageFormat.Png);
                                bmp.Save(thumbSavePath);
                                image = new FileContentResult(ms.ToArray(), "image/png");
                            }
                        }
                        return image;
                    }
                }
                else
                {
                    //có => đọc file
                    return  new FilePathResult(thumbSavePath, "image/png");
                }
            }
            return null;
        }
    }
}