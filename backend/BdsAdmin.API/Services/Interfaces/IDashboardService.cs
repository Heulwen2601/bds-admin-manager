using BdsAdmin.API.DTOs;

namespace BdsAdmin.API.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetAdminDashboardAsync();
    Task<DashboardResponse> GetSellerDashboardAsync(Guid sellerId);
    Task<DashboardResponse> GetConsultantPerformanceAsync();
}
