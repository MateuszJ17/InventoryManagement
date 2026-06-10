namespace InventoryManagement.Database.Entities;

public class Customer(Guid customerId, string name, string regionalCode)
{
    public Guid CustomerId { get; set; } = customerId;
    public string Name { get; set; } = name;
    public string RegionalCode { get; set; } = regionalCode;
}