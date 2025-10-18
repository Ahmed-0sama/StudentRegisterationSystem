using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Courses
{
	public class AddPrerequisiteDto
	{
			public Guid CourseId { get; set; }
			public Guid PrerequisiteCourseId { get; set; }
	}
}
