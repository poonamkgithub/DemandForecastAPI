using System.ComponentModel.DataAnnotations;

namespace DFPlanningApi.DTOs;

public class DemandForecastRequest
{
    [Required, StringLength(50, MinimumLength = 1)]
    public string ProductCode { get; set; } = string.Empty;

    [Required, StringLength(50, MinimumLength = 1)]
    public string LocationCode { get; set; } = string.Empty;

    public DateOnly ForecastDate { get; set; }

    [Range(typeof(decimal), "0", "1000000000")]
    public decimal BaselineDemand { get; set; }

    [Range(typeof(decimal), "0", "500")]
    public decimal PromotionUpliftPercent { get; set; }

    [Range(typeof(decimal), "-1000000000", "1000000000")]
    public decimal ManualAdjustment { get; set; }
}
