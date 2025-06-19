using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace Frontend.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly HttpClient _httpClient;

		public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			_httpClient = httpClientFactory.CreateClient();
		}

		public IActionResult Index()
		{
			// Check if the user is authenticated
			if (User.Identity != null && User.Identity.IsAuthenticated)
			{
				// Redirect to the Home action
				return RedirectToAction("Home", "Home");
			}
			ViewData["ShowGradient"] = false;
			return View();
		}

		public async Task<IActionResult> Home()
		{
			var events = new List<EventViewModel>();

			try
			{
				// Call the API to fetch events
				var response = await _httpClient.GetAsync("https://planstock-api.runasp.net/api/events");

				if (response.IsSuccessStatusCode)
				{
					var jsonResponse = await response.Content.ReadAsStringAsync();
					events = JsonSerializer.Deserialize<List<EventViewModel>>(jsonResponse, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					}) ?? new List<EventViewModel>();
				}
				else
				{
					_logger.LogError($"Failed to fetch events. Status Code: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while fetching events.");
			}

			// Pass the events to the view
			ViewData["Events"] = events;

			return View();
		}

		public IActionResult SignIn()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
