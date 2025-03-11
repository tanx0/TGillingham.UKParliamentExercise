using AutoMapper;
using FluentValidation;
using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Services.Exceptions;
using UKParliament.CodeTest.Services.Models;
using UKParliament.CodeTest.Services;
using ValidationException = UKParliament.CodeTest.Services.Exceptions.ValidationException;
public class PersonService(
    IPersonRepository personRepository,
    IValidator<PersonDetailsDto> validator,
    IMapper mapper) : IPersonService
{

    public async Task<IEnumerable<PersonDetailsDto>> GetPeopleAsync()
    {
        IEnumerable<PersonEntity> peopleEntities = await personRepository.GetPeopleAsync();
        return mapper.Map<IEnumerable<PersonDetailsDto>>(peopleEntities);
    }

    public async Task<PersonDetailsDto> GetPersonByIdAsync(int id)
    {
        var personEntity = await personRepository.GetPersonAsync(id);
        if (personEntity != null)
            return mapper.Map<PersonDetailsDto>(personEntity);
        else
            throw new NotFoundException("Person not found");
    }

    public async Task UpdatePersonAsync(PersonDetailsDto personDto)
    {
        var personEntity = await personRepository.GetPersonAsync(personDto.Id) ?? throw new NotFoundException("Person not found");
        var validationResult = await validator.ValidateAsync(personDto);

        if (validationResult.IsValid)
        {
            mapper.Map(personDto, personEntity);
            await personRepository.UpdatePersonAsync(personEntity);
        }
        else
        {
            var errors = validationResult.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
            throw new ValidationException(errors);
        }
    }

    public async Task AddPersonAsync(PersonDetailsDto personDto)
    {
        var validationResult = await validator.ValidateAsync(personDto);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
            throw new ValidationException(errors);
        }

        var personEntity = mapper.Map<PersonEntity>(personDto);
        await personRepository.AddPersonAsync(personEntity);
    }


}
