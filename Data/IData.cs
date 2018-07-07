using System.Collections;
using System.Collections.Generic;

namespace Data
{
	public interface IData
	{
		void Save(LogEntity model);
		void Save(IEnumerable<LogEntity> models);
	}
}