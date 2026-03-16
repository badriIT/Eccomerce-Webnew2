using Eccomerce_Web.Modules.Orders.Dtos.Response;

public class OrdersAdminDto
{
    public int TotalOrders { get; set; }
    public double TotalRevenue { get; set; }
    public List<OrderDto> Orders { get; set; }
}