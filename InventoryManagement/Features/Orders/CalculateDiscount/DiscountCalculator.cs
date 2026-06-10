namespace InventoryManagement.Features.Orders.CalculateDiscount;

public class DiscountCalculator(
    IHolidaysDaysProvider holidaysDaysProvider,
    ILogger<DiscountCalculator> logger) : IDiscountCalculator
{
    public decimal CalculateDiscount(int unitsPurchased, IReadOnlyCollection<decimal> productPrices, DateOnly date)
    {
        var volumeDiscount = CalculateVolumeDiscount(unitsPurchased);
        var seasonalDiscount = CalculateSeasonalDiscount(productPrices, date);

        return Math.Max(volumeDiscount, seasonalDiscount);
    }
    
    private decimal CalculateVolumeDiscount(int unitsPurchased)
    {
        return unitsPurchased switch
        {
            >= 50 => 0.3m,
            >= 10 => 0.2m,
            >= 5 => 0.1m,
            _ => 0m
        };
    }

    private decimal CalculateSeasonalDiscount(IReadOnlyCollection<decimal> productPrices, DateOnly date)
    {

        if (holidaysDaysProvider.IsBlackFriday(date))
        {
            logger.LogInformation("Black Friday discount");
            return 0.25m;
        }

        if (holidaysDaysProvider.GetHolidays(date.Year).Contains(date))
        {
            logger.LogInformation("Holiday discount");
            return productPrices.Max() / productPrices.Sum() * 0.15m;
        }

        logger.LogInformation("No discount");
        return 0m;
    }
}