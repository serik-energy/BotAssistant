using BotAssistant.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BotAssistant.Core
{
    /// <summary>
    /// weather service
    /// </summary>
    public class OWeatherMap
    {
        /// <summary>
        /// Receiving current weather by city
        /// </summary>
        /// <param name="query">city name</param>
        /// <param name="lang">language</param>
        /// <returns>Json responce from weather service as <see cref="WeatherObject" /></returns>
        public async Task<WeatherObject> GetWeatherData(string query, string lang = "en")
        {
            try
            {
                using (HttpClient OWMHttpClient = new HttpClient())
                {
                    string response = await OWMHttpClient.GetStringAsync(new Uri($"http://api.openweathermap.org/data/2.5/weather?q={query}&appid={ResourcesManager.OWMAppID}&units=metric&lang={lang}"));
                    WeatherObject owmResponde = JsonConvert.DeserializeObject<WeatherObject>(response);
                    if (owmResponde != null) return owmResponde;
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Receiving current weather by geolocation
        /// </summary>
        /// <param name="geo">location</param>
        /// <param name="lang">language</param>
        /// <returns>Json responce from weather service as <see cref="WeatherObject" /></returns>
        public async Task<WeatherObject> GetWeatherData(GeoCoordinates geo, string lang = "en")
        {
            try
            {
                using (HttpClient OWMHttpClient = new HttpClient())
                {
                    string response = await OWMHttpClient.GetStringAsync(new Uri($"http://api.openweathermap.org/data/2.5/weather?lat={geo.Latitude}&lon={geo.Longitude}&appid={ResourcesManager.OWMAppID}&units=metric&lang={lang}"));
                    WeatherObject owmResponde = JsonConvert.DeserializeObject<WeatherObject>(response);
                    if (owmResponde != null) return owmResponde;
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Receiving forecast data by city
        /// </summary>
        /// <param name="query">city name</param>
        /// <param name="dt">date</param>
        /// <param name="lang">language</param>
        /// <returns>Json responce from weather service as <see cref="WeatherObject" /></returns>
        public async Task<WeatherForecastObject> GetForecastData(string query, DateTime dt, string lang = "en")
        {
            try
            {
                int days = (dt - DateTime.Now).Days + 1;

                using (HttpClient OWMHttpClient = new HttpClient())
                {
                    string response = await OWMHttpClient.GetStringAsync(new Uri($"http://api.openweathermap.org/data/2.5/forecast/daily?q={query}&appid={ResourcesManager.OWMAppID}&units=metric&cnt={days}&lang={lang}"));

                    WeatherForecastObject owmResponde = JsonConvert.DeserializeObject<WeatherForecastObject>(response);
                    if (owmResponde != null) return owmResponde;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Receiving forecast data by geolocation
        /// </summary>
        /// <param name="geo">location</param>
        /// <param name="dt">date</param>
        /// <param name="lang">language</param>
        /// <returns>Json responce from weather service as <see cref="WeatherObject" /></returns>
        public async Task<WeatherForecastObject> GetForecastData(GeoCoordinates geo, DateTime dt, string lang = "en")
        {
            try
            {
                int days = (dt - DateTime.Now).Days + 1;

                using (HttpClient OWMHttpClient = new HttpClient())
                {
                    string response = await OWMHttpClient.GetStringAsync(new Uri($"http://api.openweathermap.org/data/2.5/forecast/daily?lat={geo.Latitude}&lon={geo.Longitude}&appid={ResourcesManager.OWMAppID}&units=metric&cnt={days}&lang={lang}"));

                    WeatherForecastObject owmResponde = JsonConvert.DeserializeObject<WeatherForecastObject>(response);
                    if (owmResponde != null) return owmResponde;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}