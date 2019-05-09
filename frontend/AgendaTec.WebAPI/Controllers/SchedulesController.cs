using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace AgendaTech.WebAPI.Controllers
{
    [Authorize]
    public class SchedulesController : ApiController
    {
        private readonly IScheduleFacade _scheduleFacade;
        private readonly IUserFacade _userFacade;

        public SchedulesController()
        {
            _scheduleFacade = new ScheduleFacade();
            _userFacade = new UserFacade();
        }

        [HttpPost]
        public IHttpActionResult Create(List<TSchedules> schedules)
        {           
            try
            {
                var availabilityCheck = _scheduleFacade.CheckAvailability(schedules, out string errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                    return StatusCode(HttpStatusCode.InternalServerError);

                if (!string.IsNullOrEmpty(availabilityCheck))                
                    return Ok(availabilityCheck);                

                schedules.ForEach(schedule =>
                {
                    if (schedule.IDSchedule.Equals(0))
                        _scheduleFacade.Insert(schedule, out errorMessage);
                    else
                        _scheduleFacade.Update(schedule, out errorMessage);
                });

                if (!string.IsNullOrEmpty(errorMessage))
                    return StatusCode(HttpStatusCode.InternalServerError);

                return StatusCode(HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public IHttpActionResult Get(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email.Trim()))
                    return StatusCode(HttpStatusCode.InternalServerError);

                var loggedUser = _userFacade.GetLoggedUserByEmail(email, out string errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                    return StatusCode(HttpStatusCode.InternalServerError);

                var schedules = _scheduleFacade.GetGrid(loggedUser.IDCustomer, 0, 0, loggedUser.Id, DateTime.Now, null, null, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                    return StatusCode(HttpStatusCode.InternalServerError);

                return Ok(schedules);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(int idSchedule)
        {            
            try
            {
                _scheduleFacade.Delete(idSchedule, out string errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                    return StatusCode(HttpStatusCode.InternalServerError);
                else
                    return StatusCode(HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
    }
}