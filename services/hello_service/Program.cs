using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// Tworzenie aplikacji webowej
var builder = WebApplication.CreateBuilder(args);
// ---


// Dodawanie usług do Aplikacji
// Tutaj możemy zarządzać wszystkimi usługami, które będą używane w naszej aplikacji
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
// ---


// Integracja z Azure Application Insights i OpenTelemetry
// Usunąć, jeśli nie korzystasz z Azure Application Insights lub OpenTelemetry
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
    tracing.ConfigureResource(resource => resource.AddService("user-service")));
// ---


// Budowanie aplikacji
var app = builder.Build();
// ---


// Konfiguracja środowiska deweloperskiego
// Tutaj zarządzamy co się uruchamia w środowisku deweloperskim
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
// ---


// Definicja endpointu GET /hello, który zwraca "Hello, World!"
app.MapGet("/hello", () => { return "Hello, World!"; }).WithName("GetHello");
// ---


// Definicja endpointu GET /health, który zwraca status zdrowia usługi
app.MapGet("/health", () => Results.Text("Healthy")).WithName("HealthCheck");
// ---


// Uruchomienie aplikacji
app.Run();
// ---