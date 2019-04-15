using AgendaTech.Business.Contracts;
using AgendaTech.Business.Helpers;
using AgendaTech.Infrastructure.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.Infrastructure.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

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
                    professionals = professionals.Where(x => x.Name.Contains(professionalName)).ToList();
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
                    Phone = x.Phone,
                    Email = x.Email
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
                professionals = _commonRepository.GetAll();

                if (idCustomer > 0)
                    professionals = professionals.Where(x => x.IDCustomer.Equals(idCustomer)).ToList();                

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
                    Phone = x.Phone,
                    Email = x.Email
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
                    Phone = result.Phone,
                    Email = result.Email
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
                _commonRepository.Update(e);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }
    }
}
