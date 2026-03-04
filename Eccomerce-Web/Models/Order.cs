public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public int UserId { get; set; }

    public List<OrderItem> Products { get; set; }
}