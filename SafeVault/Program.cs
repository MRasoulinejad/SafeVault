using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SafeVault.Services;
using SafeVault.Models;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Database connection
var connectionString = "Data Source=safevault.db";

// Create service instances
var userService = new UserService(connectionString);
var authService = new AuthService(userService);

// Register services
builder.Services.AddSingleton(userService);
builder.Services.AddSingleton(authService);

// Add Authentication with JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = authService.GetValidationParameters();
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// --- Routes ---

// Register a new user (with role)
app.MapPost("/register", (string username, string email, string password, string role, AuthService auth) =>
{
    var success = auth.Register(username, email, password, role);
    return success ? Results.Ok("User registered") : Results.BadRequest("Username already exists");
});

// Login and get JWT
app.MapPost("/login", (string username, string password, AuthService auth) =>
{
    var token = auth.Login(username, password);
    return token != null ? Results.Ok(new { token }) : Results.Unauthorized();
});

// Protected route: only admins can access
app.MapGet("/admin", [Microsoft.AspNetCore.Authorization.Authorize(Roles = "admin")] () =>
{
    return Results.Ok("Welcome, admin! You have access to this area.");
});

// Protected route: any authenticated user
app.MapGet("/profile", [Microsoft.AspNetCore.Authorization.Authorize] (HttpContext context) =>
{
    var username = context.User.Identity?.Name;
    var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
    return Results.Ok($"Hello {username}, your role is {role}");
});

// Start the app
app.Run();
