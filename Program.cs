using Microsoft.EntityFrameworkCore;
using UpscaleWeb.Models;

var builder = WebApplication.CreateBuilder(args);
// 1. Obtener la cadena de conexión de los secretos/configuración
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar el DbContext para que use SQL Server
builder.Services.AddDbContext<UpscalePruebaContext>(options =>
    options.UseSqlServer(connectionString));

// 3. (Opcional pero recomendado) Configurar Cookies para el Login
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "UserLoginCookie";
        config.LoginPath = "/Account/Login"; // Ruta a la que redirigir si no está logueado
    });

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Welcome}/{id?}");

app.Run();
