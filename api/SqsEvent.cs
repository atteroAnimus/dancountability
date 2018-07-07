using System.Collections.Generic;

namespace Api
{
	public class SqsEvent
	{
		public List<Record> Records { get; set; }
	}
}