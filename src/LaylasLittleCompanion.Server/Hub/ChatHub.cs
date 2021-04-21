using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MvcChatBot.Models;

namespace MvcChatBot.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, MessageTypeEnum messageType)
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
            await Clients.All.SendAsync("LaylaMessage", user, message, action);
        }

        public async Task Raid(int raiderCount)
        {
            await Clients.All.SendAsync("Bops", raiderCount);
        }

        public async Task PlaySoundMessage(string user, string message)
        {
            await Clients.All.SendAsync("SoundMessage", user, message);
        }

        public async Task UpdateBrowser()
        {

            await Clients.All.SendAsync("TriggerRain");


        }

        public Task SendMessageToGroup(string message)
        {
            return Clients.Group("SignalR Users").SendAsync("ReceiveMessage", message);
        }
    }

}