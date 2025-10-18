using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Classes;

namespace StudentRegisterationSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ClassController : ControllerBase
	{
		private readonly IClassService _classService;
		public ClassController(IClassService classService)
		{
			_classService = classService;
		}
		[HttpPost("create")]
		public async Task<ActionResult<ClassDto>> CreateClass([FromBody] CreateClassDto dto)
		{
			try
			{
				if (dto == null)
					return BadRequest(new { message = "Invalid class data" });

				if (string.IsNullOrWhiteSpace(dto.ClassCode))
					return BadRequest(new { message = "Class code is required" });

				if (dto.MaxCapacity <= 0)
					return BadRequest(new { message = "Max capacity must be greater than 0" });

				if (dto.CourseId == Guid.Empty)
					return BadRequest(new { message = "Course is required" });

				if (dto.DoctorId == Guid.Empty)
					return BadRequest(new { message = "Doctor is required" });

				var result = await _classService.CreateClass(dto);
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error creating class: {ex.Message}");
				Console.WriteLine($"Stack trace: {ex.StackTrace}");

				return StatusCode(500, new
				{
					message = "Error creating class",
					detail = ex.Message
				});
			}
		}

		[HttpPost("{classId}/schedule")]
		[Authorize(Roles = "Admin,Doctor")]
		public async Task<ActionResult<ClassScheduleDto>> AddScheduleToClass(
			Guid classId,
			[FromBody] AddScheduleDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var result = await _classService.AddScheduleToClass(classId, dto);
				return CreatedAtAction(nameof(GetClassSchedules), new { classId = classId }, result);
			}
			catch (ArgumentException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error adding schedule" });
			}
		}

		[HttpGet("{classId}")]
		public async Task<ActionResult<ClassDto>> GetClassById(Guid classId)
		{
			try
			{
				var result = await _classService.GetClassById(classId);
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error retrieving class" });
			}
		}

		[HttpGet("{classId}/schedules")]
		public async Task<ActionResult<List<ClassScheduleDto>>> GetClassSchedules(Guid classId)
		{
			try
			{
				var result = await _classService.GetClassSchedules(classId);
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error retrieving schedules" });
			}
		}

		[HttpDelete("schedule/{scheduleId}")]
		public async Task<IActionResult> RemoveScheduleFromClass(Guid scheduleId)
		{
			try
			{
				var result = await _classService.RemoveScheduleFromClass(scheduleId);
				return Ok(new { message = "Schedule removed successfully" });
			}
			catch (ArgumentException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error removing schedule" });
			}
		}

		[HttpGet("{classId}/enrollment-count")]
		public async Task<ActionResult<int>> GetClassEnrollmentCount(Guid classId)
		{
			try
			{
				var result = await _classService.GetClassEnrollmentCount(classId);
				return Ok(new { enrollmentCount = result });
			}
			catch (ArgumentException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error retrieving enrollment count" });
			}
		}

		[HttpGet("available")]
		public async Task<ActionResult<List<AvailableClassDto>>> GetAvailableClassesForStudent()
		{
			try
			{
				var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

				if (string.IsNullOrEmpty(userId))
					return Unauthorized(new { message = "User ID not found in token" });

				var result = await _classService.GetAvailableClassesForStudent(userId);
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error retrieving available classes" });
			}
		}

		[HttpPost("register")]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> RegisterStudentToClass([FromBody] RegisterToClassDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

				if (string.IsNullOrEmpty(userId))
					return Unauthorized(new { message = "User ID not found in token" });

				var result = await _classService.RegisterStudentToClass(userId, dto.ClassId, dto.RegistrationPeriodId);
				return Ok(new { message = "Successfully registered for class" });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error registering for class" });
			}
		}

		[HttpGet("my-classes")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<List<RegisteredClassDto>>> GetStudentRegisteredClasses()
		{
			try
			{
				var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

				if (string.IsNullOrEmpty(userId))
					return Unauthorized(new { message = "User ID not found in token" });

				var result = await _classService.GetStudentRegisteredClasses(userId);
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error retrieving student classes" });
			}
		}

		[HttpPost("drop/{registrationId}")]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> DropClassRegistration(Guid registrationId)
		{
			try
			{
				var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

				if (string.IsNullOrEmpty(userId))
					return Unauthorized(new { message = "User ID not found in token" });

				var result = await _classService.DropClassRegistration(registrationId, userId);
				return Ok(new { message = "Class dropped successfully" });
			}
			catch (ArgumentException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error dropping class" });
			}
		}

		[HttpGet("check-conflict/{classId}")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<bool>> HasScheduleConflict(Guid classId)
		{
			try
			{
				var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

				if (string.IsNullOrEmpty(userId))
					return Unauthorized(new { message = "User ID not found in token" });

				var result = await _classService.HasScheduleConflict(userId, classId);
				return Ok(new { hasConflict = result });
			}
			catch (ArgumentException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error checking schedule conflict" });
			}
		}

		[HttpGet("all")]
		
		public async Task<ActionResult<List<ClassDto>>> GetAllClasses()
		{
			try
			{
				var result = await _classService.GetAllClasses();
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error retrieving classes" });
			}
		}
	}
}
	
