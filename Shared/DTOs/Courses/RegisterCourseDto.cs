using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Courses
{
	public class RegisterCourseDto
	{
		public Guid StudentId { get; set; }
		public Guid RegisterationPeriodId { get; set; }
		public Guid CourseId { get; set; }
	}
}
