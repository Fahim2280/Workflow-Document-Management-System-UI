using System.Text.Json;
using Workflow_Document_Management_System_UI.DTOs;

namespace Workflow_Document_Management_System_UI.Services
{
    public class AdminApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public AdminApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<List<AdminSelectViewModel>>> GetAllAdminsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/list");
                var responseContent = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<AdminSelectViewModel>>>(responseContent, _jsonOptions);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AdminSelectViewModel>> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }
    }
}
