using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Shared.DTOs.Registeration;

namespace StudentRegisterationSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RegisterationMangementController : ControllerBase
	{
		private readonly IRegistrationService _registrationService;
		public RegisterationMangementController(IRegistrationService registrationService)
		{
			_registrationService = registrationService;
		}
		[HttpPost("CreateRegistrationPeriod")]
		public async Task<IActionResult> CreateRegistrationPeriod([FromBody] CreateRegisterationPeriodDto period)
		{
			try
			{
				var createdPeriod = await _registrationService.CreateRegistrationPeriodAsync(period);
				return Ok(createdPeriod);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception)
			{
				return StatusCode(500, new { message = "An error occurred while creating the registration period." });
			}
		}
		[HttpPut("periods/{id}/toggle")]
		public async Task<IActionResult> TogglePeriodStatus(Guid id)
		{

			var period = await _registrationService.GetRegisterationPeriodByIdAsync(id);
			if (period == null)
				return NotFound(new { message = "Registration period not found." });

			if (!period.IsActive)
			{
				if (period.EndDate <= DateTime.UtcNow)
					return BadRequest(new { message = "Cannot activate a period that has already ended." });

				var activeExists = await _registrationService.IsRegistrationOpenAsync();
				if (activeExists)
					return BadRequest(new { message = "Cannot activate this period because another period is already active." });
			}

			period.IsActive = !period.IsActive;

			var updated = await _registrationService.UpdateRegistertrationPeriodAsync(period);
			if (!updated)
				return StatusCode(500, new { message = "Failed to update registration period." });

			return Ok(new
			{
				message = "Registration period status updated successfully.",
				period = new
				{
					period.Id,
					period.SemesterName,
					period.StartDate,
					period.EndDate,
					period.IsActive
				}
			});
		}
		[HttpGet("periods/{id}")]
		public async Task<IActionResult> GetPeriodById(Guid id)
		{
			var period = await _registrationService.GetRegisterationPeriodByIdAsync(id);
			if (period == null)
				return NotFound();
			return Ok(period);
		}
		[HttpGet("GetActive")]
		public async Task<IActionResult> GetActiveRegistrationPeriod()
		{
			var period = await _registrationService.GetActiveRegisterationPeriodAsync();
			if (period == null)
				return NotFound(new { message = "No active registration period found." });
			return Ok(period);
		}
		[HttpPut("UpdatePeriod/{id}")]
		public async Task<IActionResult> UpdatePeriod(Guid id, [FromBody] RegisterationPeriodDto dto)
		{
			if (id != dto.Id)
				return BadRequest(new { message = "ID mismatch." });

			try
			{
				var updated = await _registrationService.UpdateRegistertrationPeriodAsync(dto);
				if (!updated)
					return NotFound(new { message = "Registration period not found." });

				return Ok(new { message = "Registration period updated successfully.", period = dto });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
		[HttpGet("periods")]
		public async Task<IActionResult> GetAllPeriods()
		{
			var periods = await _registrationService.GetAllRegisterationPeriodsAsync();
			return Ok(periods);
		}
	}
}
