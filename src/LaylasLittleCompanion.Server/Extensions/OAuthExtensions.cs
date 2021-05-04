using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LaylasLittleCompanion.Server.Extensions
{
	public static class OAuthExtensions
	{
		public static void AddOIDCTwitch(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddAuthentication(options =>
			{
				options.DefaultScheme = "Cookies";
				options.DefaultChallengeScheme = "oidc";
				options.DefaultSignOutScheme = "oidc";
			})
			.AddCookie("Cookies")
			.AddOpenIdConnect("oidc", options =>
			{
				options.SignInScheme = "Cookies";

				options.Authority = "https://id.twitch.tv/oauth2";

				options.ResponseType = "code";

				options.ClientId = configuration["TwitchConfiguration:ClientId"]; ;
				options.ClientSecret = configuration["TwitchConfiguration:ClientSecret"];

				options.CallbackPath = new PathString("/signin-oidc");
				options.AutomaticRefreshInterval = TimeSpan.FromHours(1);
				options.SaveTokens = true;

				string[] scopes = configuration.GetSection("TwitchConfiguration:Scopes").Get<string[]>();

				options.Scope.Clear();
				foreach (var scope in scopes)
				{
					options.Scope.Add(scope);
				}
				
				options.TokenValidationParameters.NameClaimType = "preferred_username";
			});

			//services.AddAuthorization(options =>
			//{
			//	options.FallbackPolicy = new AuthorizationPolicyBuilder()
			//	.
			//		.Build();
			//});
		}

		
	}
}