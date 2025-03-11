namespace UKParliament.CodeTest.Data;

public interface IDepartmentRepositorySync
{
    public IEnumerable<DepartmentEntity> GetDepartments();
}

public interface IDepartmentRepository
{
    public Task<IEnumerable<DepartmentEntity>> GetDepartmentsAsync();

    Task<bool> DepartmentExistsAsync(int departmentId);
}
