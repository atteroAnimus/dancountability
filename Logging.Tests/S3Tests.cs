using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Common;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Logging.Tests
{
	public class S3Tests
	{

		private readonly ITestOutputHelper _output;
		
		public S3Tests(ITestOutputHelper output)
		{
			_output = output;
		}
		[Fact]
		public void TestFileLegal()
		{
			//[]:;|=,
			var pattern = @"[\[\]:;\|=,\s]";
			var regex = new Regex(pattern);

			var input = @"stuff,and=things|are=bad and things:and;stuff";

			var output = regex.Replace(input, "-");
			
			const string validationOutput = "stuff-and-things-are-bad-and-things-and-stuff";
			
			Assert.Equal(validationOutput, output);

		}

		[Fact]
		public void TestTelemetry()
		{
			var mockUtilities = new Mock<LoggingUtilities>();
			
			var mockS3 = new Mock<IAmazonS3>();
			var message = "this is some kind of message";
			var environment = "unit-test";
			mockS3.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
				.Returns<PutObjectRequest, CancellationToken>(
				(x, y) =>
				{
					Assert.Equal(message, x.ContentBody);
					_output.WriteLine(x.Key);
					_output.WriteLine(x.BucketName);
					Assert.Contains("Telemetry", x.Key);
					return Task.FromResult(new PutObjectResponse());
				}).Verifiable();
			
			var mockConfig = new Mock<IAppConfig>();
			mockConfig.Setup(x => x.GetEnvironment()).Returns(environment);
			
			mockUtilities.Setup(x => x.S3Client()).Returns(mockS3.Object);
			var logger = new Logger(mockConfig.Object, mockUtilities.Object);
			logger.Telemetry(message);
			mockS3.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
			
		}
	}
}