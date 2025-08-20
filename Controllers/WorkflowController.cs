using Microsoft.AspNetCore.Mvc;
using Workflow_Document_Management_System_UI.DTOs;
using Workflow_Document_Management_System_UI.Services;

namespace Workflow_Document_Management_System_UI.Controllers
{
    public class WorkflowController : Controller
    {
        private readonly WorkflowApiService _workflowApiService;
        private readonly AdminApiService _adminApiService;

        public WorkflowController(WorkflowApiService workflowApiService, AdminApiService adminApiService)
        {
            _workflowApiService = workflowApiService;
            _adminApiService = adminApiService;
        }

        // GET: Workflow/Index
        public async Task<IActionResult> Index(string searchTerm = "", string filterType = "", bool filterActive = true)
        {
            var response = await _workflowApiService.GetAllWorkflowsAsync();
            var viewModel = new WorkflowListViewModel
            {
                SearchTerm = searchTerm,
                FilterType = filterType,
                FilterActive = filterActive
            };

            if (response.Success && response.Data != null)
            {
                var workflows = response.Data;

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    workflows = workflows.Where(w => w.WorkflowName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                                   w.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true).ToList();
                }

                if (!string.IsNullOrEmpty(filterType))
                {
                    workflows = workflows.Where(w => w.WorkflowType.Equals(filterType, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                workflows = workflows.Where(w => w.IsActive == filterActive).ToList();

                viewModel.Workflows = workflows;
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to load workflows";
            }

            return View(viewModel);
        }

        // GET: Workflow/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var response = await _workflowApiService.GetWorkflowByIdAsync(id);

            if (response.Success && response.Data != null)
            {
                return View(response.Data);
            }

            TempData["ErrorMessage"] = response.Message ?? "Workflow not found";
            return RedirectToAction(nameof(Index));
        }

        // GET: Workflow/Create
        public async Task<IActionResult> Create()
        {
            var adminResponse = await _adminApiService.GetAllAdminsAsync();
            var viewModel = new CreateWorkflowViewModel();

            if (adminResponse.Success && adminResponse.Data != null)
            {
                viewModel.AvailableAdmins = adminResponse.Data;
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to load admin list";
            }

            return View(viewModel);
        }

        // POST: Workflow/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateWorkflowViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _workflowApiService.CreateWorkflowAsync(model);

                if (response.Success)
                {
                    TempData["SuccessMessage"] = response.Message ?? "Workflow created successfully";
                    return RedirectToAction(nameof(Details), new { id = response.Data });
                }

                if (response.Errors != null && response.Errors.Any())
                {
                    foreach (var error in response.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                else
                {
                    ModelState.AddModelError("", response.Message ?? "Failed to create workflow");
                }
            }

            // Reload admin list
            var adminResponse = await _adminApiService.GetAllAdminsAsync();
            if (adminResponse.Success && adminResponse.Data != null)
            {
                model.AvailableAdmins = adminResponse.Data;
                // Restore selected state
                foreach (var admin in model.AvailableAdmins)
                {
                    admin.IsSelected = model.AssignedAdminIds.Contains(admin.AdminId);
                }
            }

            return View(model);
        }

        // GET: Workflow/Analytics
        public async Task<IActionResult> Analytics(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var analyticsResponse = await _workflowApiService.GetWorkflowAnalyticsAsync(fromDate, toDate);
            var workflowsResponse = await _workflowApiService.GetAllWorkflowsAsync();

            var viewModel = new
            {
                Analytics = analyticsResponse.Success ? analyticsResponse.Data : new List<WorkflowAnalyticsViewModel>(),
                Filter = new AnalyticsFilterViewModel
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    AvailableWorkflows = workflowsResponse.Success && workflowsResponse.Data != null
                        ? workflowsResponse.Data.Select(w => new WorkflowSelectViewModel
                        {
                            WorkflowId = w.WorkflowId,
                            WorkflowName = w.WorkflowName,
                            WorkflowType = w.WorkflowType
                        }).ToList()
                        : new List<WorkflowSelectViewModel>()
                }
            };

            if (!analyticsResponse.Success)
            {
                TempData["ErrorMessage"] = analyticsResponse.Message ?? "Failed to load analytics";
            }

            return View(viewModel);
        }

        // GET: Workflow/AdminWorkload
        public async Task<IActionResult> AdminWorkload(int? workflowId = null)
        {
            var workloadResponse = await _workflowApiService.GetAdminWorkloadAnalyticsAsync(workflowId);
            var workflowsResponse = await _workflowApiService.GetAllWorkflowsAsync();

            var viewModel = new
            {
                AdminWorkloads = workloadResponse.Success ? workloadResponse.Data : new List<AdminWorkloadViewModel>(),
                Filter = new AnalyticsFilterViewModel
                {
                    WorkflowId = workflowId,
                    AvailableWorkflows = workflowsResponse.Success && workflowsResponse.Data != null
                        ? workflowsResponse.Data.Select(w => new WorkflowSelectViewModel
                        {
                            WorkflowId = w.WorkflowId,
                            WorkflowName = w.WorkflowName,
                            WorkflowType = w.WorkflowType
                        }).ToList()
                        : new List<WorkflowSelectViewModel>()
                }
            };

            if (!workloadResponse.Success)
            {
                TempData["ErrorMessage"] = workloadResponse.Message ?? "Failed to load admin workload data";
            }

            return View(viewModel);
        }
    }
}
