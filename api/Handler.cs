using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.AspNetCore.WebUtilities;
using Environment = System.Environment;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Api
{
    public class Handler
    {
        public APIGatewayProxyResponse Log(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var query = QueryHelpers.ParseQuery(request.Body);
            var items = query.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value))
                .ToList();

            var text = items.FirstOrDefault(x => x.Key.ToLower() == "text").Value;
            var token = items.FirstOrDefault(x => x.Key.ToLower() == "token").Value;
            var checkToken = Environment.GetEnvironmentVariable("al-slack-verification-token");
            Console.WriteLine($"checkToken: {checkToken} | token: {token}");
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
                    var logs = Table.LoadTable(client, "ActivityLogs");
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