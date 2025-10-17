using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Registeration
{
	public class RegisteredCourseDto
	{
		public Guid RegistrationId { get; set; }
		public Guid CourseId { get; set; }
		public string CourseName { get; set; }
		public string CourseCode { get; set; }
		public int CreditHours { get; set; }
		public string Status { get; set; }
		public string PeriodSemester { get; set; }
	}
}
