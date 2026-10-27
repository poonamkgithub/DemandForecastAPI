namespace DFPlanningApi.DTOs;

public class DemandForecastResponse
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public DateOnly ForecastDate { get; set; }
    public decimal BaselineDemand { get; set; }
    public decimal PromotionUpliftPercent { get; set; }
    public decimal ManualAdjustment { get; set; }
    public decimal ForecastQuantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
