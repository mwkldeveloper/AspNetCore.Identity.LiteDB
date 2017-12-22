using AspNetCore.Identity.LiteDB.Models;
using LiteDB;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.LiteDB
{
	public class LiteDbUserStore<TUser> : IUserStore<TUser>
	, IUserRoleStore<TUser>
	, IUserPasswordStore<TUser>
	//,IUserLoginStore<IdentityUser>
	//, IUserClaimStore<TUser>
	//,IUserSecurityStampStore<IdentityUser> 
	where TUser : ApplicationUser, new()
	{
		private readonly LiteCollection<TUser> _usersCollection;
		public LiteDbUserStore(LiteDatabase dbContext)
		{
			_usersCollection = dbContext.GetCollection<TUser>();
			_usersCollection.EnsureIndex(u => u.Id, true);
			_usersCollection.EnsureIndex(u => u.Name, true);
			_usersCollection.EnsureIndex(u => u.Roles);
		}

		#region IUserStore<TUser>
		public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			};
			await Task.Run(() => _usersCollection.Insert(user), cancellationToken);

			return IdentityResult.Success;
		}

		public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			};
			await Task.Run(() => _usersCollection.Delete(user.Id), cancellationToken);

			return IdentityResult.Success;
		}

		public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(_usersCollection.FindOne(u => u.Id == new Guid(userId)));
		}

		public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(_usersCollection.Find(u => u.NormalizedUserName == normalizedUserName).FirstOrDefault());
		}

		public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.NormalizedUserName);
		}

		public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.Id.ToString());
		}

		public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.Name);
		}

		public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			user.NormalizedUserName = normalizedName ?? throw new ArgumentNullException(nameof(normalizedName));

			_usersCollection.Update(user);

			return Task.CompletedTask;
		}

		public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			user.NormalizedUserName = userName ?? throw new ArgumentNullException(nameof(userName));

			_usersCollection.Update(user);

			return Task.CompletedTask;
		}

		public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			await Task.Run(() => _usersCollection.Update(user), cancellationToken);

			return IdentityResult.Success;
		}
		#endregion
		#region IUserPasswordStore<TUser>
		public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			user.PasswordHash = passwordHash;

			return Task.CompletedTask;
		}

		public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.PasswordHash != null);
		}
		#endregion

		#region IUserRoleStore<TUser>
		public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.Roles.Add(roleName);
			_usersCollection.Update(user);
			return Task.CompletedTask;
		}

		public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.Roles.Remove(roleName);
			_usersCollection.Update(user);
			return Task.CompletedTask;
		}

		public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult<IList<string>>(user.Roles);
		}

		public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(user.Roles.Contains(roleName));
		}

		public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult<IList<TUser>>(_usersCollection.Find(u => u.Roles.Contains(roleName)).ToList());
		}
		#endregion

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~UserStore() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

		#region IUserClaimStore<TUser>
		//public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
		//{
		//	cancellationToken.ThrowIfCancellationRequested();


		//	if (user == null)
		//	{
		//		throw new ArgumentNullException(nameof(user));
		//	}

		//	return Task.FromResult<IList<Claim>>(user.Claims.ToList());
		//}

		//public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
		//{
		//	cancellationToken.ThrowIfCancellationRequested();


		//	if (user == null)
		//	{
		//		throw new ArgumentNullException(nameof(user));
		//	}

		//	if (claims == null)
		//	{
		//		throw new ArgumentNullException(nameof(claims));
		//	}

		//	foreach (var claim in claims)
		//	{
		//		user.Claims.Add(claim);
		//	}
		//	_usersCollection.Update(user);
		//	return Task.CompletedTask;
		//}

		//public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
		//{
		//	cancellationToken.ThrowIfCancellationRequested();

		//	if (user == null)
		//	{
		//		throw new ArgumentNullException(nameof(user));
		//	}

		//	if (claim == null)
		//	{
		//		throw new ArgumentNullException(nameof(claim));
		//	}

		//	if (newClaim == null)
		//	{
		//		throw new ArgumentNullException(nameof(newClaim));
		//	}

		//	user.Claims.Remove(claim);
		//	user.Claims.Add(newClaim);
		//	_usersCollection.Update(user);
		//	return Task.CompletedTask;
		//}

		//public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
		//{
		//	cancellationToken.ThrowIfCancellationRequested();

		//	if (user == null)
		//	{
		//		throw new ArgumentNullException(nameof(user));
		//	}

		//	if (claims == null)
		//	{
		//		throw new ArgumentNullException(nameof(claims));
		//	}

		//	foreach (var claim in claims)
		//	{
		//		user.Claims.Remove(claim);
		//	}
		//	_usersCollection.Update(user);
		//	return Task.CompletedTask;
		//}

		//public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
		//{
		//	cancellationToken.ThrowIfCancellationRequested();

		//	if (claim == null)
		//	{
		//		throw new ArgumentNullException(nameof(claim));
		//	}

		//	var query = _usersCollection.Find(l => l.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value));

		//	return Task.FromResult(query.ToList() as IList<TUser>);
		//}
		#endregion

	}
}
