namespace InventoryManagement.Features.Orders.CalculateDiscount;

public interface IDiscountCalculator
{
    decimal CalculateDiscount(int unitsPurchased, IReadOnlyCollection<decimal> productPrices, DateOnly date);
}