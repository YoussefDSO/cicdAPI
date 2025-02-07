using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ✅ Lägg till CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ✅ Lägg till Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Caesar Cipher API",
        Version = "v1",
        Description = "Ett enkelt API för att kryptera och dekryptera text med Caesar Chiffer"
    });
});

var app = builder.Build();

// ✅ Aktivera CORS och Swagger
app.UseCors();
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Caesar Cipher API v1");
        c.RoutePrefix = string.Empty;
    });
}

// ✅ Caesar Cipher funktioner
string CaesarEncrypt(string input, int shift)
{
    var result = new StringBuilder();
    foreach (var c in input)
    {
        if (char.IsLetter(c))
        {
            var shiftedChar = char.IsUpper(c)
                ? (char)(((c - 'A' + shift) % 26) + 'A')
                : (char)(((c - 'a' + shift) % 26) + 'a');
            result.Append(shiftedChar);
        }
        else
        {
            result.Append(c);
        }
    }
    return result.ToString();
}

string CaesarDecrypt(string input, int shift) => CaesarEncrypt(input, 26 - shift);

// ✅ API Endpoints
app.MapGet("/", () => Results.Ok("API is running!"));

// ✅ Kryptering endpoint
app.MapPost("/encrypt", async (HttpContext context) =>
{
    try
    {
        var request = await context.Request.ReadFromJsonAsync<RequestData>();
        if (request == null || string.IsNullOrWhiteSpace(request.Text))
            return Results.BadRequest("Text is required.");

        return Results.Ok(new { encrypted = CaesarEncrypt(request.Text, 3) });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

// ✅ Avkryptering endpoint
app.MapPost("/decrypt", async (HttpContext context) =>
{
    try
    {
        var request = await context.Request.ReadFromJsonAsync<RequestData>();
        if (request == null || string.IsNullOrWhiteSpace(request.Text))
            return Results.BadRequest("Text is required.");

        return Results.Ok(new { decrypted = CaesarDecrypt(request.Text, 3) });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

// ✅ Kör på rätt port
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");

public class RequestData
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
