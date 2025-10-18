using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Classes
{
	public class ClassDto
	{
		public Guid ClassId { get; set; }
		public Guid CourseId { get; set; }
		public string CourseName { get; set; } = string.Empty;
		public string ClassCode { get; set; } = string.Empty;
		public string DoctorName { get; set; } = string.Empty;
		public int MaxCapacity { get; set; }
		public string Status { get; set; } = string.Empty;
	}
}
