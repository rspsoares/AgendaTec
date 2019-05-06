using AgendaTech.Business.Contracts;
using AgendaTech.Business.Entities;
using AgendaTech.Portal.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AgendaTech.Portal.Helper;

namespace AgendaTech.Portal.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserFacade _userFacade;
        
        public UsersController(IUserFacade userFacade)
        {
            _userFacade = userFacade;            
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

            userDTO.FirstName = userDTO.FirstName;
            userDTO.LastName = userDTO.LastName;

            var checkResult = _userFacade.CheckDuplicatedUser(userDTO, out string errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o usuário." }, JsonRequestBehavior.AllowGet);

            if (!string.IsNullOrEmpty(checkResult))
                return Json(new { Success = false, errorMessage = checkResult }, JsonRequestBehavior.AllowGet);

            if (string.IsNullOrEmpty(userDTO.Id))
            {
                var user = new ApplicationUser
                {
                    IDCustomer = userDTO.IDCustomer,
                    IDRole = userDTO.IdRole,
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    UserName = userDTO.Email,
                    Email = userDTO.Email,
                    IsEnabled = userDTO.IsEnabled
                };

                var result = await userManager.CreateAsync(user, "AgendaTec123");
                if (!result.Succeeded)
                    return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o usuário." }, JsonRequestBehavior.AllowGet);
            }
            else
                _userFacade.Update(userDTO, out errorMessage);

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
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os grupos de usuários." }, JsonRequestBehavior.AllowGet);
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