using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaylasLittleCompanion.Server.Models
{
	public class ChatEventArgs : EventArgs
	{
			public string Action { get; init; }		
	}
}
