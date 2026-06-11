namespace InventoryManagement.Features.Orders.CalculateDiscount;

public class HolidaysDaysProvider : IHolidaysDaysProvider
{
    public IReadOnlyCollection<DateOnly> GetHolidays(int year)
    {
        return
        [
            new DateOnly(year, 1, 1),   // New Year
            new DateOnly(year, 1, 6),   // Epiphany
            new DateOnly(year, 5, 1),   // Labor Day
            new DateOnly(year, 5, 3),   // Constitution Day
            new DateOnly(year, 8, 15),  // Assumption
            new DateOnly(year, 11, 1),  // All Saints
            new DateOnly(year, 11, 11), // Independence Day
            new DateOnly(year, 12, 25), // Christmas
            new DateOnly(year, 12, 26) // Christmas 2nd day
        ];
    }
    
    public bool IsBlackFriday(DateOnly date)
    {
        return date.Month == 11
               && date.DayOfWeek == DayOfWeek.Friday
               && date.Day is >= 22 and <= 28;
    }
}