namespace LokyPet.Client.Services;

/// <summary>
/// Configuración de redes sociales leída desde appsettings.json → SocialLinks.
/// </summary>
public sealed class SocialLinksConfig
{
    public SocialLinkEntry Instagram { get; set; } = new();
    public SocialLinkEntry Facebook { get; set; } = new();
}

public sealed class SocialLinkEntry
{
    public string Handle { get; set; } = "";
    public string Url { get; set; } = "";
}
