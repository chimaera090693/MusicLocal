using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            return View("/Views/Home.cshtml");
        }
    }
}