using BusinessLogic.Interfaces;
using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
	public class DepartmentService: IDepartmentService
	{
		private readonly AppDbContext _context;

		public DepartmentService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<GetDepartmentDto>> GetAllAsync()
		{
			return await _context.Departments
				.Select(d => new GetDepartmentDto
				{
					Id = d.Id,
					Name = d.Name,
					FacultyId = d.FacultyId
				})
				.ToListAsync();
		}
		public async Task<GetDepartmentDto> GetByIdAsync(int id)
		{
			var dept = await _context.Departments.FindAsync(id);
			if (dept == null) return null;
			return new GetDepartmentDto { Id = dept.Id, Name = dept.Name, FacultyId = dept.FacultyId };
		}

		public async Task<GetDepartmentDto> CreateAsync(AddDepartmentDto dto)
		{
			var dept = new Department { Name = dto.Name, FacultyId = dto.FacultyId };
			_context.Departments.Add(dept);
			await _context.SaveChangesAsync();

			return new GetDepartmentDto
			{
				Id = dept.Id,
				Name = dept.Name,
				FacultyId = dept.FacultyId
			};
		}

		public async Task<List<GetDepartmentDto>> GetByFacultyIdAsync(Guid facultyId)
		{
			return await _context.Departments
				.Where(d => d.FacultyId == facultyId)
				.Select(d => new GetDepartmentDto { Id = d.Id, Name = d.Name, FacultyId = d.FacultyId })
				.ToListAsync();
		}
	}
}
