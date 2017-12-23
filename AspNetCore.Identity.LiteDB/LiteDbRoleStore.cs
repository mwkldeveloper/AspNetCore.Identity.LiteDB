using LiteDB;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.LiteDB
{
	public class LiteDbRoleStore<TRole> : IRoleStore<TRole> where TRole : ApplicationRole, new()
	{

		private readonly LiteCollection<TRole> _rolesCollection;
		public LiteDbRoleStore(LiteDatabase dbContext)
		{
			_rolesCollection = dbContext.GetCollection<TRole>();
			_rolesCollection.EnsureIndex(r => r.Id, true);
			_rolesCollection.EnsureIndex(r => r.Name, true);
		}

		public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			};
			await Task.Run(() => _rolesCollection.Insert(role), cancellationToken);

			return IdentityResult.Success;
		}

		public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			};
			await Task.Run(() => _rolesCollection.Delete(role.Id), cancellationToken);

			return IdentityResult.Success;
		}

		public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (roleId == null)
			{
				throw new ArgumentNullException(nameof(roleId));
			};
			return Task.FromResult(_rolesCollection.FindOne(r => r.Id == new Guid(roleId)));
		}

		public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (normalizedRoleName == null)
			{
				throw new ArgumentNullException(nameof(normalizedRoleName));
			};
			return Task.FromResult(_rolesCollection.FindOne(r => r.NormalizedRoleName == normalizedRoleName));
		}

		public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			return Task.FromResult(role.NormalizedRoleName);
		}

		public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			return Task.FromResult(role.Id.ToString());
		}

		public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			return Task.FromResult(role.Name.ToString());
		}

		public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			role.NormalizedRoleName = normalizedName ?? throw new ArgumentNullException(nameof(normalizedName));

			_rolesCollection.Update(role);

			return Task.CompletedTask;
		}

		public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			role.Name = roleName ?? throw new ArgumentNullException(nameof(roleName));

			_rolesCollection.Update(role);

			return Task.CompletedTask;
		}

		public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			await Task.Run(() => _rolesCollection.Update(role), cancellationToken);

			return IdentityResult.Success;
		}

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
		// ~RoleStore() {
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
	}
}
