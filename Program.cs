using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.OpenApi.Models;
using Backend.Seeds;
using Backend.Services;
using Backend.Profiles;
using System.Xml.Linq;
using System.Xml.XPath;
using Backend.Filter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
{
    var Env = Environment.GetEnvironmentVariables();
    // string connectionString = $"Server=mssql_db,1443;Database=phantom_mask;User ID=sa;Password={Env["PW"]};TrustServerCertificate=true";
    // Console.WriteLine(connectionString);
    // options.UseSqlServer(connectionString);
    string connectionString = $"Server=mysql_db;Port=3306;Database={Env["DB"]};User ID={Env["USER"]};Password={Env["PW"]}";
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(9, 2, 0)));
});
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<PharmacyService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend API", Version = "v1" });
    foreach (string xmlFile in Directory.GetFiles(AppContext.BaseDirectory, "*.xml")) {
        // 從單純 IncludeXml，改為是讀 XML，用 Filter 修改
        XDocument xmlDoc = XDocument.Load(xmlFile);
        options.IncludeXmlComments(() => new XPathDocument(xmlDoc.CreateReader()), true);
        options.SchemaFilter<EnumSchemaFilter>(xmlDoc);
    }
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
    // dbContext.Database.Migrate();  // Apply migrations and create the database

    if (dbContext.Pharmacies.Count() == 0)
    {
        // run seed
        await DbSeeder.SeedAsync(dbContext, app.Environment);
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
