namespace InventoryManagement.Features.Orders.CalculatePrice;

public interface IFinalPriceCalculator
{
    decimal CalculateFinalPrice(
        decimal originalTotal,
        int unitsPurchased,
        string regionalCode,
        DateOnly date,
        IReadOnlyCollection<decimal> productPrices);
}