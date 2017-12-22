using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebAPI
{
	public class TestRequirement : AuthorizationHandler<TestRequirement>, IAuthorizationRequirement
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TestRequirement requirement)
		{
			if (context.User.IsInRole("User"))
			{
				context.Succeed(requirement);
			}
			else
			{
				context.Fail();
				
			}
			return Task.CompletedTask;
		}
	}
}
