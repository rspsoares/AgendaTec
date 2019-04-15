using AgendaTech.Infrastructure.DatabaseModel;
using System.Collections.Generic;

namespace AgendaTech.Business.Contracts
{
    public interface IServiceFacade
    {
        List<TCGServices> GetGrid(int idCustomer, string serviceName, out string errorMessage);
        List<TCGServices> GetServiceNameCombo(int idCustomer, out string errorMessage);
        TCGServices GetServiceById(int idService, out string errorMessage);
        TCGServices Insert(TCGServices e, out string errorMessage);
        void Update(TCGServices e, out string errorMessage);
    }
}
