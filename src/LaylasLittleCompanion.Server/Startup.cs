using Blazored.LocalStorage;
using LaylasLittleCompanion.Server.Extensions;
using LaylasLittleCompanion.Server.Models;
using LaylasLittleCompanion.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrelloNet;


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
			// tresting twitch integration https://github.com/FiniteSingularity/tau
			services.Configure<TwitchConfiguration>(Configuration.GetSection("TwitchConfiguration"));

			services.AddHttpClient();
			services.AddSignalR();


			services.AddRazorPages();
			services.AddServerSideBlazor();
			services.AddGraphQLClient();			
			services.AddBlazoredLocalStorage();

			services.AddOIDCTwitch(Configuration);

			//add config for trello
			services.AddTrelloService(Configuration);

			//register services



		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
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
			});
		}
	}
}
