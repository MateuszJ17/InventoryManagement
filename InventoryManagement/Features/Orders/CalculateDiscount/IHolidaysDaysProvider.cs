namespace InventoryManagement.Features.Orders.CalculateDiscount;

public interface IHolidaysDaysProvider
{
    IReadOnlyCollection<DateOnly> GetHolidays(int year);
    bool IsBlackFriday(DateOnly date);
}