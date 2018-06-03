using System;
using Api;
using Xunit;

namespace api.Tests
{
	public class ApiTests
	{
		[Fact]
		public void TestConstruction()
		{
			var handler = new Handler();
		}
	}
}