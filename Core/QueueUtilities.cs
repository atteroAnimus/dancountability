using Amazon.SQS;

namespace Core
{
	public class QueueUtilities
	{
		public virtual IAmazonSQS GetQueuer(AmazonSQSConfig sqsConfig)
		{
			return new AmazonSQSClient(sqsConfig);
		}
	}
}