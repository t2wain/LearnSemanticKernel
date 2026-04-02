using System.ComponentModel;

namespace AICommon.Tools
{
    public class TimeTool
    {
        [Description("Get the current date")]
        public virtual string Date(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("D", formatProvider);
        }

        [Description("Get the date of the last day matching the supplied week day name in English. Example: Che giorno era 'Martedi' scorso -> dateMatchingLastDayName 'Tuesday' => Tuesday, 16 May, 2023")]
        public virtual string DateMatchingLastDayName([Description("The day name to match")] DayOfWeek input, IFormatProvider? formatProvider = null)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
            for (int i = 1; i <= 7; i++)
            {
                dateTimeOffset = dateTimeOffset.AddDays(-1.0);
                if (dateTimeOffset.DayOfWeek == input)
                {
                    break;
                }
            }

            return dateTimeOffset.ToString("D", formatProvider);
        }

        [Description("Get the current day of the month")]
        public virtual string Day(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("dd", formatProvider);
        }

        [Description("Get the current day of the week")]
        public virtual string DayOfWeek(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("dddd", formatProvider);
        }

        [Description("Get the date offset by a provided number of days from today")]
        public virtual string DaysAgo([Description("The number of days to offset from today")] double input, IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.AddDays(0.0 - input).ToString("D", formatProvider);
        }

        [Description("Get the current clock hour")]
        public virtual string Hour(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("h tt", formatProvider);
        }

        [Description("Get the current clock 24-hour number")]
        public virtual string HourNumber(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("HH", formatProvider);
        }

        [Description("Get the minutes on the current hour")]
        public virtual string Minute(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("mm", formatProvider);
        }

        [Description("Get the current month name")]
        public virtual string Month(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("MMMM", formatProvider);
        }

        [Description("Get the current month number")]
        public virtual string MonthNumber(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("MM", formatProvider);
        }

        [Description("Get the current date and time in the local time zone")]
        public virtual string Now(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("f", formatProvider);
        }

        [Description("Get the seconds on the current minute")]
        public virtual string Second(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("ss", formatProvider);
        }

        [Description("Get the current time")]
        public virtual string Time(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("hh:mm:ss tt", formatProvider);
        }

        [Description("Get the local time zone name")]
        public virtual string TimeZoneName()
        {
            return TimeZoneInfo.Local.DisplayName;
        }

        [Description("Get the local time zone offset from UTC")]
        public virtual string TimeZoneOffset(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("%K", formatProvider);
        }

        [Description("Get the current date")]
        public virtual string Today(IFormatProvider? formatProvider = null)
        {
            return Date(formatProvider);
        }

        [Description("Get the current UTC date and time")]
        public virtual string UtcNow(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.UtcNow.ToString("f", formatProvider);
        }

        [Description("Get the current year")]
        public virtual string Year(IFormatProvider? formatProvider = null)
        {
            return DateTimeOffset.Now.ToString("yyyy", formatProvider);
        }
    }
}
