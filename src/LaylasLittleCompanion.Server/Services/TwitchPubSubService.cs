//using Microsoft.AspNetCore.SignalR.Client;
//using LaylasLittleCompanion.Server.Models;
//using System;
//using TwitchLib.PubSub;
//using TwitchLib.PubSub.Events;
//using LaylasLittleCompanion.Server.Models.Enums;

//namespace LaylasLittleCompanion.Server.Services
//{
//	public class TwitchPubSubService
//	{

//		private readonly TwitchPubSub _client;		
//		private readonly TwitchConfiguration _settings;

//		public TwitchPubSubService(
//			TwitchConfiguration settings)
//		{
//			_settings = settings;

//			_client = new TwitchPubSub();

//			_client.OnPubSubServiceConnected += onPubSubServiceConnected;
//			_client.OnListenResponse += onListenResponse;
//			_client.OnStreamUp += onStreamUp;
//			_client.OnStreamDown += onStreamDown;
//			//_client.OnRewardRedeemed += onRewardRedeemed;
//			_client.OnFollow += onFollow;

//			_client.ListenToFollows(_settings.ChannelId);
//			_client.ListenToRewards(_settings.ChannelId);

//			_client.Connect();
//		}


//		private void onPubSubServiceConnected(object sender, EventArgs e)
//		{
//			// SendTopics accepts an oauth optionally, which is necessary for some topics
//			_client.SendTopics($"oauth:{_settings.ChannelAuthToken}");
//		}
//		//https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming#:~:text=Avoid%20Async%20Void
		

//		private async void onFollow(object sender, OnFollowArgs e)
//		{
//			Console.WriteLine($"follow from pubsub {e.DisplayName}");			
//		}

//		private void onListenResponse(object sender, OnListenResponseArgs e)
//		{
//			if (!e.Successful)
//				throw new Exception($"Failed to listen! Response: {e.Response}");
//		}

//		private void onStreamUp(object sender, OnStreamUpArgs e)
//		{
//			Console.WriteLine($"Stream just went up! Play delay: {e.PlayDelay}, server time: {e.ServerTime}");
//		}

//		private void onStreamDown(object sender, OnStreamDownArgs e)
//		{
//			Console.WriteLine($"Stream just went down! Server time: {e.ServerTime}");
//		}
//	}
//}
