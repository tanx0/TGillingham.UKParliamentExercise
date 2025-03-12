using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Services.Exceptions;
using UKParliament.CodeTest.Services.Models;
namespace UKParliament.CodeTest.Services;

public class DepartmentService(IDepartmentRepository departmentRepository) : IDepartmentService
{
    public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync()
    {
        IEnumerable<DepartmentEntity> departmentEntities = await departmentRepository.GetDepartmentsAsync();

        if(!departmentEntities.Any())
        {
            throw new NotFoundException("No departments found");
        }
        return departmentEntities.Select(departmentEntity => new DepartmentDto
        {
            Id = departmentEntity.Id,
            DepartmentName = departmentEntity.Name
        });
    }
}