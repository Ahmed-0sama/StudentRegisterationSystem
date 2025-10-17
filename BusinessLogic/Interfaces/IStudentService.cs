using Shared.DTOs.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
	public interface IStudentService
	{
		Task<StudentDto> GetSudentData(string userId);
	}
}
