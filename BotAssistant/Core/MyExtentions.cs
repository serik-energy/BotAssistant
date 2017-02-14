using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotAssistant.Core
{
    /// <summary>
    /// class with some extentions
    /// </summary>
    public static class MyExtentions
    {
        /// <summary>
        /// try to find EntityRecommendation deep in LuisResult
        /// </summary>
        /// <param name="result"></param>
        /// <param name="type">the entity type</param>
        /// <param name="entity">the found entity</param>
        /// <returns></returns>
        public static bool TryFindEntityDeep(this LuisResult result, string type, out EntityRecommendation entity)
        {
            try
            {
                entity = null;
                if (!result.TryFindEntity(type, out entity))
                {
                    List<EntityRecommendation> entities = new List<EntityRecommendation>();
                    foreach (Microsoft.Bot.Builder.Luis.Models.Action action in result.TopScoringIntent.Actions)
                        foreach (var param in action.Parameters)
                            entities.AddRange(param.Value);
                    entity = entities?.FirstOrDefault(e => e.Type == type);
                }
                return entity != null;
            }
            catch (Exception)
            {
                entity = null;
                return false;
            }
        }
        /// <summary>
        /// convert <see cref="string" /> to <see cref="DateTime" />
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(this string dt)
        {
            if (dt.Contains("-")) { dt = dt.Replace("-", "/"); }

            if (string.IsNullOrEmpty(dt)) { return DateTime.Now.AddHours(ResourcesManager.Timezone); }
            DateTime _dt;
            try
            {
                _dt = System.Convert.ToDateTime(dt);
            }
            catch (FormatException)
            {
                _dt = DateTime.Now.AddHours(ResourcesManager.Timezone);
            }
            return _dt;
        }
        /// <summary>
        /// convert <see cref="double" /> unixTimeStamp to <see cref="DateTime" />
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(this double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        /// <summary>
        /// check message for geolocation
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool isHaveTelegramLocation(this IMessageActivity message)
        {
            return message?.Entities?.Where(ent => ent.Type == "Place")?.FirstOrDefault() != null;
        }
    }
}