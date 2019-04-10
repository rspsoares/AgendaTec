using AgendaTech.Infrastructure.DatabaseModel;
using System.Collections.Generic;

namespace AgendaTech.Business.Contracts
{
    public interface IProfessionalFacade
    {
        List<TCGProfessionals> GetGrid(int idCustomer, string professionalName, out string errorMessage);
        TCGProfessionals GetProfessionalById(int idProfessional, out string errorMessage);
        TCGProfessionals Insert(TCGProfessionals e, out string errorMessage);
        void Update(TCGProfessionals e, out string errorMessage);
    }
}
