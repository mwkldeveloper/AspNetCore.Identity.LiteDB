using AspNetCore.Identity.LiteDB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebAPI.Controllers
{
	[Route("api")]
	public class AccountController
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IConfiguration _configuration;
		public AccountController(
		   UserManager<ApplicationUser> userManager,
		   SignInManager<ApplicationUser> signInManager,
			IConfiguration configuration
		   )
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_configuration = configuration;
		}

		[Route("login")]
		[HttpPost]
		public async Task<object> Login([FromBody] LoginDto model)
		{
			var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

			if (result.Succeeded)
			{
				var appUser = await _userManager.FindByNameAsync(model.Email);
				return await GenerateJwtToken(model.Email, appUser);
			}

			throw new ApplicationException("INVALID_LOGIN_ATTEMPT");
		}

		[Route("register")]
		[HttpPost]
		public async Task<object> Register([FromBody] RegisterDto model)
		{
			var user = new ApplicationUser
			{
				Name = model.Email,
				Email = model.Email
			};
			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, false);
				return await GenerateJwtToken(model.Email, user);
			}

			throw new ApplicationException("UNKNOWN_ERROR");
		}

		private async Task<object> GenerateJwtToken(string email, ApplicationUser user)
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
			};
			foreach (var userRole in user.Roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, userRole));
			}
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

			var token = new JwtSecurityToken(
				_configuration["JwtIssuer"],
				_configuration["JwtIssuer"],
				claims,
				expires: expires,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public class LoginDto
		{
			[Required]
			public string Email { get; set; }

			[Required]
			public string Password { get; set; }

		}

		public class RegisterDto
		{
			[Required]
			public string Email { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
			public string Password { get; set; }
		}
	}
}
