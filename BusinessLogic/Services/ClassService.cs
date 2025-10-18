using BusinessLogic.Interfaces;
using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
	public class ClassService:IClassService
	{
		private readonly AppDbContext _context;
		private const int MAX_CREDIT_HOURS = 18;

		public ClassService(AppDbContext context)
		{
			_context = context;
		}
		public async Task<bool> RegisterStudentToClass(string studentId, Guid classId, Guid registrationPeriodId)
		{
			var student = await _context.Students
				.Include(s => s.ClassRegistrations)
					.ThenInclude(cr => cr.Class)
						.ThenInclude(c => c.Course)
				.FirstOrDefaultAsync(s => s.User.Id == studentId);

			if (student == null)
				throw new ArgumentException("Student not found");

			var classEntity = await _context.Classes
				.Include(c => c.StudentRegisterations)
				.Include(c => c.Course)
				.FirstOrDefaultAsync(c => c.Id == classId);

			if (classEntity == null)
				throw new ArgumentException("Class not found");

			// Validate student is from correct department
			if (classEntity.Course.DepartmentId != student.DepartmentId)
				throw new InvalidOperationException("Student cannot register for class from different department");

			// Check capacity
			if (classEntity.StudentRegisterations.Count >= classEntity.MaxCapacity)
				throw new InvalidOperationException("Class is at full capacity");

			// Check if already registered to this class
			var alreadyRegistered = student.ClassRegistrations
				.Any(cr => cr.ClassId == classId && cr.Status == "Active");

			if (alreadyRegistered)
				throw new InvalidOperationException("Student is already registered for this class");

			// Validate registration period
			var registrationPeriod = await _context.RegistrationPeriods
				.FirstOrDefaultAsync(rp => rp.Id == registrationPeriodId && rp.IsActive);

			if (registrationPeriod == null)
				throw new InvalidOperationException("Invalid or inactive registration period");

			// Check if student is already registered for the same course in a different class
			var alreadyRegisteredForCourse = student.ClassRegistrations
				.Any(cr => cr.Class.CourseId == classEntity.CourseId
					&& cr.Status == "Active"
					&& cr.Semester == registrationPeriod.SemesterName);

			if (alreadyRegisteredForCourse)
				throw new InvalidOperationException("You are already registered for this course in another class");

			// Check credit hour limit for the registration period
			var currentSemesterRegistrations = student.ClassRegistrations
				.Where(cr => cr.Status == "Active"
					&& cr.RegistrationPeriodId == registrationPeriodId
					&& cr.Semester == registrationPeriod.SemesterName)
				.ToList();

			var currentCreditHours = currentSemesterRegistrations
				.Sum(cr => cr.Class.Course.CreditHours);

			var newTotalCreditHours = currentCreditHours + classEntity.Course.CreditHours;

			if (newTotalCreditHours > MAX_CREDIT_HOURS)
			{
				throw new InvalidOperationException(
					$"Cannot register. This would exceed the maximum credit hour limit of {MAX_CREDIT_HOURS}. " +
					$"Current: {currentCreditHours} hours, Attempting to add: {classEntity.Course.CreditHours} hours, " +
					$"Total would be: {newTotalCreditHours} hours"
				);
			}

			// Check for schedule conflicts
			var hasConflict = await HasScheduleConflict(studentId, classId);
			if (hasConflict)
				throw new InvalidOperationException("Schedule conflict detected with existing class");

			var registration = new ClassRegistration
			{
				StudentId = student.Id,
				ClassId = classId,
				RegistrationPeriodId = registrationPeriodId,
				Semester = registrationPeriod.SemesterName,
				Status = "Active",
				RegisteredAt = DateTime.UtcNow
			};

			_context.ClassRegistrations.Add(registration);
			await _context.SaveChangesAsync();

			return true;
		}

		// NEW: Helper method to get current credit hours for a student
		public async Task<int> GetStudentCurrentCreditHours(string studentId, Guid registrationPeriodId)
		{
			var student = await _context.Students
				.Include(s => s.ClassRegistrations)
					.ThenInclude(cr => cr.Class)
						.ThenInclude(c => c.Course)
				.FirstOrDefaultAsync(s => s.User.Id == studentId);

			if (student == null)
				return 0;

			var registrationPeriod = await _context.RegistrationPeriods
				.FirstOrDefaultAsync(rp => rp.Id == registrationPeriodId);

			if (registrationPeriod == null)
				return 0;

			return student.ClassRegistrations
				.Where(cr => cr.Status == "Active"
					&& cr.RegistrationPeriodId == registrationPeriodId
					&& cr.Semester == registrationPeriod.SemesterName)
				.Sum(cr => cr.Class.Course.CreditHours);
		}

		public async Task<ClassDto> CreateClass(CreateClassDto dto)
		{
			var course = await _context.Courses
				.Include(c => c.Department)
				.FirstOrDefaultAsync(c => c.Id == dto.CourseId);

			if (course == null)
				throw new ArgumentException("Course not found");

			var doctor = await _context.Doctors
				.Include(d => d.User)
				.FirstOrDefaultAsync(d => d.Id == dto.DoctorId);

			if (doctor == null)
				throw new ArgumentException("Doctor not found");

			var doctorAssignment = await _context.DoctorCourses
				.FirstOrDefaultAsync(dc => dc.DoctorId == dto.DoctorId && dc.CourseId == dto.CourseId);

			if (doctorAssignment == null)
				throw new InvalidOperationException("Doctor is not assigned to this course");

			var existingClass = await _context.Classes
				.FirstOrDefaultAsync(c => c.CourseId == dto.CourseId && c.ClassCode == dto.ClassCode);

			if (existingClass != null)
				throw new InvalidOperationException($"Class with code '{dto.ClassCode}' already exists for this course");

			var classEntity = new Class
			{
				CourseId = dto.CourseId,
				DoctorId = dto.DoctorId,
				ClassCode = dto.ClassCode,
				MaxCapacity = dto.MaxCapacity
			};

			_context.Classes.Add(classEntity);
			await _context.SaveChangesAsync();

			return new ClassDto
			{
				ClassId = classEntity.Id,
				CourseId = classEntity.CourseId,
				CourseName = course.CourseName,
				ClassCode = classEntity.ClassCode,
				DoctorName = doctor.User.FirstName + " " + doctor.User.LastName,
				MaxCapacity = classEntity.MaxCapacity,
			};
		}

		public async Task<ClassScheduleDto> AddScheduleToClass(Guid classId, AddScheduleDto dto)
		{
			var classEntity = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId);

			if (classEntity == null)
				throw new ArgumentException("Class not found");

			if (dto.StartTime >= dto.EndTime)
				throw new InvalidOperationException("Start time must be before end time");

			var validDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
			if (!validDays.Contains(dto.DayOfWeek, StringComparer.OrdinalIgnoreCase))
				throw new InvalidOperationException($"Invalid day of week: {dto.DayOfWeek}");

			var existingSchedule = await _context.ClassSchedules
				.FirstOrDefaultAsync(cs => cs.ClassId == classId
					&& cs.DayOfWeek == dto.DayOfWeek
					&& cs.StartTime == dto.StartTime
					&& cs.EndTime == dto.EndTime);

			if (existingSchedule != null)
				throw new InvalidOperationException("This schedule already exists for this class");

			var schedule = new ClassSchedule
			{
				ClassId = classId,
				DayOfWeek = dto.DayOfWeek,
				StartTime = dto.StartTime,
				EndTime = dto.EndTime
			};

			_context.ClassSchedules.Add(schedule);
			await _context.SaveChangesAsync();

			return new ClassScheduleDto
			{
				ScheduleId = schedule.Id,
				DayOfWeek = schedule.DayOfWeek,
				StartTime = schedule.StartTime,
				EndTime = schedule.EndTime
			};
		}

		public async Task<List<AvailableClassDto>> GetAvailableClassesForStudent(string userId)
		{
			var student = await _context.Students
		.Include(s => s.Department)
		.Include(s => s.ClassRegistrations)
			.ThenInclude(cr => cr.Class)
		.Include(s => s.CourseRegistrations) // Add this
		.FirstOrDefaultAsync(s => s.UserId == userId);

			if (student == null)
				throw new ArgumentException("Student not found");

			if (student.Department == null)
				throw new InvalidOperationException("Student's department not found");

			var registeredClassIds = student.ClassRegistrations
				.Where(cr => cr.Status == "Active")
				.Select(cr => cr.ClassId)
				.ToList();

			var registeredCourseIds = student.ClassRegistrations
				.Where(cr => cr.Status == "Active")
				.Select(cr => cr.Class.CourseId)
				.ToList();

			// Get completed courses - any course where student HAS A GRADE
			var completedCourseIds = student.CourseRegistrations?
				.Where(cr => !string.IsNullOrEmpty(cr.Grade))
				.Select(cr => cr.CourseId)
				.ToList() ?? new List<Guid>();

			var availableClasses = await _context.Classes
				.Where(c => c.Course.DepartmentId == student.DepartmentId
					&& !registeredClassIds.Contains(c.Id)
					&& !registeredCourseIds.Contains(c.CourseId))
				.Include(c => c.Course)
					.ThenInclude(c => c.PrerequisiteFor)
					.ThenInclude(p => p.PrerequisiteCourse)
				.Include(c => c.Doctor)
					.ThenInclude(d => d.User)
				.Include(c => c.Schedules)
				.Include(c => c.StudentRegisterations)
				.ToListAsync();

			var result = availableClasses
				.Where(c => c.StudentRegisterations.Count < c.MaxCapacity)
				.Where(c => // Filter by prerequisites
				{
					if (c.Course.PrerequisiteFor == null || !c.Course.PrerequisiteFor.Any())
						return true;

					return c.Course.PrerequisiteFor.All(cp =>
						completedCourseIds.Contains(cp.PrerequisiteCourseId));
				})
				.Select(c => new AvailableClassDto
				{
					ClassId = c.Id,
					CourseId = c.CourseId,
					CourseName = c.Course.CourseName,
					ClassCode = c.ClassCode,
					DoctorName = c.Doctor.User.FirstName + " " + c.Doctor.User.LastName,
					MaxCapacity = c.MaxCapacity,
					AvailableSeats = c.MaxCapacity - c.StudentRegisterations.Count,
					Schedules = c.Schedules.Select(s => new ClassScheduleDto
					{
						ScheduleId = s.Id,
						DayOfWeek = s.DayOfWeek,
						StartTime = s.StartTime,
						EndTime = s.EndTime
					}).ToList()
				})
				.ToList();

			return result;
		}

		public async Task<List<RegisteredClassDto>> GetStudentRegisteredClasses(string studentId)
		{
			var student = await _context.Students
				.Include(s => s.ClassRegistrations)
					.ThenInclude(cr => cr.Class)
						.ThenInclude(c => c.Course)
				.Include(s => s.ClassRegistrations)
					.ThenInclude(cr => cr.Class)
						.ThenInclude(c => c.Doctor)
							.ThenInclude(d => d.User)
				.Include(s => s.ClassRegistrations)
					.ThenInclude(cr => cr.Class)
						.ThenInclude(c => c.Schedules)
				.FirstOrDefaultAsync(s => s.User.Id == studentId);

			if (student == null)
				throw new ArgumentException("Student not found");

			var registeredClasses = student.ClassRegistrations
				.Where(cr => cr.Status == "Active")
				.Select(cr => new RegisteredClassDto
				{
					RegistrationId = cr.Id,
					ClassId = cr.ClassId,
					CourseId = cr.Class.CourseId,
					CourseName = cr.Class.Course.CourseName,
					ClassCode = cr.Class.ClassCode,
					DoctorName = cr.Class.Doctor.User.FirstName + " " + cr.Class.Doctor.User.LastName,
					Grade = cr.Grade,
					Status = cr.Status,
					Semester = cr.Semester,
					Schedules = cr.Class.Schedules.Select(s => new ClassScheduleDto
					{
						ScheduleId = s.Id,
						DayOfWeek = s.DayOfWeek,
						StartTime = s.StartTime,
						EndTime = s.EndTime
					}).ToList()
				})
				.ToList();

			return registeredClasses;
		}

		public async Task<bool> DropClassRegistration(Guid registrationId, string studentId)
		{
			var registration = await _context.ClassRegistrations
				.Include(cr => cr.Student)
				.FirstOrDefaultAsync(cr => cr.Id == registrationId);

			if (registration == null)
				throw new ArgumentException("Registration not found");

			if (registration.Student.UserId != studentId)
				throw new UnauthorizedAccessException("Cannot drop another student's class registration");

			if (registration.Status == "Dropped")
				throw new InvalidOperationException("This class has already been dropped");

			registration.Status = "Dropped";
			_context.ClassRegistrations.Update(registration);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<ClassDto> GetClassById(Guid classId)
		{
			var classEntity = await _context.Classes
				.Include(c => c.Course)
				.Include(c => c.Doctor)
					.ThenInclude(d => d.User)
				.Include(c => c.Schedules)
				.Include(c => c.StudentRegisterations)
				.FirstOrDefaultAsync(c => c.Id == classId);

			if (classEntity == null)
				throw new ArgumentException("Class not found");

			return new ClassDto
			{
				ClassId = classEntity.Id,
				CourseId = classEntity.CourseId,
				CourseName = classEntity.Course.CourseName,
				ClassCode = classEntity.ClassCode,
				DoctorName = classEntity.Doctor.User.FirstName + " " + classEntity.Doctor.User.LastName,
				MaxCapacity = classEntity.MaxCapacity
			};
		}

		public async Task<List<ClassScheduleDto>> GetClassSchedules(Guid classId)
		{
			var classEntity = await _context.Classes
				.FirstOrDefaultAsync(c => c.Id == classId);

			if (classEntity == null)
				throw new ArgumentException("Class not found");

			var schedules = await _context.ClassSchedules
				.Where(cs => cs.ClassId == classId)
				.OrderBy(cs => GetDayOrder(cs.DayOfWeek))
				.ThenBy(cs => cs.StartTime)
				.Select(s => new ClassScheduleDto
				{
					ScheduleId = s.Id,
					DayOfWeek = s.DayOfWeek,
					StartTime = s.StartTime,
					EndTime = s.EndTime
				})
				.ToListAsync();

			return schedules;
		}

		public async Task<bool> RemoveScheduleFromClass(Guid scheduleId)
		{
			var schedule = await _context.ClassSchedules
				.FirstOrDefaultAsync(cs => cs.Id == scheduleId);

			if (schedule == null)
				throw new ArgumentException("Schedule not found");

			var classScheduleCount = await _context.ClassSchedules
				.CountAsync(cs => cs.ClassId == schedule.ClassId);

			if (classScheduleCount <= 1)
				throw new InvalidOperationException("Cannot remove the last schedule from a class. A class must have at least one schedule.");

			_context.ClassSchedules.Remove(schedule);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<int> GetClassEnrollmentCount(Guid classId)
		{
			var classEntity = await _context.Classes
				.FirstOrDefaultAsync(c => c.Id == classId);

			if (classEntity == null)
				throw new ArgumentException("Class not found");

			var enrollmentCount = await _context.ClassRegistrations
				.CountAsync(cr => cr.ClassId == classId && cr.Status == "Active");

			return enrollmentCount;
		}

		public async Task<bool> HasScheduleConflict(string studentId, Guid classId)
		{
			try
			{
				var classSchedules = await _context.ClassSchedules
					.Where(cs => cs.ClassId == classId)
					.ToListAsync();

				if (!classSchedules.Any())
					return false;

				var student = await _context.Students
					.Include(s => s.ClassRegistrations)
						.ThenInclude(cr => cr.Class)
							.ThenInclude(c => c.Schedules)
					.FirstOrDefaultAsync(s => s.User.Id == studentId);

				if (student == null)
					throw new ArgumentException("Student not found");

				var studentSchedules = student.ClassRegistrations
					.Where(cr => cr.Status == "Active")
					.SelectMany(cr => cr.Class.Schedules)
					.ToList();

				foreach (var newSchedule in classSchedules)
				{
					var conflict = studentSchedules.Any(s =>
						s.DayOfWeek == newSchedule.DayOfWeek &&
						SchedulesOverlap(s.StartTime, s.EndTime, newSchedule.StartTime, newSchedule.EndTime)
					);

					if (conflict)
						return true;
				}

				return false;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("Error checking schedule conflict: " + ex.Message);
			}
		}

		public async Task<List<ClassDto>> GetAllClasses()
		{
			try
			{
				var classes = await _context.Classes
					.Include(c => c.Course)
					.Include(c => c.Doctor)
					.ToListAsync();

				var classDtos = classes.Select(c => new ClassDto
				{
					ClassId = c.Id,
					CourseId = c.CourseId,
					CourseName = c.Course?.CourseName ?? string.Empty,
					ClassCode = c.ClassCode,
					DoctorName = c.Doctor?.FullName ?? string.Empty,
					MaxCapacity = c.MaxCapacity
				}).ToList();

				return classDtos;
			}
			catch (Exception ex)
			{
				throw new Exception("Error retrieving classes from database", ex);
			}
		}

		private bool SchedulesOverlap(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
		{
			return start1 < end2 && start2 < end1;
		}

		private int GetDayOrder(string dayOfWeek)
		{
			return dayOfWeek?.ToLower() switch
			{
				"monday" => 1,
				"tuesday" => 2,
				"wednesday" => 3,
				"thursday" => 4,
				"friday" => 5,
				"saturday" => 6,
				"sunday" => 7,
				_ => 8
			};
		}
	}
}