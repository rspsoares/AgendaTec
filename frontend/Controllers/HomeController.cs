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
        private AuthorizationHelper claimHelper = new AuthorizationHelper();
        private UsuarioLogado usuarioLogado = new UsuarioLogado();
     
        public HomeController()
        {            
            usuarioLogado = claimHelper.ObterUsuarioLogado();            
        }

        [AuthorizeUser(AccessLevel = "/Home")]
        public ActionResult Index()
        {
            ViewBag.NomeUsuario = usuarioLogado.Nome;
            return View();
        }

        [AuthorizeUser(AccessLevel = "/Home")]
        public ActionResult Dashboard()
        {
            ViewBag.NomeUsuario = usuarioLogado.Nome;
            return View();
        }
        
        public string PesquisarInscricoes()
        {
          //  CadastrosFacade _cadastro = new CadastrosFacade();
            string msg = string.Empty;

            //List<InscricoesCompletaDto> lstInscricoes = _cadastro.RetornarInscricoesCompletas(0, out msg);

         //   if (msg != string.Empty)
                return JsonConvert.SerializeObject("MSG: Ocorreu um erro ao obter as Inscrições.");
        //    else
        //        return JsonConvert.SerializeObject(lstInscricoes);
        }    
    }
}
