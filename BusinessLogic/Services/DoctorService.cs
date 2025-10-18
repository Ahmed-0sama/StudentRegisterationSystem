using BusinessLogic.Interfaces;
using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Courses;
using Shared.DTOs.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
	public class DoctorService:IDoctorService
	{
		private readonly AppDbContext _dbcontext;
		public DoctorService(AppDbContext dbcontext)
		{
			_dbcontext = dbcontext;
		}
		public async Task<List<DoctorDto>> GetAllDoctorsAsync()
		{
			try
			{
				var doctors = await _dbcontext.Doctors
					.Select(d => new DoctorDto
					{
						Id = d.Id,
						FullName = d.FullName
					})
					.ToListAsync();

				return doctors;
			}
			catch
			{
				
				return new List<DoctorDto>();
			}
		}
		public async Task<bool> RemoveDoctorAsync(Guid courseId, Guid doctorId)
		{
			if (courseId == Guid.Empty || doctorId == Guid.Empty)
				return false;

			var doctorCourse = await _dbcontext.DoctorCourses
				.FirstOrDefaultAsync(dc => dc.CourseId == courseId && dc.DoctorId == doctorId);

			if (doctorCourse == null)
				return false;

			_dbcontext.DoctorCourses.Remove(doctorCourse);
			await _dbcontext.SaveChangesAsync();

			return true;
		}

		public async Task<DoctorDto?> GetDoctorByIdAsync(Guid id)
		{
			if (id == Guid.Empty)
				return null;

			var doctor = await _dbcontext.Doctors
				.Where(d => d.Id == id)
				.Select(d => new DoctorDto
				{
					Id = d.Id,
					FullName=d.FullName 
				})
				.FirstOrDefaultAsync();

			return doctor;
		}

		public async Task<List<DoctorDto>> GetDoctorsByCourseAsync(Guid courseId)
		{
			if (courseId == Guid.Empty)
				return new List<DoctorDto>();

			var doctors = await _dbcontext.DoctorCourses
				.Where(dc => dc.CourseId == courseId)
				.Include(dc => dc.Doctor)
				.Select(dc => new DoctorDto
				{
					Id = dc.Doctor.Id,
					FullName = dc.Doctor.FullName
				})
				.ToListAsync();

			return doctors;
		}
		public async Task<bool> AssignDoctorToCourse(AssignDoctorDto dto)
		{
			var course = await _dbcontext.Courses.FindAsync(dto.CourseId);
			if (course == null)
				throw new Exception("Course not found");

			var doctor = await _dbcontext.Doctors.FindAsync(dto.DoctorId);
			if (doctor == null)
				throw new Exception("Doctor not found");

			// Check if already assigned
			var existingAssignment = await _dbcontext.DoctorCourses
				.FirstOrDefaultAsync(dc => dc.DoctorId == dto.DoctorId && dc.CourseId == dto.CourseId);

			if (existingAssignment != null)
				throw new Exception("Doctor already assigned to this course");

			var doctorCourse = new DoctorCourse
			{
				DoctorId = dto.DoctorId,
				CourseId = dto.CourseId,
				AcademicYear = dto.AcademicYear
			};

			_dbcontext.DoctorCourses.Add(doctorCourse);
			await _dbcontext.SaveChangesAsync();

			return true;
		}

	}
}
