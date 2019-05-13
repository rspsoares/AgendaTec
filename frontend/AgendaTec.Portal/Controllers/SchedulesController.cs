using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Portal.Helper;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AgendaTec.Portal.Controllers
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
            var customer = string.IsNullOrEmpty(idCustomer) ? 0 : int.Parse(idCustomer);
            var professional = string.IsNullOrEmpty(idProfessional) ? 0 : int.Parse(idProfessional);
            var service = string.IsNullOrEmpty(idService) ? 0 : int.Parse(idService);
            var consumer = idConsumer ?? string.Empty;
            var dateInitial = string.IsNullOrEmpty(dateFrom) ? (DateTime?)null : DateTime.Parse(dateFrom);
            var dateFinal = string.IsNullOrEmpty(dateTo) ? (DateTime?)null : DateTime.Parse(dateTo);
            var bonusCheck = string.IsNullOrEmpty(bonus) ? (bool?)null : bonus.Equals("true") ? true : false;

            var schedules = _scheduleFacade.GetGrid(customer, professional, service, consumer, dateInitial, dateFinal, bonusCheck, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", Total = 0, errorMessage = "Houve um erro ao obter as agendas." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = schedules, Total = schedules.Count, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAppointment(string idSchedule)
        {
            var schedule = _scheduleFacade.GetScheduleById(int.Parse(idSchedule), out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", errorMessage = "Houve um erro ao obter o agendamento." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = schedule, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAppointment(ScheduleDTO schedule)
        {
            var schedules = new List<ScheduleDTO>
            {
                schedule
            };

            var availabilityCheck = _scheduleFacade.CheckAvailability(schedules, out string errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao verificar a disponibilidade da agenda." }, JsonRequestBehavior.AllowGet);

            if (!string.IsNullOrEmpty(availabilityCheck))
                return Json(new { Success = false, errorMessage = availabilityCheck }, JsonRequestBehavior.AllowGet);

            if (schedule.Id.Equals(0))
                _scheduleFacade.Insert(schedule, out errorMessage);
            else
                _scheduleFacade.Update(schedule, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o agendamento." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteAppointments(List<ScheduleDTO> schedules)
        {
            _scheduleFacade.Delete(schedules, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao excluir o agendamento." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RescheduleAppointment(List<ScheduleDTO> schedules, string newDate)
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


        [HttpGet]
        public JsonResult GetTodaysAppointments()
        {

            

            //if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", errorMessage = "Houve um erro ao obter o agendamento." }, JsonRequestBehavior.AllowGet);
            //else
              //  return Json(new { Success = true, Data = schedules, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

    }
}