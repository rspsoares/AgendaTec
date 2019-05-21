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
    public class DirectMailingFacade : IDirectMailingFacade
    {
        private readonly ICommonRepository<TDirectMailing> _commonRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public DirectMailingFacade()
        {
            _commonRepository = new CommonRepository<TDirectMailing>();
        }

        public List<DirectMailingDTO> GetGrid(int idCustomer, string description, out string errorMessage)
        {
            var result = new List<DirectMailingDTO>();
            var mailings = new List<TDirectMailing>();

            errorMessage = string.Empty;

            try
            {
                mailings = _commonRepository.Filter(x => x.IDCustomer.Equals(idCustomer)).ToList();

                if (!string.IsNullOrEmpty(description))                    
                    mailings = _commonRepository.Filter(x => x.Description.Contains(description));

                result = Mapper.Map<List<TDirectMailing>, List<DirectMailingDTO>>(mailings);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return result;            
        }

        public DirectMailingDTO GetDirectMailingById(int idMailing, out string errorMessage)
        {
            var mailing = new DirectMailingDTO();

            errorMessage = string.Empty;

            try
            {
                var result = _commonRepository.GetById(idMailing);
                mailing = Mapper.Map<TDirectMailing, DirectMailingDTO>(result);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return mailing;
        }

        public DirectMailingDTO Insert(DirectMailingDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var result = Mapper.Map<DirectMailingDTO, TDirectMailing>(e);
                result = _commonRepository.Insert(result);
                e.Id = result.IDMail;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return e;
        }

        public void Update(DirectMailingDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var result = Mapper.Map<DirectMailingDTO, TDirectMailing>(e);
                _commonRepository.Update(result.IDMail, result);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }

        public void Delete(int idDirectMailing, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                _commonRepository.Delete(idDirectMailing);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }
    }
}
