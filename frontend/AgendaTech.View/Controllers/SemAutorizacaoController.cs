using AgendaTech.View.Models;
using AgendaTech.View.Authorization;
using System.Web.Mvc;

namespace AgendaTech.View.Controllers
{
    [AllowAnonymous]
    public class SemAutorizacaoController : Controller
    {
        private UsuarioLogado usuarioLogado = new UsuarioLogado();
        private AuthorizationHelper claimHelper = new AuthorizationHelper();

        public SemAutorizacaoController()
        {
            usuarioLogado = claimHelper.ObterUsuarioLogado();
        }

        //
        // GET: /SemAutorizacao/

        public ActionResult Index()
        {
            ViewBag.NomeUsuario = usuarioLogado.Nome;
            return View();
        }

        [Authorize]
        public ActionResult Interno()
        {
            return View();
        }
    }
}
