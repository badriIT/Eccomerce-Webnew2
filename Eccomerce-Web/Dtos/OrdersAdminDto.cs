using Eccomerce_Web.Dtos;

public class OrdersAdminDto
{
    public int TotalOrders { get; set; }
    public double TotalRevenue { get; set; }
    public List<OrderDto> Orders { get; set; }
}