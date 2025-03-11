
using Microsoft.EntityFrameworkCore;
using UKParliament.CodeTest.Data;
using Xunit;


public class PersonRepositoryTests : IDisposable
{
    private readonly PersonManagerContext _context;
    private readonly PersonRepository _repository;

    public PersonRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<PersonManagerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        _context = new PersonManagerContext(options);
        _repository = new PersonRepository(_context);

        SetUpDb();
    }

    private void SetUpDb()
    {
        _context.People.AddRange(
            new PersonEntity { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1) },
            new PersonEntity { Id = 2, FirstName = "Jane", LastName = "Smith", DateOfBirth = new DateTime(1985, 5, 20) }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetPeople_ShouldReturnAllPeople()
    {
        // Act
        var people = (await _repository.GetPeopleAsync()).ToList();

        // Assert
        Assert.Equal(2, people.Count);
    }

    [Fact]
    public async Task GetPerson_ShouldReturnCorrectPerson_WhenExists()
    {
        // Act
        var person = await _repository.GetPersonAsync(1);

        // Assert
        Assert.NotNull(person);
        Assert.Equal("John", person.FirstName);
        Assert.Equal("Doe", person.LastName);
    }

    [Fact]
    public async Task GetPerson_ShouldReturnNull_WhenPersonDoesNotExist()
    {
        // Act
        var person = await _repository.GetPersonAsync(99); // non-existent Id

        // Assert
        Assert.Null(person);
    }

    [Fact]
    public async Task AddPerson_ShouldAddPersonSuccessfully()
    {
        // Arrange
        var newPerson = new PersonEntity
        {
            Id = 3,
            FirstName = "Alice",
            LastName = "Brown",
            DateOfBirth = new DateTime(1992, 7, 15)
        };

        // Act
        await _repository.AddPersonAsync(newPerson);
        var person = await _repository.GetPersonAsync(3);

        // Assert
        Assert.NotNull(person);
        Assert.Equal("Alice", person.FirstName);
    }

    [Fact]
    public async Task UpdatePerson_ShouldUpdateExistingPerson()
    {
        // Arrange
        var person = await _repository.GetPersonAsync(1);
        person.LastName = "UpdatedLastName";

        // Act
        await _repository.UpdatePersonAsync(person);
        var updatedPerson = await _repository.GetPersonAsync(1);

        // Assert
        Assert.NotNull(updatedPerson);
        Assert.Equal("UpdatedLastName", updatedPerson.LastName);
    }

    [Fact]
    public async Task SearchForPerson_ShouldReturnPerson_WhenExists()
    {
        // Act
        var person = await _repository.SearchForPersonAsync("Jane", "Smith", new DateTime(1985, 5, 20));

        // Assert
        Assert.NotNull(person);
        Assert.Equal(2, person.Id);
    }

    [Fact]
    public async Task SearchForPerson_ShouldReturnNull_WhenPersonDoesNotExist()
    {
        // Act
        var person = await _repository.SearchForPersonAsync("NonExistent", "Person", DateTime.Today);

        // Assert
        Assert.Null(person);
    }

    public void Dispose()
    {
        _context.Dispose(); // Cleanup after each test
    }
}
