using System;

namespace PowerApps.Samples.Metadata
{
    public class DateTimeBehavior
    {
        public DateTimeBehaviorValues Value { get; set; }
    }

    public struct DateTimeBehaviorValues
    {
        public static implicit operator string(DateTimeBehaviorValues value)
        {
            return value.ToString();
        }

        public static implicit operator DateTimeBehaviorValues(string value)
        {
            switch (value)
            {
                case "UserLocal":
                    return UserLocal;

                case "DateOnly":
                    return DateOnly;

                case "TimeZoneIndependent":
                    return TimeZoneIndependent;

                default:
                    throw new ArgumentOutOfRangeException($"'{value}' is not a valid DateTimeBehaviorValues value.");
            }
        }

        public const string UserLocal = "UserLocal";
        public const string DateOnly = "DateOnly";
        public const string TimeZoneIndependent = "TimeZoneIndependent";
    }
}