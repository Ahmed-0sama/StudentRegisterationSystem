using Shared.DTOs.Courses;
using Shared.DTOs.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
	public interface IDoctorService
	{
		Task<List<DoctorDto>> GetAllDoctorsAsync();
		Task<DoctorDto?> GetDoctorByIdAsync(Guid id);
		Task<List<DoctorDto>> GetDoctorsByCourseAsync(Guid courseId);
		Task<bool> AssignDoctorToCourse(AssignDoctorDto dto);
		Task<bool> RemoveDoctorAsync(Guid courseId, Guid doctorId);
	}
}
