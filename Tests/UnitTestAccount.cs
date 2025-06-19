using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using Backend;

namespace Tests
{
    public class UnitTestAccount : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UnitTestAccount(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> GetJwtTokenAsync(string email = "user4@example.com", string password = "TEST!f123456test")
        {
            var loginContent = new FormUrlEncodedContent(new[]
            {
                   new KeyValuePair<string, string>("Email", email),
                   new KeyValuePair<string, string>("Password", password)
               });

            var response = await _client.PostAsync("/api/auth/login", loginContent);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tokenObj = JsonDocument.Parse(json);
            return tokenObj.RootElement.GetProperty("JWToken").GetString();
        }

        // ============ CHANGE USERNAME ============

        [Fact]
        public async Task ChangeUsername_Success()
        {
            var token = await GetJwtTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("NewUsername", "TEST_USER1")
            });

            var response = await _client.PostAsync("/api/account/change-username", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ============ CHANGE EMAIL ============

        [Fact]
        public async Task ChangeEmail_Success()
        {
            var token = await GetJwtTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("NewEmail", "newemail@example.com")
            });

            var response = await _client.PostAsync("/api/account/change-email", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ============ CHANGE PASSWORD ============

        [Fact]
        public async Task ChangePassword_Success()
        {
            var token = await GetJwtTokenAsync("user4@example.com", "TEST!f123456test");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("CurrentPassword", "TEST!f123456test"),
                new KeyValuePair<string, string>("NewPassword", "TEST!f123456test1")
            });

            var response = await _client.PostAsync("/api/account/change-password", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
