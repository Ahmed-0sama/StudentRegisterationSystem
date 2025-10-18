using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Courses;
using System.Security.Claims;

namespace StudentRegisterationSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CourseController : ControllerBase
	{
		private readonly ICourseService _courseService;
		public CourseController(ICourseService courseService)
		{
			_courseService = courseService;
		}
		[Authorize]
		[HttpGet("RegisteredCourses")]
		public async Task<IActionResult> GetRegisteredCoursesAsync()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId))
				return Unauthorized(new { success = false, message = "Unable to identify student." });

			var courses = await _courseService.GetRegisteredCourses(userId);
			return Ok(new { success = true, data = courses });
		}

		[HttpGet("GetAllCourses")]
		public async Task<IActionResult> GetAllCoursesAsync()
		{
			try
			{
				var courses = await _courseService.GetAllCourses();
				return Ok(courses);
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}
	
		[HttpGet("GetCourseByIdAsync{courseId}")]
		public async Task<IActionResult> GetCourseByIdAsync(Guid courseId)
		{
			if (courseId == Guid.Empty)
				return BadRequest(new { success = false, message = "Invalid course ID" });

			try
			{
				var course = await _courseService.GetCourseById(courseId);
				if (course == null)
					return NotFound(new { success = false, message = "Course not found" });

				return Ok(course);
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}

		[HttpGet("AvailableCourses")]
		public async Task<IActionResult> GetAvailableCoursesForStudentAsync()
		{
			try
			{
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				if (string.IsNullOrEmpty(userIdClaim))
					return Unauthorized(new { success = false, message = "Invalid or missing user ID in token." });

				// Pass string directly to service
				var courses = await _courseService.GetAvailableCoursesForStudent(userIdClaim);

				return Ok(new { success = true, data = courses });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}

		[HttpPost("RegisterCourse")]
		public async Task<IActionResult> RegisterCourseAsync([FromBody] RegisterCourseDto dto)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId))
				return Unauthorized(new { success = false, message = "Unable to identify student from token." });

			if (dto == null)
				return BadRequest(new { success = false, message = "Course registration data is required" });

			try
			{
				var result = await _courseService.RegisterCourse(userId, dto);
				return Ok(new { success = result, message = "Course registered successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}

			[HttpPost("CreateCourse")]
		public async Task<IActionResult> CreateCourseAsync([FromBody] CreateCourseDto dto)
		{
			if (dto == null)
				return BadRequest(new { success = false, message = "Course data is required" });

			try
			{
				var createdCourse = await _courseService.CreateCourse(dto);
				return Ok(new { success = true, data = createdCourse, message = "Course created successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}
		[HttpGet("GetCoursesByFaculty/{department}")]
		public async Task<IActionResult> GetCoursesByFaculty(Guid department)
		{
			var courses = await _courseService.GetCoursesByDepartment(department);
			return Ok(new { success = true, data = courses });
		}
		[HttpPost("AddPrerequisite")]
		public async Task<IActionResult> AddPrerequisite([FromBody] AddPrerequisiteDto dto)
		{
			if (dto == null)
				return BadRequest(new { success = false, message = "Prerequisite data is required" });

			try
			{
				await _courseService.AddPrerequisite(dto);
				return Ok(new { success = true, message = "Prerequisite added successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}

		[HttpDelete("RemovePrerequisite")]
		public async Task<IActionResult> RemovePrerequisite([FromQuery] Guid courseId, [FromQuery] Guid prerequisiteCourseId)
		{
			try
			{
				await _courseService.RemovePrerequisite(courseId, prerequisiteCourseId);
				return Ok(new { success = true, message = "Prerequisite removed successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}
		[HttpGet("CompletedCourses")]
		[Authorize]
		public async Task<IActionResult> GetCompletedCourses()
		{
			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(userId))
					return Unauthorized(new { success = false, message = "User ID not found" });

				var courses = await _courseService.GetCompletedCoursesForStudent(userId);
				return Ok(new { success = true, data = courses });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}
	}
}
