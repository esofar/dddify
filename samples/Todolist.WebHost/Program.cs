using Dddify.Guids;
using Microsoft.EntityFrameworkCore;
using Todolist.Domain.Repositories;
using Todolist.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDddify(cfg =>
{
    cfg.UseApiResultWrapper();
    cfg.UseJsonLocalization();
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options
    .UseSqlite(builder.Configuration.GetConnectionString("Default"))
    .EnableSensitiveDataLogging());

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Todolist.WebHost.xml"));
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Todolist.Application.xml"));
});

builder.Services.AddDddify(cfg =>
{
    // Sets the DateTimeKind for date and time values.
    cfg.WithDateTimeKind(DateTimeKind.Utc);
    // Sets the type of sequential GUID to be used.
    cfg.WithSequentialGuidType(SequentialGuidType.SequentialAsString);
    // Adds the JSON localization extension
    cfg.UseJsonLocalization();
    // Adds the API result wrapper extension.
    cfg.UseApiResultWrapper();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLocalization();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
