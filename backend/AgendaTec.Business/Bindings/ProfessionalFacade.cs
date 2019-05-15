using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Infrastructure.Contracts;
using AgendaTec.Infrastructure.DatabaseModel;
using AgendaTec.Infrastructure.Repositories;
using AutoMapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AgendaTec.Business.Bindings
{
    public class ProfessionalFacade : IProfessionalFacade
    {
        private readonly ICommonRepository<TCGProfessionals> _commonRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public ProfessionalFacade()
        {
            _commonRepository = new CommonRepository<TCGProfessionals>();
        }

        public List<ProfessionalDTO> GetGrid(int idCustomer, string professionalName, out string errorMessage)
        {
            var result = new List<ProfessionalDTO>();
            var professionals = new List<TCGProfessionals>();
            
            errorMessage = string.Empty;

            try
            {
                professionals = _commonRepository.GetAll();

                if (idCustomer > 0)
                    professionals = professionals.Where(x => x.IDCustomer.Equals(idCustomer)).ToList();

                if (!string.IsNullOrEmpty(professionalName))
                    professionals = professionals.Where(x => x.Name.ToUpper().Contains(professionalName.ToUpper())).ToList();

                result = Mapper.Map<List<TCGProfessionals>, List<ProfessionalDTO>>(professionals);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return result
                .OrderBy(x => x.IdCustomer)
                .ThenBy(x => x.Name)
                .ToList();
        }

        public List<ProfessionalDTO> GetProfessionalNameCombo(int idCustomer, Guid idUser, out string errorMessage)
        {
            var result = new List<ProfessionalDTO>();
            var professionals = new List<TCGProfessionals>();

            errorMessage = string.Empty;

            try
            {
                professionals = _commonRepository.Filter(x => x.IDCustomer.Equals(idCustomer));

                if(!idUser.Equals(Guid.Empty))
                    professionals = professionals.Where(x => x.IDUser.Equals(idUser)).ToList();

                result = Mapper.Map<List<TCGProfessionals>, List<ProfessionalDTO>>(professionals);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return result
                .OrderBy(x => x.Name)                
                .ToList();
        }

        public List<ProfessionalDTO> GetProfessionalNameComboClient(int idCustomer, bool authenticated, out string errorMessage)
        {
            var result = new List<ProfessionalDTO>();
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

                result = Mapper.Map<List<TCGProfessionals>, List<ProfessionalDTO>>(professionals);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return result
                .OrderBy(x => x.Name)
                .ToList();
        }

        public ProfessionalDTO GetProfessionalById(int idProfessional, out string errorMessage)
        {
            var result = new ProfessionalDTO();
            var professional = new TCGProfessionals();

            errorMessage = string.Empty;

            try
            {
                professional = _commonRepository.GetById(idProfessional);
                result = Mapper.Map<TCGProfessionals, ProfessionalDTO>(professional);
              
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return result;
        }

        public ProfessionalDTO GetProfessionalByUserId(string idUser, out string errorMessage)
        {
            var result = new ProfessionalDTO();
            var professional = new TCGProfessionals();

            errorMessage = string.Empty;

            try
            {
                professional = _commonRepository.Filter(x => x.IDUser.Equals(idUser)).SingleOrDefault();
                result = Mapper.Map<TCGProfessionals, ProfessionalDTO>(professional);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return result;
        }


        public ProfessionalDTO Insert(ProfessionalDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var result = Mapper.Map<ProfessionalDTO, TCGProfessionals>(e);
                result = _commonRepository.Insert(result);
                e.Id = result.IDProfessional;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return e;
        }

        public void Update(ProfessionalDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var result = Mapper.Map<ProfessionalDTO, TCGProfessionals>(e);
                _commonRepository.Update(result.IDProfessional, result);
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
