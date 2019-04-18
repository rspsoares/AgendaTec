using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.View.Authorization;
using AgendaTech.View.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AgendaTech.View.Controllers
{
    [Authorize]
    public class SchedulesController : Controller
    {
        private readonly IScheduleFacade _scheduleFacade;
        private AuthorizationHelper _claimHelper = new AuthorizationHelper();
        private UsuarioLogado _usuarioLogado = new UsuarioLogado();

        public SchedulesController(IScheduleFacade customerFacade)
        {
            _scheduleFacade = customerFacade;
            _usuarioLogado = _claimHelper.ObterUsuarioLogado();
        }

        [AuthorizeUser(AccessLevel = "/Administracao")]
        public ActionResult Index()
        {
            ViewBag.NomeUsuario = _usuarioLogado.Nome;
            return View();
        }

        [HttpGet]
        public JsonResult GetGrid(string idCustomer, string idProfessional, string idService, string idConsumer, string dateFrom, string dateTo, string bonus)
        {
            var professional = string.IsNullOrEmpty(idProfessional) ? 0 : int.Parse(idProfessional);
            var service = string.IsNullOrEmpty(idService) ? 0 : int.Parse(idService);
            var consumer = string.IsNullOrEmpty(idConsumer) ? Guid.Empty : Guid.Parse(idConsumer);
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
            string errorMessage = string.Empty;

            //Não permitir agendar pro mesmo profissional no mesmo dia/hora


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
        public JsonResult CheckAvailability(List<TSchedules> schedules, string newDate)
        {
            var availability = _scheduleFacade.CheckAvailability(schedules, newDate, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, Data = "", errorMessage = "Houve um erro ao verificar a disponibilidade." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, Data = availability, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RescheduleAppointment(List<TSchedules> schedules, string newDate)
        {         
            _scheduleFacade.Reschedule(schedules, newDate, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao excluir o agendamento." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}