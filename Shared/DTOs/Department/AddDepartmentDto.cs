using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Department
{
	public class AddDepartmentDto
	{
		public string Name { get; set; }
		public Guid FacultyId { get; set; }
	}
}
