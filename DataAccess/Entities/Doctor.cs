using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class Doctor
	{
		[Key]
		public Guid Id { get; set; }=Guid.NewGuid();
		//application user foreign key
		[ForeignKey(nameof(User))]
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		public ICollection<DoctorCourse> DoctorCourses { get; set; }
	}
}
