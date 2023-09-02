using Xunit;
using AutoFixture;
using Moq;
using FluentAssertions;
using XUnitApi.Services;
using XUnitApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using XUnitApi.Models;
using FluentAssertions.Equivalency;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using XUnitApi.Data;

namespace XunitApiTest.Controllers
{
    public class FormTableControllerTest
    {
        private readonly IFixture _fixture;
        private readonly Mock<IFormTableRepository> _mockInterface;
        private readonly FormTableController _sut;

        public FormTableControllerTest()
        {
            _fixture = new Fixture();
            _mockInterface = _fixture.Freeze<Mock<IFormTableRepository>>();
            _sut = new FormTableController(_mockInterface.Object);
        }
        #region edit

        [Fact]
        public async Task EditForm_ReturnsOk_WhenFormExists()
        {
            // Arrange
            var formName = _fixture.Create<string>();
            var updatedField = new Form
            {
                Name = formName,

            };
            var existingField = new Form
            {
                Name = formName,

            };

            _mockInterface.Setup(s => s.EditForm(formName, updatedField)).ReturnsAsync(existingField);

            // Act
            var result = await _sut.EditForm(formName, updatedField);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be("Successfully Updated");

            // Verify that the service method was called with the correct arguments
            _mockInterface.Verify(s => s.EditForm(formName, updatedField), Times.Once);
        }

        [Fact]
        public async Task EditRecord_ReturnsNotFound_WhenFieldNotFound()
        {
            // Arrange
            var formName = _fixture.Create<string>();
            var updatedField = new Form
            {
                Name = formName,

            };
            _mockInterface.Setup(s => s.EditForm(formName, updatedField)).ReturnsAsync((Form)null);

            // Act
            var result = await _sut.EditForm(formName, updatedField);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("No Such Form Exists");

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.EditForm(formName, updatedField), Times.Once);
        }

        [Fact]
        public async Task EditRecordn_ReturnsBadRequest_WhenExceptionThrow()
        {
            // Arrange

            var formName = _fixture.Create<string>();
            var updatedField = new Form
            {
                Name = formName,

            };

            _mockInterface.Setup(s => s.EditForm(formName, updatedField)).ThrowsAsync(new Exception("Test exception"));
            // Act
            var result = await _sut.EditForm(formName, updatedField);
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Test exception");

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.EditForm(formName, updatedField), Times.Once);
        }

        #endregion

        #region delete

        [Fact]
        public async Task DeleteRecord_SuccessfulDeletion_WhenValidInput()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var existingField = new Form();
            _mockInterface.Setup(x => x.FormExists(id))
                      .Returns(true);

            _mockInterface.Setup(x => x.DeleteForm(id)).Callback(() => { });

            // Act
            var result = await _sut.DeleteForm(id);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
              .Subject.Value.Should().Be("Form Deleted");

            // Verify that the methods were called as expected
            _mockInterface.Verify(i => i.FormExists(id), Times.Once);
            _mockInterface.Verify(i => i.DeleteForm(id), Times.Once);

        }

        [Fact]
        public async Task DeleteForm_ReturnsNotFound_WhenFormDoesNotExist()
        {
            // Arrange
            var id = _fixture.Create<Guid>(); // Use a specific GUID that doesn't exist

            // Configure FormExists to return false for the given non-existent id
            _mockInterface.Setup(x => x.FormExists(id)).Returns(false);

            // Act
            var result = await _sut.DeleteForm(id);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();

            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be("Form not found");

            // Verify that FormExists was called as expected
            _mockInterface.Verify(i => i.FormExists(id), Times.Once);

            // Verify that DeleteForm was not called
            _mockInterface.Verify(i => i.DeleteForm(It.IsAny<Guid>()), Times.Never);
        }

        #endregion

        #region get all forms by type
        [Fact]
        public async Task GetAllFormsByFormType_ReturnsOkResult_WithValidFormType()
        {
            // Arrange
            var formType = "SampleFormType"; // Specify a valid form type for testing.
            var expectedForms = new List<Form> { new Form { Id = Guid.NewGuid(), Type = formType } };

            _mockInterface.Setup(repo => repo.GetAllFormsByFormType(formType))
                          .ReturnsAsync(expectedForms);

            // Act
            var result = await _sut.GetAllFormsByFormType(formType);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultForms = Assert.IsType<List<Form>>(okResult.Value);
            Assert.Equal(expectedForms.Count, resultForms.Count);
        }

        [Fact]
        public async Task GetAllFormsByFormType_ReturnsNotFound_WithInvalidFormType()
        {
            // Arrange
            var invalidFormType = "NonExistentFormType"; // Specify an invalid form type for testing.

            _mockInterface.Setup(repo => repo.GetAllFormsByFormType(invalidFormType))
                          .ReturnsAsync((List<Form>)null);

            // Act
            var result = await _sut.GetAllFormsByFormType(invalidFormType);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No form existing with the given name", notFoundResult.Value);
        }

        [Fact]
        public async Task GetAllFormsByFormType_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var formType = "SampleFormType"; // Specify a valid form type for testing.

            _mockInterface.Setup(repo => repo.GetAllFormsByFormType(formType))
                          .ThrowsAsync(new Exception("An error occurred"));

            // Act
            var result = await _sut.GetAllFormsByFormType(formType);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var exceptionMessage = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal("An error occurred", exceptionMessage);
        }

        #endregion

        [Fact]
        public async Task AddFormForTable_ValidInput_ReturnsOk()
        {
            // Arrange
            var tableName = "exampleTable";
            var validForm = new Form
            {
                Id = Guid.NewGuid(), // Generate a new unique GUID for the form
                RatebookId = Guid.NewGuid(), // Set a valid RatebookId
                TableId = Guid.NewGuid(), // Set a valid TableId
                AddChangeDeleteFlag = "Add", // Set other properties accordingly
                Sequence = 1,
                SubSequence = 2,
                Type = "SomeType",
                MinOccurs = 0,
                MaxOccurs = 10,
                Number = "123",
                Name = "Sample Form",
                Comment = "This is a sample form",
                HelpText = "Help text goes here",
            };

            var repositoryMock = new Mock<IFormTableRepository>();
            repositoryMock
                .Setup(repo => repo.AddFormForTableName(tableName, It.IsAny<Form>()))
                .ReturnsAsync(true);

            var controller = new FormTableController(repositoryMock.Object);

            // Act
            var result = await controller.AddFormForTable(tableName, validForm) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Form added successfully.", result.Value);
        }

        [Fact]
        public async Task AddFormForTable_InvalidInput_ReturnsBadRequest()
        {
            var tableName = "exampleTable";
            var invalidForm = new Form(); // This form is intentionally left invalid

            var repositoryMock = new Mock<IFormTableRepository>();
            var controller = new FormTableController(repositoryMock.Object);
            controller.ModelState.AddModelError("FieldName", "Error message"); // Simulate model validation error

            // Act
            var result = await controller.AddFormForTable(tableName, invalidForm) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.IsType<ValidationProblemDetails>(result.Value); // Check the specific type
        }

        [Fact]
        public async Task AddFormForTable_TableNotFound_ReturnsNotFound()
        {
            // Arrange
            var tableName = "nonExistentTable";
            var validForm = new Form
            {
                Id = Guid.NewGuid(), // Generate a new unique GUID for the form
                RatebookId = Guid.NewGuid(), // Set a valid RatebookId
                TableId = Guid.NewGuid(), // Set a valid TableId
                AddChangeDeleteFlag = "Add", // Set other properties accordingly
                Sequence = 1,
                SubSequence = 2,
                Type = "SomeType",
                MinOccurs = 0,
                MaxOccurs = 10,
                Number = "123",
                Name = "Sample Form",
                Comment = "This is a sample form",
                HelpText = "Help text goes here",
            };

            var repositoryMock = new Mock<IFormTableRepository>();
            repositoryMock
                .Setup(repo => repo.AddFormForTableName(tableName, It.IsAny<Form>()))
                .ReturnsAsync(false);

            var controller = new FormTableController(repositoryMock.Object);

            // Act
            var result = await controller.AddFormForTable(tableName, validForm) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Aotable record not found for the specified table name.", result.Value);
        }

        [Fact]
        public async Task AddFormForTable_ExceptionOccurs_ReturnsInternalServerError()
        {
            // Arrange
            var tableName = "exampleTable";
            var validForm = new Form
            {
                Id = Guid.NewGuid(), // Generate a new unique GUID for the form
                RatebookId = Guid.NewGuid(), // Set a valid RatebookId
                TableId = Guid.NewGuid(), // Set a valid TableId
                AddChangeDeleteFlag = "Add", // Set other properties accordingly
                Sequence = 1,
                SubSequence = 2,
                Type = "SomeType",
                MinOccurs = 0,
                MaxOccurs = 10,
                Number = "123",
                Name = "Sample Form",
                Comment = "This is a sample form",
                HelpText = "Help text goes here",
            };

            var repositoryMock = new Mock<IFormTableRepository>();
            repositoryMock
                .Setup(repo => repo.AddFormForTableName(tableName, It.IsAny<Form>()))
                .ThrowsAsync(new Exception("Simulated exception"));

            var controller = new FormTableController(repositoryMock.Object);

            // Act
            var result = await controller.AddFormForTable(tableName, validForm) as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }

        // You can write similar tests for the GetFormsAndTableName action
    }


















    
}


