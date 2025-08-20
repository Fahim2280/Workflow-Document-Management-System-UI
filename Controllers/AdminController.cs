using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using Workflow_Document_Management_System_UI.DTOs;

namespace Workflow_Document_Management_System_UI.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            FloatParseHandling = FloatParseHandling.Decimal,
            DateParseHandling = DateParseHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Error = (sender, args) => {
                // Handle JSON errors gracefully
                args.ErrorContext.Handled = true;
            }
        };

        public AdminController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7271"; // Adjust port as needed
        }

        private T SafeDeserialize<T>(string json) where T : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON Deserialization Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"JSON Content: {json}");
                return null;
            }
        }

        // GET: Admin/Login
        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to dashboard
            if (IsAdminLoggedIn())
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Admin/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var loginResponse = JsonConvert.DeserializeObject<AdminLoginResponseDto>(responseContent);

                        if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Username))
                        {
                            // Store session information
                            HttpContext.Session.SetString("AdminUsername", loginResponse.Username);
                            HttpContext.Session.SetString("AdminAccessLevel", loginResponse.AccessLevel ?? "Read-Only");

                            TempData["SuccessMessage"] = "Login successful!";
                            return RedirectToAction("Dashboard");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid login response");
                        }
                    }
                    catch (JsonException)
                    {
                        ModelState.AddModelError("", "Server returned invalid response");
                    }
                }
                else
                {
                    try
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponseDto>(responseContent);
                        ModelState.AddModelError("", errorResponse?.Message ?? "Login failed");
                    }
                    catch (JsonException)
                    {
                        ModelState.AddModelError("", $"Login failed with status: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred. Please try again.");
                // Log the actual error for debugging
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            }

            return View(model);
        }

        //GET: Admin/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var username = HttpContext.Session.GetString("AdminUsername");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Get current admin info
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Admin/current");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var currentAdmin = JsonConvert.DeserializeObject<dynamic>(content);

                    // Create a proper object with all needed properties
                    ViewBag.CurrentAdmin = new
                    {
                        username = currentAdmin?.username?.ToString() ?? username,
                        accessLevel = currentAdmin?.accessLevel?.ToString() ?? HttpContext.Session.GetString("AdminAccessLevel"),
                        hasWriteAccess = (currentAdmin?.accessLevel?.ToString() == "Read-Write") ||
                                       (currentAdmin?.hasWriteAccess == true)
                    };
                }
                else
                {
                    // Fallback to session data
                    ViewBag.CurrentAdmin = new
                    {
                        username = username,
                        accessLevel = HttpContext.Session.GetString("AdminAccessLevel") ?? "Read-Only",
                        hasWriteAccess = HttpContext.Session.GetString("AdminAccessLevel") == "Read-Write"
                    };
                }

                // Get all admins list - this is optional for the dashboard load
                try
                {
                    var adminsResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Admin/list");
                    if (adminsResponse.IsSuccessStatusCode)
                    {
                        var adminsContent = await adminsResponse.Content.ReadAsStringAsync();
                        var adminsList = JsonConvert.DeserializeObject<dynamic>(adminsContent);
                        ViewBag.AdminsList = adminsList;
                    }
                }
                catch (Exception ex)
                {
                    // Don't fail the dashboard load if admin list fails
                    System.Diagnostics.Debug.WriteLine($"Failed to load admin list: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading dashboard: {ex.Message}";

                // Set fallback data so dashboard can still load
                ViewBag.CurrentAdmin = new
                {
                    username = username,
                    accessLevel = HttpContext.Session.GetString("AdminAccessLevel") ?? "Read-Only",
                    hasWriteAccess = HttpContext.Session.GetString("AdminAccessLevel") == "Read-Write"
                };
            }

            return View();
        }

        // GET: Admin/CreateAdmin
        [HttpGet]
        public IActionResult CreateAdmin()
        {
            var session = HttpContext.Session;
            var username = session.GetString("AdminUsername");
            var accessLevel = session.GetString("AdminAccessLevel");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            if (accessLevel != "Read-Write")
            {
                TempData["ErrorMessage"] = "You don't have permission to create admins";
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        // POST: Admin/CreateAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(CreateAdminDto model)
        {
            var session = HttpContext.Session;
            var username = session.GetString("AdminUsername");
            var accessLevel = session.GetString("AdminAccessLevel");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            if (accessLevel != "Read-Write")
            {
                TempData["ErrorMessage"] = "You don't have permission to create admins";
                return RedirectToAction("Dashboard");
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Admin/create", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Admin created successfully!";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    ModelState.AddModelError("", errorResponse?.message?.ToString() ?? "Failed to create admin");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            return View(model);
        }

        // POST: Admin/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Call API logout
                await _httpClient.PostAsync($"{_apiBaseUrl}/api/Admin/logout", null);
            }
            catch
            {
                // Continue with local logout even if API call fails
            }

            // Clear session
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Logged out successfully";
            return RedirectToAction("Login");
        }

        // GET: Admin/AdminList (Ajax endpoint)
        [HttpGet]
        public async Task<IActionResult> GetAdminsList()
        {
            var session = HttpContext.Session;
            var username = session.GetString("AdminUsername");
            if (string.IsNullOrEmpty(username))
            {
                return Json(new { success = false, message = "Not authorized" });
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"Calling API: {_apiBaseUrl}/api/Admin/list");

                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Admin/list");
                var content = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"API Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"API Response Content: {content}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(content);
                    return Json(new { success = true, data = result });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"API call failed: {response.StatusCode} - {content}");
                    return Json(new { success = false, message = $"API returned {response.StatusCode}: {content}" });
                }
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Request Exception: {httpEx.Message}");
                return Json(new { success = false, message = $"Connection error: {httpEx.Message}" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Exception: {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Add this helper method to your AdminController
        private bool IsAdminLoggedIn()
        {
            try
            {
                var session = HttpContext.Session;
                return session != null && !string.IsNullOrEmpty(session.GetString("AdminUsername"));
            }
            catch
            {
                return false;
            }
        }

        private string GetCurrentAdminUsername()
        {
            try
            {
                var session = HttpContext.Session;
                return session?.GetString("AdminUsername");
            }
            catch
            {
                return null;
            }
        }

        private string GetCurrentAdminAccessLevel()
        {
            try
            {
                var session = HttpContext.Session;
                return session?.GetString("AdminAccessLevel");
            }
            catch
            {
                return null;
            }
        }
    }
}