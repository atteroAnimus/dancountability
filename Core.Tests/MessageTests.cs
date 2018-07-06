using System;
using System.Collections.Generic;
using Amazon;
using Amazon.Runtime.Internal.Util;
using Amazon.SQS;
using Core.Models;
using Data;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Core.Tests
{
	public class MessageTests
	{
		private readonly ITestOutputHelper _output;

		public MessageTests(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void TestBufferRawMessage()
		{
			var queueMock = new Mock<IQueuable>();
			queueMock.Setup(x => x.Push(It.IsAny<InsertionModel>())).Callback<InsertionModel>(x =>
					_output.WriteLine($"successfully pushed {x.ActivityType} to queue"))
				.Verifiable();
			var dataMock = new Mock<IData>();
			dataMock.Setup(x => x.Save(It.IsAny<LogEntity>()))
				.Callback<LogEntity>(x => _output.WriteLine($"successfully wrote {x.ActivityId} to database"))
				.Verifiable();
			var messageHandler = new MessageHandler(queueMock.Object, dataMock.Object);
			messageHandler.BufferRawMessage(":eggplant:");
			queueMock.Verify(x => x.Push(It.IsAny<InsertionModel>()), Times.Once());
		}

		[Theory]
		[InlineData(1)]
		public void TestPersistMessage(int numberOfMessages)
		{
			var numMessagesInQueue = numberOfMessages;
			var dataMock = new Mock<IData>();
			dataMock.Setup(x => x.Save(It.IsAny<IEnumerable<LogEntity>>()))
				.Callback<IEnumerable<LogEntity>>(x => _output.WriteLine("wrote stuff to database"))
				.Verifiable();
			var queueMock = new Mock<IQueuable>();
			queueMock.Setup(x => x.Push(It.IsAny<InsertionModel>()))
				.Callback<InsertionModel>(x =>
					_output.WriteLine($"successfully pushed {x.ActivityType} to queue"))
				.Verifiable();
			queueMock.Setup(x => x.Pop<InsertionModel>())
				.Returns(() =>
				{
					if (numMessagesInQueue <= 0) return null;
					--numMessagesInQueue;
					return new InsertionModel(ActivityType.Sb, DateTime.UtcNow);
				});

			var messageHandler = new MessageHandler(queueMock.Object, dataMock.Object);
			var arg = new List<InsertionModel>
			{
				new InsertionModel(ActivityType.P, DateTime.Now)
			};
			messageHandler.PersistMessage(arg);
			dataMock.Verify(x => x.Save(It.IsAny<IEnumerable<LogEntity>>()), Times.Exactly(numberOfMessages));
		}

		[Fact]
		public void TestDeserialize()
		{
			const string input = "{\"ActivityType\":0,\"InsertionDate\":\"2018-07-06 06:55:40\"}";
			var insertionModel = JsonConvert.DeserializeObject<InsertionModel>(input);
			Assert.NotNull(insertionModel);
			Assert.Equal(ActivityType.Unknown, insertionModel.ActivityType);
		}
	}
}