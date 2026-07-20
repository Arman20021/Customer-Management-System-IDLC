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


builder.Services.AddScoped<DeleteCustomerRepository>();
builder.Services.AddScoped<DeleteCustomerService>();

builder.Services .AddScoped<UploadCustomerDocumentRepository>();

builder.Services.AddScoped<UploadCustomerDocumentService>();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Customer Management API v1");

    options.RoutePrefix = string.Empty;
});

//for docker auto migrate
using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<CmsDbContext>();
        db.Database.Migrate();
    }

if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();