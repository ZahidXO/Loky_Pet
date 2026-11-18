namespace LokyPet.Application.Interfaces;
using LokyPet.Application.DTOs;

public interface IDashboardMetricsService
{
    Task<DashboardMetricsDto> GetMetricsAsync(DateTime? from = null, DateTime? to = null);
}
