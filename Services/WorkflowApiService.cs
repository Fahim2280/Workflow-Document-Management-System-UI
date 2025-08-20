using System.Text.Json;
using System.Text;
using Workflow_Document_Management_System_UI.DTOs;

namespace Workflow_Document_Management_System_UI.Services
{
    public class WorkflowApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions;

        public WorkflowApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<int>> CreateWorkflowAsync(CreateWorkflowViewModel model)
        {
            try
            {
                var createDto = new
                {
                    WorkflowName = model.WorkflowName,
                    WorkflowType = model.WorkflowType,
                    Description = model.Description,
                    AssignedAdminIds = model.AssignedAdminIds
                };

                var json = JsonSerializer.Serialize(createDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/workflow/create", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<int>>(responseContent, _jsonOptions);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<WorkflowResponseViewModel>>> GetAllWorkflowsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/workflow/list");
                var responseContent = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<WorkflowResponseViewModel>>>(responseContent, _jsonOptions);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<WorkflowResponseViewModel>> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<WorkflowDetailViewModel>> GetWorkflowByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/workflow/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<WorkflowDetailViewModel>>(responseContent, _jsonOptions);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<WorkflowDetailViewModel> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<WorkflowAnalyticsViewModel>>> GetWorkflowAnalyticsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (fromDate.HasValue)
                    queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
                if (toDate.HasValue)
                    queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");

                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
                var response = await _httpClient.GetAsync($"api/analytics/workflow{queryString}");
                var responseContent = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<WorkflowAnalyticsViewModel>>>(responseContent, _jsonOptions);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<WorkflowAnalyticsViewModel>> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<AdminWorkloadViewModel>>> GetAdminWorkloadAnalyticsAsync(int? workflowId = null)
        {
            try
            {
                var queryString = workflowId.HasValue ? $"?workflowId={workflowId}" : "";
                var response = await _httpClient.GetAsync($"api/analytics/admin-workload{queryString}");
                var responseContent = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<AdminWorkloadViewModel>>>(responseContent, _jsonOptions);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AdminWorkloadViewModel>> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
