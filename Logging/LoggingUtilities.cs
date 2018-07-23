using Amazon.S3;

namespace Logging
{
	public class LoggingUtilities
	{
		public virtual IAmazonS3 S3Client()
		{
			return new AmazonS3Client();
		}
	}
}