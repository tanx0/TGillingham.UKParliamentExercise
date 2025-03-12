using Microsoft.AspNetCore.Mvc;
using UKParliament.CodeTest.Services;
using UKParliament.CodeTest.Services.Exceptions;
using UKParliament.CodeTest.Services.Models;
using UKParliament.CodeTest.Web.Mappings;
using UKParliament.CodeTest.Web.ViewModels;

namespace UKParliament.CodeTest.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class PersonController(IPersonService personService) : ControllerBase
{
    
    [HttpGet]    
    public async Task<ActionResult<IEnumerable<PersonViewModel>>> GetPeopleAsync()
    {
        try
        {
            IEnumerable<PersonDetailsDto> people = await personService.GetPeopleAsync();

            return Ok(people.ToViewModel());                        
        }
        catch
        {
            return StatusCode(500);
        }
    }


    [Route("{id:int}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PersonViewModel>> GetByIdAsync(int id)
    {
        try
        {
            PersonDetailsDto person = await personService.GetPersonByIdAsync(id);
            
            return Ok(person.ToViewModel());
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch 
        {
            return StatusCode(500);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]    
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AddPersonAsync(PersonViewModel person)
    {
        try
        {
            var createdPerson = await personService.AddPersonAsync(person.ToDto());

            var createdPersonViewModel = createdPerson.ToViewModel(); 

            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdPerson.Id }, createdPersonViewModel);
        }
        catch (ValidationException e)
        {
            return BadRequest(new { e.Errors });
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]    
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdatePersonAsync(PersonViewModel person)
    {
        try
        {            
            await personService.UpdatePersonAsync(person.ToDto());
            return NoContent();            
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { ex.Errors } );
        }
        catch
        {
            return StatusCode(500);
        }

    }
}