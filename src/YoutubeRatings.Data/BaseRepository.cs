using System.Data.SQLite;

namespace YoutubeRatings.Data
{
    public abstract class BaseRepository
    {
        protected static SQLiteConnection SimpleDbConnection()
        {
            return new SQLiteConnection("Data Source=Database.sqlite3");
        }
    }
}
