namespace LokyPet.Application.DTOs;

public sealed class DashboardMetricsDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageTicket { get; set; }
    public int TotalItemsSold { get; set; }
    public decimal ShippingRevenue { get; set; }
    public int LocalOrders { get; set; }
    public int NationalOrders { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
    public List<SalesByDateDto> SalesByDate { get; set; } = new();
    public List<SalesByCategoryDto> SalesByCategory { get; set; } = new();
}

public sealed class SalesByDateDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int Orders { get; set; }
}

public sealed class SalesByCategoryDto
{
    public string Category { get; set; } = "";
    public decimal Revenue { get; set; }
    public int ItemsSold { get; set; }
}
