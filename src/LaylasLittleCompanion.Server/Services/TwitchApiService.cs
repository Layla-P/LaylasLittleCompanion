using LaylasLittleCompanion.Server.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.FollowerService;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace LaylasLittleCompanion.Server.Services
{
    public class TwitchApiService
    {
        private LiveStreamMonitorService Monitor;
        private TwitchAPI API;
        private readonly TwitchConfiguration _settings;

        public TwitchApiService(IOptions<TwitchConfiguration> settings)
        {
			_settings = settings.Value;
			API = new TwitchAPI();
			API.Settings.ClientId = _settings.ClientId;
			API.Settings.AccessToken = _settings.ChannelAuthToken;
			Task.Run(() => ConfigLiveMonitorAsync());
        }
		public async Task<string> GetStatsAsync()
		{
			var currentStream = await API.V5.Streams.GetStreamByUserAsync(_settings.ChannelId);
			return $"Current stats for {currentStream.Stream.Channel.DisplayName}: {currentStream.Stream.Viewers} viewers, {currentStream.Stream.Channel.Views} views and {currentStream.Stream.Channel.Followers}.";
		}
		private async Task ConfigLiveMonitorAsync()
        {
            API = new TwitchAPI();

            API.Settings.ClientId = _settings.ClientId;
            API.Settings.AccessToken = _settings.ChannelAuthToken;

            Monitor = new LiveStreamMonitorService(API, 30);
            //FollowerService = new FollowerService(API, 30, 10);

            List<string> channels = new List<string> { _settings.ChannelId};
            Monitor.SetChannelsById(channels);

            Monitor.OnStreamOnline += Monitor_OnStreamOnline;
            Monitor.OnStreamOffline += Monitor_OnStreamOffline;
            Monitor.OnStreamUpdate += Monitor_OnStreamUpdate;

            Monitor.OnServiceStarted += Monitor_OnServiceStarted;
            Monitor.OnChannelsSet += Monitor_OnChannelsSet;


            //FollowerService.SetChannelsById(channels);
            //FollowerService.OnChannelsSet += Follower_OnChannelsSet;
            //FollowerService.OnNewFollowersDetected += Follower_OnNewFollow;
            //FollowerService.OnServiceStarted += Follower_OnServiceStarted;

            Monitor.Start(); //Keep at the end!
            //FollowerService.Start();

            await Task.Delay(-1);
        }

        //private void Follower_OnNewFollow(object sender, OnNewFollowersDetectedArgs e)
        //{
           
                     
        //    e.NewFollowers.ForEach(async(follower) => {
        //        Console.WriteLine($"New follower: {follower.FromUserName}");
        //        await _connection.InvokeAsync("PlaySoundMessage", follower.FromUserName, "follow");
        //    });
        //}

        //private void Follower_OnServiceStarted(object sender, OnServiceStartedArgs e)
        //{
        //    Console.WriteLine("OnFollowerService Started");
        //}


        //private void Follower_OnChannelsSet(object sender, OnChannelsSetArgs e)
        //{
        //    Console.WriteLine("Follower OnChannelSet from Api");
        //}


        private void Monitor_OnStreamOnline(object sender, OnStreamOnlineArgs e)
        {
            Console.WriteLine("Stream online from Api");
        }

        private void Monitor_OnStreamUpdate(object sender, OnStreamUpdateArgs e)
        {
            Console.WriteLine("Stream updated from Api");
        }

        private void Monitor_OnStreamOffline(object sender, OnStreamOfflineArgs e)
        {
            Console.WriteLine("Stream offline from Api");
        }

        private void Monitor_OnChannelsSet(object sender, OnChannelsSetArgs e)
        {
            Console.WriteLine("Monitor OnChannelSet from Api");
        }

        private void Monitor_OnServiceStarted(object sender, OnServiceStartedArgs e)
        {
            Console.WriteLine("OnMonitorService Started");
        }
    }
}