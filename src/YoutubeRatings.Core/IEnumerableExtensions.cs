using System.Collections.Generic;
using System.Linq;
using YoutubeRatings.Data;

namespace YoutubeRatings.Core
{
	public static class IEnumerableExtensions
	{
		public static T Latest<T>(this IEnumerable<T> items) where T : Event
		{
			return items.OrderByDescending(x => x.DateTime).First();
		}
	}
}
