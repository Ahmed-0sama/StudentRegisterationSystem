using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class DoctorCourse
	{
		[ForeignKey("Doctor")]
		public Guid DoctorId { get; set; }
		[ForeignKey("Course")]
		public Guid CourseId { get; set; }

		public string? AcademicYear { get; set; }

		public Doctor Doctor { get; set; }
		public Course Course { get; set; }
	}
}
