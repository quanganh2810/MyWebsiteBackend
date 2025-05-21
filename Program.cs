using FoodApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Get PORT from environment (Railway sets it dynamically)
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Get DATABASE_URL from environment (Railway sets this)
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL") 
                  ?? builder.Configuration.GetConnectionString("DatabaseUrl");

Console.WriteLine($"Database URL: {databaseUrl}");

if (string.IsNullOrEmpty(databaseUrl))
{
    throw new InvalidOperationException("DATABASE_URL environment variable is not set.");
}

// Convert DATABASE_URL to a valid connection string
var connectionString = !string.IsNullOrEmpty(databaseUrl)
    ? ConvertRenderPostgresUrlToConnectionString(databaseUrl)
    : builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add controller services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Define a named CORS policy
var corsPolicy = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply CORS policy
app.UseCors(corsPolicy);

// Enable Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use Authorization
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();

static string ConvertRenderPostgresUrlToConnectionString(string url)
{
    // Remove the "postgresql://" prefix
    var cleanUrl = url.Replace("postgresql://", "");

    // Split the user info and host/db info
    var userInfoAndHost = cleanUrl.Split('@');
    var userInfo = userInfoAndHost[0]; // quanganh:qAjBcUmHsKzvOQReaIQfs5lVojXLzZey
    var hostAndDb = userInfoAndHost[1]; // dpg-...render.com/mydb_8tch

    var userPass = userInfo.Split(':');
    var username = userPass[0];
    var password = userPass[1];

    var hostAndDbSplit = hostAndDb.Split('/');
    var host = hostAndDbSplit[0];
    var database = hostAndDbSplit[1];

    // Build Npgsql-compatible connection string
    return $"Host={host};Username={username};Password={password};Database={database};SSL Mode=Require;Trust Server Certificate=true;";
}