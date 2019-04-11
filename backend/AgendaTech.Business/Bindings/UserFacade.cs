using AgendaTech.Business.Contracts;
using AgendaTech.Business.Entities;
using AgendaTech.Infrastructure.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.Infrastructure.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AgendaTech.Business.Bindings
{
    public class UserFacade : IUserFacade
    {
        private readonly ICommonRepository<UserAccounts> _commonRepository;
        private ICommonRepository<TCGUserGroup> _userGroupRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public UserFacade()
        {
            _commonRepository = new CommonRepository<UserAccounts>();
            _userGroupRepository = new CommonRepository<TCGUserGroup>();
        }

        public List<UserAccountDTO> GetGrid(string name, string login, int idCustomer, int idUserGroup, out string errorMessage)
        {
            var users = new List<UserAccounts>();
            
            errorMessage = string.Empty;

            try
            {
                users = _commonRepository.GetAll();

                if(!string.IsNullOrEmpty(name))                
                    users = users.Where(x => string.Concat(x.FirstName, " ", x.LastName).Contains(name)).ToList();

                if (!string.IsNullOrEmpty(login))
                    users = users.Where(x => x.Username.Contains(login)).ToList();

                if (idCustomer > 0)
                    users = users.Where(x => x.Source.Equals(idCustomer)).ToList();
                                   
                if (idUserGroup > 0)
                    users = users.Where(x => x.Inscription.Equals(idUserGroup)).ToList();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return users
                .Select(x => new UserAccountDTO()
                {
                    IDUser = x.Key,
                    UserName = x.Username,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    FullName = $"{x.FirstName} {x.LastName}",
                    Email = x.Email,
                    IDUserGroup = x.TCGUserGroup.IDUserGroup,
                    GroupDescription = x.TCGUserGroup.Descripton,
                    IDCustomer = x.TCGCustomers?.IDCustomer ?? 0,
                    CustomerName = x.TCGCustomers?.CompanyName ?? string.Empty,
                    Active = x.IsLoginAllowed                    
                })
                .OrderBy(x => x.UserName)
                .ToList();
        }

        public UserAccountDTO GetUserById(int idUser, out string errorMessage)
        {
            var user = new UserAccountDTO();

            errorMessage = string.Empty;

            try
            {  
                var result = _commonRepository.GetById(idUser);
                
                user = new UserAccountDTO()
                {
                    IDUser = result.Key,
                    UserName = result.Username,
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                    FullName = $"{result.FirstName} {result.LastName}",
                    Email = result.Email,
                    IDUserGroup = result.Inscription,
                    GroupDescription = result.TCGUserGroup.Descripton,
                    IDCustomer = result.TCGCustomers?.IDCustomer ?? 0,
                    CustomerName = result.TCGCustomers?.CompanyName ?? string.Empty,
                    Active = result.IsLoginAllowed
                };
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return user;
        }

        public UserAccountDTO GetUserByUq(Guid uq, out string errorMessage)
        {
            var user = new UserAccounts();
            errorMessage = string.Empty;

            try
            {
                 user = _commonRepository
                    .GetAll()
                    .Where(x => x.ID.Equals(uq))
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return new UserAccountDTO()
            {
                IDUser = user.Key,
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                IDUserGroup = user.Inscription,
                GroupDescription = user.TCGUserGroup.Descripton,
                IDCustomer = user.TCGCustomers?.IDCustomer ?? 0,
                CustomerName = user.TCGCustomers?.CompanyName ?? string.Empty,
                Active = user.IsLoginAllowed
            };
        }

        public List<UserAccountDTO> GetUserGroups(EnUserType userGroup, out string errorMessage)
        {
            var userGroups = new List<TCGUserGroup>();
         
            errorMessage = string.Empty;

            try
            {
                userGroups = _userGroupRepository.GetAll();

                if (!userGroup.Equals(EnUserType.Administrator))
                    userGroups = userGroups.Where(x => !x.IDUserGroup.Equals((int)EnUserType.Administrator)).ToList();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return userGroups
                 .Select(x => new UserAccountDTO()
                 {                    
                     IDUserGroup = x.IDUserGroup,
                     GroupDescription = x.Descripton,                    
                 })
                 .OrderBy(x => x.GroupDescription)
                 .ToList();
        }
        
        public void Update(UserAccountDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var user = _commonRepository.GetById(e.IDUser);

                Enum.TryParse(user.Inscription.ToString(), out EnUserType userType);

                user.FirstName = e.FirstName;
                user.LastName = e.LastName;
                user.Inscription = e.IDUserGroup;
                user.Source = e.IDCustomer;
                user.Email = e.Email;
                user.IsLoginAllowed = userType.Equals(EnUserType.Consumer) ? false : e.Active;
                
                _commonRepository.Update(user);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }
    }
}
