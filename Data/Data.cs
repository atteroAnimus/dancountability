using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Common;
using IocFactory;

namespace Data
{
	public class Data : IData
	{
		private IAppConfig _config;

		public Data()
		{
			_config = Factory.Instance.Resolve<IAppConfig>();
		}

		public Data(IAppConfig config)
		{
			_config = config;
		}
		public void Save(LogEntity model)
		{

			try
			{
				using (var client = new AmazonDynamoDBClient())
				{
					var document = new Document
					{
						["Id"] = Guid.NewGuid().ToString(),
						["InsertDate"] = model.DateString,
						["ActivityText"] = model.ActivityId
					};
					var logs = Table.LoadTable(client, $"ActivityLog-{_config.GetEnvironment()}");
					var putResult = logs.PutItemAsync(document).Result;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public void Save(IEnumerable<LogEntity> models)
		{
			try
			{
				//build the documents

				var documents = models.Select(model => new Document
					{
						["Id"] = Guid.NewGuid().ToString(),
						["InsertDate"] = model.DateString,
						["ActivityText"] = model.ActivityId
					})
					.ToList();

				using (var client = new AmazonDynamoDBClient())
				{
					var logs = Table.LoadTable(client, $"ActivityLog-{_config.GetEnvironment()}");

					foreach (var document in documents)
					{
						var putResult = logs.PutItemAsync(document);
					}

				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}