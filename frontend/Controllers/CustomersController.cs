using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.View.Authorization;
using AgendaTech.View.Models;
using System.Web.Mvc;

namespace AgendaTech.View.Controllers
{
    public class CustomersController : Controller
    {      
        private readonly ICustomerFacade _customerFacade;
        private AuthorizationHelper claimHelper = new AuthorizationHelper();       
        private UsuarioLogado usuarioLogado = new UsuarioLogado();

        public CustomersController(ICustomerFacade customerFacade)
        {
            _customerFacade = customerFacade;
            usuarioLogado = claimHelper.ObterUsuarioLogado(false);
        }
        
        [AuthorizeUser(AccessLevel = "/Administracao")]
        public ActionResult Index()
        {
            ViewBag.NomeUsuario = usuarioLogado.Nome;
            return View();
        }

        [HttpGet]
        public JsonResult GetGrid(string customerName)
        {            
            var customers = _customerFacade.GetGrid(customerName, out string errorMessage);
            
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Sucess = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os clientes." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Sucess = true, Data = customers, Total = customers.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCustomer(string idCustomer)
        {
            var customers = _customerFacade.GetCustomerById(int.Parse(idCustomer), out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Sucess = false, Data = "", errorMessage = "Houve um erro ao obter os clientes." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Sucess = true, Data = customers, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
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
                return Json(new { Sucess = false, errorMessage = "Houve um erro ao salvar o cliente." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Sucess = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}