using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Identity.LiteDB
{
	public class ApplicationRole
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Name { get; set; }

		public string NormalizedRoleName { get; set; }
		
	}

}
