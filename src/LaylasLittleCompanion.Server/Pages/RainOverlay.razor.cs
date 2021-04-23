using System;
using System.Threading.Tasks;
using LaylasLittleCompanion.Server.Models;
using LaylasLittleCompanion.Server.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace LaylasLittleCompanion.Server.Pages
{
	public partial class RainOverlay : ComponentBase, IDisposable
	{
		const string JsModulePath = "js/boops.js";
		const string ScriptLoaderModulePath = "js/scriptloader.js";
		const string MatterJsPath = "js/matter.js";
		[Inject] IJSRuntime JsRuntime { get; set; }
		[Inject] TwitchClientService twitchClient { get; set; }

		Lazy<Task<IJSObjectReference>> scriptLoaderModuleTask;
		Lazy<Task<IJSObjectReference>> moduleTask;


		public ElementReference Boops { get; set; }

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			
			if (firstRender)
			{
				twitchClient.TestEvent += DoStuff;

				//scriptLoaderModuleTask = new(() => JsRuntime.InvokeAsync<IJSObjectReference>("import", ScriptLoaderModulePath)
				//								.AsTask());
				//var scriptLoaderModule = await scriptLoaderModuleTask.Value;
				//await scriptLoaderModule.InvokeVoidAsync("loadScript", MatterJsPath);

				//moduleTask = new(() => JsRuntime.InvokeAsync<IJSObjectReference>("import", JsModulePath)
				//								.AsTask());
				//var module = await moduleTask.Value;
				//((IJSInProcessRuntime)JsRuntime).InvokeVoid("initialize", Boops, DotNetObjectReference.Create(this));
				//await module.InvokeVoidAsync("initialize", Boops, DotNetObjectReference.Create(this));
				//await JsRuntime.InvokeVoidAsync("initialize", Boops, DotNetObjectReference.Create(this));
			}
		}

		async void DoStuff(object sender, ChatEventArgs e)
		{
			//moduleTask = new(() => JsRuntime.InvokeAsync<IJSObjectReference>("import", JsModulePath)
			//									.AsTask());
			//var module = await moduleTask.Value;
			//await module.InvokeVoidAsync("Booper", e.Action);
			await JsRuntime.InvokeVoidAsync("Booper", e.Action);
			await InvokeAsync(StateHasChanged);
		}

		public void Dispose()
		{ 
			twitchClient.TestEvent -= DoStuff;
		}
		//public async ValueTask DisposeAsync()
		//{
		//	if (moduleTask.IsValueCreated)
		//	{
		//		var module = await moduleTask.Value;
		//		await module.DisposeAsync();
		//	}
		//	if (scriptLoaderModuleTask.IsValueCreated)
		//	{
		//		var module = await scriptLoaderModuleTask.Value;
		//		await module.DisposeAsync();
		//	}
		//}
	}
}
