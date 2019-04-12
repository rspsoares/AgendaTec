using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.View.Authorization;
using AgendaTech.View.Models;
using System.Web.Mvc;

namespace AgendaTech.View.Controllers
{
    [Authorize]
    public class ServicesController : Controller
    {
        private readonly IServiceFacade _serviceFacade;
        private AuthorizationHelper _claimHelper = new AuthorizationHelper();
        private UsuarioLogado _usuarioLogado = new UsuarioLogado();

        public ServicesController(IServiceFacade serviceFacade)
        {
            _serviceFacade = serviceFacade;
            _usuarioLogado = _claimHelper.ObterUsuarioLogado();
        }
        
        public ActionResult Index()
        {
            ViewBag.NomeUsuario = _usuarioLogado.Nome;
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
        public JsonResult SaveService(TCGServices service)
        {
            string errorMessage = string.Empty;

            if (service.IDService.Equals(0))
                _serviceFacade.Insert(service, out errorMessage);
            else
                _serviceFacade.Update(service, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o serviço." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
} 