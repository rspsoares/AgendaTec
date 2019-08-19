using AgendaTec.Business.Entities;
using System.Collections.Generic;

namespace AgendaTec.Business.Contracts
{
    public interface IUserFacade
    {
        List<UserAccountDTO> GetGrid(string name, string email, int idCustomer, string idRole, out string errorMessage);
        UserAccountDTO GetUserById(string iUser, out string errorMessage);  
        UserAccountDTO GetUserByEmail(string email, out string errorMessage);
        List<UserAccountDTO> GetRolesCombo(EnUserType userGroup, out string errorMessage);
        List<UserAccountDTO> GetUserNamesCombo(int idCustomer, out string errorMessage);
        List<UserAccountDTO> GetProfessionalNamesCombo(int idCustomer, out string errorMessage);
        List<UserAccountDTO> GetConsumerNamesCombo(int idCustomer, out string errorMessage);
        void Update(UserAccountDTO e, out string errorMessage);
        void UpdateRequiredFields(UserAccountDTO e, out string errorMessage);
        string CheckDuplicatedUser(UserAccountDTO userAccount, out string errorMessage);
        List<UserAccountDTO> GetUserRecipients(int idCustomer, out string errorMessage);
        void UpdateAdminUsersByCustomer(int idCustomer, out string errorMessage);
        bool GetUserIsRoot(int idCustomer, string idUser);
        bool GetUserIsRoot(int idCustomer, int idRole);
        void CheckUserAssociatedWithCustomer(UserAssociatedCustomerDTO e, out string errorMessage);        
        void SendResetPasswordEmail(string userEmail, string userFullName, string subject, string body);
        void ImportUserFile(int idCustomer, string filePath, out string errorMessage);
    }
}
