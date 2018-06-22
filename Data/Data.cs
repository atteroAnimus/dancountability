using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace Data
{
	public class Data : IData
	{
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
					var logs = Table.LoadTable(client, "ActivityLog");
					var putResult = logs.PutItemAsync(document).Result;
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