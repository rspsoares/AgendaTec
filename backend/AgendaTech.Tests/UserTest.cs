using System.Linq;
using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Business.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgendaTech.Tests
{
    [TestClass]
    public class UserTest
    {
        private readonly IUserFacade _userRepository;

        public UserTest()
        {
            _userRepository = new UserFacade();
        }

        [TestMethod]
        public void User_GetGrid()
        {
            var users = _userRepository.GetGrid(string.Empty, string.Empty, 1, string.Empty, out string errorMessage);
            Assert.IsTrue(users.Any());
        }

        [TestMethod]
        public void User_GetRolesCombo()
        {
            var roles = _userRepository.GetRolesCombo(EnUserType.Administrator, out string errorMessage);
            Assert.IsTrue(roles.Any());
        }

        [TestMethod]
        public void User_GetUserNamesCombo()
        {
            var userNames = _userRepository.GetUserNamesCombo(1, out string errorMessage);
            Assert.IsTrue(userNames.Any());
        }

        [TestMethod]
        public void User_GetProfessionalNamesCombo()
        {
            var professionals = _userRepository.GetProfessionalNamesCombo(3, out string errorMessage);
            Assert.IsTrue(professionals.Any());
        }

        [TestMethod]
        public void User_GetConsumerNamesCombo()
        {
            var consumers = _userRepository.GetConsumerNamesCombo(3, out string errorMessage);
            Assert.IsTrue(consumers.Any());
        }

        [TestMethod]
        public void User_GetUserById()
        {
            var user = _userRepository.GetUserById("089dc1c5-2670-4221-b4a6-d92f01b7b70d", out string errorMessage);
            Assert.IsTrue(!user.Id.Equals(0));
        }

        [TestMethod]
        public void User_CheckDuplicatedUser()
        {
            var user = _userRepository.GetUserById("089dc1c5-2670-4221-b4a6-d92f01b7b70d", out string errorMessage);
            var duplicated = _userRepository.CheckDuplicatedUser(user, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void User_GetLoggedUserByEmail()
        {
            var user = _userRepository.GetLoggedUserByEmail("teste@teste.com", out string errorMessage);
            Assert.IsTrue(!user.Id.Equals(0));
        }

        [TestMethod]
        public void User_Update()
        {
            var fakeName = new Bogus.DataSets.Name();
            
            var user = _userRepository.GetUserById("b1ce56c8-32a9-4b74-b5c1-1173ac6c8366", out string errorMessage);
            user.FirstName = fakeName.FirstName(null).ToString();
            user.LastName = fakeName.LastName(null).ToString();
            _userRepository.Update(user, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }
    }
}
