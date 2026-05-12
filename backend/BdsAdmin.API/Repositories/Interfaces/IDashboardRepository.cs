namespace BdsAdmin.API.Repositories.Interfaces;

public interface IDashboardRepository
{
    Task<int> CountPropertiesAsync(Guid? sellerId = null);
    Task<int> CountPendingPropertiesAsync();
    Task<int> CountLeadsAsync(Guid? sellerId = null);
    Task<int> CountConversationsAsync(Guid? consultantId = null);
}
