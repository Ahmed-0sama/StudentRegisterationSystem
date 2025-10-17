using Shared.DTOs.Department;
using Shared.DTOs.Faculty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
	public interface IFacultyService
	{
		Task<List<FacultyDto>> GetAllFaculties();
		Task<FacultyDto> GetByIdAsync(int id);
		Task<FacultyDto> AddFacultyAsync(string name);
	}
}
