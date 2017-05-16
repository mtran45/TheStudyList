using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheStudyList.Controllers;
using TheStudyList.Domain.Entities;

namespace TheStudyList.Tests
{
    [TestClass]
    public class ManageTests
    {
        private User user;

        [TestInitialize()]
        public void Initialize()
        {
            user = new User()
            {
                Id = "1",
                UserName = "user"
            };
        }

        [TestMethod]
        public void Can_Change_Timezone()
        {
            // Arrange
            var context = new TestContext();
            context.Users.Add(user);

            ManageController target = new ManageController(context)
            {
                GetUserId = () => user.Id
            };

            // Act
            target.ChangeTimeZone("Pacific Standard Time");

            // Assert
            Assert.AreEqual("Pacific Standard Time", user.TimeZoneId);
        }
    }
}
