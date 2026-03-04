using Eccomerce_Web.Enums;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public int UserId { get; set; }
    public OrdersEnums OrdersEnums { get; set; } = OrdersEnums.Pending;
    public List<OrderItem> Products { get; set; }

}