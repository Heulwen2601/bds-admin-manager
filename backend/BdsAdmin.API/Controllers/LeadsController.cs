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
public class LeadsController(ILeadService leads, ISellerProfileService sellerProfiles) : ControllerBase
{
    [HttpGet("customer-data-policy")]
    public async Task<IActionResult> GetCustomerDataPolicyConsent() =>
        Ok(ApiResponse<CustomerDataPolicyConsentResponse>.Ok(await sellerProfiles.GetCustomerDataPolicyConsentAsync(User.GetUserId()!.Value)));

    [HttpPost("customer-data-policy/accept")]
    public async Task<IActionResult> AcceptCustomerDataPolicy() =>
        Ok(ApiResponse<CustomerDataPolicyConsentResponse>.Ok(await sellerProfiles.AcceptCustomerDataPolicyAsync(User.GetUserId()!.Value)));

    [HttpGet]
    public async Task<IActionResult> GetSellerLeads()
    {
        var userId = User.GetUserId()!.Value;
        var consent = await sellerProfiles.GetCustomerDataPolicyConsentAsync(userId);
        if (!consent.HasAccepted)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.Fail("Customer data policy must be accepted before accessing leads."));
        }

        return Ok(ApiResponse<IReadOnlyList<LeadResponse>>.Ok(await leads.GetSellerLeadsAsync(userId)));
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        var userId = User.GetUserId()!.Value;
        var consent = await sellerProfiles.GetCustomerDataPolicyConsentAsync(userId);
        if (!consent.HasAccepted)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.Fail("Customer data policy must be accepted before accessing leads."));
        }

        var lead = await leads.MarkReadAsync(userId, id);
        return lead == null ? NotFound(ApiResponse<object>.Fail("Lead not found.")) : Ok(ApiResponse<LeadResponse>.Ok(lead));
    }
}
