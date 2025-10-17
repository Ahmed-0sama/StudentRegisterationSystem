using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class RegistrationPeriod
	{
		[Key]
		public Guid Id { get; set; }=Guid.NewGuid();
		[Required,StringLength(100)]
		public string SemesterName { get; set; }
		[Required]
		public DateTime StartDate { get; set; }
		[Required]
		public DateTime EndDate { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public virtual ICollection<CourseRegistration> CourseRegistrations { get; set; }
	}
}
