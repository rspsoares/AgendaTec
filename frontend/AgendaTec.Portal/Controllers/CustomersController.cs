using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using AgendaTec.Portal.Helper;
using System.Web.Mvc;

namespace AgendaTec.Portal.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ICustomerFacade _customerFacade;        

        public CustomersController(ICustomerFacade customerFacade)
        {
            _customerFacade = customerFacade;            
        }

        public ActionResult Index()
        {
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
            var idCustomer = int.Parse(User.GetIdRole()).Equals((int)EnUserType.Administrator) ? 0 : int.Parse(User.GetIdCustomer());
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

        [HttpGet]
        public JsonResult GetCustomerHours()
        {
            var customer = _customerFacade.GetCustomerById(int.Parse(User.GetIdCustomer()), out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", errorMessage = "Houve um erro ao obter o cliente." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = customer, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveCustomer(CustomerDTO customer)
        {
            string errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(customer.CNPJ))
            {
                if (!customer.CNPJ.Length.Equals(11) && !customer.CNPJ.Length.Equals(14))
                    return Json(new { Success = false, errorMessage = "CPF / CNPJ inválido." }, JsonRequestBehavior.AllowGet);

                if (customer.CNPJ.Length.Equals(11) && !customer.CNPJ.IsCPF())
                    return Json(new { Success = false, errorMessage = "CPF inválido." }, JsonRequestBehavior.AllowGet);

                if (customer.CNPJ.Length.Equals(14) && !customer.CNPJ.IsCNPJ())
                    return Json(new { Success = false, errorMessage = "CNPJ inválido." }, JsonRequestBehavior.AllowGet);
            }

            if(!_customerFacade.CheckValidTimeRanges(customer.TimeRanges, out errorMessage))
            {
                if(!string.IsNullOrEmpty(errorMessage))
                    return Json(new { Success = false, errorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Success = false, errorMessage = "Existe sobreposição nos intervalos de horários." }, JsonRequestBehavior.AllowGet);
            }                

            if (customer.Id.Equals(0))
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