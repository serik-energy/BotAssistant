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
                case "skype":
                    return new ChannelSkype(connector);
                case "email":
                    return new ChannelEmail(connector);
                case "groupme":
                    return new ChannelGroupMe(connector);
                case "kik":
                    return new ChannelKik(connector);
                case "slack":
                    return new ChannelSlack(connector);
                case "twilio":
                    return new ChannelTwilio(connector);
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

    public class ChannelTwilio : Channel
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connector">connector client</param>
        public ChannelTwilio(ConnectorClient connector) : base(connector) { }
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

    public class ChannelSlack : Channel
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connector">connector client</param>
        public ChannelSlack(ConnectorClient connector) : base(connector) { }
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

    public class ChannelGroupMe : Channel
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connector">connector client</param>
        public ChannelGroupMe(ConnectorClient connector) : base(connector) { }
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

    public class ChannelKik : Channel
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connector">connector client</param>
        public ChannelKik(ConnectorClient connector) : base(connector) { }
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

    public class ChannelEmail : Channel
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connector">connector client</param>
        public ChannelEmail(ConnectorClient connector) : base(connector) { }
    }

    public class ChannelSkype : Channel
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connector">connector client</param>
        public ChannelSkype(ConnectorClient connector) : base(connector) { }
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