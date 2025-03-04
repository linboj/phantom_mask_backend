using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.OpenApi.Models;
using Backend.Seeds;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
{
    var Env = Environment.GetEnvironmentVariables();
    string connectionString = $"Server={(Env["DB"] == null ? "mssql:1443" : Env["DB"])};Database=phantom_mask;User ID=sa;Password={Env["PW"]};TrustServerCertificate=true";
    options.UseSqlServer(connectionString);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dbContext.Database.Migrate();  // Apply migrations and create the database

    if (dbContext.Pharmacies.Count() == 0){
        // run seed
        await DbSeeder.SeedAsync(dbContext, app.Environment);
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
