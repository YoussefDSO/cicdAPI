using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Tillåt alla CORS-förfrågningar
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ✅ Lägg till Swagger och API Explorer
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

// ✅ Aktivera CORS
app.UseCors();

// ✅ Aktivera Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Caesar Cipher API v1");
        c.RoutePrefix = string.Empty;
    });
}

// ✅ Caesar Chiffer funktioner
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

// ✅ Definiera API-endpoints
app.MapPost("/encrypt", (HttpContext context, string text) =>
{
    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest("Text is required.");
    return Results.Ok(new { encrypted = CaesarEncrypt(text, 3) });
});

app.MapPost("/decrypt", (HttpContext context, string text) =>
{
    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest("Text is required.");
    return Results.Ok(new { decrypted = CaesarDecrypt(text, 3) });
});

app.Run("http://+:5000");
