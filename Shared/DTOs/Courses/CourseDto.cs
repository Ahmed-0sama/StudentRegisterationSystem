using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Courses
{
	public class CourseDto
	{
		public Guid id { get; set; }
		public string CourseName { get; set; }
		public string CourseCode { get; set; }
		public string grade { get; set; }
		public int CreditHours { get; set; }
		public Guid DepartmentId { get; set; }
		public string DepartmentName { get; set; }
		public List<string> DoctorsNames { get; set; }
		public List<PrerequisiteDto> Prerequisites { get; set; } = new();

	}
}
