using Microsoft.EntityFrameworkCore;
using TreeStructure.Models;
using TreeStructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TreeDBContext>(options => options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["TreeDBConnection"]));
builder.Services.AddTransient<TreeService>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHealthChecks();
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Add a health check middleware
if (app.Environment.IsDevelopment())
{
    app.UseHealthChecks("/health");
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tree}/{action=Tree}");

app.Run();
