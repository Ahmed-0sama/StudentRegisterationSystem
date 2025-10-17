using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Registeration
{
	public class RegisterationPeriodDto
	{
		public Guid Id { get; set; }
		public string SemesterName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool IsActive { get; set; }
		public int totalRegisterations { get; set; }
	}
}
