using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Users
{
	public class RegisterDoctorModel
	{
			[Required]
			public string UserName { get; set; }

			[Required]
			public string FirstName { get; set; }

			[Required]
			public string LastName { get; set; }

			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Compare("Password", ErrorMessage = "Passwords do not match.")]
			public string ConfirmPassword { get; set; }

	}
}
