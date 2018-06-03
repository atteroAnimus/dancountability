using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Common;
using Core;
using IocFactory;
using Microsoft.AspNetCore.WebUtilities;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Api
{
    public class Handler
    {
	    private readonly IAppConfig _config;
	    private readonly IMessageHandler _messagHandler;
	    public Handler()
	    {
		    _config = Factory.Instance.Resolve<IAppConfig>();
		    _messagHandler = Factory.Instance.Resolve<IMessageHandler>();
	    }

	    public Handler(IAppConfig config)
	    {
		    _config = config;
	    }

	    public APIGatewayProxyResponse Log(APIGatewayProxyRequest request, ILambdaContext context)
        {
            //TODO: extract this out into business logic.  This method should only call a BL method which queues the data
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
		        _messagHandler.BufferRawMessage(text);
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