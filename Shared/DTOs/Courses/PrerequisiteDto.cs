using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Courses
{
	public class PrerequisiteDto
	{
			public Guid PrerequisiteCourseId { get; set; }
			public string PrerequisiteCourseName { get; set; }
			public string PrerequisiteCourseCode { get; set; }
		}
	}
