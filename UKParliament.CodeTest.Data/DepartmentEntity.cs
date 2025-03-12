using System.ComponentModel.DataAnnotations;

namespace UKParliament.CodeTest.Data;

public class DepartmentEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }

    // Navigation property people in the department
    public ICollection<PersonEntity> People { get; set; }
}
