using DFPlanningApi.DTOs;

namespace DFPlanningApi.Services;

public interface IDemandForecastService
{
    Task<DemandForecastResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<DemandForecastResponse>> GetAllAsync(string? productCode, string? locationCode, CancellationToken cancellationToken);
    Task<DemandForecastResponse> CreateAsync(DemandForecastRequest request, CancellationToken cancellationToken);
    Task<DemandForecastResponse?> UpdateAsync(int id, DemandForecastRequest request, CancellationToken cancellationToken);
    Task<DemandForecastResponse?> ChangeStatusAsync(int id, string status, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
