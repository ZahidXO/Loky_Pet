namespace LokyPet.Domain.ValueObjects;

public readonly record struct Sku
{
    public string Value { get; }
    public Sku(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 20)
            throw new ArgumentException("SKU must be 1-20 characters.", nameof(value));
        Value = value.Trim().ToUpperInvariant();
    }
    public override string ToString() => Value;
}
