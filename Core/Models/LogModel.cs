using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
	public class LogModel
	{
		public List<string> RawActivityTypes { get; set; }
		public DateTime TimeStamp { get; set; }

		public LogModel(string rawMessage)
		{
			var splits = rawMessage.Split(' ');
			RawActivityTypes = splits.ToList();
			TimeStamp = DateTime.UtcNow;
		}
	}
}