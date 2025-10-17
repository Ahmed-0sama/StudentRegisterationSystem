using Shared.DTOs.Courses;
using Shared.DTOs.Registeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
	public interface ICourseService
	{
		Task<List<CourseDto>> GetAvailableCoursesForStudent(string userId);
		Task<bool> RegisterCourse(string studentId, RegisterCourseDto dto);
		Task<List<RegisteredCourseDto>> GetRegisteredCourses(string studentId);
		//Task<bool> DropCourse(Guid studentId, Guid courseId);

		//admin operations
		Task<CourseDto> CreateCourse(CreateCourseDto dto);

		Task<bool> RemoveDoctorFromCourse(Guid courseId, Guid doctorId);
		Task<List<CourseDto>> GetAllCourses();
		Task<CourseDto> GetCourseById(Guid courseId);
		Task<List<CourseDto>> GetCoursesByDepartment(Guid DepartmentId);
	}
}
