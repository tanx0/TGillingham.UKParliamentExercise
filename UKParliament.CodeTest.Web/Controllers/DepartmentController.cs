using Microsoft.AspNetCore.Mvc;
using UKParliament.CodeTest.Services;
using UKParliament.CodeTest.Services.Exceptions;
using UKParliament.CodeTest.Services.Models;
using UKParliament.CodeTest.Web.ViewModels;

namespace UKParliament.CodeTest.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController(IDepartmentService departmentService) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DepartmentViewModel>> GetDepartmentsAsync()
    {
        try
        {
            IEnumerable<DepartmentDto> departments = await departmentService.GetDepartmentsAsync();
            return Ok(departments.Select(d => new DepartmentViewModel()
            {
                DepartmentId = d.Id,
                DepartmentName = d.DepartmentName
            }));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch
        {
            return StatusCode(500);
        }
        
    }
}