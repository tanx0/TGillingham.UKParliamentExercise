namespace UKParliament.CodeTest.Data;

public class PersonEntity
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime DateOfBirth { get; set; }

    // Foreign Key for Department
    public int DepartmentId { get; set; }
    public DepartmentEntity Department { get; set; }
}