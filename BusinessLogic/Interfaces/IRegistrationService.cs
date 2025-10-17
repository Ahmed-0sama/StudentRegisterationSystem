using DataAccess.Entities;
using Shared.DTOs.Registeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
	public interface IRegistrationService
	{
		Task<RegisterationPeriodDto> GetActiveRegisterationPeriodAsync();
		Task<RegistrationPeriod> CreateRegistrationPeriodAsync(CreateRegisterationPeriodDto period);
		Task<RegisterationPeriodDto> GetRegisterationPeriodByIdAsync(Guid periodId);
		Task<bool> UpdateRegistertrationPeriodAsync(RegisterationPeriodDto period);
		Task<bool> IsRegistrationOpenAsync();
		Task<CourseRegistration> GetRegistrationByIdAsync(Guid registrationId);
		Task<List<RegisterationPeriodDto>> GetAllRegisterationPeriodsAsync();


	}
}
