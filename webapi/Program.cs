using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;
using webapi.DataRepos;
using webapi.Services;
//using webapi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configure MongoDB Connection
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var mongoDBSettingsSection = configuration.GetSection("MongoDBSettings");
var connectionString = mongoDBSettingsSection["ConnectionString"];
var databaseName = mongoDBSettingsSection["DatabaseName"];

var mongoClient = new MongoClient(connectionString);
var mongoDatabase = mongoClient.GetDatabase(databaseName);

builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);
builder.Services.AddSingleton<IUserLicenseService, UserLicenseService>();
builder.Services.AddScoped<WebADAMDBRepo>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
                      builder =>
                      {
                          builder.WithOrigins("https://localhost:5173")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod()
                                 .AllowCredentials();
                      });
});

builder.Services.AddControllers();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/AuthController/login";
        options.LogoutPath = "/AuthController/logout";
        options.AccessDeniedPath = "/AuthController/accessdenied"; // Optional: Customize access denied path
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401; // Unauthorized status code
            return Task.CompletedTask;
        };
    });

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Set your desired session duration
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("MyAllowSpecificOrigins");

app.UseHttpsRedirection();

app.UseAuthentication(); // Use authentication before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
