using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using ProductService.BL;
using ProductService.DAL.ProductDatabase;
using ProductService.DAL.Repositories;
using ProductService.DAL.BlobStorage;
using ProductService.DAL.EmailService;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder => {
            builder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod().AllowCredentials();
        });

});


static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

// Add services to the container.
builder.Services.Configure<ProductDatabaseSettings>(
    builder.Configuration.GetSection("ProductDatabaseSettings"));

builder.Services.Configure<BlobStorageSettings>(
    builder.Configuration.GetSection("AzureBlobStorageSettings"));

builder.Services.Configure<EmailServiceSettings>(
    builder.Configuration.GetSection("EmailServiceSettings"));

/*builder.Services.AddSingleton(
    x => new BlobServiceClient(builder.Configuration.GetValue<string>("AzureBlobStorageConnectionString")));*/

builder.Services.AddSingleton<IProductDatabaseSettings>(
    s => s.GetRequiredService<IOptions<ProductDatabaseSettings>>().Value);

builder.Services.AddSingleton<IBlobStorageSettings>(
    s => s.GetRequiredService<IOptions<BlobStorageSettings>>().Value);

builder.Services.AddSingleton<IEmailServiceSettings>(
    s => s.GetRequiredService<IOptions<EmailServiceSettings>>().Value);

builder.Services.AddSingleton<ProductDatabaseSettings>();
builder.Services.AddSingleton<BlobStorageSettings>();
builder.Services.AddSingleton<EmailServiceSettings>();
builder.Services.AddSingleton<IProductContext, ProductContext>();
builder.Services.AddSingleton<IBlobContext, BlobContext>();
builder.Services.AddHttpClient<IEmailServiceContext, EmailServiceContext>().AddPolicyHandler(GetRetryPolicy());
builder.Services.AddSingleton<IEmailServiceContext, EmailServiceContext>();
builder.Services.AddScoped<IBlobRepository, BlobRepository>();
builder.Services.AddScoped<IEmailSenderRepository, EmailSenderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductGroupRepository, ProductGroupRepository>();

builder.Services.AddScoped<ProductManager>();
builder.Services.AddScoped<ProductGroupManager>();

builder.Services.AddControllers();
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


