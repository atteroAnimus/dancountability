using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Common;
using Microsoft.AspNetCore.WebUtilities;
using Environment = System.Environment;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Api
{
    public class Handler
    {
	    private readonly IAppConfig _config;
	    public Handler()
	    {
		    _config = AppConfig.Instance;
	    }

	    public Handler(IAppConfig config)
	    {
		    _config = config;
	    }

	    public APIGatewayProxyResponse Log(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var query = QueryHelpers.ParseQuery(request.Body);
            var items = query.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value))
                .ToList();

            var text = items.FirstOrDefault(x => x.Key.ToLower() == "text").Value;
            var token = items.FirstOrDefault(x => x.Key.ToLower() == "token").Value;
            var checkToken = _config.GetParameter("al-slack-verification-token");
	        if (token != checkToken)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 403
                };
            }
            try
            {
                using (var client = new AmazonDynamoDBClient())
                {
                    var logs = Table.LoadTable(client, "ActivityLog");
                    var doc = new Document
                    {
                        ["Id"] = Guid.NewGuid().ToString(),
                        ["InsertDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss"),
                        ["ActivityText"] = text
                    };
                    var result = logs.PutItemAsync(doc).Result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Exception Occurred: {e}"
                };
            }
            
            stopwatch.Stop();
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = $"recorded successfully in {stopwatch.ElapsedMilliseconds} miliseconds"
            };
        }
    }

    
}