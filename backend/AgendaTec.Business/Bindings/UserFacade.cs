using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using AgendaTec.Infrastructure.Contracts;
using AgendaTec.Infrastructure.DatabaseModel;
using AgendaTec.Infrastructure.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            var users = new List<AspNetUsers>();
            
            errorMessage = string.Empty;

            try
            {
                users = _commonRepository.GetAll();

                if (!string.IsNullOrEmpty(name))
                    users = users.Where(x => string.Concat(x.FirstName, " ", x.LastName).ToUpper().Contains(name.ToUpper())).ToList();

                if (!string.IsNullOrEmpty(email))
                    users = users.Where(x => x.Email.ToUpper().Contains(email.ToUpper())).ToList();

                if (idCustomer > 0)
                    users = users.Where(x => x.IdCustomer.Equals(idCustomer)).ToList();

                if (!string.IsNullOrEmpty(idRole))
                    users = users.Where(x => x.IdRole.Equals(idRole)).ToList();
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return users
                .Select(x => new UserAccountDTO()
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    FullName = $"{x.FirstName} {x.LastName}",
                    CPF = x.CPF,
                    Email = x.Email,
                    Phone = x.PhoneNumber,
                    IdRole = x.AspNetRoles.Id,
                    RoleDescription = x.AspNetRoles.Name,
                    IDCustomer = x.TCGCustomers?.IDCustomer ?? 0,
                    CustomerName = x.TCGCustomers?.CompanyName ?? string.Empty,
                    IsEnabled = x.IsEnabled
                })
                .OrderBy(x => x.UserName)
                .ToList();
        }

        public UserAccountDTO GetUserById(string idUser, out string errorMessage)
        {
            var user = new UserAccountDTO();

            errorMessage = string.Empty;

            try
            {
                var result = _commonRepository.GetById(idUser);

                user = new UserAccountDTO()
                {
                    Id = result.Id,
                    UserName = result.UserName,
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                    FullName = $"{result.FirstName} {result.LastName}",
                    CPF = result.CPF,
                    Email = result.Email,
                    Phone = result.PhoneNumber,
                    IdRole = result.AspNetRoles.Id,
                    RoleDescription = result.AspNetRoles.Name,
                    IDCustomer = result.TCGCustomers?.IDCustomer ?? 0,
                    CustomerName = result.TCGCustomers?.CompanyName ?? string.Empty,
                    IsEnabled = result.IsEnabled
                };
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return user;
        }

        public UserAccountDTO GetLoggedUserByEmail(string email, out string errorMessage)
        {
            var user = new AspNetUsers();

            errorMessage = string.Empty;

            try
            {
                user = _commonRepository.Filter(x => x.Email.Equals(email)).FirstOrDefault();
                if (user == null)
                {
                    errorMessage = "E-mail não encontrado";
                    return null;
                }           
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return new UserAccountDTO()
            {              
                Id = user.Id,
                IDCustomer = user.IdCustomer,
                FullName = $"{user.FirstName} {user.LastName}"
            };
        }

        public List<UserAccountDTO> GetRolesCombo(EnUserType userGroup, out string errorMessage)
        {
            var roles = new List<AspNetRoles>();
         
            errorMessage = string.Empty;

            try
            {
                roles = _rolesRepository.GetAll();

                if (!userGroup.Equals(EnUserType.Administrator))
                    roles = roles.Where(x => !int.Parse(x.Id).Equals((int)EnUserType.Administrator)).ToList();
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return roles
                 .Select(x => new UserAccountDTO()
                 {                    
                     IdRole = x.Id,
                     RoleDescription = x.Name,
                 })
                 .OrderBy(x => x.IdRole)
                 .ToList();
        }

        public List<UserAccountDTO> GetUserNamesCombo(int idCustomer, out string errorMessage)
        {
            var users = new List<AspNetUsers>();

            errorMessage = string.Empty;

            try
            {
                users = _commonRepository.Filter(x => x.IdCustomer.Equals(idCustomer));                
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return users
                 .Select(x => new UserAccountDTO()
                 {
                     Id = x.Id,
                     UserName = $"{x.FirstName} {x.LastName}"
                 })
                 .OrderBy(x => x.RoleDescription)
                 .ToList();
        }

        public List<UserAccountDTO> GetProfessionalNamesCombo(int idCustomer, out string errorMessage)
        {
            var users = new List<AspNetUsers>();

            errorMessage = string.Empty;

            try
            {
                users = _commonRepository
                    .GetAll()
                    .Where(x => x.IdRole.Equals(((int)EnUserType.Professional).ToString()) && x.IdCustomer.Equals(idCustomer))
                    .ToList();
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return users
                 .Select(x => new UserAccountDTO()
                 {
                     Id = x.Id,
                     FullName = $"{x.FirstName} {x.LastName}"
                 })
                 .OrderBy(x => x.FullName)
                 .ToList();
        }

        public List<UserAccountDTO> GetConsumerNamesCombo(int idCustomer, out string errorMessage)
        {
            var users = new List<AspNetUsers>();

            errorMessage = string.Empty;

            try
            {
                users = _commonRepository
                    .GetAll()
                    .Where(x => x.IdRole.Equals(((int)EnUserType.Consumer).ToString()) && x.IdCustomer.Equals(idCustomer))
                    .ToList();                
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return users
                 .Select(x => new UserAccountDTO()
                 {
                     Id = x.Id,
                     FullName = $"{x.FirstName} {x.LastName}"
                 })
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
                user.IdRole = e.IdRole;
                user.IdCustomer = e.IDCustomer;
                user.Email = e.Email;
                user.PhoneNumber = e.Phone.CleanMask();
                user.IsEnabled = e.IsEnabled;

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
            var users = new List<AspNetUsers>();

            errorMessage = string.Empty;

            try
            {
                users = _commonRepository
                    .Filter(x => x.IdCustomer.Equals(idCustomer))
                    .ToList();              
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return users
                .Select(x => new UserAccountDTO()
                {
                    Id = x.Id,
                    IDCustomer = x.IdCustomer,
                    IdRole = x.IdRole,
                    FullName = $"{x.FirstName} {x.LastName}",
                    Email = x.Email,
                    Phone = x.PhoneNumber                    
                 })
                 .OrderBy(x => x.FullName)
                 .ToList();
        }
    }
}
