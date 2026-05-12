using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Helpers;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers;

[ApiController]
[Route("api/v1/seller/leads")]
[Authorize(Policy = AuthPolicies.SellerOnly)]
public class LeadsController(ILeadService leads) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSellerLeads() =>
        Ok(ApiResponse<IReadOnlyList<LeadResponse>>.Ok(await leads.GetSellerLeadsAsync(User.GetUserId()!.Value)));
}
