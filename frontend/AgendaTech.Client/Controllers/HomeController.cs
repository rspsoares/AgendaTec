using System.Web.Mvc;

namespace AgendaTech.Client.Controllers
{    
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