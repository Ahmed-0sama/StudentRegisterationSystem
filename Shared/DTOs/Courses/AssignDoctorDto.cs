using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Courses
{
	public class AssignDoctorDto
	{
		public Guid CourseId { get; set; }
		public Guid DoctorId { get; set; }
		public string AcademicYear { get; set; }
	}
}
