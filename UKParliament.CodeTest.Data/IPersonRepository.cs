using UKParliament.CodeTest.Data;

public interface IPersonRepository
{    
    Task<IEnumerable<PersonEntity>> GetPeopleAsync();
    Task<PersonEntity> AddPersonAsync(PersonEntity person);
    Task<PersonEntity?> GetPersonAsync(int id);
    Task UpdatePersonAsync(PersonEntity person);
    Task<PersonEntity?> SearchForPersonAsync(string firstName, string lastName, DateTime dateOfBirth);
}
