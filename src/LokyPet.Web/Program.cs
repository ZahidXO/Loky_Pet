using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using LokyPet.Client;
using LokyPet.Client.Services;
using LokyPet.Application.Interfaces;
using LokyPet.Infrastructure.Configuration;
using LokyPet.Infrastructure.Repositories;
using LokyPet.Domain.Policies;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Leer configuración de Supabase desde appsettings.json
var supabaseUrl = builder.Configuration.GetSection("Supabase")["Url"] ?? "";
var supabaseKey = builder.Configuration.GetSection("Supabase")["AnonKey"] ?? "";
var supabaseConfig = new SupabaseConfig { Url = supabaseUrl, AnonKey = supabaseKey };

// Registrar Supabase
builder.Services.AddSingleton(supabaseConfig);
builder.Services.AddSingleton<LokyPet.Client.Services.CatalogService>();
builder.Services.AddSingleton<SupabaseClientFactory>();

// HttpClient para los repositorios de Supabase (PostgREST)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Repositorios
builder.Services.AddScoped<IProductRepository, SupabaseProductRepository>(sp =>
    new SupabaseProductRepository(new HttpClient(), supabaseConfig));

// Carrito (localStorage)
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ICartService, LocalStorageCartService>();

// Contacto (PostgREST → contact_messages)
builder.Services.AddSingleton<IContactService>(new ContactService(supabaseConfig));

// Autenticación (Supabase GoTrue + user_profiles)
var authService = new AuthService(supabaseConfig);
builder.Services.AddSingleton<IAuthService>(authService);

// Pedidos / historial (PostgREST → orders + order_items)
builder.Services.AddSingleton<IOrderRepository>(new OrderService(supabaseConfig, authService));

// Dashboard (métricas, inventario, pedidos admin, mensajes)
builder.Services.AddSingleton(new DashboardService(supabaseConfig, authService));

// Redes sociales (desde appsettings.json → SocialLinks)
var socialSection = builder.Configuration.GetSection("SocialLinks");
builder.Services.AddSingleton(new SocialLinksConfig
{
    Instagram = new SocialLinkEntry
    {
        Handle = socialSection["Instagram:Handle"] ?? "@lokypet_",
        Url = socialSection["Instagram:Url"] ?? "https://www.instagram.com/accounts/login/"
    },
    Facebook = new SocialLinkEntry
    {
        Handle = socialSection["Facebook:Handle"] ?? "Loky Pet",
        Url = socialSection["Facebook:Url"] ?? "https://www.facebook.com/login/"
    }
});

// Políticas de negocio (configurables desde appsettings)
var shippingSection = builder.Configuration.GetSection("Shipping");
builder.Services.AddSingleton(new ShippingPolicy
{
    LocalCity = shippingSection["LocalCity"] ?? "Reynosa",
    LocalState = shippingSection["LocalState"] ?? "Tamaulipas",
    NationalShippingCost = LokyPet.Domain.ValueObjects.Money.FromMxn(
        decimal.TryParse(shippingSection["NationalCost"], out var cost) ? cost : 139m),
    NationalCarrier = shippingSection["NationalCarrier"] ?? "DHL",
    LocalDeliveryTime = shippingSection["LocalDeliveryTime"] ?? "24 a 48 horas hábiles",
    NationalDeliveryTime = shippingSection["NationalDeliveryTime"] ?? "3 a 5 días hábiles"
});

await builder.Build().RunAsync();
