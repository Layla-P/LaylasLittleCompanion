using Microsoft.AspNetCore.SignalR.Client;
using MvcChatBot.Models;
using RestSharp.Authenticators;
using System;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace MvcChatBot.Agent.Services
{
    public class TwitchPubSubService
    {

        private readonly TwitchPubSub _client;
        private readonly HubConnection _connection;
        private readonly TwitchSettings _settings;

        public TwitchPubSubService(
            TwitchSettings settings,
            HubConnection connection)
        {
            _settings = settings;

            _connection = connection;
            _connection.StartAsync();


            _client = new TwitchPubSub();

            _client.OnPubSubServiceConnected += onPubSubServiceConnected;
            _client.OnListenResponse += onListenResponse;
            _client.OnStreamUp += onStreamUp;
            _client.OnStreamDown += onStreamDown;
            _client.OnRewardRedeemed += onRewardRedeemed;
            _client.OnFollow += onFollow;

            _client.ListenToFollows(_settings.ChannelId);
            _client.ListenToRewards(_settings.ChannelId);

            _client.Connect();
        }


        private void onPubSubServiceConnected(object sender, EventArgs e)
        {
            // SendTopics accepts an oauth optionally, which is necessary for some topics
            _client.SendTopics($"oauth:{_settings.ChannelAuthToken}");
        }
        //https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming#:~:text=Avoid%20Async%20Void
        private async void onRewardRedeemed(object sender, OnRewardRedeemedArgs e)
        {
           if(e.RewardTitle == "puppy-rain")
            {

                await _connection.InvokeAsync("SendMessage", e.DisplayName, "It's a torrential downpour of destructopups!!!", MessageTypeEnum.SuperRain);
            }
        }

        private async void onFollow(object sender, OnFollowArgs e)
        {
            Console.WriteLine($"follow from pubsub {e.DisplayName}");
            await _connection.InvokeAsync("PlaySoundMessage", e.DisplayName, "follow");
        }

        private void onListenResponse(object sender, OnListenResponseArgs e)
        {
            if (!e.Successful)
                throw new Exception($"Failed to listen! Response: {e.Response}");
        }

        private void onStreamUp(object sender, OnStreamUpArgs e)
        {
            Console.WriteLine($"Stream just went up! Play delay: {e.PlayDelay}, server time: {e.ServerTime}");
        }

        private void onStreamDown(object sender, OnStreamDownArgs e)
        {
            Console.WriteLine($"Stream just went down! Server time: {e.ServerTime}");
        }
    }
}
