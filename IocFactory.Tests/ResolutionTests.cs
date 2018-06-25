using System;
using Xunit;
using Xunit.Sdk;

namespace IocFactory.Tests
{
	public class ResolutionTests
	{
		[Fact]
		public void TestResolve()
		{
			var thing = Factory.Instance.Resolve<ISomething>();
			Assert.True(thing is Something);
			Assert.False(thing is SomethingElse);
		}
	}
}