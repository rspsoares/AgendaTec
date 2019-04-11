using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.View.Authorization;
using AgendaTech.View.Models;
using System.Web.Mvc;

namespace AgendaTech.View.Controllers
{
    public class ProfessionalsController : Controller
    {
        private readonly IProfessionalFacade _professionalFacade;
        private AuthorizationHelper _claimHelper = new AuthorizationHelper();
        private UsuarioLogado _usuarioLogado = new UsuarioLogado();

        public ProfessionalsController(IProfessionalFacade professionalFacade)
        {
            _professionalFacade = professionalFacade;
            _usuarioLogado = _claimHelper.ObterUsuarioLogado();
        }      
       
        public ActionResult Index()
        {
            ViewBag.NomeUsuario = _usuarioLogado.Nome;
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
        public JsonResult SaveProfessional(TCGProfessionals professional)
        {
            string errorMessage = string.Empty;

            if (professional.IDProfessional.Equals(0))
                _professionalFacade.Insert(professional, out errorMessage);
            else
                _professionalFacade.Update(professional, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o profissional." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}