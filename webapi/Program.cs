using MongoDB.Driver;
using webapi.DataRepos;
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
builder.Services.AddScoped<WebADAMDBRepo>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
                      builder =>
                      {
                          builder.WithOrigins("https://localhost:5173") // your React app's origin
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("MyAllowSpecificOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
