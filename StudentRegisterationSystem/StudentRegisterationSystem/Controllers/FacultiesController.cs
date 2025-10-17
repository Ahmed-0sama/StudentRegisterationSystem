using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StudentRegisterationSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FacultiesController : ControllerBase
	{
		private readonly IFacultyService _facultyService;
		public FacultiesController(IFacultyService facultyService)
		{
			_facultyService = facultyService;
		}
		//[Authorize]
		[HttpGet("GetAllFaculties")]
		public async Task<IActionResult> GetAllFaculties()
		{
			var faculties = await _facultyService.GetAllFaculties();
			return Ok(faculties);
		}
		[Authorize]
		[HttpGet("GetById/{id}")]
		public async Task<IActionResult> GetFacultyById(int id)
		{
			var faculty = await _facultyService.GetByIdAsync(id);
			if (faculty == null)
			{
				return NotFound("Faculty not found.");
			}
			return Ok(faculty);
		}
		[Authorize]
		[HttpPost("AddFaculty")]
		public async Task<IActionResult> AddFaculty([FromBody] string facultyName)
		{
			if (string.IsNullOrWhiteSpace(facultyName))
			{
				return BadRequest("Faculty name cannot be empty.");
			}
			var result = await _facultyService.AddFacultyAsync(facultyName);

			if (result == null)
			{
				return StatusCode(500, "An error occurred while adding the faculty.");
			}
			return Ok(result);
		}
	}
}
