using Dapper;
using System.Collections.Generic;

namespace YoutubeRatings.Data
{
    public class Video
    {
        public string VideoId { get; set; }
        public string Title { get; set; }
    }

    public class VideoRepository : BaseRepository
    {
        public void Add(string videoId)
        {
            using (var connection = SimpleDbConnection())
            {
                connection.Open();
                connection.Execute("INSERT INTO Videos (VideoId) VALUES (@videoId)", new { videoId = videoId });
            }
        }

        public IEnumerable<Video> Get()
        {
            using (var connection = SimpleDbConnection())
            {
                connection.Open();
                return connection.Query<Video>("SELECT * FROM Videos");
            }
        }

        public Video GetById(string videoId)
        {
            using (var connection = SimpleDbConnection())
            {
                connection.Open();
                return connection.QuerySingle<Video>("SELECT * FROM Videos WHERE VideoId = @videoId", new { videoId = videoId });
            }
        }

        public void UpdateTitle(string videoId, string title)
        {
            using (var connection = SimpleDbConnection())
            {
                connection.Open();
                connection.Execute("UPDATE Videos SET Title = @title WHERE VideoId = @videoId", new
                {
                    videoId = videoId,
                    title = title
                });
            }
        }
    }
}
