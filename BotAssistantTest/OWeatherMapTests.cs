using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BotAssistant.Core;
using BotAssistant.Models;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace BotAssistantTest
{
    [TestClass]
    public class OWeatherMapTests
    {
        [TestMethod]
        public async Task GetWeatherData_WithValidQuery_ReturnWeatherObject()
        {
            // arrange 
            OWeatherMap wm = new OWeatherMap();
            string query = "Poltava", lang = "en";
            // act
            WeatherObject wo = await wm.GetWeatherData(query, lang);
            // assert
            Assert.IsNotNull(wo);
        }
        [TestMethod]
        public async Task GetWeatherData_WithInvalidQuery_ReturnNull()
        {
            // arrange 
            OWeatherMap wm = new OWeatherMap();
            string query = "6956707", lang = "en";
            // act
            WeatherObject wo = await wm.GetWeatherData(query, lang);
            // assert
            Assert.IsNull(wo);
        }
        [TestMethod]
        public async Task GetWeatherData_WithValidGeoLocation_ReturnWeatherObject()
        {
            // arrange 
            OWeatherMap wm = new OWeatherMap();
            GeoCoordinates query = new GeoCoordinates(latitude: 36, longitude: 50);
            string lang = "en";
            // act
            WeatherObject wo = await wm.GetWeatherData(query, lang);
            // assert
            Assert.IsNotNull(wo);
        }
        [TestMethod]
        public async Task GetWeatherData_WithInvalidGeoLocation_ReturnNull()
        {
            // arrange 
            OWeatherMap wm = new OWeatherMap();
            GeoCoordinates query = new GeoCoordinates();
            string  lang = "en";
            // act
            WeatherObject wo = await wm.GetWeatherData(query, lang);
            // assert
            Assert.IsNull(wo);
        }



        [TestMethod]
        public async Task GetForecastData_WithValidQuery_ReturnWeatherObject()
        {
            // arrange 
            OWeatherMap wm = new OWeatherMap();
            string query = "Poltava", lang = "en";
            DateTime dt = DateTime.Now.AddDays(3);
            // act
            WeatherForecastObject wo = await wm.GetForecastData(query,dt, lang);
            // assert
            Assert.IsNotNull(wo);
        }
        [TestMethod]
        public async Task GetForecastData_WithInvalidQuery_ReturnNull()
        {
            // arrange 
            OWeatherMap wm = new OWeatherMap();
            string query = "6956707", lang = "en";
            DateTime dt = DateTime.Now.AddDays(3);
            // act
            WeatherForecastObject wo = await wm.GetForecastData(query,dt, lang);
            // assert
            Assert.IsNull(wo);
        }
        [TestMethod]
        public async Task GetForecastData_WithValidGeoLocation_ReturnWeatherObject()
        {
            // arrange 
            OWeatherMap wm = new OWeatherMap();
            GeoCoordinates query = new GeoCoordinates(latitude: 36, longitude: 50);
            DateTime dt = DateTime.Now.AddDays(3);
            string lang = "en";
            // act
            WeatherForecastObject wo = await wm.GetForecastData(query,dt, lang);
            // assert
            Assert.IsNotNull(wo);
        }
        [TestMethod]
        public async Task GetForecastData_WithInvalidGeoLocation_ReturnNull()
        {
            // arrange 
            OWeatherMap wm = new OWeatherMap();
            GeoCoordinates query = new GeoCoordinates();
            DateTime dt = DateTime.Now.AddDays(3);
            string lang = "en";
            // act
            WeatherForecastObject wo = await wm.GetForecastData(query,dt, lang);
            // assert
            Assert.IsNull(wo);
        }
    }
}
