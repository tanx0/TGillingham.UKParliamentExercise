namespace UKParliament.CodeTest.Data;

public interface IDepartmentRepository
{
    public Task<IEnumerable<DepartmentEntity>> GetDepartmentsAsync();

    Task<bool> DepartmentExistsAsync(int departmentId);
}
