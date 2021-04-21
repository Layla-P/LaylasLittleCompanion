namespace MvcChatBot.Services
{
    public class AgentService
    {
        private readonly BroadcastService _broadcastService;

        public AgentService(BroadcastService broadcastService)
        {
            _broadcastService = broadcastService;
        }

        public void Broadcast()
        {
            _broadcastService.SendNotificationAsync();
        }
    }
}