using System;
using System.Threading.Tasks;
using LaylasLittleCompanion.Server.Hubs;
using LaylasLittleCompanion.Server.Models;
using LaylasLittleCompanion.Server.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using TwitchLib.Client.Events;

namespace LaylasLittleCompanion.Server.Pages
{
	public partial class RainOverlay : ComponentBase, IAsyncDisposable
	{
		const string JsModulePath = "./js/boops.js";
		const string MatterJsPath = "./js/matter.js";
		[Inject] IJSRuntime JsRuntime { get; set; }
		[Inject] TwitchClientService twitchClient { get; set; }
		[Inject] HubConnection hubConnection { get; set; }

		IJSObjectReference matter;
		IJSObjectReference boops;


		public ElementReference Boops { get; set; }
		
		protected override async Task OnAfterRenderAsync(bool firstRender)
		{

			if (firstRender)
			{
				matter = await JsRuntime.InvokeAsync<IJSObjectReference>("import", MatterJsPath);
				boops = await JsRuntime.InvokeAsync<IJSObjectReference>("import", JsModulePath);

				hubConnection.On<string>("ReceiveMessage", (action) =>
				{
					boops.InvokeVoidAsync("Booper", action);
					InvokeAsync(StateHasChanged);
				});
			}
		}

		async void DoStuff(object sender, OnChatCommandReceivedArgs e)
		{
			await boops.InvokeVoidAsync("Booper", e.Command.CommandText );
		}

		public async ValueTask DisposeAsync()
		{
			//twitchClient.BoopEvent -= DoStuff;
			await matter.InvokeVoidAsync("dispose");
			await matter.DisposeAsync();
			await boops.InvokeVoidAsync("dispose");
			await boops.DisposeAsync();
		}
	}
}
