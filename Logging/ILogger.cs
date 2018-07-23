using System;

namespace Logging
{
	public interface ILogger
	{
		void Log(string message);
		void Telemetry(string message);
		void Diagnostic(string message);
		void Critical(Exception ex, string message);
	}
}