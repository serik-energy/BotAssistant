﻿namespace BotAssistant.Core
{
    /// <summary>
    /// some constant values
    /// </summary>
    public class ResourcesManager
    {
        /// <summary>
        /// ID for Luis app
        /// </summary>
        public const string LUISAppID = "d34c0482-bd9a-4949-949d-74e36021260b";
        /// <summary>
        /// ID for pre-built Luis Cortana app
        /// </summary>
        public const string CortanaAppID = "c413b2ef-382c-45bd-8ff0-f76d60e2a821";
        /// <summary>
        /// subscription key for Luis
        /// </summary>
        public const string LUISSubscriptionKey = "1b04f75ee8124c63816a2b0f3dd63d6f";
        /// <summary>
        /// weather service appID
        /// </summary>
        public const string OWMAppID = "33c9da8b3522f74e5313cd76b898cff4";
        /// <summary>
        /// name of default alarm
        /// </summary>
        public const string DefaultAlarmWhat = "default";


        /// <summary>
        /// entity types
        /// </summary>
        public enum EntityTypes
        {
            /// <summary>
            /// location
            /// </summary>
            location,
            /// <summary>
            /// date and time
            /// </summary>
            datetime,
            /// <summary>
            /// condition of weather
            /// </summary>
            condition
        }
        /// <summary>
        /// null geolocation
        /// </summary>
        public const string nullGeolocation = "nullgeolocation";
        /// <summary>
        /// formatted message for display forecast
        /// </summary>
        public const string forecastFormat = "Hello, For {0}, it'll be {1} in {2}.. with low temp at {3} and high at {4}..";
        /// <summary>
        /// formatted message for display weather
        /// </summary>
        public const string weatherFormat = "Hello, It's {0} in {1}.. with low temp at {2} and high at {3}..";
        /// <summary>
        /// formatted message for display yes
        /// </summary>
        public const string yesFormat = "Hi..actually yes, it's {0} in {1}.";
        /// <summary>
        /// formatted message for display no
        /// </summary>
        public const string noFormat = "Hi.. actually No, it's {0} in {1}.";
        /// <summary>
        /// current time zone
        /// </summary>
        public const double Timezone = 1;
        /// <summary>
        /// stantdart entities
        /// </summary>
        public class Entities
        {
            /// <summary>
            /// alarm entities
            /// </summary>
            public class Alarm
            {
                /// <summary>
                /// title alarm entity
                /// </summary>
                public const string Title = "builtin.alarm.title";
                /// <summary>
                /// start time  alarm entity
                /// </summary>
                public const string Start_Time = "builtin.alarm.start_time";
                /// <summary>
                /// start date alarm entity
                /// </summary>
                public const string Start_Date = "builtin.alarm.start_date";
                /// <summary>
                /// duration alarm entity
                /// </summary>
                public const string Duration = "builtin.alarm.duration";
            }
            /// <summary>
            /// remind entities
            /// </summary>
            public class Reminder
            {
                /// <summary>
                /// original start date remind entity
                /// </summary>
                public const string Original_start_date = "builtin.reminder.original_start_date";
                /// <summary>
                /// original start time remind entity
                /// </summary>
                public const string Original_start_time = "builtin.reminder.original_start_time";
                /// <summary>
                /// text remind entity
                /// </summary>
                public const string Reminder_text = "builtin.reminder.reminder_text";
                /// <summary>
                /// start date remind entity
                /// </summary>
                public const string Start_date = "builtin.reminder.start_date";
                /// <summary>
                /// start time remind entity
                /// </summary>
                public const string Start_time = "builtin.reminder.start_time";
            }
            /// <summary>
            /// calendar entities
            /// </summary>
            public class Calendar
            {
                /// <summary>
                /// absolute location calendar entity
                /// </summary>
                public const string absolute_location = "builtin.calendar.absolute_location";
                /// <summary>
                /// title calendar entity
                /// </summary>
                public const string title = "builtin.calendar.title";
                /// <summary>
                /// start time calendar entity
                /// </summary>
                public const string start_time = "builtin.calendar.start_time";
                /// <summary>
                /// start date calendar entity
                /// </summary>
                public const string start_date = "builtin.calendar.start_date";
                /// <summary>
                /// original start time calendar entity
                /// </summary>
                public const string original_start_time = "builtin.calendar.original_start_time";
                /// <summary>
                /// original start date calendar entity
                /// </summary>
                public const string original_start_date = "builtin.calendar.original_start_date";
                /// <summary>
                /// move later time calendar entity
                /// </summary>
                public const string move_later_time = "builtin.calendar.move_later_time";
                /// <summary>
                /// move earlier time calendar entity
                /// </summary>
                public const string move_earlier_time = "builtin.calendar.move_earlier_time";
                /// <summary>
                /// implicit location calendar entity
                /// </summary>
                public const string implicit_location = "builtin.calendar.implicit_location";
                /// <summary>
                /// end time calendar entity
                /// </summary>
                public const string end_time = "builtin.calendar.end_time";
                /// <summary>
                /// end date calendar entity
                /// </summary>
                public const string end_date = "builtin.calendar.end_date";
                /// <summary>
                /// duration calendar entity
                /// </summary>
                public const string duration = "builtin.calendar.duration";
                /// <summary>
                /// destination calendar entity
                /// </summary>
                public const string destination_calendar = "builtin.calendar.destination_calendar";
            }
        }
    }
}
