# DFPlanningApi — Demand Forecasting API

A learning project for a DF (Demand Forecasting) module using ASP.NET Core 8 Web API, C#, SQL Server, EF Core, and Swagger.

## Business logic

Forecast quantity = `max(0, BaselineDemand + (BaselineDemand * PromotionUpliftPercent / 100) + ManualAdjustment)`

Example: baseline demand 1,000, promotion uplift 10%, manual adjustment -20 => forecast quantity 1,080.

This is a simplified example. Real forecasting may use historical sales, seasonality, trend, promotions, forecast accuracy metrics, and approval workflows.

## Requirements

- .NET 8 SDK
- SQL Server LocalDB or another SQL Server instance

## Run

```bash
dotnet restore
dotnet run
```

Open the HTTPS URL shown in the terminal and append `/swagger` if needed. The app creates the demo database/table on startup using `EnsureCreated()`. For a production app, use EF Core migrations and a reviewed deployment process.

## Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/demand-forecast` | Calculate and save a forecast |
| GET | `/api/demand-forecast` | List forecasts; optional `productCode` and `locationCode` filters |
| GET | `/api/demand-forecast/{id}` | Get one forecast |
| PUT | `/api/demand-forecast/{id}` | Update inputs and recalculate |
| PATCH | `/api/demand-forecast/{id}/status` | Set status to Draft, Approved, or Rejected |
| DELETE | `/api/demand-forecast/{id}` | Delete forecast |

## Sample POST body

```json
{
  "productCode": "VESSEL-A",
  "locationCode": "MUM-WH1",
  "forecastDate": "2026-11-01",
  "baselineDemand": 1000,
  "promotionUpliftPercent": 10,
  "manualAdjustment": -20
}
```

Expected `forecastQuantity`: `1080`.

## Notes

- API validates required product/location codes and quantity ranges.
- This ZIP contains source code and has not been compiled against your local machine or SQL Server instance.
