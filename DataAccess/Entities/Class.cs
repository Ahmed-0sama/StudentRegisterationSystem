using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class Class
	{
		[Key]
		public Guid Id { get; set; }=Guid.NewGuid();
		[ForeignKey("Course")]
		public Guid CourseId { get; set; }
		[ForeignKey("Doctor")]
		public Guid DoctorId { get; set; }
		public string ClassCode { get; set; }
		public int MaxCapacity { get; set; }
		public Course Course { get; set; }
		public Doctor Doctor { get; set; }
		public ICollection<ClassSchedule>Schedules { get; set; }
		public ICollection<ClassRegistration> StudentRegisterations { get; set; }
		public ICollection<CoursePrerequisite> Prerequisites { get; set; } = new List<CoursePrerequisite>();


	}
}
