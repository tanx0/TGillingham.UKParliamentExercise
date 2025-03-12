namespace UKParliament.CodeTest.Services;
using UKParliament.CodeTest.Services.Models;

public interface IPersonService
{
    public Task<PersonDetailsDto> GetPersonByIdAsync(int id);
    public Task<IEnumerable<PersonDetailsDto>> GetPeopleAsync();
    public Task<PersonDetailsDto> AddPersonAsync(PersonDetailsDto personDto);
    public Task UpdatePersonAsync(PersonDetailsDto person);
}
