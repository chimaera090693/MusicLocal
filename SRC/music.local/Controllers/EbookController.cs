using System.Web.Mvc;
using music.local.Bussiness;

namespace music.local.Controllers
{
    public class EbookController : Controller
    {
        public ActionResult Index()
        {

            ViewBag.Data = TrackProcessing.GetEbookList();

            return View("~/Views/Ebook.cshtml");
        }
    }
}