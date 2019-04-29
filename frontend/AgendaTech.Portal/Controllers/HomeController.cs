using System.Web.Mvc;

namespace AgendaTech.Portal.Controllers
{
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