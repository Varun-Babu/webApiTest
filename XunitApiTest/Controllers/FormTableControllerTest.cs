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
using Newtonsoft.Json;
using Azure;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Drawing;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _mockInterface = _fixture.Freeze<Mock<IFormTableRepository>>();
            _sut = new FormTableController(_mockInterface.Object);
        }
        #region edit

        [Fact]
        public async Task EditForm_ReturnsOk_WhenFormExists()
        {
            // Arrange
            var formName = _fixture.Create<string>();
            var updatedForm = _fixture.Create<Form>();
            updatedForm.Name = formName;
            var existingForm = _fixture.Create<Form>();
            existingForm.Name = formName;

            _mockInterface.Setup(s => s.EditForm(formName, updatedForm)).ReturnsAsync(existingForm);

            // Act
            var result = await _sut.EditForm(formName, updatedForm);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be("Successfully Updated");

            // Verify
            _mockInterface.Verify(s => s.EditForm(formName, updatedForm), Times.Once);
        }

        [Fact]
        public async Task EditForm_ReturnsNotFound_WhenFormNotFound()
        {
            // Arrange
            var formName = _fixture.Create<string>();
            var updatedForm = _fixture.Create<Form>();
            updatedForm.Name = formName;
            _mockInterface.Setup(s => s.EditForm(formName, updatedForm)).ReturnsAsync((Form)null);

            // Act
            var result = await _sut.EditForm(formName, updatedForm);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("No Such Form Exists");

            // Verify 
            _mockInterface.Verify(s => s.EditForm(formName, updatedForm), Times.Once);
        }

        [Fact]
        public async Task EditForm_ReturnsBadRequest_WhenExceptionThrow()
        {
            // Arrange

            var formName = _fixture.Create<string>();
            var updatedForm = _fixture.Create<Form>();
            updatedForm.Name = formName;

            _mockInterface.Setup(s => s.EditForm(formName, updatedForm)).ThrowsAsync(new Exception("Test exception"));
            // Act
            var result = await _sut.EditForm(formName, updatedForm);
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Test exception");

            // Verify 
            _mockInterface.Verify(s => s.EditForm(formName, updatedForm), Times.Once);
        }

        #endregion

        #region delete

        [Fact]
        public async Task DeleteForm_SuccessfulDeletion_WhenValidInput()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var existingField = _fixture.Create<Form>();
            _mockInterface.Setup(x => x.FormExists(id))
                      .Returns(true);

            _mockInterface.Setup(x => x.DeleteForm(id)).Callback(() => { });

            // Act
            var result = await _sut.DeleteForm(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>()
              .Subject.Value.Should().Be("Form Deleted");
           

            // Verify 
            _mockInterface.Verify(i => i.FormExists(id), Times.Once);
            _mockInterface.Verify(i => i.DeleteForm(id), Times.Once);

        }

        [Fact]
        public async Task DeleteForm_ReturnsNotFound_WhenFormDoesNotExist()
        {
            // Arrange
            var id = _fixture.Create<Guid>(); 
            _mockInterface.Setup(x => x.FormExists(id)).Returns(false);

            // Act
            var result = await _sut.DeleteForm(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();

            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be("Form not found");

            // Verify 
            _mockInterface.Verify(i => i.FormExists(id), Times.Once);
            _mockInterface.Verify(i => i.DeleteForm(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteForm_ExceptionThrown_ShouldReturnBadRequestWithExceptionMessage()
        {
            // Arrange
            var formId = Guid.NewGuid();
            _mockInterface.Setup(repo => repo.FormExists(formId))
                .Returns(true);
            _mockInterface.Setup(repo => repo.DeleteForm(formId))
                .ThrowsAsync(new Exception("An error occurred while deleting the form."));

            // Act
            var result = await _sut.DeleteForm(formId);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("An error occurred while deleting the form.");

            // Verify
            _mockInterface.Verify(repo => repo.FormExists(formId), Times.Once);
            _mockInterface.Verify(repo => repo.DeleteForm(formId), Times.Once);
        }

        #endregion

        #region get all forms by type

        [Fact]
        public async Task GetAllFormsByFormType_ReturnsOkResult_WithValidFormType()
        {
            // Arrange

            var formType = _fixture.Create<string>();
            var expectedForms = _fixture.Create<List<Form>>().ToList();

            foreach (var form in expectedForms)
             {
                 form.Type = formType; 
             } 

             _mockInterface.Setup(repo => repo.GetAllFormsByFormType(formType))
                           .ReturnsAsync(expectedForms);

            // Act
            var result = await _sut.GetAllFormsByFormType(formType);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            var resultForms = okResult.Value.Should().BeAssignableTo<List<Form>>().Subject;
             resultForms.Should().BeEquivalentTo(expectedForms);
            result.Should().NotBeNull();

            // Verify 
            _mockInterface.Verify(repo => repo.GetAllFormsByFormType(formType), Times.Once);
        }

        [Fact]
        public async Task GetAllFormsByFormType_ReturnsNotFound_WithInvalidFormType()
        {
            // Arrange
            var invalidFormType = _fixture.Create<string>(); 

            _mockInterface.Setup(repo => repo.GetAllFormsByFormType(invalidFormType))
                          .ReturnsAsync((List<Form>)null);

            // Act
            var result = await _sut.GetAllFormsByFormType(invalidFormType);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.StatusCode.Should().Be(404);
            var errorMessage = notFoundResult.Value.Should().BeOfType<string>().Subject;
            errorMessage.Should().Be("No form existing with the given name");
            result.Should().NotBeNull();
            // Verify
            _mockInterface.Verify(repo => repo.GetAllFormsByFormType(invalidFormType), Times.Once);
        }

        [Fact]
        public async Task GetAllFormsByFormType_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var formType = _fixture.Create<string>(); 

            _mockInterface.Setup(repo => repo.GetAllFormsByFormType(formType))
                          .ThrowsAsync(new Exception("An error occurred"));
            // Act
            var result = await _sut.GetAllFormsByFormType(formType);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);
            var exceptionMessage = badRequestResult.Value.Should().BeOfType<string>().Subject;
            exceptionMessage.Should().Be("An error occurred");
            result.Should().NotBeNull();
            // Verify 
            _mockInterface.Verify(repo => repo.GetAllFormsByFormType(formType), Times.Once);
        }

        #endregion

        #region add form

        [Fact]
        public async Task AddForm_ValidForm_ShouldReturnOkWithAddedForm()
        {
            // Arrange
            var validForm = _fixture.Create<Form>();
            _mockInterface.Setup(repo => repo.AddForm(It.IsAny<Form>()))
                .ReturnsAsync(validForm);

            // Act
            var result = await _sut.AddForm(validForm);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(validForm);
            result.Should().NotBeNull();

            // Verify 
            _mockInterface.Verify(repo => repo.AddForm(validForm), Times.Once);
        }

        [Fact]
        public async Task AddForm_InvalidForm_ShouldReturnBadRequest()
        {
            // Arrange
            Form invalidForm = null;

            // Act
            var result = await _sut.AddForm(invalidForm);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            result.Should().NotBeNull();

            // Verify 
            _mockInterface.Verify(repo => repo.AddForm(It.IsAny<Form>()), Times.Never);
        }

        [Fact]
        public async Task AddForm_TableNameNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var validForm = _fixture.Create<Form>();
            _mockInterface.Setup(repo => repo.AddForm(It.IsAny<Form>()))
                .ReturnsAsync((Form)null);

            // Act
            var result = await _sut.AddForm(validForm);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();

            // Verify 
            _mockInterface.Verify(repo => repo.AddForm(validForm), Times.Once);
        }

        [Fact]
        public async Task AddForm_Conflict_FormAlreadyExists()
        {
            // Arrange
            var form = _fixture.Create<Form>();
            _mockInterface.Setup(repo => repo.FormExists(It.IsAny<Guid>())).Returns(true);       

            // Act
            var result = await _sut.AddForm(form);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ConflictObjectResult>()
                .Which.Value.Should().Be("Form already exist");

            //verify
            _mockInterface.Verify(repo => repo.FormExists(form.Id), Times.Once);
        }
    

        #endregion

        #region get forms by tablename

        [Fact]
        public async Task GetFormsByTableName_FormsExist_ShouldReturnOkWithForms()
        {
            // Arrange
            var tableName = _fixture.Create<string>();
            var forms = _fixture.Create<List<Form>>().ToList();

            _mockInterface.Setup(repo => repo.GetFormsByTableName(tableName))
                .ReturnsAsync(forms);

            // Act
            var result = await _sut.GetFormsByTableName(tableName);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(forms);

            // Verify
            _mockInterface.Verify(repo => repo.GetFormsByTableName(tableName), Times.Once);
        }

        [Fact]
        public async Task GetFormsByTableName_NoFormsExist_ShouldReturnNotFound()
        {
            // Arrange
            var tableName = _fixture.Create<string>();
            _mockInterface.Setup(repo => repo.GetFormsByTableName(tableName))
                .ReturnsAsync(new List<Form>()); 

            // Act
            var result = await _sut.GetFormsByTableName(tableName);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();

            // Verify 
            _mockInterface.Verify(repo => repo.GetFormsByTableName(tableName), Times.Once);
        }

        [Fact]
        public async Task GetFormsByTableName_ExceptionThrown_ShouldReturnBadRequest()
        {
            // Arrange
            var tableName = _fixture.Create<string>();
            _mockInterface.Setup(repo => repo.GetFormsByTableName(tableName))
                .ThrowsAsync(new Exception("An error occurred."));

            // Act
            var result = await _sut.GetFormsByTableName(tableName);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            // Verify
            _mockInterface.Verify(repo => repo.GetFormsByTableName(tableName), Times.Once);
        }

        #endregion

        #region Get Forms and Table Name

        [Fact]
        public async Task GetFormsAndTableName_FormsExist_ShouldReturnOkWithFormsAndTableName()
        {
            // Arrange
            var tableId = Guid.NewGuid();
            var tableName = _fixture.Create<string>();
            var forms = _fixture.Create<List<Form>>().ToList();
            _mockInterface.Setup(repo => repo.GetAllFormsAndTableName(tableId))
                .ReturnsAsync((forms, tableName));

            // Act
            var result = await _sut.GetFormsAndTableName(tableId);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);

            // Verify 
            _mockInterface.Verify(repo => repo.GetAllFormsAndTableName(tableId), Times.Once);

        }

        [Fact]
        public async Task GetFormsAndTableName_NoFormsExist_ShouldReturnNotFound()
        {
            // Arrange
            var tableId = Guid.NewGuid();
            _mockInterface.Setup(repo => repo.GetAllFormsAndTableName(tableId))
                .ReturnsAsync((new List<Form>(), "NonExistentTable"));

            // Act

            var result = await _sut.GetFormsAndTableName(tableId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();

            // Verify 
            _mockInterface.Verify(repo => repo.GetAllFormsAndTableName(tableId), Times.Once);
        }

        [Fact]
        public async Task GetFormsAndTableName_ExceptionThrown_ShouldReturnBadRequest()
        {
            // Arrange
            var tableId = Guid.NewGuid();
            _mockInterface.Setup(repo => repo.GetAllFormsAndTableName(tableId))
                .ThrowsAsync(new Exception("An error occurred."));

            // Act
            var result = await _sut.GetFormsAndTableName(tableId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            // Verify 
            _mockInterface.Verify(repo => repo.GetAllFormsAndTableName(tableId), Times.Once);
        }

        #endregion

    }
}

    


















    



