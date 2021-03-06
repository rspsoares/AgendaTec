﻿using AgendaTec.Business.Entities;
using System;
using System.Collections.Generic;

namespace AgendaTec.Business.Contracts
{
    public interface IProfessionalFacade
    {
        List<ProfessionalDTO> GetGrid(int idCustomer, string professionalName, out string errorMessage);
        List<ProfessionalDTO> GetProfessionalNameCombo(int idCustomer, Guid idUser, out string errorMessage);        
        ProfessionalDTO GetProfessionalById(int idProfessional, out string errorMessage);
        ProfessionalDTO GetProfessionalByUserId(string idUser, out string errorMessage);
        ProfessionalDTO Insert(ProfessionalDTO e, out string errorMessage);
        void Update(ProfessionalDTO e, out string errorMessage);
    }
}
