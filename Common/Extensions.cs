using Amazon;

namespace Common
{
	public static class Extensions
	{
		public static RegionEndpoint ToEndpoint(this string endpointDescription)
		{
			switch (endpointDescription)
			{
				case "us-west-1":
					return RegionEndpoint.USWest1;
				default:
					return RegionEndpoint.USEast1;
			}
		}
	}
}