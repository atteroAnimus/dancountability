using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Api
{
	[Serializable]
	public class Record
	{
		[DataMember]
		public string MessageId { get; set; }
		[DataMember]
		public string ReceiptHandle { get; set; }
		[DataMember]
		public string Body { get; set; }
		[DataMember]
		public List<Attribute> Attributes { get; set; }
		[DataMember]
		public dynamic MessageAttributes { get; set; }
		[DataMember]
		public string Md5OfBody { get; set; }
		[DataMember]
		public string EventSource { get; set; }
		[DataMember]
		public string EventSourceArn { get; set; }
		[DataMember]
		public string AwsRegion { get; set; }
	}
}