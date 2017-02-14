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
            //Todo: Build error messages..
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


                switch (lang)
                {
                    case "ar":
                        //cityName = await weatherForecast.city.name.Translate("en", "ar");
                        break;
                    default:
                        cityName = weatherForecast.city.name + ", " + weatherForecast.city.country;
                        break;
                }

                replyBase = ResourcesManager.forecastFormat;

                //if (lang == "ar")
                //replyBase = await ResourcesManager.forecastFormat.Translate("en", "ar");
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

                if (lang == "ar")
                {
                    //Country is not in a good format to translate, i.e. SA, US, UAE.. etc.
                    //cityName = await weather.name.Translate("en", "ar");
                }
                else if (lang == "en")
                {
                    cityName = weather.name + ", " + weather.sys.country;
                }

                //Build a reply message
                replyBase = ResourcesManager.weatherFormat;

                if (lang == "ar")
                    //replyBase = await ResourcesManager.weatherFormat.Translate("en", "ar");
                    replyBase = string.Format(replyBase, description, cityName, lowAt, highAt);
            }

            string reply = BuildErrorMessage(replyBase);
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
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