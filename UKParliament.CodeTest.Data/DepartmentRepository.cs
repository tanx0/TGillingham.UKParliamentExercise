using Microsoft.EntityFrameworkCore;

namespace UKParliament.CodeTest.Data;

public class DepartmentRepository(PersonManagerContext ctx) : IDepartmentRepository
{
    public async Task<IEnumerable<DepartmentEntity>> GetDepartmentsAsync()
    {
        return await ctx.Departments.ToListAsync();
    }

    public async Task<bool> DepartmentExistsAsync(int departmentId)
    {
        return await ctx.Departments.AnyAsync(d => d.Id == departmentId);
    }
}
