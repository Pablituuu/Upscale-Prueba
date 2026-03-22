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

// =========================================================
// SEEDER: Crear o resetear el usuario admin automáticamente
// =========================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UpscalePruebaContext>();
    try
    {
        db.Database.EnsureCreated();

        var adminCuenta = db.Cuentas.FirstOrDefault(c => c.Username == "admin");
        if (adminCuenta == null)
        {
            // Crear detalle del admin
            var detalle = new UsuariosDetalle
            {
                Estado = true,
                NombresCompletoCabecera = "Administrador del Sistema",
                Rol = "Administrador",
                Entidad = "Upscale HQ",
                Nombres = "Admin",
                PrimerApellido = "Upscale",
                SegundoApellido = "System",
                TipoDocumento = "DNI",
                NumeroDocumento = "00000000",
                EmailPrincipal = "admin@upscale.com.pe",
                Nacionalidad = "Peruana",
                Sexo = "M"
            };
            db.UsuariosDetalles.Add(detalle);
            db.SaveChanges();

            var cuenta = new Cuenta
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Upscale2026"),
                UsuarioDetalleId = detalle.UsuarioId,
                IntentosFallidos = 0,
                RequiereCambioPassword = false
            };
            db.Cuentas.Add(cuenta);
            db.SaveChanges();
            Console.WriteLine(">>> SEEDER: Usuario admin CREADO exitosamente.");
        }
        else
        {
            // Solo resetear bloqueo si está bloqueado (no tocamos la clave)
            if (adminCuenta.IntentosFallidos > 0 || adminCuenta.BloqueadoHasta != null)
            {
                adminCuenta.IntentosFallidos = 0;
                adminCuenta.BloqueadoHasta = null;
                db.SaveChanges();
            }
            Console.WriteLine(">>> SEEDER: Usuario admin ya existe, OK.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($">>> SEEDER ERROR: {ex.Message}");
    }
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Welcome}/{id?}");

app.Run();
