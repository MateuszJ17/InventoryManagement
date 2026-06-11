namespace InventoryManagement.Database.Entities;

public class Customer(string name, string regionalCode)
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; } = name;
    public string RegionalCode { get; set; } = regionalCode;
}