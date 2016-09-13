using CreateSiteMap.UI.Models;
using CreateSiteMap.UI.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CreateSiteMap.UI.Controllers
{
    public class HomeController : Controller
    {
        private MainService _mainService;
        private HelperService _helperService;

        public HomeController()
        {
            _mainService = new MainService();
            _helperService = new HelperService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Results(string url)
        {
            PageResultView result = await _mainService.ResultsAsync(url);            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult History(string url)
        {
            string hostUrl = _helperService.GetHostFromUrl(url);
            HistoryView history;

            if (hostUrl!=null)
            {
                history = _mainService.GetHistory(hostUrl);
                return Json(history, JsonRequestBehavior.AllowGet);
            }

            return Json(HistoryView.CreateHistoryError(url, "History is empty"), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (_mainService != null)
                _mainService.Dispose();

            base.Dispose(disposing);
        }
    }
}