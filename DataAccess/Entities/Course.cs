using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class Course
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		[Required, StringLength(100)]
		public string CourseName { get; set; }
		public int CreditHours { get; set; }
		[ForeignKey("Department")]
		public Guid DepartmentId { get; set; }
		public Department Department { get; set; }
		public ICollection<StudentCourse> StudentCourses { get; set; }
		public ICollection<DoctorCourse> DoctorCourses { get; set; }
	}
}
