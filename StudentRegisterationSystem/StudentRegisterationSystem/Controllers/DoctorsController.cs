using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Courses;

namespace StudentRegisterationSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DoctorsController : ControllerBase
	{
		private readonly IDoctorService _doctorService;
		public DoctorsController(IDoctorService doctorService)
		{
			_doctorService = doctorService;
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetDoctorByIdAsync(Guid id)
		{
			var doctor = await _doctorService.GetDoctorByIdAsync(id);
			if (doctor == null)
			{
				return NotFound();
			}
			return Ok(doctor);
		}
		[HttpGet("GetAllDoctors")]
		public async Task<IActionResult> GetAllDoctorsAsync()
		{
			var doctors = await _doctorService.GetAllDoctorsAsync();
			return Ok(doctors);
		}
		[HttpGet("DoctorByCourse")]
		public async Task<IActionResult> GetDoctorByCourseAsync(Guid courseId)
		{
			var doctor = await _doctorService.GetDoctorsByCourseAsync(courseId);
			if (doctor == null)
			{
				return NotFound();
			}
			return Ok(doctor);
		}
		[HttpPost("AssignDoctor")]
		public async Task<IActionResult> AssignDoctorToCourse([FromBody] AssignDoctorDto dto)
		{
			if (dto == null)
				return BadRequest(new { success = false, message = "Invalid request data." });

			try
			{
				var result = await _doctorService.AssignDoctorToCourse(dto);
				if (result)
					return Ok(new { success = true, message = "Doctor assigned successfully." });

				return BadRequest(new { success = false, message = "Failed to assign doctor to course." });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}
	}
}
