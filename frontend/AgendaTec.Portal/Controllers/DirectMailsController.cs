using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using System;
using System.Web;
using System.Web.Mvc;

namespace AgendaTec.Portal.Controllers
{
    [Authorize]
    public class DirectMailsController : Controller
    {
        private readonly IDirectMailFacade _directMailFacade;        

        public DirectMailsController(IDirectMailFacade directMailFacade)
        {
            _directMailFacade = directMailFacade;
        }

        public ActionResult Index(string MailType)
        {
            var enMailType = (EnMailType)int.Parse(MailType);

            ViewData["MailType"] = StringExtensions.GetEnumDescription(enMailType);
            return View();
        }

        [HttpGet]
        public JsonResult GetGrid(string idCustomer, string mailType, string description)
        {
            int customer = string.IsNullOrEmpty(idCustomer) ? 0 : int.Parse(idCustomer);

            var directMails = _directMailFacade.GetGrid(customer, int.Parse(mailType), description, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter as Malas Diretas." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = directMails, Total = directMails.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDirectMail(string idDirectMail)
        {
            var directMail = _directMailFacade.GetDirectMailingById(int.Parse(idDirectMail), out string errorMessage);

            var enMailingIntervalType = (EnMailIntervalType)directMail.IntervalType;
            directMail.Interval = enMailingIntervalType.ToString();

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", errorMessage = "Houve um erro ao obter a Mala Direta." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = directMail, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveDirectMail(DirectMailDTO directMail)
        {
            string errorMessage;

            Enum.TryParse(directMail.Interval, out EnMailIntervalType enIntervalType);
            directMail.IntervalType = (int)enIntervalType;
            
            directMail.Content = HttpUtility.HtmlDecode(directMail.Content);            

            if (directMail.Id.Equals(0))
                _directMailFacade.Insert(directMail, out errorMessage);
            else
                _directMailFacade.Update(directMail, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar a Mala Direta." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetIntervalCombo()
        {            
            var intervals = _directMailFacade.GetIntervalCombo(out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter os intervalos." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = intervals, Total = intervals.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteDirectMail(string idDirectMail)
        {
            _directMailFacade.Delete(int.Parse(idDirectMail), out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao excluir a Mala Direta selecionada." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ResendDirectMail(string idDirectMail)
        {
            _directMailFacade.ResendDirectMail(int.Parse(idDirectMail), out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao excluir a Mala Direta selecionada." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}