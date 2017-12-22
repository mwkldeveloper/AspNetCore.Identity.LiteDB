# AspNetCore.Identity.LiteDB
AspNetCore Identity Provider for LiteDB

LiteDb link:
https://github.com/mbdavid/LiteDB

This provider is created following Microsoft's guide:
https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-custom-storage-providers

This provider are not fully implement all interface.
It is only implement:
```
UserStore<TUser> : IUserStore<TUser>
	, IUserRoleStore<TUser>
	, IUserPasswordStore<TUser>
  
RoleStore<TUser> :  IRoleStore<TRole>
```
  
How:
1. Create a webapi project
2. Add LiteDB in Nuget.
4. Add AspNetCore.Identity.LiteDB reference
5. In Startup.cs add:
```
services.AddSingleton(new LiteDatabase(@"MyData.db"));

// Identity Services
services.AddIdentity<ApplicationUser, ApplicationRole>()
				.AddUserStore<LiteDbUserStore<ApplicationUser>>()
				.AddRoleStore<LiteDbRoleStore<ApplicationRole>>()
				.AddDefaultTokenProviders();
```
