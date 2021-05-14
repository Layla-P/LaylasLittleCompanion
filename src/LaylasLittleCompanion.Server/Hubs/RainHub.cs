using LaylasLittleCompanion.Server.Models.Enums;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaylasLittleCompanion.Server.Hubs
{
	public class RainHub : Hub
	{
		public async Task SendMessage(MessageTypeEnum messageType)
		{
			string action = null;
			switch (messageType)
			{
				case MessageTypeEnum.Cannon:
					action = "cannon";
					break;
				case MessageTypeEnum.Waffle:
					action = "waffle";
					break;
				case MessageTypeEnum.SuperRain:
					action = "super";
					break;
				case MessageTypeEnum.Rain:
				default:
					action = "rain";
					break;
			}
			await Clients.All.SendAsync("ReceiveMessage", action);
		}
		//public async Task SendMessage(string user, string message)
		//{
		//	await Clients.All.SendAsync("ReceiveMessage", user, message);
		//}
	}
}
