using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Courses
{
	public class CreateCourseDto
	{
		public string CourseName { get; set; }
		public string CourseCode { get; set; }
		public int CreditHours { get; set; }
		public Guid DepartmentId { get; set; }
	}
}
