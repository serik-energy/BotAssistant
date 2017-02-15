using BotAssistant.Core;
using BotAssistant.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotAssistant.Dialogs
{
    public partial class BotDialog : LuisDialog<object>
    {
        /// <summary>
        /// current user
        /// </summary>
        private User currentUser;
        /// <summary>
        /// register new user
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="item">message</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("register")]
        public async Task Register(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            string name = "", post = "", age = "";
            EntityRecommendation entity;
            if (result.TryFindEntityDeep("name", out entity))
            {
                name = entity.Entity;
            }
            if (result.TryFindEntityDeep("post", out entity))
            {
                post = entity.Entity;
            }
            if (result.TryFindEntityDeep("builtin.age", out entity))
            {
                age = entity.Entity;
            }
            currentUser = new User(name[0].ToString().ToUpper() + name.Remove(0, 1), post, age);
            MessagesController.refreshUsers(context.Activity.From.Id, currentUser);
            await context.PostAsync($"user information was updated\n\n{currentUser}");
            context.Wait(MessageReceived);
        }
        /// <summary>
        /// add new link
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="item">message</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("save_link")]
        public async Task SaveLink(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            string name = "", link = "";
            EntityRecommendation linkEnt;
            EntityRecommendation nameEnt;
            if (result.TryFindEntityDeep("site_name", out nameEnt) && result.TryFindEntityDeep("builtin.url", out linkEnt))
            {
                name = nameEnt.Entity;
                link = linkEnt.Entity;
                if (currentUser != null)
                    currentUser.myLinks.Add(name, link);
                else
                    await context.PostAsync("only registered users can save links!");
                await context.PostAsync($"link {name} {link} added");
                MessagesController.refreshUsers(context.Activity.From.Id, currentUser);
            }
            else
            {
                await context.PostAsync("can`t find name or link");
            }


            context.Wait(MessageReceived);
        }
        /// <summary>
        /// delete link
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="item">message</param>
        /// <param name="result">result from Luis app</param>
        /// <returns></returns>
        [LuisIntent("delete_link")]
        public async Task DeleteLink(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            if (currentUser == null)
            {
                await context.PostAsync("only registered users can save links!");
                context.Wait(MessageReceived);
                return;
            }

            EntityRecommendation nameEnt;
            string name, link;
            if (result.TryFindEntityDeep("site_name", out nameEnt))
            {
                name = nameEnt.Entity;
                if (currentUser.myLinks.TryGetValue(name, out link))
                {
                    currentUser.myLinks.Remove(name);
                    MessagesController.refreshUsers(context.Activity.From.Id, currentUser);
                    await context.PostAsync($"link {name} {link} deleted");
                }
                else
                {
                    await context.PostAsync($"can`t find link {name}");
                }
                context.Wait(MessageReceived);
            }
            else
            {
                PromptDialog.Choice(context, AfterConfirming_DeleteLink, currentUser.myLinks, "Witch one?", "don`t understand", promptStyle: PromptStyle.Auto);
            }
        }
        /// <summary>
        /// confirm deleting link
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <param name="result">user choice</param>
        /// <returns></returns>
        private async Task AfterConfirming_DeleteLink(IDialogContext context, IAwaitable<KeyValuePair<string, string>> result)
        {
            var res = await result;
            currentUser.myLinks.Remove(res.Key);
            await context.PostAsync($"link {res.Key} {res.Value} deleted");
            MessagesController.refreshUsers(context.Activity.From.Id, currentUser);
            //throw new NotImplementedException();
            context.Wait(MessageReceived);
        }
    }
    /// <summary>
    /// user data
    /// </summary>
    [Serializable]
    public class User : IEquatable<User>
    {
        /// <summary>
        /// user age
        /// </summary>
        public string age;
        /// <summary>
        /// user name
        /// </summary>
        public string name;
        /// <summary>
        /// user post
        /// </summary>
        public string post;
        /// <summary>
        /// user links
        /// </summary>
        public Dictionary<string, string> myLinks = new Dictionary<string, string>();
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">user name</param>
        /// <param name="post">user post</param>
        /// <param name="age">user age</param>
        public User(string name, string post, string age)
        {
            this.name = name;
            this.post = post;
            this.age = age;
            myLinks.Add("habrahabr", "https://habrahabr.ru/");
            myLinks.Add("luis", "https://www.luis.ai/");
            myLinks.Add("botframework", "https://dev.botframework.com/");
        }
        /// <summary>
        /// check equality
        /// </summary>
        /// <param name="other">other alrm</param>
        /// <returns></returns>
        public bool Equals(User other)
        {
            return other != null
                && this.age == other.age
                && this.name == other.name
                && this.post == other.post;
        }
        /// <summary>
        /// check equality
        /// </summary>
        /// <param name="other">other alrm</param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            return Equals(other as User);
        }
        /// <summary>
        /// get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }
        /// <summary>
        /// convert to string
        /// </summary>
        /// <returns>converted string</returns>
        public override string ToString()
        {
            return $"name: {name}, post: {post}, age: {age}";
        }
    }
}
//implaments some additional methods for MessagesController
namespace BotAssistant
{
    public partial class MessagesController
    {
        /// <summary>
        /// users collection
        /// </summary>
        public static Dictionary<string, User> userList = new Dictionary<string, User>();
        /// <summary>
        /// add or refresh user list
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="user">alarm collection</param>
        public static void refreshUsers(string userId, User user)
        {
            if (userList.ContainsKey(userId))
            {
                if (userList[userId] != user)
                {
                    userList.Remove(userId);
                    userList.Add(userId, user);
                }
            }
            else
            {
                userList.Add(userId, user);
            }
        }
    }
}