using AgendaTec.Business.Entities;
using System.Collections.Generic;

namespace AgendaTec.Business.Contracts
{
    public interface IDirectMailFacade
    {
        List<DirectMailDTO> GetGrid(int idCustomer, string description, out string errorMessage);
        DirectMailDTO GetDirectMailingById(int idDirectMail, out string errorMessage);
        DirectMailDTO Insert(DirectMailDTO e, out string errorMessage);
        void Update(DirectMailDTO e, out string errorMessage);
        void Delete(int idDirectMail, out string errorMessage);
        List<string> GetIntervalCombo(out string errorMessage);
        void ResendDirectMail(int idDirectMail, out string errorMessage);
    }
}
