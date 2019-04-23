using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Business.Entities;
using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace AgendaTech.WebAPI.Controllers
{
    [Authorize]    
    public class ConsumerController : ApiController
    {
        private readonly IUserFacade _userFacade;        

        public ConsumerController()
        {
            _userFacade = new UserFacade();            
        }        

        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult CreateConsumer(UserAccountDTO userDTO)
        {
            try
            {
                _userFacade.CreateConsumer(userDTO, out string errorMessage);

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
        [ResponseType(typeof(string))]
        public IHttpActionResult GetLoggedConsumer(string email)
        {   
            try
            {
                if(string.IsNullOrEmpty(email.Trim()))
                    return StatusCode(HttpStatusCode.InternalServerError);

                var loggedUser = _userFacade.GetLoggedUserByEmail(email, out string errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                    return StatusCode(HttpStatusCode.InternalServerError);

                return Ok(loggedUser);
                
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }            
        }

        [HttpPut]
        [ResponseType(typeof(string))]
        public IHttpActionResult ChangePassword(UserAccountDTO userDTO)
        {
            try
            {
                _userFacade.ChangePassword(userDTO, out string errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                    return StatusCode(HttpStatusCode.InternalServerError);

                return StatusCode(HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
    }
}