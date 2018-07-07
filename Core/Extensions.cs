using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Data;

namespace Core
{
	public static class Extensions
	{
		public static LogEntity ToEntity(this InsertionModel model)
		{
			return new LogEntity
			{
				ActivityId = (int)model.ActivityType,
				DateString = model.OutputDate()
			};
		}

		public static IEnumerable<LogEntity> ToEntities(this IEnumerable<InsertionModel> models)
		{
			return models.Select(x => x.ToEntity());
		}
	}
}