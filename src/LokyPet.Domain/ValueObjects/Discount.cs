namespace LokyPet.Domain.ValueObjects;

public readonly record struct Discount(Money RegularPrice, Money FinalPrice)
{
    public bool IsActive => RegularPrice.Amount > 0 && RegularPrice.Amount > FinalPrice.Amount;
    public int Percentage => IsActive ? (int)Math.Round((1m - FinalPrice.Amount / RegularPrice.Amount) * 100m) : 0;
    public Money Savings => IsActive ? RegularPrice - FinalPrice : Money.Zero;
}
