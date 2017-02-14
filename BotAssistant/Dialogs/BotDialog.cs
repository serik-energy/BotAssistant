using BotAssistant.Core;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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
    }
}