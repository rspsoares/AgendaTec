using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AgendaTech.Portal.Controllers
{
    [Authorize]
    public class SchedulesController : Controller
    {
        private readonly IScheduleFacade _scheduleFacade;
        
        public SchedulesController(IScheduleFacade customerFacade)
        {
            _scheduleFacade = customerFacade;            
        }

        public ActionResult Index()
        {            
            return View();
        }

        [HttpGet]
        public JsonResult GetGrid(string idCustomer, string idProfessional, string idService, string idConsumer, string dateFrom, string dateTo, string bonus)
        {
            var professional = string.IsNullOrEmpty(idProfessional) ? 0 : int.Parse(idProfessional);
            var service = string.IsNullOrEmpty(idService) ? 0 : int.Parse(idService);
            var consumer = idConsumer ?? string.Empty;
            var dateInitial = string.IsNullOrEmpty(dateFrom) ? (DateTime?)null : DateTime.Parse(dateFrom);
            var dateFinal = string.IsNullOrEmpty(dateTo) ? (DateTime?)null : DateTime.Parse(dateTo);
            var bonusCheck = string.IsNullOrEmpty(bonus) ? (bool?)null : bonus.Equals("true") ? true : false;

            var schedules = _scheduleFacade.GetGrid(int.Parse(idCustomer), professional, service, consumer, dateInitial, dateFinal, bonusCheck, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter as agendas." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = schedules, Total = schedules.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAppointment(string idSchedule)
        {
            var customer = _scheduleFacade.GetScheduleById(int.Parse(idSchedule), out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", errorMessage = "Houve um erro ao obter o agendamento." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = customer, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAppointment(TSchedules schedule)
        {
            var schedules = new List<TSchedules>
            {
                schedule
            };

            var availabilityCheck = _scheduleFacade.CheckAvailability(schedules, out string errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao verificar a disponibilidade da agenda." }, JsonRequestBehavior.AllowGet);

            if (!string.IsNullOrEmpty(availabilityCheck))
                return Json(new { Success = false, errorMessage = availabilityCheck }, JsonRequestBehavior.AllowGet);

            if (schedule.IDSchedule.Equals(0))
                _scheduleFacade.Insert(schedule, out errorMessage);
            else
                _scheduleFacade.Update(schedule, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o cliente." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteAppointments(List<TSchedules> schedules)
        {
            _scheduleFacade.Delete(schedules, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao excluir o agendamento." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RescheduleAppointment(List<TSchedules> schedules, string newDate)
        {
            var availabilityCheck = _scheduleFacade.CheckAvailability(schedules, out string errorMessage, newDate);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao verificar a disponibilidade da agenda." }, JsonRequestBehavior.AllowGet);

            if (!string.IsNullOrEmpty(availabilityCheck))
                return Json(new { Success = false, errorMessage = availabilityCheck }, JsonRequestBehavior.AllowGet);

            _scheduleFacade.Reschedule(schedules, newDate, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao excluir o agendamento." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}