using Core.Models;

namespace Core
{
	public interface IMessageHandler
	{
		void BufferRawMessage(string rawLog);
		void PersistMessage(InsertionModel model);

	}
}