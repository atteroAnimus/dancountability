using System;
using System.Diagnostics;
using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Common;
using Core;
using IocFactory;
using Logging;

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

	    

	    public APIGatewayProxyResponse Log(APIGatewayProxyRequest request, ILambdaContext context)
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
	        
		    var items = _helper.ExtractValues(request.Body).ToList();
            
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