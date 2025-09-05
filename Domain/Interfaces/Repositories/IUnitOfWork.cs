using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
	public interface IUnitOfWork
	{
		IToolRepository Tools { get; }

		Task<int> SaveChangesAsync(CancellationToken ct);
	}
}
