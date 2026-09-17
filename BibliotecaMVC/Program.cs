using BibliotecaMVC.Data;
using BibliotecaMVC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Semana 7 - Inyección de dependencias
builder.Services.AddScoped<IAutorService, AutorServiceAlternativo>();

// Semana 9 - Entity Framework Core
builder.Services.AddDbContext<BibliotecaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BibliotecaConnection")));

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();