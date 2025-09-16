using System.Diagnostics.Metrics;

namespace Application.Metrics;

public class AuthMetrics
{
	private readonly Counter<int> _authAttempts;
	private readonly Counter<int> _authFailures;
	private readonly Histogram<double> _authDuration;

	public AuthMetrics(IMeterFactory meterFactory)
	{
		var meter = meterFactory.Create("TooliRent.Application.Metrics.AuthMetrics");

		_authAttempts = meter.CreateCounter<int>(
			"toilirent.auth.attempts",
			description: "Number of auth attempts"
		);

		_authFailures = meter.CreateCounter<int>(
			"toilirent.auth.failures",
			description: "Number of failed auth attempts"
		);

		_authDuration = meter.CreateHistogram<double>(
			"toilirent.auth.duration_ms",
			unit: "ms",
			description: "Auth operation duration"
		);
	}

	public void RecordAttempt(string type)
	{
		_authAttempts.Add(1, KeyValuePair.Create<string, object?>("type", type));
	}

	public void RecordFailure(string type, string reason)
	{
		_authFailures.Add(1,
			KeyValuePair.Create<string, object?>("type", type),
			KeyValuePair.Create<string, object?>("reason", reason));
	}
	public void RecordDuration(string type, double ms)
	{
		_authDuration.Record(ms, KeyValuePair.Create<string, object?>("type", type));
	}
}
