using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class Faculty
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required, StringLength(100)]
		public string Name { get; set; }
		public ICollection<Department> Departments { get; set; }
	}
}
