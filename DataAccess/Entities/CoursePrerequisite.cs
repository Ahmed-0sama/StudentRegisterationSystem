using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class CoursePrerequisite
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[ForeignKey(nameof(Course))]
		public Guid CourseId { get; set; }
		public Course Course { get; set; }

		[ForeignKey(nameof(PrerequisiteCourse))]
		public Guid PrerequisiteCourseId { get; set; }
		public Course PrerequisiteCourse { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
