using AgendaTec.Business.Entities;
using System.Collections.Generic;

namespace AgendaTec.Business.Contracts
{
    public interface IDirectMailingFacade
    {
        List<DirectMailingDTO> GetGrid(int idCustomer, string description, out string errorMessage);
        DirectMailingDTO GetDirectMailingById(int idMailing, out string errorMessage);
        DirectMailingDTO Insert(DirectMailingDTO e, out string errorMessage);
        void Update(DirectMailingDTO e, out string errorMessage);
        void Delete(int idDirectMailing, out string errorMessage);
    }
}
