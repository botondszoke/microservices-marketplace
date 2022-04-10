using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using SaleService.BL;
using SaleService.DAL;
using SaleService.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder => {
            builder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

// Add services to the container.

builder.Services.Configure<SaleDatabaseSettings>(
    builder.Configuration.GetSection("SaleDatabaseSettings"));

builder.Services.AddSingleton<ISaleDatabaseSettings>(
    s => s.GetRequiredService<IOptions<SaleDatabaseSettings>>().Value);

builder.Services.AddSingleton<SaleDatabaseSettings>();
builder.Services.AddSingleton<ISaleContext, SaleContext>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();

builder.Services.AddScoped<SaleManager>();

builder.Services.AddControllers().AddNewtonsoftJson(
    o => o.SerializerSettings
    .Converters
    .Add(new StringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection(); // Removed because current configuration of Traefik gw doesn't support https

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
