using AgendaTec.Business.Entities;
using System.Collections.Generic;

namespace AgendaTec.Business.Contracts
{
    public interface IServiceFacade
    {
        List<ServiceDTO> GetGrid(int idCustomer, string serviceName, out string errorMessage);
        List<ServiceDTO> GetServiceNameCombo(int idCustomer, out string errorMessage);
        List<ServiceDTO> GetServiceNameComboClient(int idCustomer, bool authenticated, out string errorMessage);
        ServiceDTO GetServiceById(int idService, out string errorMessage);
        ServiceDTO Insert(ServiceDTO e, out string errorMessage);
        void Update(ServiceDTO e, out string errorMessage);
    }
}
