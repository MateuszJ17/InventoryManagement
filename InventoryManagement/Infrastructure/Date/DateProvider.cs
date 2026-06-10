namespace InventoryManagement.Infrastructure.Date;

public class DateProvider : IDateProvider
{
    public DateOnly GetToday()
    {
        return DateOnly.FromDateTime(DateTime.UtcNow);
    }
}