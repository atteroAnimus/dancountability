using System;
using System.Runtime.Serialization;

namespace Api.output
{
	[Serializable]
	public class Attribute
	{
		[DataMember]
		public int ApproximateReceiveCount { get; set; }
		[DataMember]
		public int SentTimestamp { get; set; }
		[DataMember]
		public long SenderId { get; set; }
		[DataMember]
		public long ApproximateFirstReceiveTimeStamp { get; set; }
		
	}
}