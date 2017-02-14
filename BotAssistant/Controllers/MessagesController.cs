using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace BotAssistant
{
    /// <summary>
    /// Messages controller
    /// </summary>
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {
                if (activity.Type == ActivityTypes.Message)
                {
                    //if we recieve a message

                    //initialise connector
                    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    //create reply based on  received message
                    Activity reply = activity.CreateReply();
                    //we pretend that we print something
                    reply.Type = ActivityTypes.Typing;
                    //send reply to user
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    if (activity.Text == "/help")
                    {
                        //write help text
                        string text = "",
                            format3= "#### {0}\n\n*Exemple:*\n\n* {1}\n\n* {2}\n\n* {3}\n\n---\n\n",
                            format2= "#### {0}\n\n*Exemple:*\n\n* {1}\n\n* {2}\n\n---\n\n",
                            format1= "#### {0}\n\n*Exemple:*\n\n* {1}\n\n---\n\n";
                        text += "# Help\n\n## commands:\n\n---\n\n### register\n\n*Exemple:*\n\n* register me, my name is Vika, I am 22 years old, my post is a programmer\n\n* reg me\n\n* register me please\n\n---\n\n";
                        text += "### weather\n\n*Exemple:*\n\n* Is it snowing here in Kyiv?\n\n* What is forecast for next week?\n\n* What is the weather now?\n\n*Note:*\n\nif you are do not know name of city, you can send your location\n\n---\n\n";
                        text += string.Format("### Alarm:\n\n");
                        text += string.Format(format3, "Set alarm", "can you set an alarm for 12 called take antibiotics?", "set alarm at six o`clock", "set alarm at 6:00");
                        text += string.Format(format3, "Find alarm", "find take antibiotics alarm please", "find alarm", "can you find my alarm?");
                        text += string.Format(format3, "Delete alarm", "delete alarm", "delete my wake up alarm", "delete default alarm");
                        text += string.Format(format3, "Snooze", "snooze alarm", "snooze alarm for 5 minutes", "snooze my wake up alarm");
                        text += string.Format(format3, "Time remaining", "how much time until wake up alarm?", "how much longer do i have until wake up alarm?", "how soon my alarm?");
                        text += string.Format(format3, "Turn off alarm", "turn off my wake up alarm", "turn off my alarm", "turn off alarm");
                        text += string.Format(format3, "Show alarms", "show me alarms", "show alarms please", "show alarms");

                        text += string.Format("### Remind:\n\n");
                        text += string.Format(format3, "Create single reminder", "remind me to wake up at 7 am",
                            "remind me to go trick or treating with Luca at 4:40pm", "remind me to wash my car at morning");
                        text += string.Format(format2, "Change reminder", "move my wash my car reminder to evening",
                            "move my reminder about school from monday to tuesday");
                        text += string.Format(format1, "Delete reminder", "delete my picture reminder" );
                        text += string.Format(format1, "Find reminder", "show me wash my car reminder");
                        text += string.Format(format1, "Snooze", "snooze wake up reminder");
                        text += string.Format(format2, "Turn off reminder", "dismiss airport pick up reminder", "turn off picture reminder");
                        text += string.Format(format3, "Show reminds", "show me reminds", "show reminds please", "show reminds");
                        text += string.Format("### Calendar:\n\n");
                        text += string.Format(format3, "Create calendar entry", "create a calendar entry called vacation from tomorrow until next monday",
                            "make an appointment with lisa at 2pm on sunday at 123 main street",
                            "book a 2 hour meeting with joe");
                        text += string.Format(format3, "Change calendar entry", "change an appointment",
                            "move my meeting from today to tomorrow", "move my meeting with ken from monday to tuesday");
                        text += string.Format(format2, "Find calendar entry", "find my meeting with joe", "show my calendar at tomorrow");
                        text += string.Format(format3, "Show calendar", "show me calendar", "show calendar please", "show calendar");
                        text += string.Format("### My links:\n\n");
                        text += string.Format(format3, "Add link",
                            "save link LUIS https://www.luis.ai/",
                            "add new link LUIS https://www.luis.ai/",
                            "save to my links LUIS https://www.luis.ai/");
                        text += string.Format(format2, "Delete link", "delete LUIS link",
                            "remove LUIS link");
                        text += string.Format(format3, "Show links", "show me my links", "show me links please", "show links");
                        //make reply with help text
                        reply = activity.CreateReply(text);
                        //send message
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    else if (activity.Text == "/delete_user_data")
                    {
                        StateClient sc = activity.GetStateClient();
                        await sc.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                        reply = activity.CreateReply("Your data is deleted");
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    else if (activity.Text == "/start")
                    {
                        reply = activity.CreateReply("Welcome to the Bot Assistant");
                        //attach the gif animation
                        reply.Attachments.Add(new Attachment()
                        {
                            ContentUrl = "http://coolday.today/uploads/media/news/0001/07/7a1d2d6897b369cb731041bc2481dfe7a58cc6e4.jpeg",
                            ContentType = "image/jpeg",
                            Name = "Your assistant Darth Vader.\nListen to you!"
                        });
                        //send message
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    //handle system message
                    
                    HandleSystemMessage(activity);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch (Exception ex)
            {
                Activity reply = activity.CreateReply(ex.Message);
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                await connector.Conversations.ReplyToActivityAsync(reply);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return response;
            }
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}