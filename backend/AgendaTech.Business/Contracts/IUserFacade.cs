using AgendaTech.Business.Entities;
using System;
using System.Collections.Generic;

namespace AgendaTech.Business.Contracts
{
    public interface IUserFacade
    {
        List<UserAccountDTO> GetGrid(string name, string login, int idCustomer, int idUserGroup, out string errorMessage);
        UserAccountDTO GetUserById(int iUser, out string errorMessage);
        UserAccountDTO GetUserByUq(Guid uq, out string errorMessage);
        UserAccountDTO GetLoggedUserByEmail(string email, out string errorMessage);
        List<UserAccountDTO> GetUserGroupsCombo(EnUserType userGroup, out string errorMessage);
        List<UserAccountDTO> GetUserNamesCombo(int idCustomer, out string errorMessage);
        List<UserAccountDTO> GetConsumerNamesCombo(int idCustomer, out string errorMessage);
        void Update(UserAccountDTO e, out string errorMessage);
        bool VerifyPassword(string login, string password, out UserAccountDTO userAccount);
        string CheckDuplicatedUser(UserAccountDTO userAccount, out string errorMessage);
        int CreateConsumer(UserAccountDTO userAccount, out string errorMessage);
        void ChangePassword(UserAccountDTO userAccount, out string errorMessage);
    }
}
