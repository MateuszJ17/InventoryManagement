using InventoryManagement.Features.Orders.CalculateDiscount;
using InventoryManagement.Features.Orders.CalculatePrice;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InventoryManagement.Tests.UnitTests;

public class FinalPriceCalculatorTests
{
    private readonly Mock<IDiscountCalculator> _discountCalculator = new();
    private readonly FinalPriceCalculator _sut;

    public FinalPriceCalculatorTests()
    {
        _sut = new FinalPriceCalculator(_discountCalculator.Object, NullLogger<FinalPriceCalculator>.Instance);
    }

    private static readonly DateOnly AnyDate = new(2024, 6, 10);

    private void SetupDiscount(decimal discount) => _discountCalculator
        .Setup(x => x.CalculateDiscount(
            It.IsAny<int>(),
            It.IsAny<IReadOnlyCollection<decimal>>(),
            It.IsAny<DateOnly>()))
        .Returns(discount);

    [Fact]
    public void CalculateFinalPrice_WithUnknownRegion_UsesNoMultiplier()
    {
        SetupDiscount(0m);
        var prices = new[] { 100m, 200m };

        var result = _sut.CalculateFinalPrice(300m, 2, "US", AnyDate, prices);

        Assert.Equal(300m, result);
    }

    [Theory]
    [InlineData("EU", 1.15)]
    [InlineData("AS", 1.05)]
    [InlineData("XX", 1.0)]
    public void CalculateFinalPrice_AppliesRegionalMultiplier(string region, decimal multiplier)
    {
        SetupDiscount(0m);
        var prices = new[] { 100m };

        var result = _sut.CalculateFinalPrice(100m, 1, region, AnyDate, prices);

        Assert.Equal(100m * multiplier, result);
    }

    [Fact]
    public void CalculateFinalPrice_AppliesDiscountToRegionAdjustedTotal()
    {
        SetupDiscount(0.10m);
        var prices = new[] { 100m };

        var result = _sut.CalculateFinalPrice(100m, 1, "EU", AnyDate, prices);

        Assert.Equal(103.5m, result);
    }
}
