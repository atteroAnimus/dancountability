using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using Xunit;

namespace Logging.Tests
{
	public class S3Tests
	{
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
		public void TestPutObject()
		{
			var mockUtilities = new Mock<LoggingUtilities>();
			
			var mockS3 = new Mock<IAmazonS3>();
//			mockS3.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>()))
//				.Returns(new Task<PutObjectResponse>(() =>
//					{
//						Assert.Equal(0,0);
//						return new PutObjectResponse();
//					}
//				));
		}
	}
}