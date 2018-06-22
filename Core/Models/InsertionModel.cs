using System;

namespace Core.Models
{
	public class InsertionModel
	{
		public ActivityType ActivityType { get; }
		private readonly DateTime _timeStamp;
		public string InsertionDate => _timeStamp.ToString("yyyy-MM-dd hh:mm:ss"); 

		public InsertionModel(ActivityType torp, DateTime timestamp)
		{
			ActivityType = torp;
			_timeStamp = timestamp;
		}

	}
}