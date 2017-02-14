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
    [LuisModel(ResourcesManager.LUISAppID, ResourcesManager.LUISSubscriptionKey, LuisApiVersion.V2)]
    [Serializable]
    public partial class BotDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            string message = $"Sorry I did not understand: " +
                 (string.Join(", ", result.Intents.Select(i => i.Intent)) +
                 "\n\n\t" + result.Query);
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}