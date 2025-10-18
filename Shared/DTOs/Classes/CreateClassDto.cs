using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Classes
{
	public class CreateClassDto
	{
		public Guid CourseId { get; set; }
		public Guid DoctorId { get; set; }
		public string ClassCode { get; set; }      // e.g., "A", "B", "Section 1"
		public int MaxCapacity { get; set; }
		public string Location { get; set; }
	}
}
