using Xunit;
using Moq;
using FluentValidation.TestHelper;
using UKParliament.CodeTest.Services.Validators;
using UKParliament.CodeTest.Services.Models;
using UKParliament.CodeTest.Data;

namespace UKParliament.CodeTest.Tests.UnitTests
{
    public class PersonValidatorTests
    {
        private readonly PersonValidator _validator;

        public PersonValidatorTests()
        {
            _validator = new PersonValidator();
        }

        [Fact]
        public async Task Should_Have_Error_When_FirstName_Is_Empty()
        {
            var model = new PersonDto { FirstName = "", LastName = "Smith" };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.FirstName)
                .WithErrorMessage("First Name is required");
        }

        [Fact]
        public async Task Should_Have_Error_When_LastName_Is_Empty()
        {
            var model = new PersonDto { FirstName = "John", LastName = "" };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.LastName)
                .WithErrorMessage("Last Name is required");
        }

        [Fact]
        public async Task Should_Not_Have_Error_When_FirstName_And_LastName_Are_Valid()
        {
            var model = new PersonDto { FirstName = "John", LastName = "Smith" };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
            result.ShouldNotHaveValidationErrorFor(x => x.LastName);
        }
    }

    public class PersonDetailsValidatorTests
    {
        private readonly PersonDetailsValidator _validator;
        private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
        private readonly Mock<IPersonRepository> _mockPersonRepository;

        public PersonDetailsValidatorTests()
        {
            _mockDepartmentRepository = new Mock<IDepartmentRepository>();
            _mockPersonRepository = new Mock<IPersonRepository>();
            // Mock department repository to return a department with ID 1
            _mockDepartmentRepository.Setup(repo => repo.GetDepartmentsAsync())
                .ReturnsAsync([new DepartmentEntity { Id = 1, Name = "HR" }]);

            // Base validator instance
            var baseValidator = new PersonValidator();
            _validator = new PersonDetailsValidator(baseValidator, _mockDepartmentRepository.Object, _mockPersonRepository.Object);
        }

        [Fact]
        public async Task Should_Have_Error_When_DateOfBirth_Is_Empty()
        {
            var model = new PersonDetailsDto
            {
                FirstName = "John",
                LastName = "Smith",
                DateOfBirth = default
            };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
                .WithErrorMessage("Date of Birth is required");
        }

        [Fact]
        public async Task Should_Have_Error_When_DateOfBirth_Is_In_Future()
        {
            var model = new PersonDetailsDto
            {
                FirstName = "John",
                LastName = "Smith",
                DateOfBirth = DateTime.Today.AddDays(1)
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
                .WithErrorMessage("Date of Birth must not be in the future");
        }

        [Fact]
        public async Task Should_Have_Error_When_DepartmentId_Is_Invalid()
        {
            var model = new PersonDetailsDto
            {
                FirstName = "John",
                LastName = "Smith",
                DateOfBirth = DateTime.Today.AddYears(-30),
                DepartmentId = 9 // Non-existent department
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.DepartmentId)
                .WithErrorMessage("Department is invalid");
        }

        [Fact]
        public async Task Should_Have_Error_When_Person_Is_Duplicate()
        {
            PersonEntity duplicatePerson = new PersonEntity { Id = 2, FirstName = "John", LastName = "Smith", DateOfBirth = DateTime.Today.AddYears(-30) };
            _mockPersonRepository.Setup(repo => repo.SearchForPersonAsync("John", "Smith", duplicatePerson.DateOfBirth.Value))
                .ReturnsAsync(duplicatePerson);

            var model = new PersonDetailsDto
            {
                FirstName = "John",
                LastName = "Smith",
                DateOfBirth = duplicatePerson.DateOfBirth.Value
            };

            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("A person with the same first name, last name, and date of birth already exists.");
        }

        [Fact]
        public async Task Should_Not_Have_Error_When_Person_Is_Unique()
        {
            // Arrange: Ensure that no duplicate person exists
            _mockPersonRepository.Setup(repo => repo.SearchForPersonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync((PersonEntity)null); // No duplicate found

            // Arrange: Ensure that the department exists in the repository

            _mockDepartmentRepository
                .Setup(repo => repo.DepartmentExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true); // Ensure the department is found

            var model = new PersonDetailsDto
            {
                FirstName = "Jane",
                LastName = "Doe",
                DateOfBirth = DateTime.Today.AddYears(-25),
                DepartmentId = 1 // This must exist in the mocked department list
            };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert: Ensure no validation errors
            result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
            result.ShouldNotHaveValidationErrorFor(x => x.LastName);
            result.ShouldNotHaveValidationErrorFor(x => x.DateOfBirth);
            result.ShouldNotHaveValidationErrorFor(x => x.DepartmentId);
        }
    }
}
