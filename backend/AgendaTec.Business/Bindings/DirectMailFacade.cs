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

        public List<DirectMailDTO> GetGrid(int idCustomer, int mailType, string description, out string errorMessage)
        {
            var results = new List<DirectMailDTO>();
            var mailings = new List<TDirectMail>();

            errorMessage = string.Empty;

            try
            {
                mailings = _commonRepository.Filter(x => x.IDCustomer.Equals(idCustomer)).ToList();

                var enMailType = (EnMailType)mailType;
                if(!enMailType.Equals(EnMailType.All))
                    mailings = mailings.Where(x => x.MailType.Equals(mailType)).ToList();

                if (!string.IsNullOrEmpty(description))                    
                    mailings = mailings.Where(x => x.Description.Contains(description)).ToList();

                results = Mapper.Map<List<TDirectMail>, List<DirectMailDTO>>(mailings);

                results.ForEach(result =>
                {
                    var type = (EnMailType)result.MailType;
                    result.MailTypeDescription = StringExtensions.GetEnumDescription(type);
                });
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return results
                .OrderBy(x => x.MailTypeDescription)
                .ThenBy(x => x.Description)
                .ToList();            
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
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
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
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
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
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
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
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }
        }

        public List<string> GetIntervalCombo(out string errorMessage)
        {
            var intervals = new List<string>();

            errorMessage = string.Empty;

            try
            {
                intervals = Enum
                    .GetValues(typeof(EnMailIntervalType))
                    .Cast<EnMailIntervalType>()
                    .Select(v => v.ToString())
                    .OrderBy(x => x)
                    .ToList();
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
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
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }
        }

        public List<DirectMailDTO> GetPendingDirectMail(out string errorMessage)
        {
            var results = new List<DirectMailDTO>();
            var pendings = new List<TDirectMail>();

            errorMessage = string.Empty;

            try
            {
                var directMails = _commonRepository.GetAll();

                // Eventual
                pendings.AddRange(directMails.Where(x => x.IntervalType.Equals((int)EnMailIntervalType.Eventual) && (!x.LastProcessed.HasValue || x.Resend)));

                // Monthly
                pendings.AddRange(directMails.Where(x => x.IntervalType.Equals((int)EnMailIntervalType.Monthly) && x.LastProcessed.Value <= DateTime.Now.AddMonths(-1)));

                results = Mapper.Map<List<TDirectMail>, List<DirectMailDTO>>(pendings);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return results
                .OrderBy(x => x.IdCustomer)
                .ThenBy(x => x.MailType)
                .ToList();
        }
    }
}
