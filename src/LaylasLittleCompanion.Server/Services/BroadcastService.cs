using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MvcChatBot.Hubs;

namespace MvcChatBot.Services
{
    public class BroadcastService
    {
        private readonly IHubContext<ChatHub> _hub;

        public BroadcastService(IHubContext<ChatHub> hub)
        {
            _hub = hub;
        }

        public void SendNotificationAsync()
        {
            _hub.Clients.All.SendAsync("Bops", 27);
        }
    }
}