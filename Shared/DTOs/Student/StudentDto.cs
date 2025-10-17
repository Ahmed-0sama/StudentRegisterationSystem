using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Student
{
	public class StudentDto
	{
		public string Id { get; set; }
		public string firstName { get; set; }
		public string lastName { get; set; }
		public string departmentName { get; set; }
		public string facultyName { get; set; }
		public string sponserType { get; set; }
		public double gpa { get; set; }
		public int creditsEarned { get; set; }
	}
}
