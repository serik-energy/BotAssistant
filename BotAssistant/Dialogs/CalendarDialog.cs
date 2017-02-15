using BotAssistant.Core;
using BotAssistant.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace BotAssistant.Dialogs
{
    public partial class BotDialog : LuisDialog<object>
    {
        /// <summary>
        /// create calendar entry
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.calendar.create_calendar_entry")]
        public async Task Create_calendar_entry(IDialogContext context, LuisResult result)
        {
            EntityRecommendation start_date;
            if (!result.TryFindEntity(ResourcesManager.Entities.Calendar.start_date, out start_date))
            {
                start_date = new EntityRecommendation(type: ResourcesManager.Entities.Calendar.start_date) { Entity = string.Empty };
            }
            EntityRecommendation start_time;
            if (!result.TryFindEntity(ResourcesManager.Entities.Calendar.start_time, out start_time))
            {
                start_time = new EntityRecommendation(type: ResourcesManager.Entities.Calendar.start_time) { Entity = string.Empty };
            }
            EntityRecommendation end_time;
            if (!result.TryFindEntity(ResourcesManager.Entities.Calendar.end_time, out end_time))
            {
                end_time = new EntityRecommendation(type: ResourcesManager.Entities.Calendar.end_time) { Entity = string.Empty };
            }
            EntityRecommendation end_date;
            if (!result.TryFindEntity(ResourcesManager.Entities.Calendar.end_date, out end_date))
            {
                end_date = new EntityRecommendation(type: ResourcesManager.Entities.Calendar.end_date) { Entity = string.Empty };
            }

            Calendar calendar = new Calendar(context.Activity);

            var parser = new Chronic.Parser();
            var span = parser.Parse(start_date.Entity + " " + start_time.Entity);
            if (span != null)
            {
                var start = span.Start ?? span.End;
                calendar.startTime = start.Value < DateTime.Now.AddHours(ResourcesManager.Timezone) ? start.Value.AddDays(1) : start.Value;
            }
            span = parser.Parse(end_date.Entity + " " + end_time.Entity);
            if (span != null)
            {
                var end = span.Start ?? span.End;
                calendar.endTime = end.Value < calendar.startTime ? end.Value.AddDays(1) : end.Value;
            }

            EntityRecommendation duration;
            if (result.TryFindEntity(ResourcesManager.Entities.Calendar.duration, out duration))
            {
                calendar.duration = duration.GetDuration();
            }
            EntityRecommendation title;
            if (result.TryFindEntity(ResourcesManager.Entities.Calendar.title, out title))
            {
                calendar.title = title.Entity;
            }
            EntityRecommendation location;
            if (result.TryFindEntity(ResourcesManager.Entities.Calendar.absolute_location, out location) ||
                result.TryFindEntity(ResourcesManager.Entities.Calendar.implicit_location, out location))
            {
                calendar.location = location.Entity;
            }
            EntityRecommendation destination_calendar;
            if (result.TryFindEntity(ResourcesManager.Entities.Calendar.destination_calendar, out destination_calendar))
            {
                calendar.destinationCalendar = destination_calendar.Entity;
            }

            if (!string.IsNullOrEmpty(calendar.title))
            {
                this.calendarEntryes.Add(calendar);
                string reply = $"calendar entry created:\n\n{calendar}";
                await context.PostAsync(reply);
                MessagesController.refreshCalendar(context.Activity.From.Id, calendarEntryes);
            }
            else
            {
                await context.PostAsync("can not understand title of calendar entry");
            }
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// try find calendar entries by title from LuisResult
        /// </summary>
        /// <param name="result">result of Luis</param>
        /// <param name="calendars">return found calendar entries</param>
        /// <returns>true, if calendar entries found</returns>
        public bool TryFindCalendarsByTitle(LuisResult result, out List<Calendar> calendars)
        {
            calendars = null;
            string title;
            EntityRecommendation titleEnt;
            if (result.TryFindEntity(ResourcesManager.Entities.Calendar.title, out titleEnt))
            {
                title = titleEnt.Entity;
            }
            else
            {
                return false;
            }
            calendars = this.calendarEntryes.FindAll(c => c.title == title);
            return calendars.Count > 0;
        }
        /// <summary>
        /// change calendar entries
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.calendar.change_calendar_entry")]
        public async Task change_calendar_entry(IDialogContext context, LuisResult result)
        {
            EntityRecommendation start_date;
            if (!result.TryFindEntity(ResourcesManager.Entities.Calendar.start_date, out start_date))
            {
                start_date = new EntityRecommendation(type: ResourcesManager.Entities.Calendar.start_date) { Entity = string.Empty };
            }
            EntityRecommendation start_time;
            if (!result.TryFindEntity(ResourcesManager.Entities.Calendar.start_time, out start_time))
            {
                start_time = new EntityRecommendation(type: ResourcesManager.Entities.Calendar.start_time) { Entity = string.Empty };
            }
            EntityRecommendation end_time;
            if (!result.TryFindEntity(ResourcesManager.Entities.Calendar.end_time, out end_time))
            {
                end_time = new EntityRecommendation(type: ResourcesManager.Entities.Calendar.end_time) { Entity = string.Empty };
            }
            EntityRecommendation end_date;
            if (!result.TryFindEntity(ResourcesManager.Entities.Calendar.end_date, out end_date))
            {
                end_date = new EntityRecommendation(type: ResourcesManager.Entities.Calendar.end_date) { Entity = string.Empty };
            }

            Calendar calendar = new Calendar(context.Activity);

            var parser = new Chronic.Parser();
            var span = parser.Parse(start_date.Entity + " " + start_time.Entity);
            if (span != null)
            {
                var start = span.Start ?? span.End;
                calendar.startTime = start.Value < DateTime.Now.AddHours(ResourcesManager.Timezone) ? start.Value.AddDays(1) : start.Value;
            }
            span = parser.Parse(end_date.Entity + " " + end_time.Entity);
            if (span != null)
            {
                var end = span.Start ?? span.End;
                calendar.endTime = end.Value < calendar.startTime ? end.Value.AddDays(1) : end.Value;
            }
            EntityRecommendation duration;
            if (result.TryFindEntity(ResourcesManager.Entities.Calendar.duration, out duration))
            {
                calendar.duration = duration.GetDuration();
            }
            EntityRecommendation title;
            if (result.TryFindEntity(ResourcesManager.Entities.Calendar.title, out title))
            {
                calendar.title = title.Entity;
            }
            EntityRecommendation location;
            if (result.TryFindEntity(ResourcesManager.Entities.Calendar.absolute_location, out location) ||
                result.TryFindEntity(ResourcesManager.Entities.Calendar.implicit_location, out location))
            {
                calendar.location = location.Entity;
            }
            EntityRecommendation destination_calendar;
            if (result.TryFindEntity(ResourcesManager.Entities.Calendar.destination_calendar, out destination_calendar))
            {
                calendar.destinationCalendar = destination_calendar.Entity;
            }
            if (!string.IsNullOrEmpty(calendar.title))
            {
                someCalendar = calendar;
            }
            else
            {
                await context.PostAsync("can not understand title of calendar entry");
                context.Wait(MessageReceived);
                return;
            }


            List<Calendar> calendars;
            if (TryFindCalendarsByTitle(result, out calendars))
            {
                if (calendars.Count == 1)
                {
                    someCalendar.FillEmprtyFields(calendars.FirstOrDefault());


                    calendarEntryes.Remove(calendars.FirstOrDefault());
                    calendarEntryes.Add(someCalendar);
                    MessagesController.refreshCalendar(context.Activity.From.Id, calendarEntryes);
                    string reply = $"\tCalendar entry:\n\n {calendars.FirstOrDefault()}\n\n\t changed to \n\n{someCalendar}";
                    await context.PostAsync(reply);
                    context.Wait(MessageReceived);
                }
                else
                {
                    PromptDialog.Choice(context, AfterMultiFindingReplace, calendars, "Which one?", "Dont`t understand", promptStyle: PromptStyle.Auto);
                }
            }
            else
            {
                await context.PostAsync("cant find such an entry");
            }
        }

        private async Task AfterMultiFindingReplace(IDialogContext context, IAwaitable<Calendar> result)
        {
            someCalendar.FillEmprtyFields(await result);

            calendarEntryes.Remove(await result);
            calendarEntryes.Add(someCalendar);
            MessagesController.refreshCalendar(context.Activity.From.Id, calendarEntryes);
            string reply = $"\tCalendar entry:\n\n {await result}\n\n\t changed to \n\n{someCalendar}";
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// check availability for some period of time
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.calendar.check_availability")]
        public async Task check_availability(IDialogContext context, LuisResult result)
        {
            try
            {
                EntityRecommendation start_date;
                if (!result.TryFindEntity(ResourcesManager.Entities.Calendar.start_date, out start_date))
                {
                    start_date = new EntityRecommendation(type: ResourcesManager.Entities.Calendar.start_date) { Entity = string.Empty };
                }
                EntityRecommendation start_time;
                if (!result.TryFindEntity(ResourcesManager.Entities.Calendar.start_time, out start_time))
                {
                    start_time = new EntityRecommendation(type: ResourcesManager.Entities.Calendar.start_time) { Entity = string.Empty };
                }
                var parser = new Chronic.Parser();
                var span = parser.Parse(start_date.Entity + " " + start_time.Entity);
                DateTime? startTime;
                DateTime? endTime;
                if (span != null)
                {
                    var start = span.Start;
                    var end = span.End;
                    startTime = start.Value < DateTime.Now.AddHours(ResourcesManager.Timezone) ? start.Value.AddDays(1) : start.Value;
                    endTime = start.Value < DateTime.Now.AddHours(ResourcesManager.Timezone) ? end.Value.AddDays(1) : end.Value;
                    List<Calendar> findedEntries = new List<Calendar>();
                    foreach (var entry in calendarEntryes)
                    {
                        if (entry.startTime >= endTime ||
                                entry.duration != null && entry.startTime.Value.Add(entry.duration.Value) <= startTime.Value ||
                                entry.endTime != null && entry.endTime.Value <= startTime.Value)
                        {

                        }
                        else
                        {
                            findedEntries.Add(entry);
                        }
                    }
                    if (findedEntries.Count == 0)
                    {
                        await context.PostAsync($"you are free from {start.Value} to {end.Value}");
                    }
                    else
                    {
                        string str = $"you have some plans on period from {start.Value} to {end.Value}:\n\n\n\n";
                        foreach (var entry in findedEntries)
                        {
                            str += entry + "\n\n";
                        }
                        await context.PostAsync(str);
                    }

                }
                else
                {
                    string reply = "When?";
                    await context.PostAsync(reply);
                }
                context.Wait(MessageReceived);
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// delete calendar entry
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.calendar.delete_calendar_entry")]
        public async Task delete_calendar_entry(IDialogContext context, LuisResult result)
        {
            EntityRecommendation titleEnt;
            if (result.TryFindEntity(ResourcesManager.Entities.Calendar.title, out titleEnt))
            {
                string title = titleEnt.Entity;
                List<Calendar> calendars;
                if (TryFindCalendarsByTitle(result, out calendars) && calendars.Count == 1)
                {

                    this.calendarEntryes.Remove(calendars.FirstOrDefault());
                    await context.PostAsync($"\tentry:\n\n{calendars.FirstOrDefault()}\n\n\tdeleted");
                    MessagesController.refreshCalendar(context.Activity.From.Id, calendarEntryes);
                }
                else
                {
                    PromptDialog.Choice(context, AfterMultiFindingDelete, calendars, "Which one?", "Dont`t understand", promptStyle: PromptStyle.Auto);
                    return;
                }
            }
            else
            {
                PromptDialog.Choice(context, AfterMultiFindingDelete, calendarEntryes, "Which one?", "Dont`t understand", promptStyle: PromptStyle.Auto);
                return;
            }
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// delete selected entry
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task AfterMultiFindingDelete(IDialogContext context, IAwaitable<Calendar> result)
        {
            calendarEntryes.Remove(await result);
            MessagesController.refreshCalendar(context.Activity.From.Id, calendarEntryes);
            string reply = $"\tCalendar entry:\n\n {await result}\n\n\t deleted";
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }

        Calendar someCalendar;
        /// <summary>
        /// list of calendar entries for current user
        /// </summary>
        public List<Calendar> calendarEntryes = new List<Calendar>();
    }
    /// <summary>
    /// calendar
    /// </summary>
    [Serializable]
    public class Calendar : IEquatable<Calendar>
    {
        /// <summary>
        /// service url
        /// </summary>
        public string ServiceUrl;
        /// <summary>
        /// bot name
        /// </summary>
        public string botName;
        /// <summary>
        /// bot id
        /// </summary>
        public string botId;
        /// <summary>
        /// is reminded
        /// </summary>
        public bool isReminded = false;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="message">message</param>
        public Calendar(IActivity message)
        {
            this.ServiceUrl = message.ServiceUrl;
            this.botId = message.Recipient.Id;
            this.botName = message.Recipient.Name;
        }
        /// <summary>
        /// location
        /// </summary>
        public string location { get; set; }
        /// <summary>
        /// title
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// start time
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// end time
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// duration
        /// </summary>
        public TimeSpan? duration { get; set; }
        /// <summary>
        /// destination calendar
        /// </summary>
        public string destinationCalendar { get; set; }
        /// <summary>
        /// check equality
        /// </summary>
        /// <param name="other">other alrm</param>
        /// <returns></returns>
        public bool Equals(Calendar other)
        {
            return other != null
                && this.location == other.location;
        }
        /// <summary>
        /// check equality
        /// </summary>
        /// <param name="other">other alrm</param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            return Equals(other as Calendar);
        }
        /// <summary>
        /// get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.startTime.GetHashCode();
        }
        /// <summary>
        /// convert to string
        /// </summary>
        /// <returns>converted string</returns>
        public override string ToString()
        {
            string str = "";
            if (!string.IsNullOrEmpty(title))
                str += "title:\t" + title + ";\n\n";
            if (startTime != null)
                str += "start time:\t" + startTime + ";\n\n";
            if (endTime != null)
                str += "end time:\t" + endTime + ";\n\n";
            if (duration != null)
                str += "duration:\t" + duration.Value.toStr() + ";\n\n";
            if (!string.IsNullOrEmpty(location))
                str += "location:\t" + location + ";\n\n";
            if (!string.IsNullOrEmpty(destinationCalendar))
                str += "name of calendar:\t" + destinationCalendar + ";\n\n";
            return str;
        }
        /// <summary>
        /// fill empty fields
        /// </summary>
        /// <param name="other"></param>
        public void FillEmprtyFields(Calendar other)
        {
            this.botId = string.IsNullOrEmpty(this.botId) ? other.botId : this.botId;
            this.botName = string.IsNullOrEmpty(this.botName) ? other.botName : this.botName;
            this.location = string.IsNullOrEmpty(this.location) ? other.location : this.location;
            this.ServiceUrl = string.IsNullOrEmpty(this.ServiceUrl) ? other.ServiceUrl : this.ServiceUrl;
            this.destinationCalendar = string.IsNullOrEmpty(this.destinationCalendar) ? other.destinationCalendar : this.destinationCalendar;
            this.title = string.IsNullOrEmpty(this.title) ? other.title : this.title;
            this.duration = duration == null ? other.duration : this.duration;
            this.endTime = endTime == null ? other.endTime : this.endTime;
            this.startTime = startTime == null ? other.startTime : this.startTime;
        }
    }
}
//implaments some additional methods for MessagesController
namespace BotAssistant
{
    public partial class MessagesController
    {
        /// <summary>
        /// collection of calendars for each user
        /// </summary>
        static Dictionary<string, List<Calendar>> calendarList = new Dictionary<string, List<Calendar>>();
        /// <summary>
        /// add or refresh list of calendars for current user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="calendars">calendar entries collection</param>
        public static void refreshCalendar(string userId, List<Calendar> calendars)
        {
            if (calendarList.ContainsKey(userId))
            {
                if (!calendarList[userId].SequenceEqual(calendars))
                {
                    calendarList.Remove(userId);
                    calendarList.Add(userId, calendars);
                }
            }
            else
            {
                calendarList.Add(userId, calendars);
            }
        }
        /// <summary>
        /// check for calendar entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static async void CheckCalendar(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var user in calendarList)
                {
                    foreach (var calendar in user.Value)
                    {
                        if (calendar.startTime <= DateTime.Now.AddHours(ResourcesManager.Timezone) && !calendar.isReminded)
                        {
                            var conv = Channel.fabricMethod(new ConnectorClient(new Uri(calendar.ServiceUrl)), user.Key);
                            await conv.sendNewMessageAcync(new ChannelAccount(user.Key),
                                new ChannelAccount(calendar.botId, calendar.botName),
                                DateTime.Now.AddHours(ResourcesManager.Timezone).ToShortTimeString());
                            calendar.isReminded = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}