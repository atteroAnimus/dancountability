using System;
using Amazon;
using Amazon.SQS;
using Core.Models;
using Data;
using Moq;
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
		[InlineData(2)]
		[InlineData(10)]
		[InlineData(100)]
		public void TestPersistMessage(int numberOfMessages)
		{
			var numMessagesInQueue = numberOfMessages;
			var dataMock = new Mock<IData>();
			dataMock.Setup(x => x.Save(It.IsAny<LogEntity>()))
				.Callback<LogEntity>(x => _output.WriteLine($"successfully wrote {x.ActivityId} to database"))
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
			messageHandler.PersistMessage();
			dataMock.Verify(x => x.Save(It.IsAny<LogEntity>()), Times.Exactly(numberOfMessages));
		}
	}
}