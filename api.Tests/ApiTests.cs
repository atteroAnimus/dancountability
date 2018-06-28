using System;
using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Api;
using Common;
using Core;
using IocFactory;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace api.Tests
{
	public class ApiTests
	{
		private readonly ITestOutputHelper _output;

		public ApiTests(ITestOutputHelper output)
		{
			_output = output;
		}
		
		[Fact]
		public void TestSendRequest()
		{
			var helper = new Mock<ApiHelper>();
			var tokenValue = "butt";
			helper.Setup(x => x.ExtractValues(It.IsAny<string>())).Returns(new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("token",tokenValue),
				new KeyValuePair<string, string>("text","testText")
			}).Verifiable();
			var appConfig = new Mock<IAppConfig>();
			appConfig.Setup(x => x.GetParameter(It.IsAny<string>())).Returns(tokenValue);
			var messageHandler = new Mock<IMessageHandler>();
			messageHandler.Setup(x => x.BufferRawMessage(It.IsAny<string>()))
				.Callback<string>(x => _output.WriteLine($"received {x} for buffering")).Verifiable();

			var logger = new Mock<Logging.ILogger>();
			logger.Setup(x => x.Log(It.IsAny<string>())).Callback<string>(_output.WriteLine);
			var handler = new Handler(appConfig.Object,messageHandler.Object, helper.Object, logger.Object);
			
			var response = handler.Log(new APIGatewayProxyRequest(), null);//don't need real objects here because we're mocking them anyway
			helper.Verify(x => x.ExtractValues(It.IsAny<string>()), Times.Exactly(1));
			Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
			messageHandler.Verify(x => x.BufferRawMessage(It.IsAny<string>()), Times.Exactly(1));
		}

		[Fact]
		public void TestResolveCore()
		{
			var messageHandler = Factory.Instance.Resolve<IMessageHandler>();
			Assert.True(messageHandler is MessageHandler);
		}
	}
}