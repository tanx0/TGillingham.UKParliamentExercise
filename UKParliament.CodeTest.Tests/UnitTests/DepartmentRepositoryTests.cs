
using Microsoft.EntityFrameworkCore;
using UKParliament.CodeTest.Data;
using Xunit;

namespace UKParliament.CodeTest.Tests.UnitTests
{
    public class DepartmentRepositoryTests : IDisposable
    {
        private readonly PersonManagerContext _context;
        private readonly DepartmentRepository _departmentRepo;

        public DepartmentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<PersonManagerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            _context = new PersonManagerContext(options);
            _departmentRepo = new DepartmentRepository(_context);

            SetUpDb();
        }

        private void SetUpDb()
        {
            _context.Departments.AddRange(
                new DepartmentEntity { Id = 1, Name = "HR" },
                new DepartmentEntity { Id = 2, Name = "IT", }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetPeople_ShouldReturnAllPeople()
        {
            var departments = (await _departmentRepo.GetDepartmentsAsync()).ToList();
            Assert.Equal(2, departments.Count);
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
