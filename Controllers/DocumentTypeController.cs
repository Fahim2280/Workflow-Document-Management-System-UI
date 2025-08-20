using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Workflow_Document_Management_System_UI.DTOs;

namespace Workflow_Document_Management_System_UI.Controllers
{
    public class DocumentTypeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public DocumentTypeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001";
        }

        // GET: DocumentType/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{_apiBaseUrl}/api/DocumentType/list");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseDto<List<DocumentTypeResponseDto>>>(content);

                    if (apiResponse.Success)
                    {
                        return View(apiResponse.Data);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = apiResponse.Message;
                        return View(new List<DocumentTypeResponseDto>());
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve document types";
                    return View(new List<DocumentTypeResponseDto>());
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(new List<DocumentTypeResponseDto>());
            }
        }

        // GET: DocumentType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DocumentType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDocumentTypeDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_apiBaseUrl}/api/DocumentType/create", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseDto<int>>(responseContent);

                if (response.IsSuccessStatusCode && apiResponse.Success)
                {
                    TempData["SuccessMessage"] = apiResponse.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = apiResponse.Message;
                    if (apiResponse.Errors != null && apiResponse.Errors.Any())
                    {
                        foreach (var error in apiResponse.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // GET: DocumentType/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{_apiBaseUrl}/api/DocumentType/list");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseDto<List<DocumentTypeResponseDto>>>(content);

                    if (apiResponse.Success)
                    {
                        var documentType = apiResponse.Data.FirstOrDefault(dt => dt.DocumentTypeId == id);
                        if (documentType == null)
                        {
                            TempData["ErrorMessage"] = "Document type not found";
                            return RedirectToAction(nameof(Index));
                        }

                        var updateDto = new UpdateDocumentTypeDto
                        {
                            DocumentTypeId = documentType.DocumentTypeId,
                            TypeName = documentType.TypeName,
                            Description = documentType.Description,
                            AllowedExtensions = documentType.AllowedExtensions,
                            MaxFileSizeMB = documentType.MaxFileSizeMB
                        };

                        return View(updateDto);
                    }
                }

                TempData["ErrorMessage"] = "Failed to retrieve document type";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: DocumentType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateDocumentTypeDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"{_apiBaseUrl}/api/DocumentType/update", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseDto<bool>>(responseContent);

                if (response.IsSuccessStatusCode && apiResponse.Success)
                {
                    TempData["SuccessMessage"] = apiResponse.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = apiResponse.Message;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // GET: DocumentType/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{_apiBaseUrl}/api/DocumentType/list");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseDto<List<DocumentTypeResponseDto>>>(content);

                    if (apiResponse.Success)
                    {
                        var documentType = apiResponse.Data.FirstOrDefault(dt => dt.DocumentTypeId == id);
                        if (documentType == null)
                        {
                            TempData["ErrorMessage"] = "Document type not found";
                            return RedirectToAction(nameof(Index));
                        }

                        return View(documentType);
                    }
                }

                TempData["ErrorMessage"] = "Failed to retrieve document type";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: DocumentType/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.DeleteAsync($"{_apiBaseUrl}/api/DocumentType/delete/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseDto<bool>>(responseContent);

                if (response.IsSuccessStatusCode && apiResponse.Success)
                {
                    TempData["SuccessMessage"] = apiResponse.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = apiResponse.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
