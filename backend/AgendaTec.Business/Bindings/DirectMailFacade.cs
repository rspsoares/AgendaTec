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
    public class DirectMailFacade : IDirectMailFacade
    {
        private readonly ICommonRepository<TDirectMail> _commonRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public DirectMailFacade()
        {
            _commonRepository = new CommonRepository<TDirectMail>();
        }

        public List<DirectMailDTO> GetGrid(int idCustomer, string description, out string errorMessage)
        {
            var result = new List<DirectMailDTO>();
            var mailings = new List<TDirectMail>();

            errorMessage = string.Empty;

            try
            {
                mailings = _commonRepository.Filter(x => x.IDCustomer.Equals(idCustomer)).ToList();

                if (!string.IsNullOrEmpty(description))                    
                    mailings = _commonRepository.Filter(x => x.Description.Contains(description));

                result = Mapper.Map<List<TDirectMail>, List<DirectMailDTO>>(mailings);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return result;            
        }

        public DirectMailDTO GetDirectMailingById(int idDirectMail, out string errorMessage)
        {
            var mailing = new DirectMailDTO();

            errorMessage = string.Empty;

            try
            {
                var result = _commonRepository.GetById(idDirectMail);
                mailing = Mapper.Map<TDirectMail, DirectMailDTO>(result);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return mailing;
        }

        public DirectMailDTO Insert(DirectMailDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var result = Mapper.Map<DirectMailDTO, TDirectMail>(e);
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

        public void Update(DirectMailDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var result = Mapper.Map<DirectMailDTO, TDirectMail>(e);
                _commonRepository.Update(result.IDMail, result);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }

        public void Delete(int idDirectMail, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                _commonRepository.Delete(idDirectMail);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }

        public List<string> GetIntervalCombo(out string errorMessage)
        {
            var intervals = new List<string>();

            errorMessage = string.Empty;

            try
            {
                intervals = Enum
                    .GetValues(typeof(EnMailingIntervalType))
                    .Cast<EnMailingIntervalType>()
                    .Select(v => v.ToString())
                    .OrderBy(x => x)
                    .ToList();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return intervals;
        }

        public void ResendDirectMail(int idDirectMail, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var result = _commonRepository.GetById(idDirectMail);
                result.Resend = true;
                _commonRepository.Update(result.IDMail, result);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }
    }
}
