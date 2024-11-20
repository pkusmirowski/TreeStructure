using Microsoft.EntityFrameworkCore;
using TreeStructure.Models;
using TreeStructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja serwis�w
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TreeDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TreeDBConnection")));
builder.Services.AddScoped<ITreeService, TreeService>();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Obs�uga wyj�tk�w
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Middleware dla Health Checks (w ka�dym �rodowisku)
app.UseHealthChecks("/health");

// Mapowanie tras
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tree}/{action=Tree}");

app.Run();
