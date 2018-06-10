using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;

namespace Api
{
	public class ApiHelper
	{
		public virtual IEnumerable<KeyValuePair<string, string>> ExtractValues(string body)
		{
			var query = QueryHelpers.ParseQuery(body);
			var items = query.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value))
				.ToList();
			return items;
		}
	}
}