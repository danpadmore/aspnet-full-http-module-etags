using System.IO;
using System.Web.Mvc;

namespace ETag.Example.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Dummy()
        {
            var appData = Server.MapPath("~/app_data");
            var dummyPath = Path.Combine(appData, "dummy.json");
            var dummyData = System.IO.File.ReadAllText(dummyPath);

            return Json(dummyData, JsonRequestBehavior.AllowGet);
        }
    }
}