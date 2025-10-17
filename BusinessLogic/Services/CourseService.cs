using BusinessLogic.Interfaces;
using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Courses;
using Shared.DTOs.Registeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
	public class CourseService:ICourseService
	{
		private readonly AppDbContext _context;
		public CourseService(AppDbContext context)
		{
			_context = context;
		}
		public async Task<List<CourseDto>> GetAvailableCoursesForStudent(string userId)
		{
			var student = await _context.Students
				.Include(s => s.Department)
				.Include(s => s.CourseRegistrations)
				.FirstOrDefaultAsync(s => s.UserId == userId);

			if (student?.Department == null)
				return new List<CourseDto>();

			var registeredCourseIds = student.CourseRegistrations?
				.Select(cr => cr.CourseId)
				.ToList() ?? new List<Guid>();

			var availableCourses = await _context.Courses
				.Where(c => c.DepartmentId == student.DepartmentId && !registeredCourseIds.Contains(c.Id))
				.Include(c => c.Department)
				.Include(c => c.DoctorCourses)
					.ThenInclude(dc => dc.Doctor)
					.ThenInclude(d => d.User)
				.Select(c => new CourseDto
				{
					id = c.Id,
					CourseName = c.CourseName,
					CreditHours = c.CreditHours,
					DepartmentName = c.Department.Name,
					DoctorsNames = c.DoctorCourses
						.Where(dc => dc.Doctor != null && dc.Doctor.User != null)
						.Select(dc => dc.Doctor.User.FirstName ?? "Unknown")
						.ToList()
				})
				.ToListAsync();

			return availableCourses;
		}
		public async Task<List<RegisteredCourseDto>> GetRegisteredCourses(string studentId)
		{
			var student = await _context.Students
				.Include(s => s.CourseRegistrations)
				.ThenInclude(cr => cr.Course)
				.FirstOrDefaultAsync(s => s.User.Id == studentId);

			if (student == null)
				throw new ArgumentException("Student not found");

			// Get the current active semester
			var currentSemester = await _context.RegistrationPeriods
				.Where(rp => rp.IsActive)
				.Select(rp => rp.SemesterName)
				.FirstOrDefaultAsync();

			if (currentSemester == null)
				return new List<RegisteredCourseDto>(); // No active semester

			// Select only active registrations in the current semester
			var registeredCourses = student.CourseRegistrations
				.Where(cr => cr.Status == "Active" && cr.Semester == currentSemester)
				.Select(cr => new RegisteredCourseDto
				{
					RegistrationId = cr.Id,
					CourseId = cr.CourseId,
					CourseName = cr.Course.CourseName,
					CourseCode = cr.Course.CourseCode,
					CreditHours = cr.Course.CreditHours,
					PeriodSemester = cr.Semester,
					Status = cr.Status
				})
				.ToList();

			return registeredCourses;
		}
		public async Task<bool>RegisterCourse(string studentid,RegisterCourseDto dto)
		{
			var student=await _context.Students
				.Include(s=>s.CourseRegistrations)
				.FirstOrDefaultAsync(s=>s.User.Id==studentid);
			if (student == null)
			{
				throw new ArgumentException("Student Not Found");
			}
			var course = await _context.Courses.FirstOrDefaultAsync(c=>c.Id==dto.CourseId);
			if (course == null)
			{
				throw new ArgumentException("Course Not Found");
			}
			if(course.DepartmentId!=student.DepartmentId)
			{
				throw new InvalidOperationException("Cannot register for a course outside of the student's department.");
			}
			var existingRegistration = student.CourseRegistrations
				.FirstOrDefault(cr => cr.CourseId == dto.CourseId && cr.Status == "Active");
			if (existingRegistration != null)
			{
				throw new InvalidOperationException("Student is already registered for this course.");
			}
			var registrationPeriod = await _context.RegistrationPeriods
				.FirstOrDefaultAsync(rp => rp.Id == dto.RegisterationPeriodId && rp.IsActive);
			if (registrationPeriod == null)
			{
				throw new InvalidOperationException("Invalid or inactive registration period.");
			}
			var registration = new CourseRegistration
			{
				StudentId = student.Id,
				CourseId = dto.CourseId,
				RegistrationPeriodId = dto.RegisterationPeriodId,
				Semester = registrationPeriod.SemesterName,
				Status = "Active",
				RegisteredAt = DateTime.UtcNow
			};

			_context.CourseRegistration.Add(registration);
			await _context.SaveChangesAsync();
			return true;
		}
		public async Task<CourseDto> CreateCourse(CreateCourseDto dto)
		{
			var department = await _context.Departments.FindAsync(dto.DepartmentId);
			if (department == null)
				throw new Exception("Department not found");

			var course = new Course
			{
				CourseName = dto.CourseName,
				CourseCode = dto.CourseCode,
				CreditHours = dto.CreditHours,
				DepartmentId = dto.DepartmentId
			};
				_context.Courses.Add(course);
				await _context.SaveChangesAsync();


			return new CourseDto
			{
				id = course.Id,
				CourseName = course.CourseName,
				CourseCode= course.CourseCode,
				CreditHours = course.CreditHours,
				DepartmentName = department.Name,
				DoctorsNames = new List<string>()
			};
		}

		public async Task<bool> AssignDoctorToCourse(AssignDoctorDto dto)
		{
			var course = await _context.Courses.FindAsync(dto.CourseId);
			if (course == null)
				throw new Exception("Course not found");

			var doctor = await _context.Doctors.FindAsync(dto.DoctorId);
			if (doctor == null)
				throw new Exception("Doctor not found");

			// Check if already assigned
			var existingAssignment = await _context.DoctorCourses
				.FirstOrDefaultAsync(dc => dc.DoctorId == dto.DoctorId && dc.CourseId == dto.CourseId);

			if (existingAssignment != null)
				throw new Exception("Doctor already assigned to this course");

			var doctorCourse = new DoctorCourse
			{
				DoctorId = dto.DoctorId,
				CourseId = dto.CourseId,
				AcademicYear = dto.AcademicYear
			};

			_context.DoctorCourses.Add(doctorCourse);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<bool> RemoveDoctorFromCourse(Guid courseId, Guid doctorId)
		{
			var doctorCourse = await _context.DoctorCourses
				.FirstOrDefaultAsync(dc => dc.DoctorId == doctorId && dc.CourseId == courseId);

			if (doctorCourse == null)
				throw new Exception("Doctor assignment not found");

			_context.DoctorCourses.Remove(doctorCourse);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<List<CourseDto>> GetAllCourses()
		{
			return await _context.Courses
				.Include(c => c.Department)
				.Include(c => c.DoctorCourses)
					.ThenInclude(dc => dc.Doctor)
					.ThenInclude(d => d.User)
				.Select(c => new CourseDto
				{
					id = c.Id,
					CourseName = c.CourseName,
					CreditHours = c.CreditHours,
					DepartmentName = c.Department.Name,
					DoctorsNames = c.DoctorCourses.Select(dc => dc.Doctor.User.FirstName).ToList()
				})
				.ToListAsync();
		}

		public async Task<CourseDto> GetCourseById(Guid courseId)
		{
			var course = await _context.Courses
				.Include(c => c.Department)
				.Include(c => c.DoctorCourses)
					.ThenInclude(dc => dc.Doctor)
					.ThenInclude(d => d.User)
				.FirstOrDefaultAsync(c => c.Id == courseId);

			if (course == null)
				throw new Exception("Course not found");

			return new CourseDto
			{
				id = course.Id,
				CourseName = course.CourseName,
				CreditHours = course.CreditHours,
				DepartmentName = course.Department.Name,
				DoctorsNames = course.DoctorCourses.Select(dc => dc.Doctor.User.FirstName).ToList()
			};
		}
		public async Task<List<CourseDto>> GetCoursesByDepartment(Guid DepartmentId)
		{
			return await _context.Courses
				.Include(c => c.Department)
				.Include(c => c.DoctorCourses)
					.ThenInclude(dc => dc.Doctor)
					.ThenInclude(d => d.User)
				.Where(c => c.DepartmentId == DepartmentId)  
				.Select(c => new CourseDto
				{
					id = c.Id,
					CourseName = c.CourseName,
					CourseCode = c.CourseCode,
					CreditHours = c.CreditHours,
					DepartmentName = c.Department.Name,
					DoctorsNames = c.DoctorCourses
						.Select(dc => dc.Doctor.User.FirstName + " " + dc.Doctor.User.LastName) 
						.ToList()
				})
				.ToListAsync();
		}

	}
}
