using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class Department
	{
		[Key]
		public Guid Id { get; set; }=Guid.NewGuid();
		[Required,StringLength(100)]
		public string Name { get; set; }
		[ForeignKey("Faculty")]
		public Guid FacultyId { get; set; }
		public Faculty Faculty { get; set; }
		public ICollection<Course> Courses { get; set; }
		public ICollection<Student> Students { get; set; }
	}
}
