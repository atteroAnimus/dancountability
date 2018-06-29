using System;
using IocFactory;
using Xunit;
using Xunit.Abstractions;

namespace Core.Tests
{
	public class QueueableTests
	{
		[Fact]
		public void TestResolveQueueable()
		{
			var queueable = Factory.Instance.Resolve<IQueuable>();
			Assert.NotNull(queueable);
		}
	}
}