using AgendaTec.Business.Contracts;
using System;
using System.Web.Mvc;

namespace AgendaTec.Portal.Controllers
{
    [Authorize]
    public class SchedulesReportController : Controller
    {
        private readonly IReportFacade _reportFacade;

        public SchedulesReportController(IReportFacade reportFacade)
        {
            _reportFacade = reportFacade;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetGrid(string idCustomer, string dateFrom, string dateTo)
        {
            var result = new JsonResult();
            var customer = string.IsNullOrEmpty(idCustomer) ? 0 : int.Parse(idCustomer);          
            var dateInitial = DateTime.Parse(dateFrom);
            var dateFinal = DateTime.Parse(dateTo);
           
            var report = _reportFacade.GetScheduleReport(customer, dateInitial, dateFinal, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                result = Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao gerar o relatório." }, JsonRequestBehavior.AllowGet);
            else
                result = Json(new { Success = true, Data = report, Total = report.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;

            return result;
        }
    }
}