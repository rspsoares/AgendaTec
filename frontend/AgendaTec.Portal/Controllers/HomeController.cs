using System.Web.Mvc;

namespace AgendaTec.Portal.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {
        public HomeController()
        {
        
        }
        
        public ActionResult Index()
        {
            return View();
        }
    }
}