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


// Lista do przechowywania danych audytu
var auditList = new List<AuditResponse>();
// ---


// Definicja endpointu POST /audit, który przyjmuje dane audytu i zwraca je z dodatkowym ID i timestampem
app.MapPost("/audit", (Request request) =>
{
    var audit = new AuditResponse
    {
        Id = Guid.NewGuid(),
        Service = request.Service,
        Event = request.Event,
        Message = request.Message,
        Timestamp = DateTime.UtcNow
    };

    auditList.Add(audit);
    Console.WriteLine($"info: Utworzono audyt o ID - {audit.Id}");

    return Results.Created($"/audit/{audit.Id}", audit);

}).WithName("PostAudit");
// ---


// Definicja endpointu GET /audit, który zwraca listę wszystkich audytów
app.MapGet("/audit", () => Results.Ok(auditList)).WithName("GetAudit");
// ---  


// Definicja endpointu GET /health, który zwraca status zdrowia usługi
app.MapGet("/health", () => Results.Text("Healthy")).WithName("HealthCheck");
// ---


// Uruchomienie aplikacji
app.Run();
// ---


// Model danych dla żądania POST /audit
record Request(string Service, string Event, string Message);
// ---


// Model dla odpowiedzi GET i POST /audit
public class AuditResponse
{
    public Guid Id { get; set; }
    public string? Service { get; set; }
    public string? Event { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; }
}
// ---