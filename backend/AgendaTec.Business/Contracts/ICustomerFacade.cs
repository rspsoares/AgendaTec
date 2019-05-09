using AgendaTec.Business.Entities;
using System.Collections.Generic;

namespace AgendaTec.Business.Contracts
{
    public interface ICustomerFacade
    {
        List<CustomerDTO> GetGrid(string customerName, out string errorMessage);
        List<CustomerDTO> GetCompanyNameCombo(int idCustomer, out string errorMessage);
        CustomerDTO GetCustomerById(int idCustomer, out string errorMessage);
        CustomerDTO GetCustomerByKey(string customerKey, out string errorMessage);
        CustomerDTO Insert(CustomerDTO e, out string errorMessage);
        void Update(CustomerDTO e, out string errorMessage);
    }
}
