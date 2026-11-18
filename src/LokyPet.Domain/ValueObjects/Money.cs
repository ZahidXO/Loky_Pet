namespace LokyPet.Domain.ValueObjects;

public readonly record struct Money(decimal Amount)
{
    public static Money Zero => new(0m);
    public static Money FromMxn(decimal amount) => new(Math.Round(amount, 2));
    public string Format() => Amount.ToString("C2", MxDot);
    public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount);
    public static Money operator -(Money a, Money b) => new(a.Amount - b.Amount);
    public static Money operator *(Money a, int qty) => new(a.Amount * qty);

    /// <summary>CultureInfo es-MX con punto decimal (en lugar de coma). Uso compartido.</summary>
    public static readonly System.Globalization.CultureInfo MxDot = CreateMxDotCulture();
    private static System.Globalization.CultureInfo CreateMxDotCulture()
    {
        var c = (System.Globalization.CultureInfo)new System.Globalization.CultureInfo("es-MX").Clone();
        c.NumberFormat.CurrencyDecimalSeparator = ".";
        c.NumberFormat.CurrencyGroupSeparator = ",";
        return c;
    }
}
