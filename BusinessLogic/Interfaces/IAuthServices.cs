using DataAccess.Entities;
using Shared.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
	public interface IAuthServices
	{
		Task<AuthModel> RegisterAsync(RegisterModel registerModel);
		 Task<AuthModel> GetTokenAsync(Login model);
		Task<AuthModel> RegisterDoctorAsync(RegisterDoctorModel model);
	}
}
