using System.Linq;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using AgendaTech.View.Models;
using AgendaTech.View.Authorization;
using System.ComponentModel.DataAnnotations;

namespace AgendaTech.View.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        //
        // GET: /Login/        
        private readonly AuthenticationService<CustomUserAccount> _authSvc;
        private readonly UserAccountService<CustomUserAccount> _userSvc;
        private AuthorizationHelper _claimHelper = new AuthorizationHelper();
        private UsuarioLogado _usuarioLogado = new UsuarioLogado();

        public LoginController(AuthenticationService<CustomUserAccount> authSvc, UserAccountService<CustomUserAccount> userSvc)
        {
            _authSvc = authSvc;
            _userSvc = userSvc;        
        }
        
        public ActionResult Index(string sessaoExpirada = "")
        {            
            ViewBag.SesaoExpirada = sessaoExpirada;            
            return View(new EntradaModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(EntradaModel model)
        {
            if (this.ModelState.IsValid)
            {
                CustomUserAccount account;
                if (_userSvc.AuthenticateWithUsernameOrEmail(model.Username, model.Password, out account))
                {
                    _authSvc.SignIn(account, model.RememberMe);

                    if (_userSvc.IsPasswordExpired(account))
                    {
                        return this.RedirectToAction("Login", "ChangePassword");
                    }
                    else
                    {
                        if (this.Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return this.Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return this.RedirectToAction("Index", "Home");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Usuário ou Senha Inválida");
                }

            }
            return this.View(model);
        }

        [Authorize]
        public ActionResult UpdatePass()
        {
            _usuarioLogado = _claimHelper.ObterUsuarioLogado();
            ViewBag.NomeUsuario = _usuarioLogado.Nome;
            return View(new ModificarSenha());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePass(ModificarSenha modelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _userSvc.ChangePassword(User.GetUserID(), modelo.OldPassword, modelo.NewPassword);
                    ModelState.AddModelError("Sucesso", "Senha Modificada");
                    return View();
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(modelo);
        }

        public ActionResult RestorePass()
        {            
            ViewBag.NomeUsuario = _usuarioLogado.Nome;
            return View();
        }

        public ActionResult SelectCNPJ()
        {
            ViewBag.NomeUsuario = _usuarioLogado.Nome;
            return View();
        }

        public ActionResult RestorePassSucess()
        {
            ViewBag.NomeUsuario = _usuarioLogado.Nome;
            return View();
        }

        public ActionResult Success()
        {
            ViewBag.NomeUsuario = _usuarioLogado.Nome;
            return View();
        }

        public ActionResult SignOut()
        {
            _authSvc.SignOut();
            return this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RestorePass(RestauraEmail model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var account = _userSvc.GetByEmail(model.Email);
                    if (account != null)
                    {
                        if (!account.PasswordResetSecrets.Any())
                        {
                            _userSvc.ResetPassword(model.Email);

                            return View("RestorePassSucess");
                        }

                        var vm = new PasswordResetWithSecretInputModel(account.ID);
                        vm.Questions =
                            account.PasswordResetSecrets.Select(
                                x => new PasswordResetSecretViewModel
                                {
                                    QuestionID = x.PasswordResetSecretID,
                                    Question = x.Question
                                }).ToArray();

                        return View("ResetWithQuestions", vm);
                    }
                    else
                    {
                        ModelState.AddModelError("", "E-mail não existe");
                    }
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View("RestorePass");
        }

        public ActionResult Confirm(string id)
        {
            var vm = new ChangePasswordFromResetKeyInputModel()
            {
                Key = id
            };
            return View("Confirm", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(ChangePasswordFromResetKeyInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CustomUserAccount account;
                    if (_userSvc.ChangePasswordFromResetKey(model.Key, model.Password, out account))
                    {
                        if (account.IsLoginAllowed && !account.IsAccountClosed)
                        {
                            _authSvc.SignIn(account);
                            if (account.RequiresTwoFactorAuthCodeToSignIn())
                            {
                                return RedirectToAction("TwoFactorAuthCodeLogin", "Login");
                            }
                            if (account.RequiresTwoFactorCertificateToSignIn())
                            {
                                return RedirectToAction("CertificateLogin", "Login");
                            }
                        }


                        return RedirectToAction("Success");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Erro na mudança da senha. A chave deve ser inválida.");
                    }
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View();
        }
    }
}
