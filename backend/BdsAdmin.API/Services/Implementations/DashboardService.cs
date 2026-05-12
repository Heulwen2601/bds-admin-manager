using BdsAdmin.API.DTOs;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;

namespace BdsAdmin.API.Services.Implementations;

public class DashboardService(IDashboardRepository dashboard) : IDashboardService
{
    public async Task<DashboardResponse> GetAdminDashboardAsync() => new()
    {
        Properties = await dashboard.CountPropertiesAsync(),
        PendingProperties = await dashboard.CountPendingPropertiesAsync(),
        Leads = await dashboard.CountLeadsAsync(),
        Conversations = await dashboard.CountConversationsAsync()
    };

    public async Task<DashboardResponse> GetSellerDashboardAsync(Guid sellerId) => new()
    {
        Properties = await dashboard.CountPropertiesAsync(sellerId),
        Leads = await dashboard.CountLeadsAsync(sellerId)
    };

    public async Task<DashboardResponse> GetConsultantPerformanceAsync() => new()
    {
        Conversations = await dashboard.CountConversationsAsync()
    };
}
