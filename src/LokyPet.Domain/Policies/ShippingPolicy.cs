namespace LokyPet.Domain.Policies;

using LokyPet.Domain.Enums;
using LokyPet.Domain.ValueObjects;

public sealed class ShippingPolicy
{
    public string LocalCity { get; init; } = "Reynosa";
    public string LocalState { get; init; } = "Tamaulipas";
    public Money NationalShippingCost { get; init; } = Money.FromMxn(139.00m);
    public string NationalCarrier { get; init; } = "DHL";
    public string LocalDeliveryTime { get; init; } = "24 a 48 horas hábiles";
    public string NationalDeliveryTime { get; init; } = "3 a 5 días hábiles";

    public ShippingType Resolve(string city, string state)
    {
        bool isLocal = string.Equals(city?.Trim(), LocalCity, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(state?.Trim(), LocalState, StringComparison.OrdinalIgnoreCase);
        return isLocal ? ShippingType.Local : ShippingType.Nacional;
    }

    public Money GetCost(ShippingType type) =>
        type == ShippingType.Local ? Money.Zero : NationalShippingCost;

    public string? GetCarrier(ShippingType type) =>
        type == ShippingType.Nacional ? NationalCarrier : null;

    public string GetDeliveryTime(ShippingType type) =>
        type == ShippingType.Local ? LocalDeliveryTime : NationalDeliveryTime;
}
