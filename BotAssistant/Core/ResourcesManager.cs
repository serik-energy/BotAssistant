namespace BotAssistant.Core
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
    }
}
