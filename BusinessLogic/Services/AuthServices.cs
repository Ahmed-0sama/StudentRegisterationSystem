using BusinessLogic.Interfaces;
using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.DTOs.Users;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
	public class AuthServices:IAuthServices
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly AppDbContext _context;
		private readonly IJwtService _jwt;
		public AuthServices(UserManager<ApplicationUser> userManager,IJwtService jwt,AppDbContext context)
		{
			_userManager = userManager;
			_jwt= jwt;
			_context = context;
		}
		public async Task<AuthModel>RegisterAsync(RegisterModel model)
		{
			var existingUser = await _userManager.FindByNameAsync(model.UserName);
			if (existingUser != null)
			{
				return new AuthModel { Message = "User already exists" };
			}
			var user = new ApplicationUser
			{
				UserName = model.UserName,
				FirstName = model.FirstName,
				LastName = model.LastName
			};
			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
			{
				var errors = string.Join(", ", result.Errors.Select(e => e.Description));
				return new AuthModel {Message = errors };
			}
			await _userManager.AddToRoleAsync(user, "Student");
			var student = new Student
			{
				UserId = user.Id,
				FName = user.FirstName,
				LName = user.LastName,
				GPA = 0.0,
				creditsEarned = 0,
				DepartmentId = model.DepartmentId,
				SponserType = model.SponserType ?? "Self"
			};
			_context.Students.Add(student);
			await _context.SaveChangesAsync();
			var (token, expires) = await _jwt.GenerateAsync(user);
			var roles = await _userManager.GetRolesAsync(user);
			return new AuthModel
			{
				Token = token,
				ExpireOn = expires,
				RegisterationNumber = user.UserName,
				UserRoles = roles.ToList()
			};
		}
		public async Task<AuthModel> GetTokenAsync(Login model)
		{

			var user = await _userManager.FindByNameAsync(model.RegistrationNumber);

			if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
			{
				return new AuthModel { Message = "Username or Password is incorrect!" };
			}
			var jwtSecurityToken = await _jwt.GenerateAsync(user);
			var rolesList = await _userManager.GetRolesAsync(user);
			return new AuthModel
			{
				Token = jwtSecurityToken.token,
				ExpireOn = jwtSecurityToken.expires,
				RegisterationNumber = user.UserName,
				UserRoles = rolesList.ToList(),
				Message = "Login Success"
			};

		}
	}
}
