using System.Collections.Generic;
using System.Web.Mvc;
using AgendaTech.View.Authorization;
using Newtonsoft.Json;
using AgendaTech.View.Models;

namespace AgendaTech.View.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private AuthorizationHelper _claimHelper = new AuthorizationHelper();
        private UsuarioLogado _usuarioLogado = new UsuarioLogado();
     
        public HomeController()
        {            
            _usuarioLogado = _claimHelper.ObterUsuarioLogado();            
        }

        [AuthorizeUser(AccessLevel = "/Home")]
        public ActionResult Index()
        {
            ViewBag.NomeUsuario = _usuarioLogado.Nome;
            return View();
        }        
    }
}
