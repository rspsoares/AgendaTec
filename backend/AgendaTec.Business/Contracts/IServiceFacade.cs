﻿using AgendaTec.Infrastructure.DatabaseModel;
using System.Collections.Generic;

namespace AgendaTec.Business.Contracts
{
    public interface IServiceFacade
    {
        List<TCGServices> GetGrid(int idCustomer, string serviceName, out string errorMessage);
        List<TCGServices> GetServiceNameCombo(int idCustomer, out string errorMessage);
        List<TCGServices> GetServiceNameComboClient(int idCustomer, bool authenticated, out string errorMessage);
        TCGServices GetServiceById(int idService, out string errorMessage);
        TCGServices Insert(TCGServices e, out string errorMessage);
        void Update(TCGServices e, out string errorMessage);


    }
}