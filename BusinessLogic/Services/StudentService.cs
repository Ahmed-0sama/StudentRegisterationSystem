using BusinessLogic.Interfaces;
using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
	public class StudentService: IStudentService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly AppDbContext _dbContext;
		public StudentService(UserManager<ApplicationUser> userManager, AppDbContext dbContext)
		{
			_userManager = userManager;
			_dbContext = dbContext;
		}
		public async Task<StudentDto> GetSudentData(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				return null;
			}
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return null;
			}
			var roles = await _userManager.GetRolesAsync(user);
			if (!roles.Contains("Student"))
			{
				return null;
			}
			var student = await _dbContext.Students
				.Include(s => s.User)
				.Include(s => s.Department)
					.ThenInclude(d => d.Faculty)
				.FirstOrDefaultAsync(s => s.UserId == userId);
			if (student == null)
			{
				return null;
			}
			var dto = new StudentDto
			{
				firstName = student.User.FirstName,
				lastName = student.User.LastName,
				departmentName = student.Department.Name,
				facultyName = student.Department.Faculty.Name,
				sponserType = student.SponserType,
				gpa = student.GPA,
				creditsEarned = student.creditsEarned
			};
			return dto;
		}

	}
}
