using Microsoft.EntityFrameworkCore;
using TreeStructure.Models;
using TreeStructure.Services;

var builder = WebApplication.CreateBuilder(args);

//string ConnectionString = Vault.GetSecretPhrase("TreeDB");

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("TreeDBConnection");
builder.Services.AddDbContext<TreeDBContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddTransient<TreeService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tree}/{action=Tree}");

app.Run();
