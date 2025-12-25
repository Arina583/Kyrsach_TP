using Company.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Security.Claims;
using TruckingCompany.Controllers;

namespace TruckingCompany.Tests
{
    [TestFixture]
    public class AccountControllerTests
    {

        private Mock<DbSet<PersonalRoles>> mockDbSet;
        private Mock<DbClassContext> mockContext;
        private AccountController controller;

        [SetUp]
        public void Setup()
        {
            // Создаем моковские объекты для тестового контекста
            var data = new List<PersonalRoles>
            {
                new PersonalRoles { id = 1, login = "log1", password = "0710", role = "logist" },
                new PersonalRoles { id = 2, login = "dis1", password = "1234", role = "dispatcher" }

            }.AsQueryable();

            mockDbSet = new Mock<DbSet<PersonalRoles>>();
            mockDbSet.As<IQueryable<PersonalRoles>>().Setup(m => m.Provider).Returns(data.Provider);
            mockDbSet.As<IQueryable<PersonalRoles>>().Setup(m => m.Expression).Returns(data.Expression);
            mockDbSet.As<IQueryable<PersonalRoles>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockDbSet.As<IQueryable<PersonalRoles>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            mockContext = new Mock<DbClassContext>();
            mockContext.Setup(c => c.PersonalRoles).Returns(mockDbSet.Object);

            controller = new AccountController(mockContext.Object);
        }

        /*#region Tests for failed authentication
        [Test]
        public async Task Test_Login_Failed_Authentication_Returns_Error_Message()
        {
            // Arrange
            var invalidCredentials = new Dictionary<string, string>()
            {
                {"login", "invalid_login"},
                {"password", "wrong_password"}
            };

            // Act
            var result = await controller.Login(invalidCredentials["login"], invalidCredentials["password"]);

            Assert.That(controller.ModelState.Values.Any(v => v.Errors.Count > 0)); // Ошибка должна присутствовать
        }
        #endregion*/

        #region Testing authorization
        [Test]
        public void Test_DispatcherDashboard_Is_Authorized()
        {
            // Arrange
            var authUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "dispatcher") }));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = authUser }
            };

            // Act
            var result = controller.DispatcherDashboard();
        }

        [Test]
        public void Test_LogistPanel_Is_Authorized()
        {
            // Arrange
            var authUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "logist") }));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = authUser }
            };

            // Act
            var result = controller.LogistPanel();
        }
        #endregion


        [Test]
        public async Task Test_Login_EmptyLogin_ReturnsError()
        {
            string login = "";
            string password = "0710";

            var result = await controller.Login(login, password);

            // Сначала проверяем, что результат не null
            Assert.That(result, Is.Not.Null, "login вернул null");

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null, "Результат не является ViewResult");

            // Теперь проверяем ModelState
            ClassicAssert.IsTrue(controller.ModelState.ErrorCount > 0);

            // Проверяем наличие ключа "Login" перед доступом к Errors
            if (controller.ModelState.ContainsKey("login"))
            {
                Assert.That(controller.ModelState["login"].Errors.Count, Is.EqualTo(1));
            }
            else
            {
                Assert.Fail("Ключ 'login' отсутствует в ModelState");
            }
        }

        [Test]
        public async Task Test_Login_NullLogin_ReturnsError()
        {
            string login = null;
            string password = "0710";

            var result = await controller.Login(login, password);

            // Сначала проверяем, что результат не null
            Assert.That(result, Is.Not.Null, "login вернул null");

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null, "Результат не является ViewResult");

            // Теперь проверяем ModelState
            ClassicAssert.IsTrue(controller.ModelState.ErrorCount > 0);

            // Проверяем наличие ключа "Login" перед доступом к Errors
            if (controller.ModelState.ContainsKey("login"))
            {
                Assert.That(controller.ModelState["login"].Errors.Count, Is.EqualTo(1));
            }
            else
            {
                Assert.Fail("Ключ 'login' отсутствует в ModelState");
            }
        }

        [Test]
        public async Task Test_Password_EmptyLogin_ReturnsError()
        {
            string login = "log1";
            string password = "";

            var result = await controller.Login(login, password);

            // Сначала проверяем, что результат не null
            Assert.That(result, Is.Not.Null, "password вернул null");

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null, "Результат не является ViewResult");

            // Теперь проверяем ModelState
            ClassicAssert.IsTrue(controller.ModelState.ErrorCount > 0);

            // Проверяем наличие ключа "Login" перед доступом к Errors
            if (controller.ModelState.ContainsKey("password"))
            {
                Assert.That(controller.ModelState["password"].Errors.Count, Is.EqualTo(1));
            }
            else
            {
                Assert.Fail("Ключ 'password' отсутствует в ModelState");
            }
        }

    }
}
