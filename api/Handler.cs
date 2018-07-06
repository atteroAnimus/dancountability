using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SQS;
using Common;
using Core;
using Core.Models;
using IocFactory;
using Logging;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Api
{
    public class Handler
    {
	    private readonly IAppConfig _config;
	    private readonly IMessageHandler _messagHandler;
	    private readonly ILogger _logger;
	    private readonly ApiHelper _helper;
	    public Handler()
	    {
		    _config = Factory.Instance.Resolve<IAppConfig>();
		    _messagHandler = Factory.Instance.Resolve<IMessageHandler>();
		    _logger = Factory.Instance.Resolve<ILogger>();
		    _helper = new ApiHelper();
	    }

	    public Handler(IAppConfig config, IMessageHandler handler, ApiHelper helper, ILogger logger)
	    {
		    _config = config;
		    _helper = helper;
		    _logger = logger;
		    _messagHandler = handler;
	    }

	    public void Persist(dynamic sqsEvent, ILambdaContext context)
	    {
		    try
		    {
			    var records = new List<string>();

			    foreach (var record in sqsEvent.Records)
			    {
				    records.Add(record.Body);
			    }

			    Console.WriteLine($"attempting to persist ${records?.Count()} messages");
			    _messagHandler.PersistMessage(records);
		    }
		    catch (Exception e)
		    {
			    Console.WriteLine(e);
			    throw;
		    }
	    }

	    public APIGatewayProxyResponse Log(APIGatewayProxyRequest request, ILambdaContext context)
	    {
		    Console.WriteLine($"Debugging statement");
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
	        
		    var items = _helper.ExtractValues(request.Body).ToList();
            
		    var text = items.FirstOrDefault(x => x.Key.ToLower() == "text").Value;
		    var token = items.FirstOrDefault(x => x.Key.ToLower() == "token").Value;
		    var checkToken = _config.GetParameter("al-slack-verification-token");
		    
		    
		    //checking the token validation
	        
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
			    _logger.Log(e.ToString());
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