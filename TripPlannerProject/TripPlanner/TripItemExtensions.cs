using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using TripPlanner.Models;

namespace TripPlanner;

public static class TripitemExtensions
{
    public static int SumTime(this TripItem item)
    {
        TimeSpan timeSpan = new();

        switch (item.DurationType)
        {
            case TripItemDurationType.Minutes: // 1min * 1hr/60 min * 1day/24 hr
                timeSpan.Add(TimeSpan.FromDays((double)item.Duration / (60.0 * 24.0)));
                break;
            
            case TripItemDurationType.Hours: // 1hr * 1day/24hr
                timeSpan.Add(TimeSpan.FromDays((double)item.Duration / 24.0));
                break;
            
            case TripItemDurationType.Days:
                timeSpan.Add(TimeSpan.FromDays((double)item.Duration));
                break;

            case TripItemDurationType.Weeks: // 1wk * 7day/1wk
                timeSpan.Add(TimeSpan.FromDays(7.0 * (double)item.Duration));
                break;

            case TripItemDurationType.Months: // 1mth * 27day/1mth
                timeSpan.Add(TimeSpan.FromDays(27.0 * (double)item.Duration));
                break;

            case TripItemDurationType.Years: // 1yr * 365day/1yr
                timeSpan.Add(TimeSpan.FromDays(365.0 * (double)item.Duration));
                break;
        }

        return timeSpan.Days;
    }
}