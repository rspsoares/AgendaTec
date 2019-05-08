using AgendaTech.Business.Contracts;
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
    public class ProfessionalFacade : IProfessionalFacade
    {
        private readonly ICommonRepository<TCGProfessionals> _commonRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public ProfessionalFacade()
        {
            _commonRepository = new CommonRepository<TCGProfessionals>();
        }

        public List<TCGProfessionals> GetGrid(int idCustomer, string professionalName, out string errorMessage)
        {
            var professionals = new List<TCGProfessionals>();
            
            errorMessage = string.Empty;

            try
            {
                professionals = _commonRepository.GetAll();

                if (idCustomer > 0)
                    professionals = professionals.Where(x => x.IDCustomer.Equals(idCustomer)).ToList();

                if (!string.IsNullOrEmpty(professionalName))
                    professionals = professionals.Where(x => x.Name.ToUpper().Contains(professionalName.ToUpper())).ToList();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return professionals
                .Select(x => new TCGProfessionals()
                {
                    IDProfessional = x.IDProfessional,
                    IDCustomer = x.IDCustomer,
                    IDUser = x.IDUser,
                    Name = x.Name,
                    Birthday = x.Birthday,
                    Phone = x.Phone
                })
                .OrderBy(x => x.IDCustomer)
                .ThenBy(x => x.Name)
                .ToList();
        }

        public List<TCGProfessionals> GetProfessionalNameCombo(int idCustomer, Guid idUser, out string errorMessage)
        {
            var professionals = new List<TCGProfessionals>();

            errorMessage = string.Empty;

            try
            {
                professionals = _commonRepository.Filter(x => x.IDCustomer.Equals(idCustomer));

                if(!idUser.Equals(Guid.Empty))
                    professionals = professionals.Where(x => x.IDUser.Equals(idUser)).ToList();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return professionals
                .Select(x => new TCGProfessionals()
                {
                    IDProfessional = x.IDProfessional,
                    IDCustomer = x.IDCustomer,
                    Name = x.Name,
                    Birthday = x.Birthday,
                    Phone = x.Phone
                })
                .OrderBy(x => x.Name)                
                .ToList();
        }

        public List<TCGProfessionals> GetProfessionalNameComboClient(int idCustomer, bool authenticated, out string errorMessage)
        {
            var professionals = new List<TCGProfessionals>();

            errorMessage = string.Empty;

            try
            {
                if (authenticated)
                    professionals = _commonRepository.Filter(x => x.IDCustomer.Equals(idCustomer));
                else
                {
                    professionals.Add(new TCGProfessionals()
                    {
                        IDProfessional = 0,
                        Name = "Para visualizar as opções de profissionais, favor efetuar o Login."
                    });
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return professionals
                .Select(x => new TCGProfessionals()
                {
                    IDProfessional = x.IDProfessional,
                    IDCustomer = x.IDCustomer,
                    Name = x.Name,
                    Birthday = x.Birthday,
                    Phone = x.Phone
                })
                .OrderBy(x => x.Name)
                .ToList();
        }

        public TCGProfessionals GetProfessionalById(int idProfessional, out string errorMessage)
        {
            var professional = new TCGProfessionals();

            errorMessage = string.Empty;

            try
            {
                var result = _commonRepository.GetById(idProfessional);

                professional = new TCGProfessionals()
                {
                    IDProfessional = result.IDProfessional,
                    IDCustomer = result.IDCustomer,
                    IDUser = result.IDUser,
                    Name = result.Name,
                    Birthday = result.Birthday,
                    Phone = result.Phone
                };
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return professional;
        }

        public TCGProfessionals Insert(TCGProfessionals e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                e = _commonRepository.Insert(e);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return e;
        }

        public void Update(TCGProfessionals e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                _commonRepository.Update(e.IDProfessional, e);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }

        public bool CheckUserInUse(int idProfessional, string idUser, out string errorMessage)
        {
            bool userInUse = true;

            errorMessage = string.Empty;

            try
            {
                userInUse = _commonRepository
                    .GetAll()
                    .Where(x => !x.IDProfessional.Equals(idProfessional) && x.IDUser.Equals(idUser))
                    .Any();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return userInUse;
        }
    }
}
