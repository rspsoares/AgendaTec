using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Business.Entities;
using System;
using System.Net;
using System.Web.Http;

namespace AgendaTech.WebAPI.Controllers
{
    [Authorize]    
    public class ConsumersController : ApiController
    {
        private readonly IUserFacade _userFacade;        

        public ConsumersController()
        {
            _userFacade = new UserFacade();            
        }        

        [HttpPost]        
        public IHttpActionResult Create(UserAccountDTO userDTO)
        {
            try
            {
                var checkResult = _userFacade.CheckDuplicatedUser(userDTO, out string errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                    return StatusCode(HttpStatusCode.InternalServerError);

                if (!string.IsNullOrEmpty(checkResult))
                    return Ok(checkResult);

                _userFacade.CreateConsumer(userDTO, out errorMessage);

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
        public IHttpActionResult GetLogged(string email)
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