using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace Common
{
	public class AppConfig : IAppConfig
	{
		private string _region => System.Environment.GetEnvironmentVariable("region") ?? "us-east-1";
		public string Profile => System.Environment.GetEnvironmentVariable("profile") ?? "default";
		public string ServiceName => System.Environment.GetEnvironmentVariable("service-name") ?? "dancountability";
		private static string Environment => System.Environment.GetEnvironmentVariable("ENVIRONMENT_PATH") ?? "develop";
		private string ParameterPath =>
			System.Environment.GetEnvironmentVariable("parameter-path") ?? $"/dancountability/{Environment}/settings/";

		private static readonly object _syncRoot = new object();
		private Dictionary<string, string> Parameters { get; set; }
		
		
		
		public AppConfig()
		{
			var parameters = new List<Parameter>();
			Parameters = new Dictionary<string, string>();
			try
			{
				using (var client = new AmazonSimpleSystemsManagementClient())
				{
					var req = new GetParametersByPathRequest
					{
						Path = ParameterPath,
						Recursive = true,
						WithDecryption = true
					};
					string nextToken;
					do
					{
						var result = client.GetParametersByPathAsync(req).Result;
						Console.WriteLine($"api result parameters count: {result.Parameters.Count}");
						parameters.AddRange(result.Parameters);
						nextToken = result.NextToken;
					
					} while (!string.IsNullOrEmpty(nextToken));

					Console.WriteLine($"{parameters.Count} found");
				
					foreach (var p in parameters)
					{
						var name = p.Name.Replace(ParameterPath, string.Empty);
						Console.WriteLine($"found {name}");
						var value = p.Value;
						Parameters.Add(name, value);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

		}

		public string GetParameter(string parameterName)
		{
			try
			{
				return Parameters[parameterName];
			}
			catch (Exception e)
			{
				throw new Exception($"{e.Message}\n{parameterName} not found in parameter dictionary check: {ParameterPath}{parameterName} is in SSM Parameter Store.");
			}
		}

		public string GetEnvironment()
		{
			return Environment;
		}
		public string Region()
		{
			return _region;
		}
	}
}