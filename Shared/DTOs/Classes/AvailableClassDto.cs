using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Classes
{
	public class AvailableClassDto
	{
		public Guid ClassId { get; set; }
		public Guid CourseId { get; set; }
		public string CourseName { get; set; }
		public string ClassCode { get; set; }
		public string DoctorName { get; set; }
		public string Location { get; set; }
		public int MaxCapacity { get; set; }
		public int AvailableSeats { get; set; }
		public List<ClassScheduleDto> Schedules { get; set; }
	}
}
