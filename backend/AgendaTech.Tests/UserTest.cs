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
        public void User_GetAll()
        {
            var users = _userRepository.GetGrid("1", "drig", 1, 1, out string errorMessage);
            Assert.IsTrue(users.Any());
        }

        [TestMethod]
        public void User_Update()
        {
            var user = _userRepository.GetUserById(4, out string errorMessage);
            user.FirstName = "First";
            user.LastName = "Last";
            _userRepository.Update(user, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }
    }
}
