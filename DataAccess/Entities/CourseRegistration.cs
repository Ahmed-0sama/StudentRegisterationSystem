using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class CourseRegistration
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		[ForeignKey("Student")]
		public Guid StudentId { get; set; }
		[ForeignKey("Course")]
		public Guid CourseId { get; set; }
		[StringLength(5)]
		public string? Grade { get; set; }
		public string? Semester { get; set; }
		[StringLength(20)]
		public string Status { get; set; } = "Active";
		[ForeignKey(nameof(RegistrationPeriod))]
		public Guid RegistrationPeriodId { get; set; }

		public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
		public RegistrationPeriod RegistrationPeriod { get; set; }

		public Student Student { get; set; }
		public Course Course { get; set; }
	}
}
