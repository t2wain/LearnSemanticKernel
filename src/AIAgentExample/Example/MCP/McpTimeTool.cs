using AICommon.Tools;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace AIAgentExample.Example.MCP
{
    [McpServerToolType]
    public class McpTimeTool : TimeTool
    {
        [McpServerTool, Description("Get the current date")]
        public override string Date(IFormatProvider? formatProvider = null) => 
            base.Date(formatProvider);

        [McpServerTool, Description("Get the date of the last day matching the supplied week day name in English. Example: Che giorno era 'Martedi' scorso -> dateMatchingLastDayName 'Tuesday' => Tuesday, 16 May, 2023")]
        public override string DateMatchingLastDayName(
            [Description("The day name to match")] DayOfWeek input,
            IFormatProvider? formatProvider = null) => base.DateMatchingLastDayName(input, formatProvider);

        [McpServerTool, Description("Get the current day of the month")]
        public override string Day(IFormatProvider? formatProvider = null) => 
            base.Day(formatProvider);

        [McpServerTool, Description("Get the current day of the week")]
        public override string DayOfWeek(IFormatProvider? formatProvider = null) => 
            base.DayOfWeek(formatProvider);

        [McpServerTool, Description("Get the date offset by a provided number of days from today")]
        public override string DaysAgo(
            [Description("The number of days to offset from today")] double input, 
            IFormatProvider? formatProvider = null) => base.DaysAgo(input, formatProvider);

        [McpServerTool, Description("Get the current clock hour")]
        public override string Hour(IFormatProvider? formatProvider = null) => 
            base.Hour(formatProvider);

        [McpServerTool, Description("Get the current clock 24-hour number")]
        public override string HourNumber(IFormatProvider? formatProvider = null) => 
            base.HourNumber(formatProvider);

        [McpServerTool, Description("Get the minutes on the current hour")]
        public override string Minute(IFormatProvider? formatProvider = null) => 
            base.Minute(formatProvider);

        [McpServerTool, Description("Get the current month name")]
        public override string Month(IFormatProvider? formatProvider = null) => 
            base.Month(formatProvider);

        [McpServerTool, Description("Get the current month number")]
        public override string MonthNumber(IFormatProvider? formatProvider = null) => 
            base.MonthNumber(formatProvider);

        [McpServerTool, Description("Get the current date and time in the local time zone")]
        public override string Now(IFormatProvider? formatProvider = null) => 
            base.Now(formatProvider);

        [McpServerTool, Description("Get the seconds on the current minute")]
        public override string Second(IFormatProvider? formatProvider = null) => 
            base.Second(formatProvider);

        [McpServerTool, Description("Get the current time")]
        public override string Time(IFormatProvider? formatProvider = null) => 
            base.Time(formatProvider);

        [McpServerTool, Description("Get the local time zone name")]
        public override string TimeZoneName() => 
            base.TimeZoneName();

        [McpServerTool, Description("Get the local time zone offset from UTC")]
        public override string TimeZoneOffset(IFormatProvider? formatProvider = null) => 
            base.TimeZoneOffset(formatProvider);

        [McpServerTool, Description("Get the current date")]
        public override string Today(IFormatProvider? formatProvider = null) => 
            base.Today(formatProvider);

        [McpServerTool, Description("Get the current UTC date and time")]
        public override string UtcNow(IFormatProvider? formatProvider = null) => 
            base.UtcNow(formatProvider);

        [McpServerTool, Description("Get the current year")]
        public override string Year(IFormatProvider? formatProvider = null) => 
            base.Year(formatProvider);

    }
}
