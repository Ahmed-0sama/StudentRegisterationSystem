using BusinessLogic.Interfaces;
using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Department;
using Shared.DTOs.Faculty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
	public class FacultyService:IFacultyService
	{
		private readonly AppDbContext _context;

		public FacultyService(AppDbContext context)
		{
			_context = context;
		}
		public async Task<List<FacultyDto>> GetAllFaculties()
		{
			return await _context.Faculties
				.Select(f => new FacultyDto { Id = f.Id, Name = f.Name })
				.ToListAsync();
		}
		public async Task<FacultyDto> GetByIdAsync(int id)
		{
			var faculty = await _context.Faculties.FindAsync(id);
			if (faculty == null) return null;
			return new FacultyDto { Id = faculty.Id, Name = faculty.Name };
		}

		public async Task<FacultyDto> AddFacultyAsync(string dto)
		{
			var faculty = new Faculty { Name = dto };
			_context.Faculties.Add(faculty);
			await _context.SaveChangesAsync();
			return new FacultyDto
			{
				Id = faculty.Id,
				Name = faculty.Name
			};
		}
		public async Task<List<GetDepartmentDto>> GetByFacultyIdAsync(Guid facultyId)
		{
			return await _context.Departments
				.Where(d => d.FacultyId == facultyId)
				.Select(d => new GetDepartmentDto
				{ 
					Name = d.Name,
					FacultyId = d.FacultyId
				})
				.ToListAsync();
		}

	}
}
