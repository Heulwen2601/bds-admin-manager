using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Helpers;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers;

[ApiController]
public class DashboardController(IDashboardService dashboard) : ControllerBase
{
    [Authorize(Policy = AuthPolicies.AdminOnly), HttpGet("api/v1/admin/dashboard")]
    public async Task<IActionResult> Admin() => Ok(ApiResponse<DashboardResponse>.Ok(await dashboard.GetAdminDashboardAsync()));

    [Authorize(Policy = AuthPolicies.AdminOnly), HttpGet("api/v1/admin/dashboard/consultant-performance")]
    public async Task<IActionResult> ConsultantPerformance() => Ok(ApiResponse<DashboardResponse>.Ok(await dashboard.GetConsultantPerformanceAsync()));

    [Authorize(Policy = AuthPolicies.SellerOnly), HttpGet("api/v1/seller/dashboard")]
    public async Task<IActionResult> Seller() => Ok(ApiResponse<DashboardResponse>.Ok(await dashboard.GetSellerDashboardAsync(User.GetUserId()!.Value)));
}
