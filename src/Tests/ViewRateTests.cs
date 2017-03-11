using System;
using System.Collections.Generic;
using Xunit;
using YoutubeRatings.Data;

namespace YoutubeRatings.Core.Tests
{
    public class ViewRateTests
    {
        [Theory]
        [InlineData(new[] { "January 1, 2017 12:00:00 PM", "January 1, 2017 12:00:30 PM" }, new[] { 100, 101 }, 2.0)]
        [InlineData(new[] { "January 1, 2017 12:00:00 PM", "January 1, 2017 12:02:00 PM" }, new[] { 100, 101 }, 0.5)]
        public void CalculateViewsPerMinute(string[] dates,int[] counts, double expectedRate)
        {
            var views = new List<VideoViewCount>();
            for (var i = 0; i < dates.Length; i++)
                views.Add(new VideoViewCount { DateTime = DateTime.Parse(dates[i]), ViewCount = counts[i] });

            Assert.Equal(expectedRate, ViewRate.CalculateViewsPerMinute(views));
        }
    }
}
