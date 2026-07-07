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


// Przykładowe dane do endpointu GET /users
// Roles, Names, Surnames - przykładowe dane do generowania użytkowników
var roles = new[]
{
    "DevOps Engineer",
    "Developer",
    "IT Admin",
    "Security Officer"
};

var names = new[]
{
    "Alice",
    "Bob",
    "Charlie",
    "David",
    "Eve"
};

var surnames = new[]
{
    "Smith",
    "Johnson",
    "Williams",
    "Brown",
    "Jones"
};
// ---


// Generowanie przykładowej listy użytkowników
var usersList = Enumerable.Range(1, 5).Select(_ =>
{
    var name = names[Random.Shared.Next(names.Length)];
    var surname = surnames[Random.Shared.Next(surnames.Length)];
    return new UserResponse
    {
        Id = Guid.NewGuid(),
        Name = name,
        Surname = surname,
        Role = roles[Random.Shared.Next(roles.Length)],
        Age = Random.Shared.Next(18, 65),
        Phone = $"{Random.Shared.Next(100, 999)} {Random.Shared.Next(100, 999)} {Random.Shared.Next(100, 999)}",
        Email = $"{name.ToLower()}.{surname.ToLower()}@gmail.com"
    };
}).ToList();
// ---


// Definicja endpointu POST /users, który tworzy nowego użytkownika
app.MapPost("/users", async (Request request, HttpClient client) =>
{
    var newUser = new UserResponse
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Surname = request.Surname,
        Role = request.Role,
        Age = request.Age,
        Phone = request.Phone,
        Email = request.Email
    };

    usersList.Add(newUser);
    Console.WriteLine($"info: Utworzono użytkownika o ID - {newUser.Id}");

    var auditPayload = new
    {
        Service = "UserService",
        Event = "CreateUser",
        Message = $"info: Utworzono użytkownika o ID - {newUser.Id}"
    };

    try
    {
        // "http://audit-service/audit" - dla Azure Container Apps,
        // "http://audit-service:8080/audit" - dla Docker Compose
        // "http://localhost:5268/audit" - dla lokalnego testowania

        await client.PostAsJsonAsync("http://audit-service/audit", auditPayload);
        Console.WriteLine($"info: Sukcess. Wysyłanie audytu: {auditPayload}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"---------------------------------------------------");
        Console.WriteLine($"error: Błąd wysyłania audytu - {ex.Message}");
        Console.WriteLine($"---------------------------------------------------");
    }

    return Results.Created($"/users/{newUser.Id}", newUser);

}).WithName("PostUsers");
// ---


// Definicja endpointu GET /users, który zwraca listę użytkowników
app.MapGet("/users", () => { return Results.Ok(usersList); }).WithName("GetUsers");
// ---


// Definicja endpointu GET /health, który zwraca status zdrowia usługi
app.MapGet("/health", () => Results.Text("Healthy")).WithName("HealthCheck");
// ---


// Uruchomienie aplikacji
app.Run();
// ---


// Model dla żądania POST
record Request(string Name, string Surname, string Role, int Age, string Phone, string Email);
// ---


// Model dla odpowiedzi GET i POST /users
public class UserResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Role { get; set; }
    public int Age { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
// ---