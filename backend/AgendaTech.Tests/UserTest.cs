using System.Linq;
using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
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
            var users = _userRepository.GetGrid(string.Empty, string.Empty, 0, 0, out string errorMessage);
            Assert.IsTrue(users.Any());
        }

        [TestMethod]
        public void User_Update()
        {
            var fakeName = new Bogus.DataSets.Name();
            
            var user = _userRepository.GetUserById(4, out string errorMessage);
            user.FirstName = fakeName.FirstName(null).ToString();
            user.LastName = fakeName.LastName(null).ToString();
            _userRepository.Update(user, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }
    }
}
