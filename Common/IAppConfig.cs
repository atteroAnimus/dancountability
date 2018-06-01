using System.Collections.Generic;

namespace Common
{
	public interface IAppConfig
	{
		string GetParameter(string parameterName);
	}
}