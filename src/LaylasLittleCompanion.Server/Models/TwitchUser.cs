using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LaylasLittleCompanion.Server.Models
{
	public class TwitchUser
	{

		public string AccessToken { get; set; } = string.Empty;

		public string RefreshToken { get; set; } = string.Empty;

		//public event EventHandler OnLoggedOut;

		public int ExpiresIn
		{
			get { return (int)(ExpiresAt - DateTime.Now).TotalSeconds; }
			set { ExpiresAt = DateTime.Now.AddSeconds(value); }
		}

		public DateTime ExpiresAt { get; set; }

		public Uri PictureUri { get; set; }

		public string TwitchId { get; set; }

		public string DisplayName { get; set; }



		public async Task RefreshTokens(HttpClient client, TwitchConfiguration twitchConfig)
		{

			try
			{

				var payload = new FormUrlEncodedContent(new Dictionary<string, string> {
							{"grant_type", "refresh_token"},
							{"token", RefreshToken},
							{"client_id", twitchConfig.ClientId},
							{"client_secret", twitchConfig.ClientSecret}
	  });

				var response = await client.PostAsync("https://id.twitch.tv/oauth2/token", payload);
				if (!response.IsSuccessStatusCode)
				{
					Console.WriteLine(await response.Content.ReadAsStringAsync());
					throw new UnauthorizedAccessException();
				}

				var message = await response.Content.ReadAsStringAsync();
				var results = JsonSerializer.Deserialize<TwitchToken>(message);

				AccessToken = results.access_token;
				RefreshToken = results.refresh_token;
				ExpiresIn = results.expires_in;
			}
			finally
			{
				
			}

		}



	}

	public class TwitchToken
	{
		public string access_token { get; set; }
		public string refresh_token { get; set; }

		private int _expiresIn = 0;
		public int expires_in
		{
			get { return _expiresIn; }
			set
			{
				_expiresIn = value;
				ExpiresAt = DateTime.Now.AddSeconds(value);
			}
		}

		[JsonIgnore]
		public DateTime ExpiresAt { get; private set; }

		public List<string> scope { get; set; }
		public string token_type { get; set; }

	}

}
