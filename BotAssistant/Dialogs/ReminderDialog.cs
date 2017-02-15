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
        /// reminds collection
        /// </summary>
        Dictionary<string, Remind> remindByText = new Dictionary<string, Remind>();
        /// <summary>
        /// try find remind by title from LuisResult
        /// </summary>
        /// <param name="result">result of Luis</param>
        /// <param name="remind">return found remind</param>
        /// <returns>true, if remind found</returns>
        public bool TryFindRemind(LuisResult result, out Remind remind)
        {
            remind = null;
            string name = "";
            EntityRecommendation text;
            if (result.TryFindEntity(ResourcesManager.Entities.Reminder.Reminder_text, out text))
            {
                name = text.Entity;
            }
            return this.remindByText.TryGetValue(name, out remind);
        }
        /// <summary>
        /// create reminder
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.reminder.create_single_reminder")]
        public async Task Create_single_reminder(IDialogContext context, LuisResult result)
        {
            EntityRecommendation reminderText;
            if (!result.TryFindEntity(ResourcesManager.Entities.Reminder.Reminder_text, out reminderText))
            {
                await context.PostAsync("Remind what?");
            }
            EntityRecommendation date;
            if (!result.TryFindEntity(ResourcesManager.Entities.Reminder.Start_date, out date))
            {
                date = new EntityRecommendation(type: ResourcesManager.Entities.Reminder.Start_date) { Entity = string.Empty };
            }
            EntityRecommendation time;
            if (!result.TryFindEntity(ResourcesManager.Entities.Reminder.Start_time, out time))
            {
                time = new EntityRecommendation(type: ResourcesManager.Entities.Reminder.Start_time) { Entity = string.Empty };
            }
            var parser = new Chronic.Parser();
            var span = parser.Parse(date.Entity + " " + time.Entity);
            if (span != null && reminderText != null)
            {
                var when = span.Start ?? span.End;
                var remind = new Remind(context.Activity) { Text = reminderText.Entity, When = when.Value < DateTime.Now.AddHours(ResourcesManager.Timezone) ? when.Value.AddDays(1) : when.Value };
                this.remindByText[remind.Text] = remind;
                string reply = $"remind {remind} created";
                await context.PostAsync(reply);
                MessagesController.refreshRemind(context.Activity.From.Id, remindByText);
            }
            else
            {
                await context.PostAsync("could not find time when remind");
            }
            context.Wait(MessageReceived);
        }
        Remind newRemind;
        /// <summary>
        /// change reminder
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.reminder.change_reminder")]
        public async Task Change_reminder(IDialogContext context, LuisResult result)
        {
            Remind remind;
            if (TryFindRemind(result, out remind))
            {
                EntityRecommendation date;
                if (!result.TryFindEntity(ResourcesManager.Entities.Reminder.Start_date, out date))
                {
                    date = new EntityRecommendation(type: ResourcesManager.Entities.Reminder.Start_date) { Entity = string.Empty };
                }
                EntityRecommendation time;
                if (!result.TryFindEntity(ResourcesManager.Entities.Reminder.Start_time, out time))
                {
                    time = new EntityRecommendation(type: ResourcesManager.Entities.Reminder.Start_time) { Entity = string.Empty };
                }
                EntityRecommendation origDate;
                if (!result.TryFindEntity(ResourcesManager.Entities.Reminder.Original_start_date, out origDate))
                {
                    origDate = new EntityRecommendation(type: ResourcesManager.Entities.Reminder.Original_start_date) { Entity = string.Empty };
                }
                EntityRecommendation origStartTime;
                if (!result.TryFindEntity(ResourcesManager.Entities.Reminder.Original_start_time, out origStartTime))
                {
                    origStartTime = new EntityRecommendation(type: ResourcesManager.Entities.Reminder.Original_start_time) { Entity = string.Empty };
                }

                var parser = new Chronic.Parser();
                var span = parser.Parse(date.Entity + " " + time.Entity);
                var origSpan = parser.Parse(origDate.Entity + " " + origStartTime.Entity);
                if (span != null && origSpan == null)
                {
                    var when = span.Start ?? span.End;
                    newRemind = new Remind(context.Activity) { Text = remind.Text, When = when.Value < DateTime.Now.AddHours(ResourcesManager.Timezone) ? when.Value.AddDays(1) : when.Value };
                    this.remindByText[remind.Text] = newRemind;
                    string reply = $"remind {remind} changed to {newRemind}";
                    await context.PostAsync(reply);
                    MessagesController.refreshRemind(context.Activity.From.Id, remindByText);
                }
                else if (span != null && origSpan != null)
                {
                    var when = span.Start ?? span.End;
                    newRemind = new Remind(context.Activity) { Text = remind.Text, When = when.Value };
                    if (remind.When >= origSpan.Start && remind.When <= origSpan.End)
                    {
                        this.remindByText[remind.Text] = newRemind;
                        string reply = $"remind {remind} changed to {newRemind}";
                        await context.PostAsync(reply);
                    }
                    else
                    {
                        PromptDialog.Confirm(context, AfterConfirming_ChangeRemind, $"remind {remind.Text} is set on {remind.When}. Are you want to change it?", promptStyle: PromptStyle.Auto);
                        return;
                    }

                }
            }
            else
            {
                await context.PostAsync("did not find remind");
            }
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// complete changing reminder
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">user answer</param>
        /// <returns></returns>
        private async Task AfterConfirming_ChangeRemind(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                string reply = $"remind { this.remindByText[newRemind.Text]} changed to {newRemind}";
                this.remindByText[newRemind.Text] = newRemind;
                await context.PostAsync(reply);
            }
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// show reminder by title from LuisResult
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.reminder.find_reminder")]
        public async Task Find_reminder(IDialogContext context, LuisResult result)
        {
            Remind remind;
            if (TryFindRemind(result, out remind))
            {
                await context.PostAsync($"found remind {remind}");
            }
            else
            {
                await context.PostAsync("did not find remind");
            }
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// delete reminder
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.reminder.delete_reminder")]
        public async Task Delete_reminder(IDialogContext context, LuisResult result)
        {
            Remind remind;
            if (TryFindRemind(result, out remind))
            {
                this.remindByText.Remove(remind.Text);
                await context.PostAsync($"remind {remind} deleted");
                MessagesController.refreshRemind(context.Activity.From.Id, remindByText);
                context.Wait(MessageReceived);
            }
            else
            {
                PromptDialog.Choice(context, AfterConfirming_DeleteRemind, remindByText, "wich one?", "don`t understand");
            }
        }
        /// <summary>
        /// delete chosen reminder
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">user choice</param>
        /// <returns></returns>
        private async Task AfterConfirming_DeleteRemind(IDialogContext context, IAwaitable<KeyValuePair<string, Remind>> result)
        {
            var res = await result;
            this.remindByText.Remove(res.Key);
            await context.PostAsync($"remind {res.Value} deleted");
            MessagesController.refreshRemind(context.Activity.From.Id, remindByText);
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// snooze reminder
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.reminder.snooze")]
        public async Task ReminderSnooze(IDialogContext context, LuisResult result)
        {
            Remind remind;
            if (TryFindRemind(result, out remind))
            {
                remind.When = remind.When.Add(TimeSpan.FromHours(1));
                await context.PostAsync($"remind {remind} snoozed!");
            }
            else
            {
                await context.PostAsync("did not find remind");
            }
            MessagesController.refreshRemind(context.Activity.From.Id, remindByText);
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// turn off reminder
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("builtin.intent.reminder.turn_off_reminder")]
        public async Task Turn_off_reminder(IDialogContext context, LuisResult result)
        {
            if (TryFindRemind(result, out this.newRemind))
            {
                PromptDialog.Confirm(context, AfterConfirming_TurnOffRemind, "Are you sure?", promptStyle: PromptStyle.Auto);
            }
            else
            {
                await context.PostAsync("did not find remind");
                context.Wait(MessageReceived);
            }
        }
        /// <summary>
        /// checking user confirmation for turn off reminder
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="confirmation">user confirmation</param>
        /// <returns></returns>
        private async Task AfterConfirming_TurnOffRemind(IDialogContext context, IAwaitable<bool> confirmation)
        {
            if (await confirmation)
            {
                this.remindByText.Remove(this.newRemind.Text);
                await context.PostAsync($"Ok, remind {this.newRemind} disabled.");
                MessagesController.refreshRemind(context.Activity.From.Id, remindByText);
            }
            else
            {
                await context.PostAsync("Ok! We haven't modified your reminds!");
            }
            context.Wait(MessageReceived);
        }
    }
    /// <summary>
    /// reminder
    /// </summary>
    [Serializable]
    public class Remind : IEquatable<Remind>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="message">message</param>
        public Remind(IActivity message)
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
        /// is reminded
        /// </summary>
        public bool isReminded = false;
        /// <summary>
        /// convert to string
        /// </summary>
        /// <returns>converted string</returns>
        public override string ToString()
        {
            return Text + " at " + When.ToString();
        }
        /// <summary>
        /// text of remind
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// time
        /// </summary>
        public DateTime When { get; set; }
        /// <summary>
        /// check equality
        /// </summary>
        /// <param name="other">other alrm</param>
        /// <returns></returns>
        public bool Equals(Remind other)
        {
            return other != null
                && this.When == other.When
                && this.Text == other.Text
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
            return Equals(other as Remind);
        }
        /// <summary>
        /// get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Text.GetHashCode();
        }
    }
}

//implaments some additional methods for MessagesController
namespace BotAssistant
{
    public partial class MessagesController
    {
        /// <summary>
        /// collection of all reminds for each user
        /// </summary>
        static Dictionary<string, Dictionary<string, Remind>> remindList = new Dictionary<string, Dictionary<string, Remind>>();
        /// <summary>
        /// add or refresh list of reminds for current user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="reminds">remind collection</param>
        public static void refreshRemind(string userId, Dictionary<string, Remind> reminds)
        {
            if (remindList.ContainsKey(userId))
            {
                if (!remindList[userId].SequenceEqual(reminds))
                {
                    remindList.Remove(userId);
                    remindList.Add(userId, reminds);
                }
            }
            else
            {
                remindList.Add(userId, reminds);
            }
        }
        /// <summary>
        /// waiting for remind
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static async void CheckRemind(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var user in remindList)
                {
                    foreach (var remind in user.Value)
                    {
                        if (remind.Value.When <= DateTime.Now.AddHours(ResourcesManager.Timezone) && !remind.Value.isReminded)
                        {
                            var conv = Channel.fabricMethod(new ConnectorClient(new Uri(remind.Value.ServiceUrl)),
                                user.Key);
                            await conv.sendNewMessageAcync(new ChannelAccount(user.Key),
                                new ChannelAccount(remind.Value.botId, remind.Value.botName),
                                DateTime.Now.AddHours(ResourcesManager.Timezone).ToShortTimeString());
                            remind.Value.isReminded = true;
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