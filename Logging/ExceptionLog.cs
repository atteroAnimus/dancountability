using System;
using System.Runtime.InteropServices.ComTypes;

namespace Logging
 {
 	internal class ExceptionLog
 	{
 		public Exception ElException { get; set; }
 		public string ContextMessage { get; set; }

		 public ExceptionLog(Exception exception, string contextMessage)
		 {
			 ElException = exception;
			 ContextMessage = contextMessage;
		 }
 	}
 }