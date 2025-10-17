using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Department;

namespace StudentRegisterationSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DepartmentsController : ControllerBase
	{
		private readonly IDepartmentService _departmentService;
		public DepartmentsController(IDepartmentService departmentService)
		{
			_departmentService = departmentService;
		}
		[HttpGet("GetAllDepartments")]
		public async Task<IActionResult> GetAllDepartments()
		{
			var departments = await _departmentService.GetAllAsync();
			return Ok(departments);
		}
		[HttpGet("GetDepartmentsByFaculty/{facultyId}")]
		public async Task<IActionResult> GetDepartmentsByFaculty(Guid facultyId)
		{
			var departments = await _departmentService.GetByFacultyIdAsync(facultyId);

			if (departments == null || !departments.Any())
			{
				return NotFound(new { Message = "No departments found for the specified faculty." });
			}
			return Ok(departments);
		}
		[HttpPost("AddDepartment")]
		public async Task<IActionResult> AddDepartment([FromBody] AddDepartmentDto dto)
		{
			if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
			{
				return BadRequest(new { Message = "Department name cannot be empty." });
			}

			var result = await _departmentService.CreateAsync(dto);

			if (result == null)
			{
				return BadRequest(new { Message = "Failed to add department. Please check the faculty ID and try again." });
			}

			return Ok(result);
		}

	}
}
