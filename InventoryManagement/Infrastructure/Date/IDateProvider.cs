namespace InventoryManagement.Infrastructure.Date;

public interface IDateProvider
{
    DateOnly GetToday();
}