using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BotAssistant.Core;
using Microsoft.Bot.Connector;

namespace BotAssistantTest
{
    [TestClass]
    public class ChannelTests
    {
        [TestMethod]
        public void fabricMethod_ReceiveTelegramChannelId_returnChannelTelegram()
        {
            // arrange
            ConnectorClient connector = new ConnectorClient(new Uri("http://google.com"));
            string channelId = "telegram";
            // act
            Channel ch = Channel.fabricMethod(connector,channelId);
            // assert
            Assert.IsInstanceOfType(ch,typeof(ChannelTelegram));
        }
        [TestMethod]
        public void fabricMethod_ReceiveOtherChannelId_returnChannel()
        {
            // arrange
            ConnectorClient connector = new ConnectorClient(new Uri("http://google.com"));
            string channelId = "zxb";
            // act
            Channel ch = Channel.fabricMethod(connector, channelId);
            // assert
            Assert.IsInstanceOfType(ch, typeof(Channel));
        }
        [TestMethod]
        public void fabricMethod_ReceiveSkypeChannelId_returnChannelSkype()
        {
            // arrange
            ConnectorClient connector = new ConnectorClient(new Uri("http://google.com"));
            string channelId = "skype";
            // act
            Channel ch = Channel.fabricMethod(connector, channelId);
            // assert
            Assert.IsInstanceOfType(ch, typeof(ChannelSkype));
        }
        [TestMethod]
        public void fabricMethod_ReceiveEmailChannelId_returnChannelEmail()
        {
            // arrange
            ConnectorClient connector = new ConnectorClient(new Uri("http://google.com"));
            string channelId = "email";
            // act
            Channel ch = Channel.fabricMethod(connector, channelId);
            // assert
            Assert.IsInstanceOfType(ch, typeof(ChannelEmail));
        }
        [TestMethod]
        public void fabricMethod_ReceiveGroupMeChannelId_returnChannelGroupMe()
        {
            // arrange
            ConnectorClient connector = new ConnectorClient(new Uri("http://google.com"));
            string channelId = "groupme";
            // act
            Channel ch = Channel.fabricMethod(connector, channelId);
            // assert
            Assert.IsInstanceOfType(ch, typeof(ChannelGroupMe));
        }
        [TestMethod]
        public void fabricMethod_ReceiveKikChannelId_returnChannelKik()
        {
            // arrange
            ConnectorClient connector = new ConnectorClient(new Uri("http://google.com"));
            string channelId = "kik";
            // act
            Channel ch = Channel.fabricMethod(connector, channelId);
            // assert
            Assert.IsInstanceOfType(ch, typeof(ChannelKik));
        }
        [TestMethod]
        public void fabricMethod_ReceiveOtherSlack_returnChannelSlack()
        {
            // arrange
            ConnectorClient connector = new ConnectorClient(new Uri("http://google.com"));
            string channelId = "slack";
            // act
            Channel ch = Channel.fabricMethod(connector, channelId);
            // assert
            Assert.IsInstanceOfType(ch, typeof(ChannelSlack));
        }
        [TestMethod]
        public void fabricMethod_ReceiveTwilioChannelId_returnChannelTwilio()
        {
            // arrange
            ConnectorClient connector = new ConnectorClient(new Uri("http://google.com"));
            string channelId = "twilio";
            // act
            Channel ch = Channel.fabricMethod(connector, channelId);
            // assert
            Assert.IsInstanceOfType(ch, typeof(ChannelTwilio));
        }
    }
}
