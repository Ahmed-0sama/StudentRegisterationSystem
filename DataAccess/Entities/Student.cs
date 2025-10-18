using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class Student
	{
		[Key]
		public Guid Id { get; set; }=Guid.NewGuid();
		[Required, StringLength(100)]
		public string FName { get; set; }
		public string LName { get; set; }
		[EmailAddress, StringLength(100)]
		public double GPA { get; set; }
		public int creditsEarned { get; set; }
		public DateTime DateOfBirth { get; set; }
		//application user foreign key
		[ForeignKey(nameof(User))]
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		[StringLength(200)]
		public string SponserType { get; set; }
		[ForeignKey(nameof(Department))]
		public Guid DepartmentId { get; set; }
		public Department Department { get; set; }
		//need to remove
		public ICollection<CourseRegistration> CourseRegistrations { get; set; }
		public ICollection<ClassRegistration> ClassRegistrations { get; set; }

	}

}
