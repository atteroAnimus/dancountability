using System;

namespace Core.Models
{
	public class InsertionModel
	{
		public ActivityType ActivityType { get; }
		public DateTime InsertionDate { get; set; }

		public string OutputDate()
		{
			return InsertionDate.ToString("yyyy-MM-dd hh:mm:ss");
		}

		public InsertionModel()
		{
			
		}
		public InsertionModel(ActivityType torp, DateTime timestamp)
		{
			ActivityType = torp;
			InsertionDate = timestamp;
		}

	}
}