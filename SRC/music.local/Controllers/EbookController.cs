using System.Web.Mvc;
using music.local.Bussiness;
using music.local.Filter;

namespace music.local.Controllers
{   
    [CustomAuthFilter]
    public class EbookController : Controller
    {
        public ActionResult Index()
        {

            ViewBag.Data = TrackProcessing.GetEbookList();

            return View("~/Views/Ebook.cshtml");
        }
    }
}