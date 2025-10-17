using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Users;

namespace StudentRegisterationSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IAuthServices _authServices;
		public UserController(IAuthServices authServices)
		{
			_authServices = authServices;
		}
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var result = await _authServices.RegisterAsync(model);
			if (!string.IsNullOrEmpty(result.Message))
			{
				return BadRequest(result.Message);
			}
			return Ok(result);
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] Login model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _authServices.GetTokenAsync(model);

			// Fail only if token is null or empty
			if (string.IsNullOrEmpty(result.Token))
			{
				return BadRequest(result.Message ?? "Username or Password is incorrect!");
			}

			// Success
			return Ok(result);
		}
	}
}
