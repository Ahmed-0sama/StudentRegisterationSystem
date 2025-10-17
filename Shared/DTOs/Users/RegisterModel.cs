using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Users
{
	public class RegisterModel
	{
		[Required]
		public string FirstName { get; set; } = string.Empty;
		
		[Required]
		public string LastName { get; set; } = string.Empty;
		
		[Required]
		public string UserName { get; set; } = string.Empty;
		
		[Required]
		[MinLength(6)]
		public string Password { get; set; } = string.Empty;
		[Required]
		public Guid DepartmentId { get; set; }
		public string SponserType { get; set; }
	}
}
