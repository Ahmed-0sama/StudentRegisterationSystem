using System;
using System.Collections.Generic;

namespace Shared.DTOs.Users
{
	public class AuthModel
	{
		public string? RegisterationNumber { get; set; }
		public List<string>? UserRoles { get; set; }
		public string? Token { get; set; }
		public string? Message { get; set; }
		public DateTime ExpireOn { get; set; }
	}
}
