using System.Globalization;

namespace Maliwan.Domain.Core.Extensions;

public static class DateTimeExtension
{
    public static DateTime FromUtcToBrTimeZone(this DateTime utcDate)
    {
        var timeLista = TimeZoneInfo.GetSystemTimeZones();

        var brTimeZone = timeLista.FirstOrDefault(tz => tz.BaseUtcOffset == new TimeSpan(-3, 0, 0));

        return TimeZoneInfo.ConvertTimeFromUtc(utcDate, brTimeZone);
    }

    public static TimeSpan Diff(this DateTime date1, DateTime date2)
    {
        var startDate = date1 < date2 ? date1 : date2;
        var endDate = date1 < date2 ? date2 : date1;
        return endDate - startDate;
    }
}