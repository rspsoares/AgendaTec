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

        public ProfessionalsController(IProfessionalFacade professionalFacade, IProfessionalServiceFacade professionalServiceFacade)
        {
            _professionalFacade = professionalFacade;
            _professionalServiceFacade = professionalServiceFacade;
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
                var userInUse = _professionalFacade.CheckUserInUse(professional.Email, out errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                    return Json(new { Success = false, errorMessage = "Houve um erro na verificação do profissional." }, JsonRequestBehavior.AllowGet);

                if (userInUse)
                    return Json(new { Success = false, errorMessage = "Este e-mail já está sendo utilizado por outro funcionário." }, JsonRequestBehavior.AllowGet);

                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                var user = new ApplicationUser
                {
                    //IDCustomer = professional.IdCustomer,
                    IDRole = ((int)EnUserType.Professional).ToString(),
                    FirstName = professional.Name.Substring(0, professional.Name.IndexOf(" ")),
                    LastName = professional.Name.Substring(professional.Name.IndexOf(" ") + 1),
                    CPF = professional.CPF.CleanMask(),
                    UserName = professional.Email,
                    Email = professional.Email,
                    PhoneNumber = professional.Phone.CleanMask(),
                    IsEnabled = true,
                    RootUser = false
                };

                var result = await userManager.CreateAsync(user, "AgendaTec123");
                if (!result.Succeeded)
                    return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o usuário." }, JsonRequestBehavior.AllowGet);

                professional.IdUser = user.Id;                
                _professionalFacade.Insert(professional, out errorMessage);
            }                
            else
            {
                var userInUse = _professionalFacade.CheckUserInUse(professional.Id, professional.IdUser, out errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                    return Json(new { Success = false, errorMessage = "Houve um erro na verificação do profissional." }, JsonRequestBehavior.AllowGet);

                if (userInUse)
                    return Json(new { Success = false, errorMessage = "Este usuário já está sendo utilizado em outro funcionário." }, JsonRequestBehavior.AllowGet);

                _professionalFacade.Update(professional, out errorMessage);
            }                

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