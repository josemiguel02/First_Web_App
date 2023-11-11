using First_Web_App.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace First_Web_App.Endpoints;

public class UsersEndpoints
{
    private static List<User> _users = new()
    {
        new User() { Id = 1, Username = "JhonDoe", Email = "jhondoe@mail.com", Role = "Admin" },
        new User() { Id = 2, Username = "JoseDP", Email = "jose@mail.com", Role = "Developer" },
        new User() { Id = 3, Username = "JaneDoe", Email = "janedoe@mail.com", Role = "Designer" }
    };

    public static void Map(WebApplication app, string prefix)
    {
        // GET - ALL USERS
        app.MapGet(prefix, GetUsers);

        // GET - SINGLE USER
        app.MapGet($"{prefix}/{{id:int}}", GetSingleUser);

        // POST - CREATE USER
        app.MapPost(prefix, CreateUser);

        // DELETE - REMOVE USER
        app.MapDelete($"{prefix}/{{id:int}}", DeleteUser);

        // PUT - UPDATE USER
        app.MapPut($"{prefix}/{{id:int}}", UpdateUser);
    }

    private static async Task GetUsers(HttpContext context)
    {
        await context.Response.WriteAsJsonAsync(_users);
    }

    private static async Task GetSingleUser(HttpRequest request, HttpResponse response)
    {
        var userId = request.RouteValues["id"] as string;

        if (userId == null || !int.TryParse(userId, out _))
        {
            response.StatusCode = StatusCodes.Status400BadRequest;

            await response.WriteAsJsonAsync(new { Message = "Ingrese un ID correcto." });
            return;
        }

        var user = _users.Find(user => user.Id == int.Parse(userId));

        if (user != null)
        {
            await response.WriteAsJsonAsync(user);
            return;
        }

        response.StatusCode = StatusCodes.Status404NotFound;
        await response.WriteAsJsonAsync(new { Message = $"No existe usuario con ID: {userId}" });
    }

    private static async Task<IResult> CreateUser(HttpRequest req, HttpResponse res)
    {
        if (!req.HasJsonContentType())
        {
            return Results.BadRequest(
                new { Message = "Usuario es requerido." }
            );
        }

        var user = await req.ReadFromJsonAsync<User>();

        if (user != null)
        {
            var newUser = new User()
            {
                Id = _users.Count + 1,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };

            _users.Add(newUser);

            return Results.Ok(
                new { Message = "Usuario creado con exito.", user = newUser }
            );
        }

        return Results.BadRequest(
            new { Message = "Ingrese un usuario correcto." }
        );
    }

    private static IResult DeleteUser(HttpRequest req, HttpResponse res)
    {
        var userId = req.RouteValues["id"] as string;

        if (userId == null || !int.TryParse(userId, out _))
        {
            return Results.BadRequest(
                new { Message = "Ingrese un ID correcto." }
            );
        }

        var user = _users.Find(user => user.Id == int.Parse(userId));

        if (user != null)
        {
            _users.Remove(user);

            return Results.Ok(
                new { Message = "Usuario eliminado con exito.", user }
            );
        }


        return Results.NotFound(
            new { Message = $"No existe usuario con ID: {userId}" }
        );
    }

    private static async Task<IResult> UpdateUser(HttpRequest req, HttpResponse res)
    {
        var userId = req.RouteValues["id"] as string;

        if (userId == null || !int.TryParse(userId, out _))
        {
            return Results.BadRequest(
                new { Message = "Ingrese un ID correcto." }
            );
        }

        if (!req.HasJsonContentType())
        {
            return Results.BadRequest(
                new { Message = "Usuario es requerido." }
            );
        }

        var user = await req.ReadFromJsonAsync<User>();
        var idx = _users.FindIndex(u => u.Id == int.Parse(userId));

        if (user != null && idx != -1)
        {
            _users[idx] = user;

            return Results.Ok(
                new { Message = "Usuario actualizado con exito.", user }
            );
        }

        return Results.NotFound(
            new { Message = $"No existe usuario con ID: {userId}" }
        );
    }
}