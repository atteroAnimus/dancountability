namespace Core
 {
 	public interface IQueuable
 	{
 		void Push<T>(T item);
 		T Pop<T>();
 	}
 }