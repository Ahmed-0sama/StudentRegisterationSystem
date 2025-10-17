using BusinessLogic.Interfaces;
using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Registeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
	public class RegisterationService : IRegistrationService
	{
		private readonly AppDbContext _dbContext;
		public RegisterationService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<RegistrationPeriod> CreateRegistrationPeriodAsync(CreateRegisterationPeriodDto periodDto)
		{
			if(string.IsNullOrEmpty(periodDto.SemesterName))
			{
				throw new ArgumentException("Semester name cannot be empty.");
			}
			bool shouldBeActive = periodDto.EndDate > DateTime.UtcNow;

			// Check if an active period already exists
			if (shouldBeActive)
			{
				var activeExists = await _dbContext.RegistrationPeriods.AnyAsync(rp => rp.IsActive);
				if (activeExists)
					throw new InvalidOperationException("Cannot create a new active period while another one is active.");
			}

			var period = new RegistrationPeriod
			{
				SemesterName = periodDto.SemesterName,
				StartDate = periodDto.StartDate,
				EndDate = periodDto.EndDate,
				IsActive= shouldBeActive

			};
			_dbContext.RegistrationPeriods.Add(period);
			await  _dbContext.SaveChangesAsync();
			return period;
		}

		public async Task<bool> DropCourseAsync(Guid registrationId)
		{
			var registration = await _dbContext.CourseRegistration
				.FirstOrDefaultAsync(cr => cr.Id == registrationId);
			if (registration == null)
			{
				return false;
			}
			var now = DateTime.UtcNow;
			var period = await _dbContext.RegistrationPeriods
				.FirstOrDefaultAsync(rp=>rp.IsActive&&rp.StartDate<=now&&rp.EndDate>=now);
			if (period == null)
			{
				throw new InvalidOperationException("Course dropping is not allowed outside of an active registration period.");
			}
			_dbContext.CourseRegistration.Remove(registration);
			await _dbContext.SaveChangesAsync();
			return true;

		}

		public async Task<RegisterationPeriodDto> GetActiveRegisterationPeriodAsync()
		{
			var now = DateTime.UtcNow;
			var activeperiod= await _dbContext.RegistrationPeriods.Where(st=>st.IsActive&&st.StartDate<=now&&st.EndDate>=now)
				.Select(st => new RegisterationPeriodDto
				{
					Id = st.Id,
					SemesterName = st.SemesterName,
					StartDate = st.StartDate,
					EndDate = st.EndDate,
					IsActive = st.IsActive
				})
				.FirstOrDefaultAsync();
			return activeperiod;
		}
		public async Task<RegisterationPeriodDto> GetRegisterationPeriodByIdAsync(Guid periodId)
		{
			var period = await _dbContext.RegistrationPeriods
				.Where(rp => rp.Id == periodId)
				.Select(rp => new RegisterationPeriodDto
				{
					Id = rp.Id,
					SemesterName = rp.SemesterName,
					StartDate = rp.StartDate,
					EndDate = rp.EndDate,
					IsActive = rp.IsActive
				})
				.FirstOrDefaultAsync();

			return period;
		}

		public async Task<CourseRegistration> GetRegistrationByIdAsync(Guid registrationId)
		{
			if (registrationId == Guid.Empty)
			{
				throw new ArgumentException("Registration ID cannot be empty.");
			}
			var registration = await _dbContext.CourseRegistration
				.FirstOrDefaultAsync(cr => cr.Id == registrationId);
			return registration;
		}
		public async Task<bool> IsRegistrationOpenAsync()
		{
			var now = DateTime.UtcNow;
			var isOpen = await _dbContext.RegistrationPeriods
				.AnyAsync(rp => rp.IsActive && rp.StartDate <= now && rp.EndDate >= now);
			return isOpen;
		}

		public async Task<bool> UpdateRegistertrationPeriodAsync(RegisterationPeriodDto period)
		{
			if(period==null)
			{
				throw new ArgumentNullException("Period cannot be null.");
			}
			if(string.IsNullOrEmpty(period.SemesterName))
			{
				throw new ArgumentException("Semester name cannot be empty.");
			}
			if (period.StartDate>=period.EndDate)
			{
				throw new ArgumentException("Start date must be earlier than end date.");
			}
			var existingPeriod = await _dbContext.RegistrationPeriods.
				FirstOrDefaultAsync(rp => rp.Id == period.Id);
			if (existingPeriod == null)
			{
				return false;
			}
			existingPeriod.SemesterName = period.SemesterName;
			existingPeriod.StartDate = period.StartDate;
			existingPeriod.EndDate = period.EndDate;
			existingPeriod.IsActive = period.IsActive;
			_dbContext.RegistrationPeriods.Update(existingPeriod);
			await _dbContext.SaveChangesAsync();
			return true;
		}
		public async Task<List<RegisterationPeriodDto>> GetAllRegisterationPeriodsAsync()
		{
			var periods = await _dbContext.RegistrationPeriods
				.Select(rp => new RegisterationPeriodDto
				{
					Id = rp.Id,
					SemesterName = rp.SemesterName,
					StartDate = rp.StartDate,
					EndDate = rp.EndDate,
					IsActive = rp.IsActive
				}).OrderByDescending(rp => rp.StartDate)
				.ToListAsync();
			return periods;
		}
	}
}
