using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Services.Models;
using Xunit;
using AutoMapper;
using UKParliament.CodeTest.Services.Mappers;

namespace UKParliament.CodeTest.Tests
{
    public class AutoMapperCollectionTests
    {
        private readonly IMapper _mapper;

        public AutoMapperCollectionTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PersonMappingProfile>();
            });

            config.AssertConfigurationIsValid();
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Map_PersonEntityCollection_To_PersonDetailsDtoCollection()
        {
            // Arrange
            var entities = new List<PersonEntity>
        {
            new() { Id = 1, FirstName = "Person1Name", LastName = "Person1LastName", DateOfBirth = new DateTime(1990, 1, 1), DepartmentId = 2 },
            new() { Id = 2, FirstName = "Person2Name", LastName = "Person2LastName", DateOfBirth = new DateTime(1985, 5, 20), DepartmentId = 3 }
        };

            // Act
            var dtos = _mapper.Map<IEnumerable<PersonDetailsDto>>(entities);

            // Assert
            Assert.NotNull(dtos);
            Assert.Equal(2, dtos.Count());

            var dtoList = dtos.ToList();
            Assert.Equal(1, dtoList[0].Id);
            Assert.Equal("Person1Name", dtoList[0].FirstName);
            Assert.Equal(2, dtoList[1].Id);
            Assert.Equal("Person2Name", dtoList[1].FirstName);
        }
        [Fact]
        public void PersonEntity_To_PersonDetailsDto_Should_Map_Correctly()
        {
            // Arrange
            var entity = new PersonEntity
            {
                Id = 1,
                FirstName = "Person1Name",
                LastName = "Person1LastName",
                DateOfBirth = new DateTime(1990, 1, 1),
                DepartmentId = 2
            };

            // Act
            var dto = _mapper.Map<PersonDetailsDto>(entity);

            // Assert
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.FirstName, dto.FirstName);
            Assert.Equal(entity.LastName, dto.LastName);
            Assert.Equal(entity.DateOfBirth, dto.DateOfBirth);
            Assert.Equal(entity.DepartmentId, dto.DepartmentId);
        }
    }
}
