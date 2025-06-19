using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.Text.Json.Nodes;
using System.Net.Http.Headers;

namespace Frontend.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;

        // Constructor to inject configuration and HttpClient  
        public AccountController(IConfiguration configuration, HttpClient httpClient)
        {
            _apiBaseUrl = configuration["ApiUrls:AuthenticationApi"]!;
            _httpClient = httpClient;
        }

        // GET: /Account/SignIn  
        [HttpGet]
        public IActionResult SignIn(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Store return URL for redirection after login  
            ViewData["ShowGradient"] = false;
            return View();
        }

        // POST: /Account/SignIn  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SigninViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Store return URL for redirection after login  
            if (ModelState.IsValid)
            {
                string requestUrl = $"{_apiBaseUrl}/Authenticate/login"; // API endpoint for login  

                // Prepare login payload  
                var loginPayload = new
                {
                    email = model.Email,
                    password = model.Password
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(loginPayload),
                    Encoding.UTF8,
                    "application/json"
                );

                // Send login request to API  
                var response = await _httpClient.PostAsync(requestUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<JwtTokenResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (string.IsNullOrEmpty(tokenResponse?.Token))
                    {
                        ModelState.AddModelError(string.Empty, "Received an invalid token from the server.");
                        return View(model);
                    }

                    // Store JWT token in session for server-side API calls  
                    HttpContext.Session.SetString("JWToken", tokenResponse.Token);

                    // Parse JWT token and extract claims  
                    var handler = new JwtSecurityTokenHandler();
                    var jwtSecurityToken = handler.ReadJwtToken(tokenResponse.Token);

                    var claims = new List<Claim>(jwtSecurityToken.Claims); // Copy claims from JWT  

                    // Ensure essential claims for MVC are present  
                    if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier) && claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub))
                    {
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value));
                    }
                    if (!claims.Any(c => c.Type == ClaimTypes.Name) && claims.Any(c => c.Type == JwtRegisteredClaimNames.NameId))
                    {
                        claims.Add(new Claim(ClaimTypes.Name, claims.First(c => c.Type == JwtRegisteredClaimNames.NameId).Value));
                    }
                    else if (!claims.Any(c => c.Type == ClaimTypes.Name) && claims.Any(c => c.Type == "unique_name"))
                    {
                        claims.Add(new Claim(ClaimTypes.Name, claims.First(c => c.Type == "unique_name").Value));
                    }

                    // Create authentication cookie  
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Persistent cookie for "Remember Me" functionality  
                        ExpiresUtc = tokenResponse.Expiration, // Align cookie expiration with JWT expiration  
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Redirect to return URL or home page  
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Home", "Home");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Login failed. Status: {response.StatusCode}. {(string.IsNullOrWhiteSpace(errorContent) ? "" : $"Details: {errorContent}")}");
                }
            }

            ViewData["ShowGradient"] = false;
            return View(model);
        }

        // GET: /Account/SignOut  
        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Clear authentication cookie  
            HttpContext.Session.Remove("JWToken"); // Clear JWT from session  
            return RedirectToAction("Index", "Home"); // Redirect to home page  
        }

        // GET: /Account/SignUp  
        [HttpGet]
        public IActionResult SignUp()
        {
            ViewData["ShowGradient"] = false;
            return View();
        }

        // POST: /Account/SignUp  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                string requestUrl = $"{_apiBaseUrl}/Authenticate/register"; // API endpoint for registration  

                // Prepare registration payload  
                var payload = new
                {
                    username = model.Username,
                    email = model.Email,
                    password = model.Password
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                // Send registration request to API  
                var response = await _httpClient.PostAsync(requestUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Registration successful! Please sign in."; // Success message  
                    ViewData["ShowGradient"] = false;
                    return RedirectToAction("SignIn"); // Redirect to SignIn page  
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorResponse = JsonDocument.Parse(error);
                        if (errorResponse.RootElement.TryGetProperty("message", out var messageElement))
                        {
                            ModelState.AddModelError(string.Empty, $"Registration failed: {messageElement.GetString()}");
                        }
                        else if (errorResponse.RootElement.TryGetProperty("title", out var titleElement))
                        {
                            ModelState.AddModelError(string.Empty, $"Registration failed: {titleElement.GetString()}");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Registration failed: " + error);
                        }
                    }
                    catch (JsonException)
                    {
                        ModelState.AddModelError(string.Empty, "Registration failed: " + error);
                    }
                }
            }

            ViewData["ShowGradient"] = false;
            return View(model);
        }

        // GET: /Account/AccessDenied  
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Account  
        [Authorize]
        public IActionResult Account()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeUsername(string NewUsername)
        {
            var token = HttpContext.Session.GetString("JWToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("NewUsername", NewUsername)
        });

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/account/change-username", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Username updated to: {NewUsername}";
            }
            else
            {
                ViewBag.ErrorUsername = GetErrorMessage(responseContent);
                return View("Account");
            }

            return RedirectToAction("Account");
        }

        // POST: /Account/ChangeEmail  
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeEmail(string NewEmail)
        {
            var token = HttpContext.Session.GetString("JWToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("NewEmail", NewEmail)
        });

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/account/change-email", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Email updated to: {NewEmail}";
            }
            else
            {
                ViewBag.ErrorEmail = GetErrorMessage(responseContent);
                return View("Account");
            }

            return RedirectToAction("Account");
        }

        // POST: /Account/ChangePassword  
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword)
        {
            var token = HttpContext.Session.GetString("JWToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("CurrentPassword", CurrentPassword),
            new KeyValuePair<string, string>("NewPassword", NewPassword)
        });

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/account/change-password", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Password updated successfully!";
            }
            else
            {
                ViewBag.ErrorPassword = GetErrorMessage(responseContent);
                return View("Account");
            }

            return RedirectToAction("Account");
        }

        private string GetErrorMessage(string responseContent)
        {
            try
            {
                var errorJson = JsonDocument.Parse(responseContent);
                var root = errorJson.RootElement;

                if (root.TryGetProperty("message", out var message))
                {
                    return message.GetString() ?? "An error occurred.";
                }
                else if (root.TryGetProperty("title", out var title))
                {
                    return title.GetString() ?? "An error occurred.";
                }
                else if (root.TryGetProperty("Errors", out var errors) && errors.ValueKind == JsonValueKind.Array)
                {
                    // Join all errors into a single string
                    return string.Join(" ", errors.EnumerateArray().Select(e => e.GetString()));
                }
                else
                {
                    return responseContent; // fallback to raw response
                }
            }
            catch (JsonException)
            {
                return "Failed to parse error response.";
            }
        }

    }
}