﻿using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using AgendaTech.View.Models;
using BrockAllen.MembershipReboot;

namespace AgendaTech.View.Controllers
{
    [AllowAnonymous]
    public class RegistrarController : Controller
    {
        private readonly AuthenticationService<CustomUserAccount> _authSvc;
        private readonly UserAccountService<CustomUserAccount> _userSvc;      

        public RegistrarController(AuthenticationService<CustomUserAccount> authSvc, UserAccountService<CustomUserAccount> userSvc)
        {
            _authSvc = authSvc;
            _userSvc = userSvc;       
        }
        
        public ActionResult Index()
        {
            Registro model = new Registro();
            IEnumerable<TipoPerfil> perfis = System.Enum.GetValues(typeof(TipoPerfil)).Cast<TipoPerfil>();
            model.Roles = from tpPerfil in perfis
                          select new SelectListItem
                          {
                              Text = GetDescription(tpPerfil),
                              Value = ((int)tpPerfil).ToString()
                          };

            return View(model);
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult ConfirmSucess()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Registro modelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var account = _userSvc.CreateAccount(modelo.Username, modelo.Password, modelo.Email);

                    account.FirstName = modelo.FirstName;
                    account.LastName = modelo.LastName;
                    account.Source = modelo.Role;                    

                    _userSvc.AddClaims(account.ID, BuildClaims(modelo.Role));
                    _userSvc.Update(account);

                    ViewData["RequireAccountVerification"] = _userSvc.Configuration.RequireAccountVerification;
                    return View("Success", modelo);
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(modelo);
        }

        private UserClaimCollection BuildClaims(int papel)
        {
            UserClaimCollection uc = new UserClaimCollection();

            if (Between(papel, 1, 2))
            {
                uc.Add(ClaimTypes.Role, "Administrador");
                if (papel == 1)
                    uc.Add(ClaimTypes.Role, "Administrador");
                else if (papel == 2)
                    uc.Add(ClaimTypes.Role, "Suporte");
            }
            else if (Between(papel, 3, 4))
            {
                uc.Add(ClaimTypes.Role, "Mosaic");
                if (papel == 3)
                    uc.Add(ClaimTypes.Role, "Suporte");
                else if (papel == 4)
                    uc.Add(ClaimTypes.Role, "Usuário");
            }
            return uc;
        }

        bool Between(int value, int a, int b)
        {
            return value > a && value < b;
        }

        [AllowAnonymous]
        public ActionResult Confirm(string id)
        {
            var account = _userSvc.GetByVerificationKey(id);
            if (account.HasPassword())
            {
                var vm = new ChangeEmailFromKeyInputModel
                {
                    Key = id
                };
                return View("Confirm", vm);
            }
            else
            {
                try
                {
                    _userSvc.VerifyEmailFromKey(id, out account);
                 
                    _authSvc.SignIn(account);
                    return RedirectToAction("ConfirmSucess");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                return View("Confirm", null);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(ChangeEmailFromKeyInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {    
                    _userSvc.VerifyEmailFromKey(model.Key, model.Password, out CustomUserAccount account);
               
                    _authSvc.SignIn(account);
                    return RedirectToAction("ConfirmSucess");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View("Confirm", model);
        }

        public static string GetDescription(System.Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }
    }
}
