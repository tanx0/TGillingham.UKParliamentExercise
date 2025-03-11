using Microsoft.EntityFrameworkCore.Internal;

namespace UKParliament.CodeTest.Data;

public class DepartmentEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    // Navigation property people in the department
    public ICollection<PersonEntity> People { get; set; }
}
