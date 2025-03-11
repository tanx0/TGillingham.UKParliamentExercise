namespace UKParliament.CodeTest.Services;

using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Services.Models;

public interface IPersonService
{
    public Task<PersonDetailsDto> GetPersonByIdAsync(int id);
    public Task<IEnumerable<PersonDetailsDto>> GetPeopleAsync();
    public Task AddPersonAsync(PersonDetailsDto person);
    public Task UpdatePersonAsync(PersonDetailsDto person);
}
