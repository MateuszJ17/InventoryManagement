using InventoryManagement.Features.Orders.CalculateDiscount;

namespace InventoryManagement.Features.Orders.CalculatePrice;

public class FinalPriceCalculator(
    IDiscountCalculator discountCalculator,
    ILogger<FinalPriceCalculator> logger)
    : IFinalPriceCalculator
{
    public decimal CalculateFinalPrice(
        decimal originalTotal,
        int unitsPurchased,
        string regionalCode,
        DateOnly date,
        IReadOnlyCollection<decimal> productPrices)
    {
        var multiplier = GetRegionalMultiplier(regionalCode);
        
        logger.LogInformation("Regional multiplier: {Multiplier}", multiplier);
        
        var regionAdjustedPrices = productPrices.Select(p => p * multiplier).ToList();
        var regionAdjustedTotal = regionAdjustedPrices.Sum();

        var biggestDiscount = discountCalculator.CalculateDiscount(unitsPurchased, regionAdjustedPrices, date);

        return regionAdjustedTotal - (regionAdjustedTotal * biggestDiscount);
    }

    private static decimal GetRegionalMultiplier(string regionalCode) => regionalCode switch
    {
        "EU" => 1.15m,
        "AS" => 1.05m,
        _    => 1m
    };
}