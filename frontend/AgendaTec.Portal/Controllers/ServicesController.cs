using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using System.Web.Mvc;

namespace AgendaTec.Portal.Controllers
{
    [Authorize]
    public class ServicesController : Controller
    {
        private readonly IServiceFacade _serviceFacade;        

        public ServicesController(IServiceFacade serviceFacade)
        {
            _serviceFacade = serviceFacade;            
        }

        public ActionResult Index()
        {            
            return View();
        }

        [HttpGet]
        public JsonResult GetGrid(string idCustomer, string serviceName)
        {
            int customer = string.IsNullOrEmpty(idCustomer) ? 0 : int.Parse(idCustomer);
            var services = _serviceFacade.GetGrid(customer, serviceName, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os serviços." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = services, Total = services.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetService(string idService)
        {
            var service = _serviceFacade.GetServiceById(int.Parse(idService), out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", errorMessage = "Houve um erro ao obter o serviço." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = service, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveService(ServiceDTO service)
        {
            string errorMessage = string.Empty;

            if (service.Id.Equals(0))
                _serviceFacade.Insert(service, out errorMessage);
            else
                _serviceFacade.Update(service, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o serviço." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetServiceNameCombo(string filter)
        {
            var customer = string.IsNullOrEmpty(filter) ? 0 : int.Parse(filter);
            var services = _serviceFacade.GetServiceNameCombo(customer, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os serviços." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = services, Total = services.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }       
    }
}