using System;
using Common;
using Data;
using IocFactory;
using Xunit;
using Xunit.Abstractions;
namespace Core.Tests
{
	
	public class AppConfigTests
	{
		[Fact]
		public void TestInstantiate()
		{
			var thing = Factory.Instance.Resolve<IAppConfig>();
			Assert.True(thing is AppConfig);
			Assert.NotNull(Factory.Instance.Resolve<IAppConfig>());
			Assert.NotNull(thing);
			Assert.NotNull(Factory.Instance.Resolve<IData>());
			
		}
	}
}