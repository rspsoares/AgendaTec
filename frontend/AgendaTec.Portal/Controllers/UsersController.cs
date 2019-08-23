using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Portal.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AgendaTec.Portal.Helper;
using AgendaTec.Business.Helpers;
using System;

namespace AgendaTec.Portal.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserFacade _userFacade;
        private readonly ICustomerFacade _customerFacade;
        
        public UsersController(IUserFacade userFacade, ICustomerFacade customerFacade)
        {
            _userFacade = userFacade;
            _customerFacade = customerFacade;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetRoleCombo()
        {
            var enLoggedUserType = (EnUserType)int.Parse(User.GetIdRole());
            var userGroups = _userFacade.GetRolesCombo(enLoggedUserType, out string errorMessage);            

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os grupos de usuários." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = userGroups, Total = userGroups.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGrid(string name, string email, string idCustomer, string idRole)
        {
            var users = _userFacade.GetGrid(name, email, int.Parse(idCustomer), idRole, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os clientes." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = users, Total = users.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUser(string idUser)
        {
            var user = _userFacade.GetUserById(idUser, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", errorMessage = "Houve um erro ao obter o usuário." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = user, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> SaveUser(UserAccountDTO userDTO)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var checkResult = _userFacade.CheckDuplicatedUser(userDTO, out string errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao verificar a duplicidade do usuário." }, JsonRequestBehavior.AllowGet);

            if (!string.IsNullOrEmpty(checkResult))
                return Json(new { Success = false, errorMessage = checkResult }, JsonRequestBehavior.AllowGet);

            if(!_customerFacade.CheckCPFRequired(int.Parse(userDTO.IDCustomer), userDTO.CPF))                 
                return Json(new { Success = false, errorMessage = "CPF inválido." }, JsonRequestBehavior.AllowGet);

            if (string.IsNullOrEmpty(userDTO.Id))
            {
                var user = new ApplicationUser
                {                   
                    IDRole = userDTO.IdRole,
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    CPF = userDTO.CPF.CleanMask(),
                    BirthDate = DateTime.Parse(userDTO.Birthday),
                    UserName = userDTO.Email,
                    Email = userDTO.Email,
                    PhoneNumber = userDTO.Phone.CleanMask(),
                    IsEnabled = userDTO.IsEnabled,
                    RootUser = _userFacade.GetUserIsRoot(int.Parse(userDTO.IDCustomer), int.Parse(userDTO.IdRole)),
                    DirectMail = userDTO.DirectMail
                };

                var result = await userManager.CreateAsync(user, "AgendaTec123");
                if (!result.Succeeded)
                    return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o usuário." }, JsonRequestBehavior.AllowGet);
            }
            else
                _userFacade.Update(userDTO, out errorMessage);

            var e = new UserAssociatedCustomerDTO()
            {
                IDCustomer = int.Parse(userDTO.IDCustomer),
                Email = userDTO.Email
            };
            _userFacade.CheckUserAssociatedWithCustomer(e, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao associar o usuário ao Customer." }, JsonRequestBehavior.AllowGet);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o usuário." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckUserIsConsumer(string idRole)
        {
            var enLoggedUserType = (EnUserType)int.Parse(User.GetIdRole());
            var consumer = int.Parse(idRole).Equals((int)EnUserType.Consumer) && !enLoggedUserType.Equals(EnUserType.Administrator);
            return Json(new { Data = consumer }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUserNameCombo(string filter)
        {
            var customer = string.IsNullOrEmpty(filter) ? 0 : int.Parse(filter);
            var userGroups = _userFacade.GetUserNamesCombo(customer, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os grupos de usuários." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = userGroups, Total = userGroups.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProfessionalNameCombo(string filter)
        {
            var customer = string.IsNullOrEmpty(filter) ? 0 : int.Parse(filter);
            var userGroups = _userFacade.GetProfessionalNamesCombo(customer, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os profissionais." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = userGroups, Total = userGroups.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetConsumerNamesCombo(string filter)
        {
            var customer = string.IsNullOrEmpty(filter) ? 0 : int.Parse(filter);
            var userGroups = _userFacade.GetConsumerNamesCombo(customer, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os grupos de usuários." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = userGroups, Total = userGroups.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}