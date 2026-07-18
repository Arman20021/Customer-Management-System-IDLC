using CMS.BLL.Services;
using CMS.DAL.Data;
using CMS.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(
                namingPolicy: null,
                allowIntegerValues: false));
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection was not found.");

builder.Services.AddDbContext<CmsDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<CustomerService>();

builder.Services.AddScoped<GetAllCustomersRepository>();
builder.Services.AddScoped<GetAllCustomersService>();

builder.Services.AddScoped<UpdateCustomerRepository>();
builder.Services.AddScoped<UpdateCustomerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Customer Management API v1");

    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();