using System;
using ClinicaAPI.Models;
using ClinicaAPI;

var builder = WebApplication.CreateBuilder(args); 


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

//!---------------------------------TESTE ABAIXO------------------------------------------------------------

Database.Inicializar();

Console.WriteLine("Inserindo médicos");
Repositorio.InserirMedico(new Medico
{
    Nome = "Dr. André Silva",
    Genero = new Genero('M'),
    CRM = new CRM("123456-SP"),
    Especialidade = "Cardiologia"
});

Repositorio.InserirMedico(new Medico
{
    Nome = "Dra. Paula Martins",
    Genero = new Genero('F'),
    CRM = new CRM("654321-SP"),
    Especialidade = "Dermatologia"
});

Console.WriteLine("Inserindo clientes...");
Repositorio.InserirCliente(new Cliente
{
    Nome = "Maria Clara",
    Genero = new Genero('F'),
    CPF = new CPF("12345678909"),
    DataNascimento = new DataNascimento(new DateTime(1995, 8, 20))
});

Repositorio.InserirCliente(new Cliente
{
    Nome = "Carlos Eduardo",
    Genero = new Genero('M'),
    CPF = new CPF("98765432100"),
    DataNascimento = new DataNascimento(new DateTime(1988, 12, 5))
});

Console.WriteLine("Inserindo consultas...");
var medico1 = Repositorio.ListarMedicos()[0];
var cliente1 = Repositorio.ListarClientes()[0];
var cliente2 = Repositorio.ListarClientes()[1];

Repositorio.InserirConsulta(new Consulta
{
    MedicoId = medico1.Id,
    PacienteId = cliente1.Id,
    DataHora = "2025-07-15 09:00"
});

Repositorio.InserirConsulta(new Consulta
{
    MedicoId = medico1.Id,
    PacienteId = cliente2.Id,
    DataHora = "2025-07-15 10:30"
});

Console.WriteLine("\n🏥 Médicos:");
Repositorio.ListarMedicos().ForEach(m => Console.WriteLine($"• {m.Nome} ({m.Especialidade})"));

Console.WriteLine("\n🧑‍🤝‍🧑 Pacientes:");
Repositorio.ListarClientes().ForEach(c => Console.WriteLine($"• {c.Nome} (CPF: {c.CPF})"));

Console.WriteLine("\n📅 Consultas:");
Repositorio.ListarConsultas().ForEach(c => Console.WriteLine("• " + c));

//!---------------------------------TESTE ACIMA--------------------------------------------- rodar no console: dotnet run --project ClinicaAPI/ClinicaAPI.csproj -v:n


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}



