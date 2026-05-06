using Dddify.Localization;
using Dddify.ResultWrapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming(options =>
    {
        options.UseTimeZoneIdProvider<FixedTimeZoneIdProvider>();
    });

    cfg.AddDbContextWithUnitOfWork<ApplicationDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
    });
});

// Add services to the container.
builder.Services
    .AddRazorPages()
    .AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRequestLocalization();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

public class AppBusinessExceptionResourceTypeResolver
    : IBusinessExceptionResourceTypeResolver
{
    public Type Resolve(BusinessException exception)
        => exception.ErrorCode switch
        {
            string code when code.StartsWith("order_") => typeof(OrderResource),
            string code when code.StartsWith("todo_") => typeof(TodoResource),
            _ => typeof(SharedResource)
        };
}


public record OrderResource();
public record TodoResource();