using AgendaTech.Infrastructure.DatabaseModel;
using System.Collections.Generic;

namespace AgendaTech.Business.Contracts
{
    public interface ICustomerFacade
    {
        List<TCGCustomers> GetGrid(string customerName, out string errorMessage);
        TCGCustomers GetCustomerById(int idCustomer, out string errorMessage);
        TCGCustomers Insert(TCGCustomers e, out string errorMessage);
        void Update(TCGCustomers e, out string errorMessage);
    }
}
