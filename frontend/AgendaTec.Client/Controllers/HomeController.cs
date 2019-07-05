using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using AgendaTec.Client.Helper;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AgendaTec.Client.Controllers
{    
    public class HomeController : Controller
    {        
        private readonly IProfessionalServiceFacade _professionalServiceFacade;
        private readonly IScheduleFacade _scheduleFacade;
        private readonly ICustomerFacade _customerFacade;
        private readonly IUserFacade _userFacade;

        public HomeController(IProfessionalServiceFacade professionalServiceFacade, IScheduleFacade scheduleFacade, ICustomerFacade customerFacade, IUserFacade userFacade)
        {     
            _professionalServiceFacade = professionalServiceFacade;
            _scheduleFacade = scheduleFacade;
            _customerFacade = customerFacade;
            _userFacade = userFacade;
        }

        public ActionResult Index(string customerKey)
        {
            if (!string.IsNullOrEmpty(customerKey))
            {
                var customer = _customerFacade.GetCustomerByKey(customerKey, out string errorMessage);
                Session["IdCustomer"] = customer.Id.Equals(0) ? (int?)null : customer.Id;
            }
            else if(Session["IdCustomer"] == null)
                Session["IdCustomer"] = "0";

            return View();
        }

        [HttpGet]
        public JsonResult GetServices()
        {
            var customer = Session["IdCustomer"] == null ? 0 : int.Parse(Session["IdCustomer"].ToString());
            var services = _professionalServiceFacade.GetServicesComboClient(customer, User.Identity.IsAuthenticated, out string errorMessage);

            return Json(services, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProfessionals(string idService)
        {
            var customer = Session["IdCustomer"] == null ? 0 : int.Parse(Session["IdCustomer"].ToString());
            var service = string.IsNullOrEmpty(idService) ? 0 : int.Parse(idService);
            var professionals = _professionalServiceFacade.GetProfessionalNameComboClient(customer, service, User.Identity.IsAuthenticated, out string errorMessage);

            return Json(professionals, JsonRequestBehavior.AllowGet);            
        }

        [HttpGet]
        public JsonResult GetAvailableHours(string idProfessional, string idService, string selectedDate)
        {            
            var idCustomer = Session["IdCustomer"] == null ? 0 : int.Parse(Session["IdCustomer"].ToString());
            var professional = string.IsNullOrEmpty(idProfessional) ? 0 : int.Parse(idProfessional);
            var service = string.IsNullOrEmpty(idService) ? 0 : int.Parse(idService);
            var date = string.IsNullOrEmpty(selectedDate) ? DateTime.MinValue : DateTime.Parse(selectedDate);
            
            var availables = _scheduleFacade.GetAvailableHours(idCustomer, professional, service, date, User.GetIdUser(), User.Identity.IsAuthenticated, out string errorMessage);            

            if(string.IsNullOrEmpty(errorMessage))
                return Json(availables, JsonRequestBehavior.AllowGet);
            else
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAppointment(ScheduleDTO schedule)
        {
            if(!User.Identity.IsAuthenticated)
                return Json(new { Success = false, errorMessage = "É necessário estar logado para realizar um agendamento." }, JsonRequestBehavior.AllowGet);
            
            schedule.IdCustomer = int.Parse(User.GetIdCustomer());
            schedule.IdConsumer = User.GetIdUser();

            var schedules = new List<ScheduleDTO>
            {
                schedule
            };

            var checkRequiredFields = _scheduleFacade.CheckRequiredFields(schedule.IdConsumer, out string errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao verificar os campos cadastrais." }, JsonRequestBehavior.AllowGet);

            if(!checkRequiredFields)
                return Json(new { Success = false, errorMessage = "Required fields" }, JsonRequestBehavior.AllowGet);

            var availabilityCheck = _scheduleFacade.CheckAvailability(schedules, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao verificar a disponibilidade da agenda." }, JsonRequestBehavior.AllowGet);

            if (!string.IsNullOrEmpty(availabilityCheck))
                return Json(new { Success = false, errorMessage = availabilityCheck }, JsonRequestBehavior.AllowGet);

            _scheduleFacade.Insert(schedule, out errorMessage);            

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o agendamento." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUserRequiredFields()
        {
            var consumer = _userFacade.GetUserById(User.GetIdUser(), out string errorMessage);

            if (string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = true, Data = consumer, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);            
            else
                return Json(new { Success = false, Data = consumer, errorMessage = "Houve um erro ao obter os campos obrigatórios." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveUserRequiredFields(UserAccountDTO consumer)
        {
            if (!consumer.CPF.IsCPF())
                return Json(new { Success = false, errorMessage = "CPF inválido." }, JsonRequestBehavior.AllowGet);

            consumer.Id = User.GetIdUser();

            _userFacade.UpdateRequiredFields(consumer, out string errorMessage);
            
            if (string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = false,errorMessage = "Houve um erro ao obter os campos obrigatórios." }, JsonRequestBehavior.AllowGet);
        }
    }
}