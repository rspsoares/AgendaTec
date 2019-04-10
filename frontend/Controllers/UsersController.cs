using AgendaTech.Business.Contracts;
using AgendaTech.Business.Entities;
using AgendaTech.View.Authorization;
using AgendaTech.View.Models;
using BrockAllen.MembershipReboot;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AgendaTech.View.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserFacade _userFacade;
        private AuthorizationHelper _claimHelper = new AuthorizationHelper();
        private UsuarioLogado _usuarioLogado = new UsuarioLogado();
        private readonly UserAccountService<CustomUserAccount> _userSvc;

        public UsersController(IUserFacade userFacade, UserAccountService<CustomUserAccount> userSvc)
        {
            _userFacade = userFacade;
            _usuarioLogado = _claimHelper.ObterUsuarioLogado();
            _userSvc = userSvc;
        }
      
        public ActionResult Index()
        {
            ViewBag.NomeUsuario = _usuarioLogado.Nome;
            return View();
        }

        [HttpGet]
        public JsonResult GetUserGroupCombo()
        {
            var userGroups = _userFacade.GetUserGroups(_usuarioLogado.Inscricao, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os grupos de usuários." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = userGroups, Total = userGroups.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGrid(string name, string login, string idCustomer, string idUserGroup)
        {
            var users = _userFacade.GetGrid(name, login, int.Parse(idCustomer), int.Parse(idUserGroup), out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os clientes." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = users, Total = users.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUser(string idUser)
        {
            var user = _userFacade.GetUserById(int.Parse(idUser), out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", errorMessage = "Houve um erro ao obter o usuário." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = user, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveUser(UserAccountDTO userDTO)
        {
            string errorMessage = string.Empty;

            var fullNameSplit = userDTO.FullName.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            userDTO.FirstName = fullNameSplit.FirstOrDefault();
            userDTO.LastName = string.Join(" ", fullNameSplit.Skip(1));

                if (userDTO.IDUser.Equals(0))                
                    userDTO.IDUser = _userSvc.CreateAccount(userDTO.UserName, "AgendaTech123", userDTO.Email).Key;                   
           
            _userFacade.Update(userDTO, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o usuário." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCurrentCustomer()
        {
            var currentCustomer = string.Empty;

            currentCustomer = _usuarioLogado.Inscricao.Equals(EnUserType.Administrator)
                ? string.Empty
                : _userFacade.GetUserByUq(_usuarioLogado.uqUsuario, out string errorMessage)?.CustomerName ?? string.Empty;

            return Json(new { Data = currentCustomer }, JsonRequestBehavior.AllowGet);
        }
    }    
}