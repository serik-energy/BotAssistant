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
        /// list of alarms for current user
        /// </summary>
        private readonly Dictionary<string, Alarm> alarmByWhat = new Dictionary<string, Alarm>();
        /// <summary>
        /// try find alarm by title from LuisResult
        /// </summary>
        /// <param name="result">result of Luis</param>
        /// <param name="alarm">return found alarm</param>
        /// <returns>true, if alarm found</returns>
        public bool TryFindAlarm(LuisResult result, out Alarm alarm)
        {
            alarm = null;
            string what;
            EntityRecommendation title;
            if (result.TryFindEntity(ResourcesManager.Entities.Alarm.Title, out title))
            {
                what = title.Entity;
            }
            else
            {
                what = ResourcesManager.DefaultAlarmWhat;
            }
            return this.alarmByWhat.TryGetValue(what, out alarm);
        }

        /// <summary>
        /// delete alarm by title from LuisResult
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.alarm.delete_alarm")]
        public async Task DeleteAlarm(IDialogContext context, LuisResult result)
        {
            Alarm alarm;
            if (TryFindAlarm(result, out alarm))
            {
                this.alarmByWhat.Remove(alarm.What);
                await context.PostAsync($"alarm {alarm} deleted");
                MessagesController.refreshAlarms(context.Activity.From.Id, alarmByWhat);
            }
            else
            {
                await context.PostAsync("did not find alarm");
            }
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// show alarm by title from LuisResult
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.alarm.find_alarm")]
        public async Task FindAlarm(IDialogContext context, LuisResult result)
        {
            Alarm alarm;
            if (TryFindAlarm(result, out alarm))
            {
                await context.PostAsync($"found alarm {alarm}");
            }
            else
            {
                await context.PostAsync("did not find alarm");
            }
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// set alarm by data from LuisResult
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.alarm.set_alarm")]
        public async Task SetAlarm(IDialogContext context, LuisResult result)
        {
            EntityRecommendation title;
            if (!result.TryFindEntity(ResourcesManager.Entities.Alarm.Title, out title))
            {
                title = new EntityRecommendation(type: ResourcesManager.Entities.Alarm.Title) { Entity = ResourcesManager.DefaultAlarmWhat };
            }
            EntityRecommendation date;
            if (!result.TryFindEntity(ResourcesManager.Entities.Alarm.Start_Date, out date))
            {
                date = new EntityRecommendation(type: ResourcesManager.Entities.Alarm.Start_Date) { Entity = string.Empty };
            }
            EntityRecommendation time;
            if (!result.TryFindEntity(ResourcesManager.Entities.Alarm.Start_Time, out time))
            {
                time = new EntityRecommendation(type: ResourcesManager.Entities.Alarm.Start_Time) { Entity = string.Empty };
            }
            var parser = new Chronic.Parser();
            var span = parser.Parse(date.Entity + " " + time.Entity);
            if (span != null)
            {
                var when = span.Start ?? span.End;
                var alarm = new Alarm(context.Activity) { What = title.Entity, When = when.Value < DateTime.Now.AddHours(ResourcesManager.Timezone) ? when.Value.AddDays(1) : when.Value };
                this.alarmByWhat[alarm.What] = alarm;
                string reply = $"alarm {alarm} created";
                await context.PostAsync(reply);
                MessagesController.refreshAlarms(context.Activity.From.Id, alarmByWhat);
            }
            else
            {
                await context.PostAsync("could not find time for alarm");
            }
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// find alarm by title from LuisResult and snoozing it
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.alarm.snooze")]
        public async Task AlarmSnooze(IDialogContext context, LuisResult result)
        {
            Alarm alarm;
            if (TryFindAlarm(result, out alarm))
            {
                EntityRecommendation durEnt;
                if (result.TryFindEntityDeep(ResourcesManager.Entities.Alarm.Duration, out durEnt))
                {
                    alarm.When = alarm.When.Add(durEnt.GetDuration().Value);
                    await context.PostAsync($"alarm {alarm} snoozed!");

                }
                else
                {
                    alarm.When = alarm.When.Add(TimeSpan.FromMinutes(7));
                    await context.PostAsync($"alarm {alarm} snoozed!");
                }
            }
            else
            {
                await context.PostAsync("did not find alarm");
            }
            MessagesController.refreshAlarms(context.Activity.From.Id, alarmByWhat);
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// show time remaining for alarm
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.alarm.time_remaining")]
        public async Task TimeRemaining(IDialogContext context, LuisResult result)
        {
            Alarm alarm;
            if (TryFindAlarm(result, out alarm))
            {
                var now = DateTime.Now.AddHours(ResourcesManager.Timezone);
                if (alarm.When > now)
                {
                    var remaining = alarm.When.Subtract(DateTime.Now);
                    await context.PostAsync($"There is {remaining} remaining for alarm {alarm}.");
                }
                else
                {
                    await context.PostAsync($"The alarm {alarm} expired already.");
                }
            }
            else
            {
                await context.PostAsync("did not find alarm");
            }
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// turn off alarm from list
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.alarm.turn_off_alarm")]
        public async Task TurnOffAlarm(IDialogContext context, LuisResult result)
        {
            if (alarmByWhat.Count > 0)
            {
                //run subdialog to select alarm
                PromptDialog.Choice(context, AfterConfirming_TurnOffAlarm, alarmByWhat, "Wich one?", "don`t understend", promptStyle: PromptStyle.Auto);
            }
            else
            {
                await context.PostAsync("did not find alarm");
                context.Wait(MessageReceived);
            }
        }
        /// <summary>
        /// process the results of subdialog
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="confirmation">chosen alarm</param>
        /// <returns></returns>
        public async Task AfterConfirming_TurnOffAlarm(IDialogContext context, IAwaitable<KeyValuePair<string, Alarm>> confirmation)
        {
            var al = await confirmation;
            this.alarmByWhat.Remove(al.Key);
            await context.PostAsync($"Ok, alarm {al.Value} disabled.");
            MessagesController.refreshAlarms(context.Activity.From.Id, alarmByWhat);
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// if not understand
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.alarm.alarm_other")]
        public async Task AlarmOther(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("what ?");
            context.Wait(MessageReceived);
        }
    }
    /// <summary>
    /// Alarm
    /// </summary>
    [Serializable]
    public sealed class Alarm : IEquatable<Alarm>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="message">message</param>
        public Alarm(IActivity message)
        {
            this.ServiceUrl = message.ServiceUrl;
            this.botId = message.Recipient.Id;
            this.botName = message.Recipient.Name;
        }
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
        /// time
        /// </summary>
        public DateTime When { get; set; }
        /// <summary>
        /// title
        /// </summary>
        public string What { get; set; }
        /// <summary>
        /// convert to string
        /// </summary>
        /// <returns>converted string</returns>
        public override string ToString()
        {
            return $"[{this.What} at {this.When}]";
        }

        /// <summary>
        /// check equality
        /// </summary>
        /// <param name="other">other alrm</param>
        /// <returns></returns>
        public bool Equals(Alarm other)
        {
            return other != null
                && this.When == other.When
                && this.What == other.What
                && this.ServiceUrl == other.ServiceUrl
                && this.botId == other.botId
                && this.botName == other.botName;
        }
        /// <summary>
        /// check equality
        /// </summary>
        /// <param name="other">other alrm</param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            return Equals(other as Alarm);
        }
        /// <summary>
        /// get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.What.GetHashCode();
        }
    }
}

//implaments some additional methods for MessagesController
namespace BotAssistant
{
    public partial class MessagesController
    {
        /// <summary>
        /// collection of all alarms for each user
        /// </summary>
        static Dictionary<string, Dictionary<string, Alarm>> alarmList = new Dictionary<string, Dictionary<string, Alarm>>();
        /// <summary>
        /// add or refresh list of alarms for current user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="alarms">alarm collection</param>
        public static void refreshAlarms(string userId, Dictionary<string, Alarm> alarms)
        {
            if (alarmList.ContainsKey(userId))
            {
                if (!alarmList[userId].SequenceEqual(alarms))
                {
                    alarmList.Remove(userId);
                    alarmList.Add(userId, alarms);
                }
            }
            else
            {
                alarmList.Add(userId, alarms);
            }
        }
        /// <summary>
        /// waiting for alarm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static async void checkAlarm(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var user in alarmList)
                {
                    foreach (var alarm in user.Value)
                    {
                        if (alarm.Value.When <= DateTime.Now.AddHours(ResourcesManager.Timezone))
                        {
                            var conv = Channel.fabricMethod(new ConnectorClient(new Uri(alarm.Value.ServiceUrl)), user.Key);
                            await conv.sendNewMessageAcync(new ChannelAccount(user.Key),
                                new ChannelAccount(alarm.Value.botId, alarm.Value.botId),
                                DateTime.Now.AddHours(ResourcesManager.Timezone).ToShortTimeString());
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