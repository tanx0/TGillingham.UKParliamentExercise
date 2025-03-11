using Xunit;
using Moq;
using UKParliament.CodeTest.Services;
using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Services.Exceptions;
using System.Threading.Tasks;
using UKParliament.CodeTest.Services.Models;

namespace UKParliament.CodeTest.Tests.Services
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
        private readonly DepartmentService _departmentService;

        public DepartmentServiceTests()
        {
            _mockDepartmentRepository = new Mock<IDepartmentRepository>();
            _departmentService = new DepartmentService(_mockDepartmentRepository.Object);
        }

        [Fact]
        public async Task GetDepartments_Should_Return_DepartmentDtos_When_Departments_Exist()
        {
            // Arrange
            var departmentEntities = new List<DepartmentEntity>
            {
                new DepartmentEntity { Id = 1, Name = "HR" },
                new DepartmentEntity { Id = 2, Name = "IT" }
            };
            _mockDepartmentRepository.Setup(repo => repo.GetDepartmentsAsync())
                .ReturnsAsync(departmentEntities);

            // Act
            var result = await _departmentService.GetDepartmentsAsync();
            List<DepartmentDto> resultList = result.ToList();

            // Assert
            Assert.Equal(2, resultList.Count);            
            Assert.Equal(1, resultList[0].Id);            
            Assert.Equal("HR", resultList[0].DepartmentName);
            Assert.Equal(2, resultList[1].Id);            
            Assert.Equal("IT", resultList[1].DepartmentName);
            
        }

        [Fact]
        public async Task GetDepartments_Should_Throw_NotFoundException_When_No_Departments_Exist()
        {
            // Arrange
            _mockDepartmentRepository.Setup(repo => repo.GetDepartmentsAsync())
                .ReturnsAsync(new List<DepartmentEntity>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(_departmentService.GetDepartmentsAsync);
            Assert.Equal("No departments found", exception.Message);
        }
    }
}
