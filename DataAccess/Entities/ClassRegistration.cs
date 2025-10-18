using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class ClassRegistration
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[ForeignKey("Student")]
		public Guid StudentId { get; set; }

		[ForeignKey("Class")]
		public Guid ClassId { get; set; }

		[StringLength(5)]
		public string? Grade { get; set; }

		// Status: Active, Dropped, Completed
		[StringLength(20)]
		public string Status { get; set; } = "Active";

		[ForeignKey("RegistrationPeriod")]
		public Guid RegistrationPeriodId { get; set; }

		public string? Semester { get; set; }

		public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

		public Student Student { get; set; }
		public Class Class { get; set; }
		public RegistrationPeriod RegistrationPeriod { get; set; }
	}
}
