using System.Collections;
using System.Collections.Generic;
using Core.Models;

namespace Core
{
	public interface IMessageHandler
	{
		void BufferRawMessage(string rawLog);
		void PersistMessage(IEnumerable<InsertionModel> models);

	}
}