using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using Amazon.SQS;
using Amazon.SQS.Model;
using Common;
using IocFactory;
using Newtonsoft.Json;

namespace Core
{
	public class Queuable : IQueuable
	{
		private readonly IAmazonSQS _client;
		private readonly IAppConfig _config;
		private readonly string _queueUrl;
		public Queuable()
		{
			_client = Factory.Instance.Resolve<IAmazonSQS>();
			_config = Factory.Instance.Resolve<IAppConfig>();
			_queueUrl = _config.GetParameter("INCOMING_QUEUE_URL");

		}
		public void Push<T>(T item)
		{
			var json = JsonConvert.SerializeObject(item);
			using (var client = _client)
			{
				var req = new SendMessageRequest
				{
					MessageBody = json,
					QueueUrl = _queueUrl
				};
				var sendMessageResult = client.SendMessageAsync(req).Result;
				if (sendMessageResult.HttpStatusCode != HttpStatusCode.OK)
				{
					throw new Exception($"unable to queue message for url: {_queueUrl}");
				}
			}
		}

		public T Pop<T>()
		{
			using (var client = _client)
			{
				var req = new ReceiveMessageRequest(_queueUrl);
				var receiveResult = client.ReceiveMessageAsync(req).Result;
				if (!receiveResult.Messages.Any())
				{
					return default(T);
				}

				var json = receiveResult.Messages.First().Body;
				return JsonConvert.DeserializeObject<T>(json);
			}
		}
	}
}