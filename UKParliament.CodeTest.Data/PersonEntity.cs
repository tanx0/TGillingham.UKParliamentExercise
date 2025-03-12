using System.ComponentModel.DataAnnotations;

namespace UKParliament.CodeTest.Data;

public class PersonEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public DateTime? DateOfBirth { get; set; }

    // Foreign Key for Department
    [Required]
    public int? DepartmentId { get; set; }
    public DepartmentEntity Department { get; set; }
}