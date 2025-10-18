using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
	public interface IClassService
	{
		Task<ClassDto> CreateClass(CreateClassDto dto);
		Task<ClassScheduleDto> AddScheduleToClass(Guid classId, AddScheduleDto dto);

		Task<List<AvailableClassDto>> GetAvailableClassesForStudent(string userId);
		Task<bool> RegisterStudentToClass(string studentId, Guid classId, Guid registrationPeriodId);
		Task<List<RegisteredClassDto>> GetStudentRegisteredClasses(string studentId);
		Task<bool> DropClassRegistration(Guid registrationId, string studentId);
		Task<ClassDto> GetClassById(Guid classId);
		Task<List<ClassScheduleDto>> GetClassSchedules(Guid classId);
		Task<bool> RemoveScheduleFromClass(Guid scheduleId);
		Task<int> GetClassEnrollmentCount(Guid classId);
		Task<bool> HasScheduleConflict(string studentId, Guid classId);
		Task<List<ClassDto>> GetAllClasses();
	}
}
