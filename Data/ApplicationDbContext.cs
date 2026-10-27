using Microsoft.EntityFrameworkCore;
using DFPlanningApi.Models;

namespace DFPlanningApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<DemandForecast> DemandForecasts => Set<DemandForecast>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var forecast = modelBuilder.Entity<DemandForecast>();
        forecast.HasKey(x => x.Id);
        forecast.Property(x => x.ProductCode).HasMaxLength(50).IsRequired();
        forecast.Property(x => x.LocationCode).HasMaxLength(50).IsRequired();
        forecast.Property(x => x.BaselineDemand).HasPrecision(18, 2);
        forecast.Property(x => x.PromotionUpliftPercent).HasPrecision(9, 2);
        forecast.Property(x => x.ManualAdjustment).HasPrecision(18, 2);
        forecast.Property(x => x.ForecastQuantity).HasPrecision(18, 2);
        forecast.Property(x => x.Status).HasMaxLength(20).IsRequired();
        forecast.HasIndex(x => new { x.ProductCode, x.LocationCode, x.ForecastDate });
    }
}
