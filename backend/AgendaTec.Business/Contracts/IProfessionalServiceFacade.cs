using AgendaTec.Business.Entities;
using System.Collections.Generic;

namespace AgendaTec.Business.Contracts
{
    public interface IProfessionalServiceFacade
    {
        void SaveProfessionalService(int idProfessional, List<ProfessionalServiceDTO> services, out string errorMessage);
        List<ProfessionalServiceDTO> GetAvailablesProfessionalServices(int idCustomer, int idProfessional, out string errorMessage);
        List<ProfessionalServiceDTO> GetServicesComboClient(int idCustomer, bool authenticated, out string errorMessage);
        List<ProfessionalDTO> GetProfessionalNameComboClient(int idCustomer, int idService, bool authenticated, out string errorMessage);
    }
}
