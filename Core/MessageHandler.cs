using System;
using System.Collections.Generic;
using Core.Models;
using Data;
using IocFactory;

namespace Core
{
	public class MessageHandler : IMessageHandler
	{
		private readonly IQueuable _queue;
		private readonly IData _data;
		public MessageHandler()
		{
			_queue = Factory.Instance.Resolve<IQueuable>();
			_data = Factory.Instance.Resolve<IData>();
		}

		public MessageHandler(IQueuable queuable, IData data)
		{
			_queue = queuable;
			_data = data;	
		}

		public void BufferRawMessage(string rawLog)
		{
			var log = new LogModel(rawLog);
			var insertionModels = new List<InsertionModel>();
			var now = DateTime.UtcNow;
			foreach (var rawLogName in log.RawActivityTypes)
			{
				insertionModels.Add(new InsertionModel(Convert(rawLogName), now));
			}

			foreach (var model in insertionModels)
			{
				_queue.Push(model);
			}
		}

		public void PersistMessage()
		{
			var message = _queue.Pop<InsertionModel>();
			while (message != null)
			{
				_data.Save(message.ToEntity());
				message = _queue.Pop<InsertionModel>();
			}
		}

		private static ActivityType Convert(string val)
		{
			switch (val)
			{
				case ":eggplant:":
					return ActivityType.Sb;
				case ":bikini:":
					return ActivityType.P;
				default:
					return ActivityType.Unknown;
			}
		}
	}
}