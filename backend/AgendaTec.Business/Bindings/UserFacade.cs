using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using AgendaTec.Infrastructure.Contracts;
using AgendaTec.Infrastructure.DatabaseModel;
using AgendaTec.Infrastructure.Repositories;
using AutoMapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace AgendaTec.Business.Bindings
{
    public class UserFacade : IUserFacade
    {
        private readonly ICommonRepository<AspNetUsers> _commonRepository;
        private ICommonRepository<AspNetRoles> _rolesRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public UserFacade()
        {
            _commonRepository = new CommonRepository<AspNetUsers>();
            _rolesRepository = new CommonRepository<AspNetRoles>();
        }

        public List<UserAccountDTO> GetGrid(string name, string email, int idCustomer, string idRole, out string errorMessage)
        {
            var result = new List<UserAccountDTO>();
            
            errorMessage = string.Empty;

            try
            {
                var users = _commonRepository
                    .Filter(x => x.TCGCustomersAspNetUsers.Any(y => y.IDCustomer.Equals(idCustomer)))
                    .ToList();                    
                    
                if (!string.IsNullOrEmpty(name))
                    users = users.Where(x => string.Concat(x.FirstName, " ", x.LastName).ToUpper().Contains(name.ToUpper())).ToList();

                if (!string.IsNullOrEmpty(email))
                    users = users.Where(x => x.Email.ToUpper().Contains(email.ToUpper())).ToList();

                if (!string.IsNullOrEmpty(idRole))
                    users = users.Where(x => x.IdRole.Equals(idRole)).ToList();

                result = Mapper.Map<List<AspNetUsers>, List<UserAccountDTO>>(users);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result
                .OrderBy(x => x.UserName)
                .ToList();
        }

        public UserAccountDTO GetUserById(string idUser, out string errorMessage)
        {
            var result = new UserAccountDTO();

            errorMessage = string.Empty;

            try
            {
                var user = _commonRepository.GetById(idUser);
                result = Mapper.Map<AspNetUsers, UserAccountDTO>(user);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result;
        }

        public UserAccountDTO GetUserByEmail(string email, out string errorMessage)
        {
            var result = new UserAccountDTO();

            errorMessage = string.Empty;

            try
            {
                var user = _commonRepository.Filter(x => x.Email.Equals(email)).FirstOrDefault();
                if (user == null)
                    return null;

                result = Mapper.Map<AspNetUsers, UserAccountDTO>(user);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result;
        }

        public List<UserAccountDTO> GetRolesCombo(EnUserType userGroup, out string errorMessage)
        {
            var result = new List<UserAccountDTO>();

            errorMessage = string.Empty;

            try
            {
                var roles = _rolesRepository.GetAll();

                if (!userGroup.Equals(EnUserType.Administrator))
                    roles = roles.Where(x => !int.Parse(x.Id).Equals((int)EnUserType.Administrator)).ToList();

                result = Mapper.Map<List<AspNetRoles>, List<UserAccountDTO>>(roles);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result
                 .OrderBy(x => x.IdRole)
                 .ToList();
        }

        public List<UserAccountDTO> GetUserNamesCombo(int idCustomer, out string errorMessage)
        {
            var result = new List<UserAccountDTO>();

            errorMessage = string.Empty;

            try
            {
                var users = _commonRepository
                    .Filter(x => x.TCGCustomersAspNetUsers.Any(y => y.IDCustomer.Equals(idCustomer)))
                    .ToList();

                result = Mapper.Map<List<AspNetUsers>, List<UserAccountDTO>>(users);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result
                 .OrderBy(x => x.RoleDescription)
                 .ToList();
        }

        public List<UserAccountDTO> GetProfessionalNamesCombo(int idCustomer, out string errorMessage)
        {
            var result = new List<UserAccountDTO>();

            errorMessage = string.Empty;

            try
            {
                var users = _commonRepository
                    .GetAll()
                    .Where(x => x.IdRole.Equals(((int)EnUserType.Professional).ToString()))// && x.IdCustomer.Equals(idCustomer))
                    .ToList();

                result = Mapper.Map<List<AspNetUsers>, List<UserAccountDTO>>(users);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result
                 .OrderBy(x => x.FullName)
                 .ToList();
        }

        public List<UserAccountDTO> GetConsumerNamesCombo(int idCustomer, out string errorMessage)
        {
            var result = new List<UserAccountDTO>();

            errorMessage = string.Empty;

            try
            {
                var users = _commonRepository
                    .GetAll()
                    .Where(x => x.IdRole.Equals(((int)EnUserType.Consumer).ToString())) //&& x.IdCustomer.Equals(idCustomer))
                    .ToList();

                result = Mapper.Map<List<AspNetUsers>, List<UserAccountDTO>>(users);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result
                 .OrderBy(x => x.RoleDescription)
                 .ToList();
        }

        public void Update(UserAccountDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var user = _commonRepository.Filter(x => x.Id.Equals(e.Id)).Single();

                user.FirstName = e.FirstName;
                user.LastName = e.LastName;
                user.CPF = e.CPF.CleanMask();
                user.BirthDate = DateTime.Parse(e.Birthday);
                user.IdRole = e.IdRole;                
                user.Email = e.Email;
                user.PhoneNumber = e.Phone.CleanMask();
                user.IsEnabled = e.IsEnabled;
                //user.RootUser = UserIsRoot(user.TCGCustomersAspNetUsers.TCGCustomers.RootCompany, int.Parse(e.IdRole));
                user.DirectMail = e.DirectMail;

                _commonRepository.Update(e.Id, user);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }
        }

        public void UpdateRequiredFields(UserAccountDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var user = _commonRepository.Filter(x => x.Id.Equals(e.Id)).Single();

                user.CPF = e.CPF.CleanMask();
                user.BirthDate = DateTime.Parse(e.Birthday);
                user.PhoneNumber = e.Phone.CleanMask();

                _commonRepository.Update(e.Id, user);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }
        }

        public string CheckDuplicatedUser(UserAccountDTO userAccount, out string errorMessage)
        {
            var users = new List<AspNetUsers>();
            var checkResult = string.Empty;
            errorMessage = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(userAccount.Id))
                    users = _commonRepository.Filter(x => x.Email.Equals(userAccount.Email));
                else
                    users = _commonRepository.Filter(x => !x.Id.Equals(userAccount.Id) && x.Email.Equals(userAccount.Email));

                checkResult = users.Any() ? "Esse e-mail já está em uso." : string.Empty;
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return checkResult;
        }     

        public List<UserAccountDTO> GetUserRecipients(int idCustomer, out string errorMessage)
        {
            var result = new List<UserAccountDTO>();

            errorMessage = string.Empty;

            try
            {
                var users = _commonRepository.GetAll()
                  //  .Filter(x => x.IdCustomer.Equals(idCustomer) && x.DirectMail)
                    .ToList();

                result = Mapper.Map<List<AspNetUsers>, List<UserAccountDTO>>(users);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result
                 .OrderBy(x => x.FullName)
                 .ToList();
        }

        public void UpdateAdminUsersByCustomer(int idCustomer, out string errorMessage)
        {
            var users = new List<AspNetUsers>();

            errorMessage = string.Empty;

            try
            {
                users = _commonRepository.GetAll()
                 //   .Filter(x => x.IdCustomer.Equals(idCustomer))
                    .Where(x => int.Parse(x.IdRole).Equals((int)EnUserType.Administrator))
                    .ToList();

                users.ForEach(user =>
                {
             //       user.RootUser = UserIsRoot(user.TCGCustomers.RootCompany, int.Parse(user.IdRole));
                    _commonRepository.Update(user.Id, user);
                });
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }
        }

        public bool GetUserIsRoot(int idCustomer, string idUser)
        {
            ICustomerFacade customerFacade = new CustomerFacade();
            var root = false;

            string errorMessage;

            try
            {
                var customer = customerFacade.GetCustomerById(idCustomer, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))                
                    return false;

                var user = GetUserById(idUser, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                    return false;

                root = UserIsRoot(customer.Root, int.Parse(user.IdRole));
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return root;
        }

        public bool GetUserIsRoot(int idCustomer, int idRole)
        {
            ICustomerFacade customerFacade = new CustomerFacade();
            var root = false;

            string errorMessage;

            try
            {
                var customer = customerFacade.GetCustomerById(idCustomer, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                    return false;

                root = UserIsRoot(customer.Root, idRole);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return root;
        }

        private bool UserIsRoot(bool customerRoot, int idRole)
        {
            ICustomerFacade customerFacade = new CustomerFacade();
            var root = false;

            string errorMessage;

            try
            {                
                root = idRole.Equals((int)EnUserType.Administrator) && customerRoot;
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return root;
        }

        public void SendResetPasswordEmail(string userEmail, string userFullName, string subject, string body)
        {
            var mailHelper = new SendMailHelper();

            try
            {
                var mailMessage = new MailMessage()
                {
                    From = new MailAddress("nao.responder@agendatec.com", "Recuperação de Senha", Encoding.UTF8),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                    Priority = MailPriority.High,
                    SubjectEncoding = Encoding.GetEncoding("ISO-8859-1"),
                    BodyEncoding = Encoding.GetEncoding("ISO-8859-1")
                };

                mailMessage.To.Add(new MailAddress(userEmail, userFullName, Encoding.UTF8));

                mailHelper.SendMail(mailMessage);                
            }
            catch (Exception ex)
            {
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");                
            }            
        }
    }
}
