using Eccomerce_Web.Enums;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public int UserId { get; set; }
    public OrdersEnums OrdersEnums { get; set; }
    public List<OrderItem> Products { get; set; }

}