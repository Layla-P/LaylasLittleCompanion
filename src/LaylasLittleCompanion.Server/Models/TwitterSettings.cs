using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaylasLittleCompanion.Server.Models
{
	public class TwitterSettings
	{
		public string ApiKey { get; set; }
		public string ApiSecret { get; set; }
		public string BearerToken { get; set; }
		public string AccessToken { get; set; }
		public string AccessSecret { get; set; }
	}
}
