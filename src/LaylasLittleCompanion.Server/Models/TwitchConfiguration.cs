namespace LaylasLittleCompanion.Server.Models
{
	public class TwitchConfiguration
	{
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string BotName { get; set; }
		public string AuthToken { get; set; }
		public string Channel { get; set; }
		public string ChannelId { get; set; }
		public string ChannelAuthToken { get; set; }
		public string[] Scopes { get; set; }
	}
}
