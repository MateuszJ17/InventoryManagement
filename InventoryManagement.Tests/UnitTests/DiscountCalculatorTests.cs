using InventoryManagement.Features.Orders.CalculateDiscount;
using Microsoft.Extensions.Logging.Abstractions;

namespace InventoryManagement.Tests.UnitTests;

public class DiscountCalculatorTests
{
    private readonly DiscountCalculator _sut = new(new HolidaysDaysProvider(), NullLogger<DiscountCalculator>.Instance);

    private static readonly DateOnly RegularDay = new(2024, 6, 10);
    private static readonly DateOnly BlackFriday = new(2024, 11, 22);
    private static readonly DateOnly NewYear = new(2024, 1, 1);

    [Theory]
    [InlineData(1, 0.0)]
    [InlineData(4, 0.0)]
    [InlineData(5, 0.1)]
    [InlineData(9, 0.1)]
    [InlineData(10, 0.2)]
    [InlineData(49, 0.2)]
    [InlineData(50, 0.3)]
    [InlineData(100, 0.3)]
    public void CalculateDiscount_OnRegularDay_AppliesVolumeDiscount(int units, decimal expected)
    {
        var prices = new[] { 100m };

        var result = _sut.CalculateDiscount(units, prices, RegularDay);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateDiscount_OnBlackFriday_WithLowVolume_AppliesBlackFridayDiscount()
    {
        var prices = new[] { 100m, 200m };

        var result = _sut.CalculateDiscount(1, prices, BlackFriday);

        Assert.Equal(0.25m, result);
    }

    [Fact]
    public void CalculateDiscount_OnHoliday_WithLowVolume_AppliesSeasonalHolidayDiscount()
    {
        var prices = new[] { 100m, 200m };

        var result = _sut.CalculateDiscount(1, prices, NewYear);

        Assert.Equal(200m / 300m * 0.15m, result);
    }

    [Fact]
    public void CalculateDiscount_TakesTheBiggestOfVolumeAndSeasonal()
    {
        var prices = new[] { 100m };

        var result = _sut.CalculateDiscount(50, prices, BlackFriday);

        Assert.Equal(0.3m, result);
    }
}
