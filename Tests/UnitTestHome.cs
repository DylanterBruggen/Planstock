using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Frontend;
using System.Net;

namespace Tests
{
    public class HomePageNavigationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public HomePageNavigationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Scenario1_SignInButton_HasCorrectLink()
        {
            var response = await _client.GetAsync("/");
            var pageContent = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains("href=\"/Account/SignIn\"", pageContent);
        }

        [Fact]
        public async Task Scenario2_ExploreEventsButton_HasCorrectLink()
        {
            var response = await _client.GetAsync("/");
            var pageContent = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains("href=\"/Events/Index\"", pageContent);
        }

        [Fact]
        public async Task Scenario3_CreateEventButton_RedirectsUnauthenticatedUserToLogin()
        {
            var response = await _client.GetAsync("/Createevent/Index");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("/Account/SignIn", response.Headers.Location?.OriginalString);
        }

        [Fact]
        public async Task Scenario4_HelpLinkInFooter_HasCorrectLink()
        {
            var response = await _client.GetAsync("/");
            var pageContent = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains("href=\"/Help/Index\"", pageContent);
        }

        [Fact]
        public async Task Scenario5_BrokenLink_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/Deze/Url/Bestaat/Niet");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}