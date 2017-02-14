using BotAssistant.Core;
using BotAssistant.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BotAssistant.Dialogs
{
    public partial class BotDialog
    {
        /// <summary>
        /// process weather intent
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="item">message</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("Weather")]
        public async Task Weather(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            Activity message = await item as Activity;
            OWeatherMap weatherService = new OWeatherMap();

            string city = null, time = null;
            GeoCoordinates geo = null;

            EntityRecommendation locationEnt, timeEnt;
            if (result.TryFindEntityDeep(ResourcesManager.EntityTypes.location.ToString(), out locationEnt))
            {
                city = locationEnt.Entity;
            }
            if (result.TryFindEntityDeep(ResourcesManager.EntityTypes.datetime.ToString(), out timeEnt))
            {
                time = timeEnt.Entity;
            }
            if (time == null)
            {
                time = DateTime.Now.ToShortDateString(); //Default time is now..
            }

            if (city == null)
            {
                await context.PostAsync("Can`t catch the city(");
                return;
            }
            if (city == ResourcesManager.nullGeolocation)
            {
                var entity = message.Entities.Where(ent => ent.Type == "Place")?.FirstOrDefault();
                Place place = entity.GetAs<Place>();
                geo = place.Geo.ToObject<GeoCoordinates>();
            }

            DateTime requestedDt = time.ConvertToDateTime();
            string replyBase;
            string lang = message.Locale;

            if ((requestedDt - DateTime.Now).Days > -1)
            {
                //Forecast Requested
                var weatherForecast = city != ResourcesManager.nullGeolocation ?
                    await weatherService.GetForecastData(city, requestedDt, lang) :
                    await weatherService.GetForecastData(geo, requestedDt, message.Locale);

                List lastDayWeather = weatherForecast.list.Last();

                string description = lastDayWeather.weather.FirstOrDefault()?.description;
                DateTime date = lastDayWeather.dt.ConvertToDateTime();
                string lowAt = Math.Round(lastDayWeather.temp.min) + "°";
                string highAt = Math.Round(lastDayWeather.temp.max) + "°";
                string cityName = "";

                cityName = weatherForecast.city.name + ", " + weatherForecast.city.country;

                replyBase = ResourcesManager.forecastFormat;

                replyBase = string.Format(replyBase, date.ToString("D", new CultureInfo($"pl-PL")), description, cityName, lowAt, highAt);
            }
            else
            {
                var weather = city != ResourcesManager.nullGeolocation ?
                    await weatherService.GetWeatherData(city, lang) :
                    await weatherService.GetWeatherData(geo, lang);

                string description = weather.weather.FirstOrDefault()?.description;
                string lowAt = weather.main.temp_min + "";
                string highAt = weather.main.temp_min + "";
                string cityName = "";

                cityName = weather.name + ", " + weather.sys.country;

                //Build a reply message
                replyBase = ResourcesManager.weatherFormat;

                replyBase = string.Format(replyBase, description, cityName, lowAt, highAt);
            }

            string reply = BuildErrorMessage(replyBase);
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// process weather condition intent
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="item">message</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("Condition")]
        public async Task Condition(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            Activity message = await item as Activity;
            OWeatherMap weatherService = new OWeatherMap();
            string city = null, condition = null;
            GeoCoordinates geo = null;
            EntityRecommendation locationEnt, conditionEnt;
            if (result.TryFindEntityDeep(ResourcesManager.EntityTypes.location.ToString(), out locationEnt))
            {
                city = locationEnt.Entity;
            }
            if (result.TryFindEntityDeep(ResourcesManager.EntityTypes.condition.ToString(), out conditionEnt))
            {
                condition = conditionEnt.Entity;
            }
            if (city == null)
            {
                await context.PostAsync("Can`t catch the city(");
                return;
            }
            if (city == ResourcesManager.nullGeolocation)
            {
                var entity = message.Entities.Where(ent => ent.Type == "Place")?.FirstOrDefault();
                Place place = entity.GetAs<Place>();
                geo = place.Geo.ToObject<GeoCoordinates>();

            }
            string lang = message.Locale;
            var weatherForecast = city != ResourcesManager.nullGeolocation ?
                await weatherService.GetWeatherData(city, lang) :
                await weatherService.GetWeatherData(geo, lang);
            string description = weatherForecast.weather.FirstOrDefault()?.description;
            string status = weatherForecast.weather.FirstOrDefault()?.main;
            string cityName;

            cityName = weatherForecast.name + ", " + weatherForecast.sys.country;
            #region Language Builder
            description = description.Replace("nice", "clear|sun|bright|fine|partially cloudy").Replace("good", "clear|sun|bright|fine").Replace("bad", "rain|snow|cloud").Replace("cold", "snow|hail|sleet|blizzard").Replace("day", "").Replace("night", "").Replace("morning", "").Replace("afternoon", "");
            #endregion

            string Format;
            string reply;
            if (condition.ToLower().StartsWith(status.ToLower()) || description.Contains(condition))
            {
                Format = ResourcesManager.yesFormat;
            }
            else //Condition is false
            {
                Format = ResourcesManager.noFormat;
            }
            Format = string.Format(Format, description, weatherForecast.name);
            reply = BuildErrorMessage(Format);
            await context.PostAsync(reply);
        }
        /// <summary>
        /// building error message
        /// </summary>
        /// <param name="errorcode">error code</param>
        /// <returns>error message</returns>
        private string BuildErrorMessage(string errorcode)
        {
            return DateTime.Now.ToShortDateString() + " :: " + errorcode;
        }
    }
}