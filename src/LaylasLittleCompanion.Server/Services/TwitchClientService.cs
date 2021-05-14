using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LaylasLittleCompanion.Server.Models;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Microsoft.Extensions.Options;

using TwitchLib.Api;
using LaylasLittleCompanion.Server.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using LaylasLittleCompanion.Server.Models.Enums;

namespace LaylasLittleCompanion.Server.Services
{
	public class TwitchClientService
	{
		private readonly TwitchClient _client;
		private readonly TwitchConfiguration _settings;
		private readonly TrelloService _trelloService;
		private readonly TwitchApiService _apiService;
		private readonly TwitchCommands _commands;
		private List<User> liveCodersTeamMembers;
		private List<string> welcomedMemberIds = new List<string>();
		private readonly HubConnection _connection;
		//public event EventHandler<OnChatCommandReceivedArgs> BoopEvent;

		public TwitchClientService(
			IOptions<TwitchConfiguration> settings,
			TwitchApiService apiService,
			TrelloService trelloService,
			HubConnection connection,
			TwitchCommands commands)
		{
			_settings = settings.Value;
			_apiService = apiService;
			_trelloService = trelloService;
			_connection = connection;
			_commands = commands;
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
			//_client.OnMessageReceived += Client_MessageReceived;

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
			//Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
			//_client.SendMessage(e.Channel, "Hello lovelies, I'm Layla's little helper!");

			//liveCodersTeamMembers = await GetTeamMembers("livecoders");
		}
		//private void Client_MessageReceived(object sender, OnMessageReceivedArgs e)
		//{
		//	var userId = e.ChatMessage.UserId;
		//	var userDisplayName = e.ChatMessage.DisplayName;
		//	var username = e.ChatMessage.Username;
		//	if (e.ChatMessage.IsVip || e.ChatMessage.IsSubscriber || e.ChatMessage.IsModerator)
		//	{
		//		if (!welcomedMemberIds.Contains(userId))
		//		{
		//			welcomedMemberIds.Add(userId);
		//			_client.SendMessage(e.ChatMessage.Channel, $"Welcome back {userDisplayName}, thanks for choosing to hang out with us 🤗💖");
		//		}
		//	}
		//	else if (liveCodersTeamMembers.Any(c => c._Id == userId))
		//	{
		//		if (!welcomedMemberIds.Contains(userId))
		//		{
		//			welcomedMemberIds.Add(userId);
		//			var url = $"https://twitch.tv/{username}";
		//			_client.SendMessage(e.ChatMessage.Channel, $"Welcome to chat, {userDisplayName}! They are a member of the Livecoders 🎉! Check them out on {url}");
		//		}
		//	}


		//}
		//private async Task<List<User>> GetTeamMembers(string teamName)
		//{
		//	using (var client = new HttpClient())
		//	{
		//		client.DefaultRequestHeaders.Add("Client-ID", _settings.ClientId);
		//		client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v5+json");
		//		var response = await client.GetAsync($"https://api.twitch.tv/kraken/teams/{teamName}");
		//		var jsonString = await response.Content.ReadAsStringAsync();

		//		var options = new JsonSerializerOptions
		//		{
		//			PropertyNameCaseInsensitive = true,
		//		};

		//		var teamResponse = JsonSerializer.Deserialize<TeamResponse>(jsonString, options);

		//		return teamResponse.Users;
		//	}

		//}
		private async void Client_OnCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{
			var response = await _commands.Received(e);
			_client.SendMessage(_settings.Channel, response);
			
		}

		private async void Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
		{
			int.TryParse(e.RaidNotification.MsgParamViewerCount, out var count);

			count = count != 0 ? count : 1;
			//await _hub.Clients.All.SendAsync("Raid", count);

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
				//await _hub.Clients.All.SendAsync("SendMessage", e.GiftedSubscription.DisplayName, "Waffling", MessageTypeEnum.Cannon);
				//await _hub.Clients.All.SendAsync("PlaySoundMessage", e.GiftedSubscription.DisplayName, "cannon");
				_client.SendMessage(e.Channel,
					   $"Woweee! {e.GiftedSubscription.DisplayName} just gifted {e.GiftedSubscription.MsgParamRecipientDisplayName} a subscription! Thank you so much <3");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Gifted sub action failed: {ex.Message}");
			}
		}
		
	}
}