using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Users;

public class Login
{
	[Required(ErrorMessage = "Registration Number is required")]
	public string RegistrationNumber { get; set; } = string.Empty;

	[Required(ErrorMessage = "Password is required")]
	[MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
	public string Password { get; set; } = string.Empty;
}