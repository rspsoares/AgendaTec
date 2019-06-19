using System.Linq;
using AgendaTec.Business.Bindings;
using AgendaTec.Business.Contracts;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bogus.Extensions.Brazil;
using System.Text.RegularExpressions;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;

namespace AgendaTec.Tests
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
            var professionals = _userRepository.GetProfessionalNamesCombo(1, out string errorMessage);
            Assert.IsTrue(professionals.Any());
        }

        [TestMethod]
        public void User_GetConsumerNamesCombo()
        {
            var consumers = _userRepository.GetConsumerNamesCombo(1, out string errorMessage);
            Assert.IsTrue(consumers.Any());
        }

        [TestMethod]
        public void User_GetUserById()
        {
            var user = _userRepository.GetUserById("039a1db4-d562-4014-8f68-17dff6a388e1", out string errorMessage);
            Assert.IsTrue(!user.Id.Equals(0));
        }

        [TestMethod]
        public void User_CheckDuplicatedUser()
        {
            var user = _userRepository.GetUserById("039a1db4-d562-4014-8f68-17dff6a388e1", out string errorMessage);
            var duplicated = _userRepository.CheckDuplicatedUser(user, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void User_GetLoggedUserByEmail()
        {
            var user = _userRepository.GetUserByEmail("teste@ary.com.br", out string errorMessage);
            Assert.IsTrue(!user.Id.Equals(0));
        }

        [TestMethod]
        public void User_Update()
        {
            var fakeName = new Bogus.DataSets.Name();
            
            var user = _userRepository.GetUserById("095c880b-50bb-4c6f-8cda-07de6a765317", out string errorMessage);
            user.FirstName = fakeName.FirstName(null).ToString();
            user.LastName = fakeName.LastName(null).ToString();
            _userRepository.Update(user, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void User_UpdateAdminUsersByCustomer()
        {
            ProfilesHelper.Initialize();
            _userRepository.UpdateAdminUsersByCustomer(1, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void User_UpdateRequiredFields()
        {
            ProfilesHelper.Initialize();
            
            var fakeConsumer = new Faker<UserAccountDTO>()
                .RuleFor(t => t.Id, f => "7e70e65f-54da-48ec-b162-90f62ddd3048")
                .RuleFor(t => t.CPF, f => Regex.Replace(f.Person.Cpf(), @"[^\d]", ""))
                .RuleFor(t => t.Phone, f => f.Phone.PhoneNumber().CleanMask())
                .RuleFor(t => t.Birthday, f => f.Date.Recent().ToString());

            _userRepository.UpdateRequiredFields(fakeConsumer, out string errorMessage);

            ProfilesHelper.Reset();

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

        }
    }
}
