using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotAssistant.Core
{
    /// <summary>
    /// here implement some methods to work with messages
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// connector client
        /// </summary>
        protected ConnectorClient connector;

        /// <summary>
        /// create specific channel
        /// </summary>
        /// <param name="connector">connector client</param>
        /// <param name="ChannelId">id of channel</param>
        /// <returns></returns>
        public static Channel fabricMethod(ConnectorClient connector, string ChannelId)
        {
            //for each channel return his own inheritor of channel class
            switch (ChannelId)
            {
                case "telegram":
                    return new ChannelTelegram(connector);
                default:
                    return new Channel(connector);
            }
        }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connector">connector client</param>
        protected Channel(ConnectorClient connector)
        {
            this.connector = connector;
        }
        /// <summary>
        /// create a new conversation and send a new message
        /// </summary>
        /// <param name="User">user account</param>
        /// <param name="Bot">bot account</param>
        /// <param name="Text">message text</param>
        /// <returns>response</returns>
        public virtual async Task<ResourceResponse> sendNewMessageAcync(
            ChannelAccount User,
            ChannelAccount Bot,
            string Text)
        {
            var conversationId = await connector.Conversations.CreateDirectConversationAsync(Bot, User);
            IMessageActivity message = Activity.CreateMessageActivity();
            message.From = Bot;
            message.Recipient = User;
            message.Conversation = new ConversationAccount(id: conversationId.Id);
            message.Text = Text;
            message.Locale = "en-Us";
            return await connector.Conversations.SendToConversationAsync((Activity)message);
        }
    }
    /// <summary>
    /// Channel for Telegram
    /// </summary>
    public class ChannelTelegram : Channel
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connector">connector client</param>
        public ChannelTelegram(ConnectorClient connector) : base(connector) { }
        /// <summary>
        /// create a new conversation and send a new message
        /// </summary>
        /// <param name="User">user account</param>
        /// <param name="Bot">bot account</param>
        /// <param name="Text">message text</param>
        /// <returns>response</returns>
        public override Task<ResourceResponse> sendNewMessageAcync(ChannelAccount User, ChannelAccount Bot, string Text)
        {
            return base.sendNewMessageAcync(User, Bot, Text);
        }
    }
}