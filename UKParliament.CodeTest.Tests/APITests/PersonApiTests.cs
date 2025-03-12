using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Playwright;
using Xunit;

namespace UKParliament.CodeTest.Tests.APITests
{
    public class PersonApiTests : IAsyncLifetime
    {
        private IAPIRequestContext _request;
        private string _baseUrl;

        public async Task InitializeAsync()
        {            
            _baseUrl = TestConfig.BaseUrl;

            var playwright = await Playwright.CreateAsync();
            var apiRequest = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = _baseUrl,
                IgnoreHTTPSErrors = true
            });

            _request = apiRequest;
        }

        [Fact]
        public async Task GetPeople_Should_Return_ListOfPersons()
        {
            //Arrange
            //create 2 people in case if the database is empty
            CreatePersonWithLastName(Guid.NewGuid().ToString());
            CreatePersonWithLastName(Guid.NewGuid().ToString());
            // Act
            IAPIResponse response = await _request.GetAsync("api/person");

            // Assert
            Assert.Equal(200, response.Status);

            var persons = JsonSerializer.Deserialize<List<PersonDto>>(await response.TextAsync());
            Assert.NotNull(persons);
            Assert.True(persons.Count() >= 2, "At least 2 people should exist in the database");
        }

        [Fact]
        public async Task AddPerson_Should_Return_CreatedPerson()
        {
            // Arrange
            string generatedLastName = Guid.NewGuid().ToString();
            var newPerson = new PersonDto
            {
                FirstName = "Joe",
                LastName = generatedLastName,
                DateOfBirth = "1990-01-01",
                DepartmentId = 1
            };
            var jsonBody = JsonSerializer.Serialize(newPerson);
            // Act
            var response = await _request.PostAsync("api/person", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            },
                Data = jsonBody
            });

            // Assert
            Assert.Equal(201, response.Status);

            var createdPerson = JsonSerializer.Deserialize<PersonDto>(await response.TextAsync());
            Assert.NotNull(createdPerson);
            Assert.Equal("Joe", createdPerson.FirstName);
            Assert.Equal(generatedLastName, createdPerson.LastName);
        }

        [Fact]
        public async Task Put_Person_Should_Update_Existing_Person()
        {
            // Arrange
            string generatedLastName = Guid.NewGuid().ToString();
            var response = await CreatePersonWithLastName(generatedLastName);

            // Assert
            Assert.Equal(201, response.Status);
            PersonDto createdPerson = JsonSerializer.Deserialize<PersonDto>(await response.TextAsync());
            Assert.NotNull(createdPerson);
            Assert.Equal(generatedLastName, createdPerson.LastName);

            //Update created person
            createdPerson.FirstName = "John";

            await UpdatePerson(createdPerson);
            
            // Assert

            PersonDto getPerson = await GetPerson(createdPerson.Id);            

            Assert.Equal("John", getPerson.FirstName);

        }

        private async Task<PersonDto> GetPerson(int id)
        {
            var response = await _request.GetAsync($"api/person/{id}");
            Assert.Equal(200, response.Status);

            var person = JsonSerializer.Deserialize<PersonDto>(await response.TextAsync());
            Assert.NotNull(person);
            return person;

        }

        private async Task UpdatePerson(PersonDto createdPerson)
        {
            var jsonBody = JsonSerializer.Serialize(createdPerson);
            //return await _request.PutAsync($"/person/{createdPerson.Id}", new APIRequestContextOptions
            var response = await _request.PutAsync($"api/person", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                Data = jsonBody
            });
            Assert.True(response.Ok);

            Assert.Equal(204, response.Status);
        }

 
        private async Task<IAPIResponse> CreatePersonWithLastName(string lastName)
        {
            var newPerson = new
            {
                FirstName = "Joe",
                LastName = lastName,
                DateOfBirth = "1990-01-01",
                DepartmentId = 1
            };
            var jsonBody = JsonSerializer.Serialize(newPerson);
            return await _request.PostAsync("api/person", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            },
                Data = jsonBody
            });
        }

        public async Task DisposeAsync()
        {
            await _request.DisposeAsync();
        }
    }

    internal class PersonDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public string DateOfBirth { get; set; }

        [JsonPropertyName("departmentId")]
        public int DepartmentId { get; set; }
    }

}
