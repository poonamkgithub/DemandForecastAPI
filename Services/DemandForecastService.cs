using Microsoft.EntityFrameworkCore;
using DFPlanningApi.Data;
using DFPlanningApi.DTOs;
using DFPlanningApi.Models;

namespace DFPlanningApi.Services;

public class DemandForecastService : IDemandForecastService
{
    private readonly ApplicationDbContext _db;

    public DemandForecastService(ApplicationDbContext db) => _db = db;

    public async Task<DemandForecastResponse> CreateAsync(
        DemandForecastRequest request, CancellationToken cancellationToken)
    {
        var forecast = BuildForecast(request);
        _db.DemandForecasts.Add(forecast);
        await _db.SaveChangesAsync(cancellationToken);
        return ToResponse(forecast);
    }

    public async Task<List<DemandForecastResponse>> GetAllAsync(
        string? productCode, string? locationCode, CancellationToken cancellationToken)
    {
        var query = _db.DemandForecasts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(productCode))
            query = query.Where(x => x.ProductCode == productCode.Trim().ToUpperInvariant());

        if (!string.IsNullOrWhiteSpace(locationCode))
            query = query.Where(x => x.LocationCode == locationCode.Trim().ToUpperInvariant());

        var items = await query.OrderBy(x => x.ForecastDate).ToListAsync(cancellationToken);
        return items.Select(ToResponse).ToList();
    }

    public async Task<DemandForecastResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var forecast = await _db.DemandForecasts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return forecast is null ? null : ToResponse(forecast);
    }

    public async Task<DemandForecastResponse?> UpdateAsync(
        int id, DemandForecastRequest request, CancellationToken cancellationToken)
    {
        var forecast = await _db.DemandForecasts
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (forecast is null) return null;

        var recalculated = BuildForecast(request);
        forecast.ProductCode = recalculated.ProductCode;
        forecast.LocationCode = recalculated.LocationCode;
        forecast.ForecastDate = recalculated.ForecastDate;
        forecast.BaselineDemand = recalculated.BaselineDemand;
        forecast.PromotionUpliftPercent = recalculated.PromotionUpliftPercent;
        forecast.ManualAdjustment = recalculated.ManualAdjustment;
        forecast.ForecastQuantity = recalculated.ForecastQuantity;
        // Keep status unchanged when editing forecast inputs.
        await _db.SaveChangesAsync(cancellationToken);
        return ToResponse(forecast);
    }

    public async Task<DemandForecastResponse?> ChangeStatusAsync(
        int id, string status, CancellationToken cancellationToken)
    {
        var forecast = await _db.DemandForecasts
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (forecast is null) return null;

        forecast.Status = status;
        await _db.SaveChangesAsync(cancellationToken);
        return ToResponse(forecast);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var forecast = await _db.DemandForecasts.FindAsync(new object[] { id }, cancellationToken);
        if (forecast is null) return false;
        _db.DemandForecasts.Remove(forecast);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static DemandForecast BuildForecast(DemandForecastRequest request)
    {
        // Demo rule: baseline demand plus promotion uplift plus manual adjustment.
        var uplift = request.BaselineDemand * request.PromotionUpliftPercent / 100m;
        var forecastQuantity = Math.Max(0m, request.BaselineDemand + uplift + request.ManualAdjustment);

        return new DemandForecast
        {
            ProductCode = request.ProductCode.Trim().ToUpperInvariant(),
            LocationCode = request.LocationCode.Trim().ToUpperInvariant(),
            ForecastDate = request.ForecastDate,
            BaselineDemand = request.BaselineDemand,
            PromotionUpliftPercent = request.PromotionUpliftPercent,
            ManualAdjustment = request.ManualAdjustment,
            ForecastQuantity = decimal.Round(forecastQuantity, 2),
            Status = "Draft",
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private static DemandForecastResponse ToResponse(DemandForecast x) => new()
    {
        Id = x.Id,
        ProductCode = x.ProductCode,
        LocationCode = x.LocationCode,
        ForecastDate = x.ForecastDate,
        BaselineDemand = x.BaselineDemand,
        PromotionUpliftPercent = x.PromotionUpliftPercent,
        ManualAdjustment = x.ManualAdjustment,
        ForecastQuantity = x.ForecastQuantity,
        Status = x.Status,
        CreatedAtUtc = x.CreatedAtUtc
    };
}
