using AgendaTech.Business.Contracts;
using AgendaTech.Business.Helpers;
using AgendaTech.Infrastructure.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AgendaTech.Business.Bindings
{
    public class ProfessionalFacade : IProfessionalFacade
    {
        private readonly ICommonRepository<TCGProfessionals> _commonRepository;

        public ProfessionalFacade()
        {
            _commonRepository = new CommonRepository<TCGProfessionals>();
        }

        public List<TCGProfessionals> GetGrid(int idCustomer, string professionalName, out string errorMessage)
        {
            var professionals = new List<TCGProfessionals>();
            var sbSqlQuery = new StringBuilder();
            var parameters = new List<SqlParameter>();

            errorMessage = string.Empty;

            try
            {
                sbSqlQuery.Append("SELECT * FROM TCGProfessionals (NOLOCK) WHERE ");

                if (idCustomer > 0 || !string.IsNullOrEmpty(professionalName))
                {
                    if (idCustomer > 0)
                    {
                        sbSqlQuery.Append("IDCustomer = @idCustomer AND ");
                        parameters.Add(new SqlParameter() { ParameterName = "IDCustomer", Value = idCustomer });
                    }

                    if (!string.IsNullOrEmpty(professionalName))
                    {
                        sbSqlQuery.Append("Name LIKE '%' + @Name + '%'");
                        parameters.Add(new SqlParameter() { ParameterName = "Name", Value = professionalName });
                    }
                }

                var sqlQuery = sbSqlQuery.ToString().Trim();
                sqlQuery = CommonHelper.RemoveLastOccurrence(sqlQuery, "WHERE");
                sqlQuery = CommonHelper.RemoveLastOccurrence(sqlQuery, "AND");

                professionals = _commonRepository.SqlQuery(sqlQuery, parameters.ToArray());
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return professionals
                .Select(x => new TCGProfessionals()
                {
                    IDProfessional = x.IDProfessional,
                    IDCustomer = x.IDCustomer,
                    Name = x.Name,
                    Birthday = x.Birthday,
                    Phone = x.Phone,
                    Email = x.Email
                })
                .OrderBy(x => x.IDCustomer)
                .ThenBy(x => x.Name)
                .ToList();
        }

        public TCGProfessionals GetProfessionalById(int idProfessional, out string errorMessage)
        {
            var professional = new TCGProfessionals();

            errorMessage = string.Empty;

            try
            {
                var result = _commonRepository.GetById(idProfessional);

                professional = new TCGProfessionals()
                {
                    IDProfessional = result.IDProfessional,
                    IDCustomer = result.IDCustomer,
                    Name = result.Name,
                    Birthday = result.Birthday,
                    Phone = result.Phone,
                    Email = result.Email
                };
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return professional;
        }

        public TCGProfessionals Insert(TCGProfessionals e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                e = _commonRepository.Insert(e);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return e;
        }

        public void Update(TCGProfessionals e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                _commonRepository.Update(e);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
