using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <summary>
        /// get duration from <see cref="EntityRecommendation" />
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static TimeSpan? GetDuration(this EntityRecommendation duration)
        {
            try
            {
                TimeSpan? ts = null;
                int period = 0;
                string str;// = "PT2H,PT2M,PT2S,P2W,P2D,P2M,P2Y,";
                if (duration.Resolution.TryGetValue("duration", out str))
                {
                    string st1 = str.Remove(0, 2);
                    st1 = st1.Remove(st1.Count() - 1);
                    string st2 = str.Remove(0, 1);
                    st2 = st2.Remove(st2.Count() - 1);
                    if (str.StartsWith("PT") && int.TryParse(st1, out period))
                    {
                        switch (str.Last())
                        {
                            case 'H'://hour
                                ts = TimeSpan.FromHours(period);
                                break;
                            case 'M'://minute
                                ts = TimeSpan.FromMinutes(period);
                                break;
                            case 'S'://second
                                ts = TimeSpan.FromSeconds(period);
                                break;
                            default:
                                return null;
                        }
                    }
                    else if (str.StartsWith("P") && int.TryParse(st2, out period))
                    {
                        switch (str.Last())
                        {
                            case 'D'://day
                                ts = TimeSpan.FromDays(period);
                                break;
                            case 'W'://week
                                ts = TimeSpan.FromDays(period * 7);
                                break;
                            case 'M'://month
                                ts = TimeSpan.FromDays(period * 30);
                                break;
                            case 'Y'://year
                                ts = TimeSpan.FromDays(period * 365);
                                break;
                            default:
                                return null;
                        }
                    }
                }
                return ts;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //convert TimeSpan to string
        /// <summary>
        /// convert <see cref="TimeSpan"/> to <see cref="string"/> 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static string toStr(this TimeSpan ts)
        {
            string str = "";
            if (ts.Days != 0)
            {
                str += ts.Days + " days";
            }
            if (ts.Hours != 0)
            {
                str += ts.Hours.ToString() + (!string.IsNullOrEmpty(str) ? ", " : "") + " hours";
            }
            if (ts.Minutes != 0)
            {
                str += ts.Minutes.ToString() + (!string.IsNullOrEmpty(str) ? ", " : "") + " minutes";
            }
            if (ts.Seconds != 0)
            {
                str += ts.Seconds.ToString() + (!string.IsNullOrEmpty(str) ? ", " : "") + " seconds";
            }
            return str;
        }
    }
}