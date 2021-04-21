using LaylasLittleCompanion.Server.Models;
using LaylasLittleCompanion.Server.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LaylasLittleCompanion.Server.Extensions
{
	public static class TrelloExtensions
	{
		public static void AddTrelloService(this IServiceCollection services, IConfiguration configuration)
		{

			var lists = new List<TrelloList>();
			var trelloLists = configuration.GetSection("TrelloSettings:TrelloLists")
				.GetChildren().ToList();

			foreach (var l in trelloLists)
			{
				var list = new TrelloList();
				l.Bind(list);
				lists.Add(list);
			}

			TrelloSettings trelloSettings = new TrelloSettings
			{
				ApiKey = configuration.GetValue<string>("TrelloSettings:ApiKey"),
				Token = configuration.GetValue<string>("TrelloSettings:Token"),
				BoardId = configuration.GetValue<string>("TrelloSettings:BoardId"),
				TrelloLists = lists
			};


			services.AddSingleton(trelloSettings);
			services.AddSingleton<TrelloService>();

		}

	}
}
