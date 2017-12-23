using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.LiteDB.Models
{
	public class ApplicationUser : IIdentity
	{
		public virtual Guid Id { get; set; } = Guid.NewGuid();

		public string UserName { get; set; }
		public string Name { get; set; }
		public virtual string Email { get; set; }
		public virtual bool EmailConfirmed { get; set; }
		public virtual String PasswordHash { get; set; }
		public string NormalizedUserName { get; internal set; }
		public string AuthenticationType { get; set; }
		public bool IsAuthenticated { get; set; }

		public virtual List<string> Roles { get; set; } = new List<string>() { "User" };
		//public List<Claim> Claims { get; set; } = new List<Claim>();

	}
}
