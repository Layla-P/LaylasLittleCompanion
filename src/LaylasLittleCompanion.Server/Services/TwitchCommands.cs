using LaylasLittleCompanion.Server.Models;
using LaylasLittleCompanion.Server.Models.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace LaylasLittleCompanion.Server.Services
{
	public class TwitchCommands
	{
		private readonly TrelloService _trelloService;
		private readonly TwitchConfiguration _settings;
		private readonly HubConnection _connection;
		private readonly TwitchApiService _apiService;

		public TwitchCommands(
			HubConnection connection,
			TrelloService trelloService,
			IOptions<TwitchConfiguration> settings,
			TwitchApiService apiService)
		{
			_connection = connection;
			_trelloService = trelloService;
			_settings = settings.Value;
			_apiService = apiService;
		}

		

		public async Task<string> Received(OnChatCommandReceivedArgs e)
		{
			return e.Command.CommandText.ToLower() switch
			{
				"trello" => "Try typing !todo/!general/!bot/!links \"title of card\" \"The description of the card or URL\"",
				"todo" => CreateTrelloCard(e.Command, "todo"),
				"general" => CreateTrelloCard(e.Command, "General Ideas"),
				"bot" => CreateTrelloCard(e.Command, "Bot Ideas"),
				"links" => CreateTrelloCard(e.Command, "links"),
				"puprain" => await MakeItRain(e.Command, MessageTypeEnum.Rain),
				"waffle" => await Waffling(e.Command),
				"swag" => await PlayBalls(e.Command),
				"stats" => await GetStats(e.Command),
				_ => null
			};
			
		}

		private string CreateTrelloCard(ChatCommand e, string listName)
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
						return _trelloService.AddNewCardAsync(testCard);
					}
					else
					{
						return "Hmmm, there was an error parsing your Trello card, please type !trello to see how to format a card command.";
					}
				}
				else
				{
					return
					 "Adding a Trello card is only available to subscribers and VIPs, but thanks for getting involved!";
				}

			}
			catch (Exception ex)
			{
				return $"{e.ChatMessage.DisplayName} That card wasn't created, sorry!!";
				Console.WriteLine($"Failed to write Trello card: {ex.Message}");
			}
		}
		private string GetEnumDescription(Enum value)
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
		private async Task<string> MakeItRain(ChatCommand e, MessageTypeEnum messageType)
		{
			//await _hub.Clients.All.SendAsync("LaylaMessage", e.ChatMessage.DisplayName, "Make it rain!!!", MessageTypeEnum.Rain);
			await _connection.InvokeAsync("SendMessage", messageType);
			return $"{e.ChatMessage.DisplayName} made it rain!";
			//await _hub.Clients.All.SendAsync("LaylaMessage", user, message, action);
		}
		private async Task<string> Waffling(ChatCommand e)
		{
			//await _hub.Clients.All.SendAsync("SendMessage", e.ChatMessage.DisplayName, "Make it rain!!!", MessageTypeEnum.Waffle);
			return "Layla is waffling!!";
		}

		private async Task<string> GetStats(ChatCommand e)
		{
			return await _apiService.GetStatsAsync();
		}
		private async Task<string> PlayBalls(ChatCommand e)
		{			
			if (e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
			{
				// _client.SendMessage(e.ChatMessage.Channel, "Time to get your balls in! Type !prizedraw in the chat to be in with a chance to win!");

				await _connection.InvokeAsync("PlaySoundMessage", e.ChatMessage.DisplayName, "balls");
				return "Type !winbooty to be in for a chance of winning some booty!";
			}
			return null;
		}

	}
}
