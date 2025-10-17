using Shared.DTOs.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
	public interface IDepartmentService
	{
		Task<List<GetDepartmentDto>> GetAllAsync();
		Task<GetDepartmentDto> GetByIdAsync(int id);
		Task<GetDepartmentDto> CreateAsync(AddDepartmentDto dto);
		Task<List<GetDepartmentDto>> GetByFacultyIdAsync(Guid facultyId);
	}
}
