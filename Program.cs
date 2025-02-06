using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// L�gg till tj�nster
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aktivera Swagger-dokumentation f�r API
app.UseSwagger();
app.UseSwaggerUI();

// Enkelt exempel p� Caesar Chiffer (shift 3)
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
            result.Append(c); // L�mna icke-bokst�ver of�r�ndrade
        }
    }
    return result.ToString();
}

string CaesarDecrypt(string input, int shift)
{
    return CaesarEncrypt(input, 26 - shift); // F�r att dekryptera anv�nder vi motsatt shift
}

// Kryptering endpoint
app.MapPost("/encrypt", (string text) =>
{
    if (string.IsNullOrEmpty(text)) return Results.BadRequest("Text is required.");
    return Results.Ok(CaesarEncrypt(text, 3)); // Anv�nd Caesar Shift 3
});

// Avkryptering endpoint
app.MapPost("/decrypt", (string text) =>
{
    if (string.IsNullOrEmpty(text)) return Results.BadRequest("Text is required.");
    return Results.Ok(CaesarDecrypt(text, 3)); // Anv�nd Caesar Shift 3 f�r dekryptering
});

// K�r API
app.Run();
