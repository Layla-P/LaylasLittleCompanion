using LaylasLittleCompanion.Server.Models;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LaylasLittleCompanion.Server.Services
{
	// The code within this TwitterProvider has been based almost exclusively on the work that was done by Danny Tuppeny
	// based on this blog post:
	// https://blog.dantup.com/2016/07/simplest-csharp-code-to-post-a-tweet-using-oauth/
	/// <summary>
	/// Contains functionality related to Twitter API
	/// </summary>
	public sealed class TwitterService
	{
		const string TwitterApiBaseUrl = "https://api.twitter.com/1.1/";
		string previousTwitterName;
		readonly string consumerKey, consumerKeySecret, accessToken, accessTokenSecret, bearerToken;
		readonly HMACSHA1 sigHasher;
		readonly DateTime epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Creates an object for sending tweets to Twitter using Single-user OAuth.
		///
		/// Get your access keys by creating an app at apps.twitter.com then visiting the
		/// "Keys and Access Tokens" section for your app. They can be found under the
		/// "Your Access Token" heading.
		/// </summary>
		public TwitterService(Microsoft.Extensions.Options.IOptions<TwitterSettings> settingOptions)
		{
			var settings = settingOptions.Value;
			this.consumerKey = settings.ApiKey;
			this.consumerKeySecret = settings.ApiSecret;
			this.accessToken = settings.AccessToken;
			this.accessTokenSecret = settings.AccessSecret;
			this.bearerToken = settings.BearerToken;

			sigHasher = new HMACSHA1(new ASCIIEncoding().GetBytes(string.Format("{0}&{1}", consumerKeySecret, accessTokenSecret)));
		}

		/// <summary>
		/// Sends a tweet with the supplied text and returns the response from the Twitter API.
		/// </summary>
		public async Task<string> Tweet(string text)
		{
			var data = new Dictionary<string, string> {
	  { "status", text },
	  { "trim_user", "1" }
	};

			return await SendRequest("statuses/update.json", data);
		}
		public async Task<string> UpdateName(string text)
		{
			await SetCurrentName();
			var data = new Dictionary<string, string> {
	  { "name", text }
	};
			var resp = await SendRequest("/account/update_profile.json", data);

			return resp;
		}

		public async Task ResetName()
		{
			await UpdateName(previousTwitterName);
		}

		private async Task SetCurrentName()
		{			
			var resp = await GetRequest("/users/show.json?screen_name=laylacodesit");
			var response = JsonDocument.Parse(resp);
			previousTwitterName = response.RootElement.GetProperty("name").GetString();
		}

		async Task<string> SendRequest(string url, Dictionary<string, string> data)
		{
			var fullUrl = TwitterApiBaseUrl + url;

			// Timestamps are in seconds since 1/1/1970.
			var timestamp = (int)((DateTime.UtcNow - epochUtc).TotalSeconds);

			// Add all the OAuth headers we'll need to use when constructing the hash.
			data.Add("oauth_consumer_key", consumerKey);
			data.Add("oauth_signature_method", "HMAC-SHA1");
			data.Add("oauth_timestamp", timestamp.ToString());
			data.Add("oauth_nonce", "a"); // Required, but Twitter doesn't appear to use it, so "a" will do.
			data.Add("oauth_token", accessToken);
			data.Add("oauth_version", "1.0");

			// Generate the OAuth signature and add it to our payload.
			data.Add("oauth_signature", GenerateSignature(fullUrl, data));

			// Build the OAuth HTTP Header from the data.
			string oAuthHeader = GenerateOAuthHeader(data);

			// Build the form data (exclude OAuth stuff that's already in the header).
			var formData = new FormUrlEncodedContent(data.Where(kvp => !kvp.Key.StartsWith("oauth_")));

			return await SendRequest(fullUrl, oAuthHeader, formData);
		}

		/// <summary>
		/// Generate an OAuth signature from OAuth header values.
		/// </summary>
		string GenerateSignature(string url, Dictionary<string, string> data)
		{
			var sigString = string.Join(
			  "&",
			  data
				.Union(data)
				.Select(kvp => string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
				.OrderBy(s => s)
			);

			var fullSigData = string.Format(
			  "{0}&{1}&{2}",
			  "POST",
			  Uri.EscapeDataString(url),
			  Uri.EscapeDataString(sigString.ToString())
			);

			return Convert.ToBase64String(sigHasher.ComputeHash(new ASCIIEncoding().GetBytes(fullSigData.ToString())));
		}

		/// <summary>
		/// Generate the raw OAuth HTML header from the values (including signature).
		/// </summary>
		string GenerateOAuthHeader(Dictionary<string, string> data)
		{
			return "OAuth " + string.Join(
			  ", ",
			  data
				.Where(kvp => kvp.Key.StartsWith("oauth_"))
				.Select(kvp => string.Format("{0}=\"{1}\"", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
				.OrderBy(s => s)
			);
		}

		/// <summary>
		/// Send HTTP Request and return the response.
		/// </summary>
		async Task<string> SendRequest(string fullUrl, string oAuthHeader, FormUrlEncodedContent formData)
		{
			using (var http = new HttpClient())
			{
				http.DefaultRequestHeaders.Add("Authorization", oAuthHeader);

				var httpResp = await http.PostAsync(fullUrl, formData);
				var respBody = await httpResp.Content.ReadAsStringAsync();

				return respBody;
			}
		}

		async Task<string> GetRequest(string url)
		{
			using (var http = new HttpClient())
			{
				var fullUrl = TwitterApiBaseUrl + url;
				http.DefaultRequestHeaders.Authorization =
						new AuthenticationHeaderValue("Bearer", bearerToken);
				var httpResp = await http.GetAsync(fullUrl);
				var respBody = await httpResp.Content.ReadAsStringAsync();

				return respBody;
			}
		}
	}
}
