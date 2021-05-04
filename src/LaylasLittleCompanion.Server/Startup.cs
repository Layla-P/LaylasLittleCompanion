using Blazored.LocalStorage;
using LaylasLittleCompanion.Server.Extensions;
using LaylasLittleCompanion.Server.Models;
using LaylasLittleCompanion.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.ResponseCompression;
using System.Linq;
using LaylasLittleCompanion.Server.Hubs;



//https://stackoverflow.com/questions/60858985/addopenidconnect-and-refresh-tokens-in-asp-net-core
namespace LaylasLittleCompanion.Server
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{

			var baseAddress = Configuration.GetValue<string>("BaseAddress");

			var  rainHubConnection = new HubConnectionBuilder()
				.WithUrl($"{baseAddress}/rainhub")
				.WithAutomaticReconnect()
				.Build();
			rainHubConnection.StartAsync();
			services.AddSingleton(rainHubConnection);
			// tresting twitch integration https://github.com/FiniteSingularity/tau
			services.Configure<TwitchConfiguration>(Configuration.GetSection("TwitchConfiguration"));
			services.Configure<TrelloSettings>(Configuration.GetSection("TrelloSettings"));
			services.Configure<TwitterSettings>(Configuration.GetSection("TwitterSettings"));

			services.AddHttpClient();
			services.AddHttpClient("TwitterClient", c =>
			{
				c.BaseAddress = new Uri("https://api.twitter.com/1.1/account/");
				c.DefaultRequestHeaders.Add("Accept", "application/json");
				c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Configuration.GetValue<string>("TwitterSettings.BearerToken"));

			});

			services.AddSingleton<TwitterService>();

			services.AddRazorPages();
			services.AddServerSideBlazor();
			services.AddGraphQLClient();			
			services.AddBlazoredLocalStorage();

			services.AddOIDCTwitch(Configuration);
			services.AddResponseCompression(opts =>
			{
				opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
					new[] { "application/octet-stream" });
			});

			services.AddTrelloService(Configuration);

			services.AddSingleton<TwitchApiService>();

			services.AddSingleton<TwitchClientService>();


			//var serviceProvider = services.BuildServiceProvider();
			//_ = serviceProvider.GetService<TwitchClientService>();





		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseResponseCompression();
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();


			app.UseEndpoints(endpoints =>
			{
				endpoints.MapBlazorHub();
				endpoints.MapRazorPages();
				endpoints.MapFallbackToPage("/_Host");
				endpoints.MapHub<RainHub>("/rainhub");
			});
		}
	}
}
