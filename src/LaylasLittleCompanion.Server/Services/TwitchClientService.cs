using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using MvcChatBot.Agent.Models;
using MvcChatBot.Models;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;


namespace MvcChatBot.Agent.Services
{
    public class TwitchClientService
    {
        private readonly TwitchClient _client;
        private readonly TwitchSettings _settings;
        private readonly HubConnection _connection;
        private readonly TrelloService _trelloService;
        private List<User> liveCodersTeamMembers;
        private List<string> welcomedMemberIds = new List<string>();


        public TwitchClientService(
            TwitchSettings settings,
            HubConnection connection,
            TrelloService trelloService)
        {
            _settings = settings;
            _connection = connection;
            _trelloService = trelloService;
            _connection.StartAsync();



            ConnectionCredentials credentials = new ConnectionCredentials(_settings.BotName, _settings.AuthToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            _client = new TwitchClient(customClient);
            _client.Initialize(credentials, _settings.Channel);
            _client.OnLog += Client_OnLog;
            _client.OnJoinedChannel += Client_OnJoinedChannel;
            _client.OnChatCommandReceived += Client_OnCommandReceived;
            _client.OnWhisperReceived += Client_OnWhisperReceived;
            _client.OnRaidNotification += Client_OnRaidNotification;
            _client.OnNewSubscriber += Client_OnNewSubscriber;
            _client.OnGiftedSubscription += Client_OnGiftSubscriber;
            _client.OnMessageReceived += Client_MessageReceived;

            _client.OnConnected += Client_OnConnected;

            _client.Connect();

        }
        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime}: {e.BotUsername} - {e.Data}");
        }
        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }
        private async void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            _client.SendMessage(e.Channel, "Hello lovelies, I'm Layla's little helper!");

            liveCodersTeamMembers = await GetTeamMembers("livecoders");
        }
        private void Client_MessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var userId = e.ChatMessage.UserId;
            var userDisplayName = e.ChatMessage.DisplayName;
            var username = e.ChatMessage.Username;
            if (e.ChatMessage.IsVip || e.ChatMessage.IsSubscriber || e.ChatMessage.IsModerator)
            {
                if (!welcomedMemberIds.Contains(userId))
                {
                    welcomedMemberIds.Add(userId);
                    _client.SendMessage(e.ChatMessage.Channel, $"Welcome back {userDisplayName}, thanks for choosing to hang out with us 🤗💖");
                }
            }
            else if (liveCodersTeamMembers.Any(c => c._Id == userId))
            {
                if (!welcomedMemberIds.Contains(userId))
                {
                    welcomedMemberIds.Add(userId);
                    var url = $"https://twitch.tv/{username}";
                    _client.SendMessage(e.ChatMessage.Channel, $"Welcome to chat, {userDisplayName}! They are a member of the Livecoders 🎉! Check them out on {url}");
                }
            }


        }
        private async Task<List<User>> GetTeamMembers(string teamName)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Client-ID", _settings.ClientId);
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v5+json");
                var response = await client.GetAsync($"https://api.twitch.tv/kraken/teams/{teamName}");
                var jsonString = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                var teamResponse = JsonSerializer.Deserialize<TeamResponse>(jsonString, options);

                return teamResponse.Users;
            }

        }
        private async void Client_OnCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {

            switch (e.Command.CommandText.ToLower())
            {

                case "trello":
                    _client.SendMessage(_settings.Channel,
                         "Try typing !todo/!general/!bot/!links \"title of card\" \"The description of the card or URL\"");
                    break;
                case "todo":
                    CreateTrelloCard(e.Command, "todo");
                    break;
                case "general":
                    CreateTrelloCard(e.Command, "General Ideas");
                    break;
                case "bot":
                    CreateTrelloCard(e.Command, "Bot Ideas");
                    break;
                case "links":
                    CreateTrelloCard(e.Command, "links");
                    break;
                case "puprain":
                    await MakeItRain(e.Command);
                    break;
                case "waffle":
                    await Waffling(e.Command);
                    break;
                //case "balls":
                //    await PlayBalls(e.Command);
                //    break;
                case "swag":
                    await PlayBalls(e.Command);
                    break;
                case "laylatest":
                    await Test(e.Command);
                    break;
                default:
                    break;


            }

        }
        private async Task MakeItRain(ChatCommand e)
        {
            await _connection.InvokeAsync("SendMessage", e.ChatMessage.DisplayName, "Make it rain!!!", MessageTypeEnum.Rain);

        }
        private async Task Waffling(ChatCommand e)
        {
            await _connection.InvokeAsync("SendMessage", e.ChatMessage.DisplayName, "Waffling", MessageTypeEnum.Waffle);
            _client.SendMessage(e.ChatMessage.Channel, "Layla is waffling!!");
        }
        private async Task PlayBalls(ChatCommand e)
        {
            Console.WriteLine(_connection.ConnectionId);
            if (e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
            {
                // _client.SendMessage(e.ChatMessage.Channel, "Time to get your balls in! Type !prizedraw in the chat to be in with a chance to win!");
                _client.SendMessage(e.ChatMessage.Channel, "Type !winbooty to be in for a chance of winning some booty!");
                await _connection.InvokeAsync("PlaySoundMessage", e.ChatMessage.DisplayName, "balls");
            }
        }
        private async void Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            int.TryParse(e.RaidNotification.MsgParamViewerCount, out var count);

            count = count != 0 ? count : 1;

            await _connection.InvokeAsync("Raid", count);
        }
        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "my_friend")
                _client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }
        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                _client.SendMessage(e.Channel,
                    $"Welcome {e.Subscriber.DisplayName} to the wafflers! Thank you for using your Twitch Prime on this channel!");
            else
                _client.SendMessage(e.Channel,
                    $"Welcome {e.Subscriber.DisplayName} to the wafflers!");
        }
        private async void Client_OnGiftSubscriber(object sender, OnGiftedSubscriptionArgs e)
        {
            try
            {
                await _connection.InvokeAsync("SendMessage", e.GiftedSubscription.DisplayName, "Waffling", MessageTypeEnum.Cannon);
               await _connection.InvokeAsync("PlaySoundMessage", e.GiftedSubscription.DisplayName, "cannon");
                _client.SendMessage(e.Channel,
                       $"Woweee! {e.GiftedSubscription.DisplayName} just gifted {e.GiftedSubscription.MsgParamRecipientDisplayName} a subscription! Thank you so much <3");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gifted sub action failed: {ex.Message}");
            }
        }

        private async Task Test(ChatCommand e)
        {
            await _connection.InvokeAsync("SendMessage", "TEST", "TEST", MessageTypeEnum.Cannon);
        }
        private void CreateTrelloCard(ChatCommand e, string listName)
        {
            try
            {
                if (e.ChatMessage.IsModerator
                       || e.ChatMessage.IsBroadcaster
                       || e.ChatMessage.IsSubscriber
                       || e.ChatMessage.IsVip)
                {
                    var messageArray = CardMessageHandler(e.ArgumentsAsString);
                    if (messageArray.Length == 2)
                    {

                        var testCard = new NewTrelloCard
                        {
                            UserName = e.ChatMessage.DisplayName,
                            CardName = messageArray[0],
                            Description = messageArray[1],
                            ListName = listName
                        };
                        var trelloResponse = _trelloService.AddNewCardAsync(testCard);
                        _client.SendMessage(_settings.Channel, trelloResponse);
                    }
                    else
                    {
                        _client.SendMessage(_settings.Channel, "Hmmm, there was an error parsing your Trello card, please type !trello to see how to format a card command.");
                    }
                }
                else
                {
                    _client.SendMessage(_settings.Channel,
                     "Adding a Trello card is only available to subscribers and VIPs, but thanks for getting involved!");
                }

            }
            catch (Exception ex)
            {
                _client.SendMessage(_settings.Channel,
                   $"{e.ChatMessage.DisplayName} That card wasn't created, sorry!!");
                Console.WriteLine($"Failed to write Trello card: {ex.Message}");
            }
        }
        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString()); DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[]; if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }
            return value.ToString();
        }
        private string[] CardMessageHandler(string message)
        {
            return message.TrimStart('"').TrimEnd('"').Split("\" \"");
        }
    }
}