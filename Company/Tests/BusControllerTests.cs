using Company.Controllers;
using Company.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Company.Tests
{
    [TestFixture]
    public class BusControllerTests
    {
        private Mock<DbClassContext> dbContextMock;
        private BusController busController;

        [SetUp]
        public void Setup()
        {
            // Создание мока для DbClassContext
            dbContextMock = new Mock<DbClassContext>();
            busController = new BusController(dbContextMock.Object);
        }

        #region Тестирование метода AddBus

        [Test]
        public void Test1()
        {
            Assert.Equals(42, 42);
        }

        [Test]
        public void AddBus_ModelIsValid_ReturnsRedirect()
        {
            // Арранжем
            var validBus = new Buses
            {
                stateNumber = "A123BC",
                status = "active",
                numberSeat = 40
            };

            // Акт
            var result = busController.AddBus(validBus) as RedirectToActionResult;

            // Ассерт
            Assert.That(result, Is.Not.Null);
            Assert.Equals("Buses", result.ActionName);
        }

        [Test]
        public void AddBus_ModelIsNotValid_ReturnsView()
        {
            // Арранжем
            var invalidBus = new Buses
            {
                stateNumber = "",
                status = "",
                numberSeat = 0
            };
            busController.ModelState.AddModelError("StateNumber", "Требуется госномер.");

            // Акт
            var result = busController.AddBus(invalidBus) as ViewResult;

            // Ассерт
            Assert.That(result, Is.Not.Null);
            Assert.Equals(invalidBus, result.ViewData.Model);
        }

        #endregion

        #region Тестирование метода EditBus

        [Test]
        public void EditBus_ExistingBus_ReturnsView()
        {
            // Арранжем
            var existingBus = new Buses { id = 1 };
            dbContextMock.Setup(ctx => ctx.Buses.Find(existingBus.id)).Returns(existingBus);

            // Акт
            var result = busController.EditBus(existingBus.id) as ViewResult;

            // Ассерт
            Assert.That(result, Is.Not.Null);
            Assert.Equals(existingBus, result.ViewData.Model);
        }

        [Test]
        public void EditBus_NonexistentBus_ReturnsNotFound()
        {
            // Арранжем
            const int nonExistentId = 999;
            dbContextMock.Setup(ctx => ctx.Buses.Find(nonExistentId)).Returns<Buses>(null);

            // Акт
            var result = busController.EditBus(nonExistentId) as NotFoundResult;

            // Ассерт
            Assert.That(result, Is.Not.Null);
        }

        #endregion

        #region Тестирование метода DeleteBus

        [Test]
        public void DeleteBus_ExistingBus_ReturnsView()
        {
            // Арранжем
            var existingBus = new Buses { id = 1 };
            dbContextMock.Setup(ctx => ctx.Buses.Find(existingBus.id)).Returns(existingBus);

            // Акт
            var result = busController.DeleteBus(existingBus.id) as ViewResult;

            // Ассерт
            Assert.That(result, Is.Not.Null);
            Assert.Equals(existingBus, result.ViewData.Model);
        }

        [Test]
        public void DeleteBus_NonexistentBus_ReturnsNotFound()
        {
            // Арранжем
            const int nonExistentId = 999;
            dbContextMock.Setup(ctx => ctx.Buses.Find(nonExistentId)).Returns<Buses>(null);

            // Акт
            var result = busController.DeleteBus(nonExistentId) as NotFoundResult;

            // Ассерт
            Assert.That(result, Is.Not.Null);
        }

        #endregion
    }
}
