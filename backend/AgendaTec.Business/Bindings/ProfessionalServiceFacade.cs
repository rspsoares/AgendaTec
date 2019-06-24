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
    public class ProfessionalServiceFacade : IProfessionalServiceFacade
    {
        private readonly ICommonRepository<TProfessionalService> _commonRepository;        
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public ProfessionalServiceFacade()
        {          
            _commonRepository = new CommonRepository<TProfessionalService>();            
        }

        public List<ProfessionalServiceDTO> GetAvailablesProfessionalServices(int idCustomer, int idProfessional, out string errorMessage)
        {
            var services = new List<ProfessionalServiceDTO>();
            var repository = new CommonRepository<TCGServices>();
            var professionalFacade = new ProfessionalFacade();

            errorMessage = string.Empty;

            try
            {
                services = repository
                    .Filter(x => x.IDCustomer.Equals(idCustomer))
                    .Select(x => new ProfessionalServiceDTO() { IdService = x.IDService, Description = x.Description, Time = x.Time, Price = x.Price })
                    .ToList();

                if (idProfessional > 0)
                {
                    var professionalServices =
                        professionalFacade.GetProfessionalById(idProfessional, out errorMessage)
                        .Services
                        .Select(x => x.IdService)
                        .ToList();

                    services = services
                        .Where(x => !professionalServices.Contains(x.IdService))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return services;
        }

        public void SaveProfessionalService(int idProfessional, List<ProfessionalServiceDTO> services, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                _commonRepository
                    .Filter(x => x.IDProfessional.Equals(idProfessional))
                    .ForEach(current =>
                    {
                        _commonRepository.Delete(current.IDProfesisonalService);
                    });

                services.ForEach(service =>
                {
                    _commonRepository.Insert(new TProfessionalService()
                    {
                        IDProfesisonalService = Guid.NewGuid(),
                        IDProfessional = idProfessional,
                        IDService = service.IdService
                    });
                });
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }
        }

        public List<ProfessionalServiceDTO> GetServicesComboClient(int idCustomer, bool authenticated, out string errorMessage)
        {
            var result = new List<ProfessionalServiceDTO>();                        
            var repositoryService = new CommonRepository<TCGServices>();

            errorMessage = string.Empty;

            try
            {
                if (authenticated)
                {
                    result = repositoryService
                        .Filter(x => x.IDCustomer.Equals(idCustomer))
                        .Select(x => new ProfessionalServiceDTO() { IdService = x.IDService, Description = x.Description, Time = x.Time, Price = x.Price })
                        .ToList();
                }
                else
                {
                    result.Add(new ProfessionalServiceDTO()
                    {
                        IdService = 0,
                        Description = "Para visualizar as opções de serviços, favor efetuar o Login."
                    });
                }                
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result
                .OrderBy(x => x.Description)
                .ToList();
        }

        public List<ProfessionalDTO> GetProfessionalNameComboClient(int idCustomer, int idService, bool authenticated, out string errorMessage)
        {
            var result = new List<ProfessionalDTO>();
            var professionals = new List<TCGProfessionals>();
            var repositoryProfessional = new CommonRepository<TCGProfessionals>();

            errorMessage = string.Empty;

            try
            {
                if (authenticated)
                {
                    if (idService > 0)
                    {
                        professionals = _commonRepository
                            .Filter(x => x.IDService.Equals(idService))
                            .Select(x => x.TCGProfessionals)
                            .ToList();
                    }
                    else
                    {
                        professionals = repositoryProfessional
                            .Filter(x => x.IDCustomer.Equals(idCustomer))
                            .ToList();
                    }
                }
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
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}
