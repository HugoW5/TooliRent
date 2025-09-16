using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Metrics.Interfaces
{
	public interface IAuthMetrics
	{
		void RecordAttempt(string type);
		void RecordFailure(string type, string reason);
		void RecordDuration(string type, double ms);
	}
}
