using System.Net.Mime;
using First_Web_App.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Construyendo App
var app = builder.Build();

// Ruta inicial
app.MapGet("/", (ctx) =>
{
    ctx.Response.ContentType = MediaTypeNames.Text.Html;

    return ctx.Response.WriteAsync("<h1>ASPNET CORE API</h1>");
});

// Seteando Endpoints - USERS
UsersEndpoints.Map(app, "/api/users");

// Iniciando App
app.Run();