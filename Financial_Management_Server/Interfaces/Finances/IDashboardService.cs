using Financial_Management_Server.DTOs.Finances;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardAsync(DashboardRequest request);
    }
}
