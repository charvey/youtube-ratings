using Dapper;
using System;
using System.Collections.Generic;

namespace YoutubeRatings.Data
{
	public interface Event
	{
		DateTime DateTime { get; }
	}

	public class VideoViewCount : Event
	{
		public DateTime DateTime { get; set; }
		public string VideoId { get; set; }
		public long ViewCount { get; set; }
	}

	public class VideoViewCountRepository : BaseRepository
	{
		public void Add(VideoViewCount vvc)
		{
			using (var connection = SimpleDbConnection())
			{
				connection.Open();
				connection.Execute(
					@"INSERT INTO VideoViewCounts
					(VideoId, ViewCount, DateTime) VALUES
					(@VideoId, @ViewCount, @DateTime)",
					vvc);
			}
		}

		public IEnumerable<VideoViewCount> Get()
		{
			using (var connection = SimpleDbConnection())
			{
				connection.Open();
				return connection.Query<VideoViewCount>("SELECT * FROM VideoViewCounts");
			}
		}

		public IEnumerable<VideoViewCount> GetById(string videoId)
		{
			using (var connection = SimpleDbConnection())
			{
				connection.Open();
				return connection.Query<VideoViewCount>(
					@"SELECT * FROM VideoViewCounts
					WHERE VideoId=@VideoId
					ORDER BY DateTime DESC",
					new { VideoId = videoId });
			}
		}
	}
}
