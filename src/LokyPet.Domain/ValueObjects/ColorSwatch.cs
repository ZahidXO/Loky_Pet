namespace LokyPet.Domain.ValueObjects;

public sealed record ColorSwatch(string Name, string[] HexValues)
{
    public bool IsDual => HexValues.Length >= 2;
}
