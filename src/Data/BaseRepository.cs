﻿using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace YoutubeRatings.Data
{
    public abstract class BaseRepository
    {
        protected static DbConnection SimpleDbConnection()
        {
            return new SqliteConnection("Data Source=Database.sqlite3");
        }
    }
}
