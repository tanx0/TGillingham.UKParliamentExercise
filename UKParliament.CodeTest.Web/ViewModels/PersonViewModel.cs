namespace UKParliament.CodeTest.Web.ViewModels;

public class PersonViewModel 
{
    public int Id { get; set; }

    
    public string FirstName { get; set; }

    
    public string LastName { get; set; }


    public string DateOfBirth { get; set; }


    public int DepartmentId { get; set; }
}

public class DepartmentViewModel
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
}