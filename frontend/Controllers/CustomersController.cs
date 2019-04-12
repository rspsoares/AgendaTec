using AgendaTech.Business.Contracts;
using AgendaTech.Business.Entities;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.View.Authorization;
using AgendaTech.View.Models;
using System.Web.Mvc;

namespace AgendaTech.View.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {      
        private readonly ICustomerFacade _customerFacade;
        private AuthorizationHelper _claimHelper = new AuthorizationHelper();       
        private UsuarioLogado _usuarioLogado = new UsuarioLogado();

        public CustomersController(ICustomerFacade customerFacade)
        {
            _customerFacade = customerFacade;
            _usuarioLogado = _claimHelper.ObterUsuarioLogado();
        }
        
        [AuthorizeUser(AccessLevel = "/Administracao")]
        public ActionResult Index()
        {
            ViewBag.NomeUsuario = _usuarioLogado.Nome;
            return View();
        }

        [HttpGet]
        public JsonResult GetGrid(string customerName)
        {            
            var customers = _customerFacade.GetGrid(customerName, out string errorMessage);
            
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os clientes." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = customers, Total = customers.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCompanyNameCombo()
        {
            var idCustomer = _usuarioLogado.Inscricao.Equals(EnUserType.Administrator) ? 0 : _usuarioLogado.IDCustomer;
            var customers = _customerFacade.GetCompanyNameCombo(idCustomer, out string errorMessage);
                
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os clientes." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = customers, Total = customers.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCustomer(string idCustomer)
        {
            var customer = _customerFacade.GetCustomerById(int.Parse(idCustomer), out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", errorMessage = "Houve um erro ao obter o cliente." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = customer, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveCustomer(TCGCustomers customer)
        {
            string errorMessage = string.Empty;

            if (customer.IDCustomer.Equals(0))
                _customerFacade.Insert(customer, out errorMessage);
            else
                _customerFacade.Update(customer, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o cliente." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}