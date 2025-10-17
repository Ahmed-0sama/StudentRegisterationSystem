using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class StudentCourse
	{
		[ForeignKey("Student")]
		public Guid StudentId { get; set; }
		[ForeignKey("Course")]
		public Guid CourseId { get; set; }
		[StringLength(5)]
		public string? Grade { get; set; }
		public string? Semester { get; set; }

		public Student Student { get; set; }
		public Course Course { get; set; }
	}
}
