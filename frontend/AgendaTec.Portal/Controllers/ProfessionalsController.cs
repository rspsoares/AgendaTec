using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using AgendaTec.Portal.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AgendaTec.Portal.Controllers
{
    [Authorize]
    public class ProfessionalsController : Controller
    {
        private readonly IProfessionalFacade _professionalFacade;
        private readonly IProfessionalServiceFacade _professionalServiceFacade;
        private readonly IUserFacade _userFacade;

        public ProfessionalsController(IProfessionalFacade professionalFacade, IProfessionalServiceFacade professionalServiceFacade, IUserFacade userFacade)
        {
            _professionalFacade = professionalFacade;
            _professionalServiceFacade = professionalServiceFacade;
            _userFacade = userFacade;
        }

        public ActionResult Index()
        {            
            return View();
        }

        [HttpGet]
        public JsonResult GetGrid(string idCustomer, string name)
        {
            int customer = string.IsNullOrEmpty(idCustomer) ? 0 : int.Parse(idCustomer);

            var professionals = _professionalFacade.GetGrid(customer, name, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os profissionais." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = professionals, Total = professionals.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProfessional(string idProfessional)
        {
            var professional = _professionalFacade.GetProfessionalById(int.Parse(idProfessional), out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", errorMessage = "Houve um erro ao obter o profissional." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = professional, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> SaveProfessional(ProfessionalDTO professional)
        {
            string errorMessage;
          
            if (professional.Id.Equals(0))
            {
                var firstName = professional.Name.IndexOf(" ").Equals(-1) ? professional.Name : professional.Name.Substring(0, professional.Name.IndexOf(" "));
                var lastName = professional.Name.IndexOf(" ").Equals(-1) ? string.Empty : professional.Name.Substring(professional.Name.IndexOf(" ") + 1);

                var user = new ApplicationUser
                {   
                    IDRole = ((int)EnUserType.Professional).ToString(),
                    FirstName = firstName,
                    LastName = lastName,
                    CPF = professional.CPF.CleanMask(),
                    UserName = professional.Email,
                    Email = professional.Email,
                    PhoneNumber = professional.Phone.CleanMask(),
                    IsEnabled = true,
                    RootUser = false,
                    DirectMail = false
                };

                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var result = await userManager.CreateAsync(user, "AgendaTec123");
                if (!result.Succeeded)
                    return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o usuário." }, JsonRequestBehavior.AllowGet);

                var e = new UserAssociatedCustomerDTO()
                {
                    IDCustomer = professional.IdCustomer,
                    Email = professional.Email
                };
                _userFacade.CheckUserAssociatedWithCustomer(e, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                    return Json(new { Success = false, errorMessage = "Houve um erro ao associar o usuário ao Customer." }, JsonRequestBehavior.AllowGet);

                professional.IdUser = user.Id;
                _professionalFacade.Insert(professional, out errorMessage);
            }                
            else                            
                _professionalFacade.Update(professional, out errorMessage);                            

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o profissional." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProfessionalNameCombo(string filter)
        {
            var customer = string.IsNullOrEmpty(filter) ? 0 : int.Parse(filter);            
            var professionals = _professionalFacade.GetProfessionalNameCombo(customer, Guid.Empty, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os profissionais." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = professionals, Total = professionals.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProfessionalServices(string idCustomer, string idProfessional)
        {
            var professional = string.IsNullOrEmpty(idProfessional) ? 0 : int.Parse(idProfessional);
            var customer = string.IsNullOrEmpty(idCustomer) ? 0 : int.Parse(idCustomer);

            var services = _professionalServiceFacade.GetAvailablesProfessionalServices(customer, professional, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os serviços do profissional." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = services, Total = services.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}