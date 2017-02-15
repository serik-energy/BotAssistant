using BotAssistant.Core;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BotAssistant.Dialogs
{
    /// <summary>
    /// class that communicate with Luis app
    /// </summary>
    [LuisModel(ResourcesManager.LUISAppID, ResourcesManager.LUISSubscriptionKey, LuisApiVersion.V2)]
    [Serializable]
    public partial class BotDialog : LuisDialog<object>
    {
        /// <summary>
        /// if Luis app have not such intent, try to forward message to cortana pre-built app
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="item">message</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            CortanaDialog childDialog = new CortanaDialog();
            await context.Forward(childDialog, CortanaDialogFinished, await item, context.CancellationToken);
        }
        /// <summary>
        /// receiving result from Luis Cortana pre-built app and try to process it again
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="awaitableResult">result from Luis Cortana pre-built app</param>
        /// <returns></returns>
        private async Task CortanaDialogFinished(IDialogContext context, IAwaitable<LuisResult> awaitableResult)
        {
            LuisResult result = await awaitableResult;
            IntentActivityHandler handler = null;
            if (result == null ||
                !this.handlerByIntent.TryGetValue(BestIntentFrom(result).Intent, out handler))
            {
                handler = this.handlerByIntent["else"];
            }

            if (handler != null)
            {
                await handler(context, Awaitable.FromItem(context.Activity.AsMessageActivity()), result);
            }
            else
            {
                var text = $"No default intent handler found.";
                throw new Exception(text);
            }
        }
        /// <summary>
        /// when can't process such intent, sending error message
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="item">message</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("else")]
        public async Task Else(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            string message = $"Sorry I did not understand: " +
                 (string.Join(", ", result.Intents.Select(i => i.Intent)) +
                 "\n\n\t" + result.Query);
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// receive message
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="item">message</param>
        /// <returns></returns>
        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {

            var message = await item;
            if (currentUser == null)
            {
                await context.PostAsync("First of all, you must register yourself");
            }
            MessagesController.refreshAlarms(message.From.Id, alarmByWhat);
            MessagesController.refreshCalendar(message.From.Id, calendarEntryes);
            MessagesController.refreshRemind(message.From.Id, remindByText);
            MessagesController.refreshUsers(message.From.Id, currentUser);
            await base.MessageReceived(context, item);
        }
        /// <summary>
        /// show all personal alarms, reminders, calendar entries and links
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="item">message</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("showall")]
        public async Task ShowAll(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            string reply = "";
            EntityRecommendation ent;
            if (result.TryFindEntityDeep("ev", out ent))
            {
                switch (ent.Entity)
                {
                    case "reminds":
                        foreach (var remind in remindByText)
                        {
                            reply += remind.Value + "\n\n----------\n\n";
                        }
                        break;
                    case "alarms":
                        foreach (var alarm in alarmByWhat)
                        {
                            reply += alarm.Value + "\n\n----------\n\n";
                        }
                        break;
                    case "calendar":
                        foreach (var calendar in calendarEntryes)
                        {
                            reply += calendar + "\n\n----------\n\n";
                        }
                        break;
                    case "links":
                        foreach (var link in currentUser.myLinks)
                        {
                            reply += "*" + link.Key + "*\n\n" + link.Value + "\n\n----------\n\n";
                        }
                        break;
                    default:
                        reply += "---\n\n**Reminds:**\n\n---\n\n ";
                        foreach (var remind in remindByText)
                        {
                            reply += remind.Value + "\n\n----------\n\n";
                        }
                        reply += "---\n\n**Alarms:**\n\n---\n\n ";
                        foreach (var alarm in alarmByWhat)
                        {
                            reply += alarm.Value + "\n\n----------\n\n";
                        }
                        reply += "---\n\n**Calendar entries:**\n\n---\n\n ";
                        foreach (var calendar in calendarEntryes)
                        {
                            reply += calendar + "\n\n----------\n\n";
                        }
                        reply += "---\n\n**My Links:**\n\n---\n\n ";
                        if (currentUser != null)
                            foreach (var link in currentUser.myLinks)
                            {
                                reply += "*" + link.Key + "*\n\n" + link.Value + "\n\n----------\n\n";
                            }
                        break;
                }
            }
            else
            {
                reply += "---\n\n**Reminds:**\n\n---\n\n ";
                foreach (var remind in remindByText)
                {
                    reply += remind.Value + "\n\n----------\n\n";
                }
                reply += "---\n\n**Alarms:**\n\n---\n\n ";
                foreach (var alarm in alarmByWhat)
                {
                    reply += alarm.Value + "\n\n----------\n\n";
                }
                reply += "---\n\n**Calendar entries:**\n\n---\n\n ";
                foreach (var calendar in calendarEntryes)
                {
                    reply += calendar + "\n\n----------\n\n";
                }
                reply += "---\n\n**My Links:**\n\n---\n\n ";
                if (currentUser != null)
                    foreach (var link in currentUser.myLinks)
                    {
                        reply += "*" + link.Key + "*\n\n" + link.Value + "\n\n----------\n\n";
                    }
            }
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// constructor
        /// </summary>
        public BotDialog() { }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="service">Luis service</param>
        public BotDialog(ILuisService service) : base(service) { }
    }
}