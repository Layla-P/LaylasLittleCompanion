using System;
using System.Collections.Generic;
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
		const string StylePath = "./js/style-control.js";
		[Inject] IJSRuntime JsRuntime { get; set; }
		[Inject] HubConnection hubConnection { get; set; }

		IJSObjectReference matter;
		IJSObjectReference boops;
		IJSObjectReference style;
		public ElementReference CanvasElement;


		protected override async Task OnInitializedAsync()
		{
			matter = await JsRuntime.InvokeAsync<IJSObjectReference>("import", MatterJsPath);
			boops = await JsRuntime.InvokeAsync<IJSObjectReference>("import", JsModulePath);
			style = await JsRuntime.InvokeAsync<IJSObjectReference>("import", StylePath);
			await style.InvokeVoidAsync("updateBody");

			await boops.InvokeVoidAsync("Init", CanvasElement);
		}

		protected override void OnParametersSet()
		{

			hubConnection.On<string>("ReceiveMessage", async (action) =>
				{
					await boops.InvokeVoidAsync("Booper", action);
					await InvokeAsync(StateHasChanged);

				});
		}


		public bool IsConnected =>
			hubConnection.State == HubConnectionState.Connected;

		public async ValueTask DisposeAsync()
		{
			//await hubConnection.DisposeAsync();
			await matter.InvokeVoidAsync("dispose");
			await matter.DisposeAsync();
			await boops.InvokeVoidAsync("dispose");
			await boops.DisposeAsync();
			await style.InvokeVoidAsync("dispose");
			await style.DisposeAsync();
		}
	}
}
