using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Amazon.S3;
using Amazon.S3.Model;
using Common;

namespace Logging
{
	
	public class Logger : ILogger
	{
		private IAppConfig _config;
		private LoggingUtilities _utilities;
		private readonly Regex _illegalFileCharacters = new Regex(@"[\[\]:;\|=,\s]");
		public Logger()
		{
			_config = new AppConfig();
			_utilities = new LoggingUtilities();
		}

		public Logger(IAppConfig config, LoggingUtilities utilities)
		{
			_config = config;
			_utilities = utilities;

		}

		public void Log(string message)
		{
			Console.WriteLine(message);
		}


		/// <summary>
		/// Telemetry logs should be things like method start/stop and messages like "starting bucket grab."  Basically things that indicate a working, breathing app.
		/// </summary>
		/// <param name="message">The message that will appear in the telemetry output</param>
		public void Telemetry(string message)
		{

			var prefix = $"{DateTime.UtcNow:yyyy-MM-dd}";
			var keyName = _illegalFileCharacters.Replace(message, "-").Substring(0,15);
			var fileNamePrefix = $"{DateTime.UtcNow:HH-mm-ss}";
			
			using (var client = _utilities.S3Client())
			{
				var putObjectRequest = new PutObjectRequest
				{
					ContentBody = message,
					BucketName = $"dancountability-log-{_config.GetEnvironment()}",
					Key = $"{prefix}{Guid.NewGuid()}"
				};

				var result = client.PutObjectAsync(putObjectRequest);
			}
		}

		/// <summary>
		/// A critical exception.  Exceptions shouldn't be exceptions unless they are critical after all.  This can be overridden with different exception types
		/// </summary>
		/// <param name="ex">Exception that was thrown</param>
		/// <param name="message">Message to describe what might be happening at exception time.</param>
		public void Critical(Exception ex, string message)
		{
			
		}
		
		/// <summary>
		/// More verbose than telemetry.  An example would be "got 3 messages from queue."  These are used for debugging
		/// </summary>
		/// <param name="message"></param>
		public void Diagnostic(string message)
		{
			
		}
	}
}