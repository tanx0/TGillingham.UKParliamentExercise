using Xunit;
using Moq;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Services.Exceptions;
using UKParliament.CodeTest.Services.Models;
using ValidationException = UKParliament.CodeTest.Services.Exceptions.ValidationException;


namespace UKParliament.CodeTest.Tests.Services
{
    public class PersonServiceTests
    {
        private readonly Mock<IPersonRepository> _mockPersonRepository;
        //private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
        private readonly Mock<IValidator<PersonDetailsDto>> _mockValidator;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PersonService _personService;

        public PersonServiceTests()
        {
            _mockPersonRepository = new Mock<IPersonRepository>();
            //_mockDepartmentRepository = new Mock<IDepartmentRepository>();
            _mockValidator = new Mock<IValidator<PersonDetailsDto>>();
            _mockMapper = new Mock<IMapper>();

            _personService = new PersonService(
                _mockPersonRepository.Object,
                //_mockDepartmentRepository.Object,
                _mockValidator.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetPeople_Should_Return_Mapped_Dtos()
        {
            // Arrange
            var peopleEntities = new List<PersonEntity>
            {
                new PersonEntity { Id = 1, FirstName = "John", LastName = "Doe" },
                new PersonEntity { Id = 2, FirstName = "Jane", LastName = "Doe" }
            };

            var peopleDtos = new List<PersonDetailsDto>
            {
                new PersonDetailsDto { Id = 1, FirstName = "John", LastName = "Doe" },
                new PersonDetailsDto { Id = 2, FirstName = "Jane", LastName = "Doe" }
            };

            _mockPersonRepository.Setup(repo => repo.GetPeopleAsync()).ReturnsAsync(peopleEntities);
            _mockMapper.Setup(m => m.Map<IEnumerable<PersonDetailsDto>>(peopleEntities)).Returns(peopleDtos);

            // Act
            var result = (await _personService.GetPeopleAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("John", result[0].FirstName);
            Assert.Equal("Jane", result[1].FirstName);
        }

        [Fact]
        public async Task GetPersonById_Should_Return_Person_When_Found()
        {
            // Arrange
            var personEntity = new PersonEntity { Id = 1, FirstName = "John", LastName = "Doe" };
            var personDto = new PersonDetailsDto { Id = 1, FirstName = "John", LastName = "Doe" };

            _mockPersonRepository.Setup(repo => repo.GetPersonAsync(1)).ReturnsAsync(personEntity);
            _mockMapper.Setup(m => m.Map<PersonDetailsDto>(personEntity)).Returns(personDto);

            // Act
            var result = await _personService.GetPersonByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public async Task GetPersonById_Should_Throw_NotFoundException_When_Not_Found()
        {
            // Arrange
            _mockPersonRepository.Setup(repo => repo.GetPersonAsync(1)).ReturnsAsync((PersonEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _personService.GetPersonByIdAsync(1));
            Assert.Equal("Person not found", exception.Message);
        }

        [Fact]
        public async Task AddPerson_Should_Add_Person_When_Valid()
        {
            // Arrange
            var personDto = new PersonDetailsDto { Id = 1, FirstName = "John", LastName = "Doe" };
            var personEntity = new PersonEntity { Id = 1, FirstName = "John", LastName = "Doe" };

            //_mockValidator.Setup(v => v.Validate(personDto))
            //    .Returns(new ValidationResult()); // No validation errors

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<PersonDetailsDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult()); // No validation errors

            _mockMapper.Setup(m => m.Map<PersonEntity>(personDto)).Returns(personEntity);

            // Act
            await _personService.AddPersonAsync(personDto);

            // Assert
            _mockPersonRepository.Verify(repo => repo.AddPersonAsync(personEntity), Times.Once);
        }

        [Fact]
        public async Task AddPerson_Should_Throw_ValidationException_When_Invalid()
        {
            // Arrange
            var personDto = new PersonDetailsDto();
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("FirstName", "First Name is required"),
                new ValidationFailure("LastName", "Last Name is required")
            };
            var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);            

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<PersonDetailsDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>  _personService.AddPersonAsync(personDto));
            Assert.Equal(2, exception.Errors.Count);
            Assert.Equal("First Name is required", exception.Errors["FirstName"]);
            Assert.Equal("Last Name is required", exception.Errors["LastName"]);
        }

        [Fact]
        public async Task UpdatePerson_Should_Update_When_Valid()
        {
            // Arrange
            var personDto = new PersonDetailsDto { Id = 1, FirstName = "John", LastName = "Doe" };
            var personEntity = new PersonEntity { Id = 1, FirstName = "John", LastName = "Doe" };

            _mockPersonRepository.Setup(repo => repo.GetPersonAsync(personDto.Id)).ReturnsAsync(personEntity);
            //_mockValidator.Setup(v => v.Validate(personDto)).Returns(new ValidationResult()); // No validation errors

            _mockValidator.Setup(v => v.ValidateAsync(personDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _mockMapper.Setup(m => m.Map(personDto, personEntity));

            // Act
            await _personService.UpdatePersonAsync(personDto);

            // Assert
            _mockPersonRepository.Verify(repo => repo.UpdatePersonAsync(personEntity), Times.Once);
        }

        [Fact]
        public async Task UpdatePerson_Should_Throw_NotFoundException_When_Person_Not_Found()
        {
            // Arrange
            var personDto = new PersonDetailsDto { Id = 1, FirstName = "John", LastName = "Doe" };

            _mockPersonRepository.Setup(repo => repo.GetPersonAsync(personDto.Id)).ReturnsAsync((PersonEntity)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<NotFoundException>(() => _personService.UpdatePersonAsync(personDto)).Result;
            Assert.Equal("Person not found", exception.Message);
        }

        [Fact]
        public async Task UpdatePerson_Should_Throw_ValidationException_When_Invalid()
        {
            // Arrange
            var personDto = new PersonDetailsDto { Id = 1, FirstName = "John", LastName = "Doe" };
            var personEntity = new PersonEntity { Id = 1, FirstName = "John", LastName = "Doe" };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("DateOfBirth", "Date of Birth is required")
            };
            var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);

            _mockPersonRepository.Setup(repo => repo.GetPersonAsync(personDto.Id)).ReturnsAsync(personEntity);
            //_mockValidator.Setup(v => v.Validate(personDto)).Returns(validationResult);

            _mockValidator
                .Setup(v => v.ValidateAsync(personDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            ValidationException exception = await Assert.ThrowsAsync<ValidationException>(() => _personService.UpdatePersonAsync(personDto));
            Assert.Single(exception.Errors);
            Assert.Equal("Date of Birth is required", exception.Errors["DateOfBirth"]);
        }
    }
}
