using Penguin.Robinhood.Interfaces;
using Penguin.Robinhood.Models;
using System;
using System.IO;

namespace Penguin.Robinhood.Extensions
{
    public static class IHasIdExtensions
    {
        public static DirectoryInfo DataPointDirectory(this IHasId id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            string path = Path.Combine(RobinhoodClient.AppData, "DataPoints", $"{id.Id}");

            return new DirectoryInfo(path);
        }

        public static FileInfo DataPointFile(this IHasId id, DataPoint dp)
        {
            return id is null
                ? throw new ArgumentNullException(nameof(id))
                : dp is null ? throw new ArgumentNullException(nameof(dp)) : id.DataPointFile(dp.BeginsAt.Value, dp.Interval);
        }

        public static FileInfo DataPointFile(this IHasId id, DateTime datePointTime, HistoricalInterval interval)
        {
            DateTime parsedRequest = new(datePointTime.Year, datePointTime.Month, datePointTime.Day, datePointTime.Hour, datePointTime.Minute, datePointTime.Second);

            string fullName = Path.Combine(id.DataPointDirectory().FullName, $"{parsedRequest.Ticks}{interval}.json");

            return new FileInfo(fullName);
        }
    }
}