using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.Extensions.Options;
using LaylasLittleCompanion.Server.Models;

namespace LaylasLittleCompanion.Server.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly HttpClient _httpClient;
		private readonly TwitchConfiguration _config;
		public LogoutModel(HttpClient httpClient, IOptions<TwitchConfiguration> options)
		{
			_config = options.Value;
			_httpClient = httpClient;
		}
        public async Task<IActionResult> OnGet()
        {
			await HttpContext.SignOutAsync("Cookies");
			return Redirect("/");
		}
    }
}
