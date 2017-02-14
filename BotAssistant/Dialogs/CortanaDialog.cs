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
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace BotAssistant.Dialogs
{
    /// <summary>
    /// class that communicate with pre-built Luis Cortana app
    /// </summary>
    [LuisModel(ResourcesManager.CortanaAppID, ResourcesManager.LUISSubscriptionKey, LuisApiVersion.V2)]
    [Serializable]
    public partial class CortanaDialog : LuisDialog<LuisResult>
    {
        /// <summary>
        /// receive result from Cortana, and return it to previous dialog
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="item">message</param>
        /// <param name="result">result from luis</param>
        /// <returns></returns>
        [LuisIntent("")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            context.Done(result);
        }
        /// <summary>
        /// constructor
        /// </summary>
        public CortanaDialog() { }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="service">luis service</param>
        public CortanaDialog(ILuisService service) : base(service) { }
    }
}