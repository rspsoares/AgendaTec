using AgendaTech.Business.Contracts;
using AgendaTech.Client.Helper;
using AgendaTech.Infrastructure.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AgendaTech.Client.Controllers
{    
    public class HomeController : Controller
    {
        private readonly IServiceFacade _serviceFacade;
        private readonly IProfessionalFacade _professionalFacade;
        private readonly IScheduleFacade _scheduleFacade;
        private readonly ICustomerFacade _customerFacade;

        public HomeController(IServiceFacade serviceFacade, IProfessionalFacade professionalFacade, IScheduleFacade scheduleFacade, ICustomerFacade customerFacade)
        {
            _serviceFacade = serviceFacade;
            _professionalFacade = professionalFacade;
            _scheduleFacade = scheduleFacade;
            _customerFacade = customerFacade;
        }

        public ActionResult Index(string customerKey)
        {
            if(!string.IsNullOrEmpty(customerKey))
            {
                var customer = _customerFacade.GetCustomerByKey(customerKey, out string errorMessage);
                Session["IdCustomer"] = customer.IDCustomer.Equals(0) ? (int?)null : customer.IDCustomer;
            }
            
            return View();
        }

        [HttpGet]
        public JsonResult GetServices()
        {
            var idCustomer = string.IsNullOrEmpty(User.GetIdCustomer()) ? 0 : int.Parse(User.GetIdCustomer());
            var services = _serviceFacade.GetServiceNameComboClient(idCustomer, User.Identity.IsAuthenticated, out string errorMessage);

            return Json(services, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProfessionals()
        {
            var idCustomer = string.IsNullOrEmpty(User.GetIdCustomer()) ? 0 : int.Parse(User.GetIdCustomer());
            var professionals = _professionalFacade.GetProfessionalNameComboClient(idCustomer, User.Identity.IsAuthenticated, out string errorMessage);

            return Json(professionals, JsonRequestBehavior.AllowGet);            
        }

        [HttpGet]
        public JsonResult GetAvailableHours(string idProfessional, string idService, string selectedDate)
        {            
            var idCustomer = string.IsNullOrEmpty(User.GetIdCustomer()) ? 0 : int.Parse(User.GetIdCustomer());
            var professional = string.IsNullOrEmpty(idProfessional) ? 0 : int.Parse(idProfessional);
            var service = string.IsNullOrEmpty(idService) ? 0 : int.Parse(idService);
            var date = DateTime.Parse(selectedDate);
            
            var availables = _scheduleFacade.GetAvailableHours(idCustomer, professional, service, date, User.GetIdUser(), User.Identity.IsAuthenticated, out string errorMessage);            

            if(string.IsNullOrEmpty(errorMessage))
                return Json(availables, JsonRequestBehavior.AllowGet);
            else
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAppointment(TSchedules schedule)
        {
            if(!User.Identity.IsAuthenticated)
                return Json(new { Success = false, errorMessage = "É necessário estar logado para realizar un agendamento." }, JsonRequestBehavior.AllowGet);
            
            schedule.IDCustomer = int.Parse(User.GetIdCustomer());
            schedule.IDConsumer = User.GetIdUser();

            var schedules = new List<TSchedules>
            {
                schedule
            };

            var availabilityCheck = _scheduleFacade.CheckAvailability(schedules, out string errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao verificar a disponibilidade da agenda." }, JsonRequestBehavior.AllowGet);

            if (!string.IsNullOrEmpty(availabilityCheck))
                return Json(new { Success = false, errorMessage = availabilityCheck }, JsonRequestBehavior.AllowGet);

            _scheduleFacade.Insert(schedule, out errorMessage);            

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o cliente." }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = true, errorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}