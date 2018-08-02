﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Common;
using IocFactory;
using Logging;

namespace Data
{
	public class Data : IData
	{
		private IAppConfig _config;
		private readonly ILogger _logger;

		public Data()
		{
			_config = Factory.Instance.Resolve<IAppConfig>();
			_logger = Factory.Instance.Resolve<ILogger>();
		}

		public Data(IAppConfig config, ILogger logger)
		{
			_config = config;
			_logger = logger;
		}
		public void Save(LogEntity model)
		{

			try
			{
				_logger.Telemetry($"trying to log ${model.ActivityId}");
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
				_logger.Critical(e, e.Message);
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
				
				_logger.Telemetry($"trying to log ${documents.Count} documents");
				

				using (var client = new AmazonDynamoDBClient())
				{
					var logs = Table.LoadTable(client, $"ActivityLog-{_config.GetEnvironment()}");

					foreach (var document in documents)
					{
						var putResult = logs.PutItemAsync(document).Result;
					}

				}
			}
			catch (Exception e)
			{
				_logger.Critical(e, e.Message);
				throw;
			}
		}
	}
}