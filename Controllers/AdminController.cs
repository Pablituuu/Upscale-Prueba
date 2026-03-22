using Microsoft.AspNetCore.Mvc;
using UpscaleWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;

namespace UpscaleWeb.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class AdminController : Controller
    {
        private readonly UpscalePruebaContext _context;

        public AdminController(UpscalePruebaContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(
            string username, string nombres, string primerApellido, string segundoApellido, 
            string rol, string entidad, string numeroDocumento, string tipoDocumento,
            string fechaNacimiento, string nacionalidad, string sexo, string emailPrincipal,
            string telefonoMovil, string tipoContratacion, string fechaContratacion,
            string? emailSecundario = null, string? telefonoSecundario = null)
 {
            // Validar campos obligatorios
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(nombres) || 
                string.IsNullOrEmpty(primerApellido) || string.IsNullOrEmpty(segundoApellido) ||
                string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(entidad) || 
                string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(tipoDocumento) ||
                string.IsNullOrEmpty(fechaNacimiento) || string.IsNullOrEmpty(nacionalidad) || 
                string.IsNullOrEmpty(sexo) || string.IsNullOrEmpty(emailPrincipal) ||
                string.IsNullOrEmpty(telefonoMovil) || string.IsNullOrEmpty(tipoContratacion) || 
                string.IsNullOrEmpty(fechaContratacion))
            {
                return Json(new { success = false, message = "Por favor, complete todos los campos obligatorios." });
            }

            if (await _context.Cuentas.AnyAsync(c => c.Username == username))
                return Json(new { success = false, message = "El username ya existe." });

            // Parsear fechas (DateOnly?)
            DateOnly? fNacVal = null; DateOnly? fConVal = null;
            if (DateOnly.TryParse(fechaNacimiento, out var fn)) fNacVal = fn;
            if (DateOnly.TryParse(fechaContratacion, out var fc)) fConVal = fc;

            // 1. Crear Detalle Full
            var detalle = new UsuariosDetalle {
                Nombres = nombres,
                PrimerApellido = primerApellido,
                SegundoApellido = segundoApellido,
                NombresCompletoCabecera = $"{nombres} {primerApellido} {segundoApellido}".Trim(),
                Rol = rol,
                Entidad = entidad,
                NumeroDocumento = numeroDocumento,
                TipoDocumento = tipoDocumento ?? "DNI",
                FechaNacimiento = fNacVal,
                Nacionalidad = nacionalidad,
                Sexo = sexo,
                EmailPrincipal = emailPrincipal,
                EmailSecundario = emailSecundario,
                TelefonoMovil = telefonoMovil,
                TelefonoSecundario = telefonoSecundario,
                TipoContratacion = tipoContratacion,
                FechaContratacion = fConVal,
                Estado = true // Activo
            };
            _context.UsuariosDetalles.Add(detalle);
            await _context.SaveChangesAsync();

            // 2. Crear Cuenta (Pass: Upscale2026)
            var cuenta = new Cuenta {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Upscale2026"),
                UsuarioDetalleId = detalle.UsuarioId,
                RequiereCambioPassword = true,
                IntentosFallidos = 0
            };
            _context.Cuentas.Add(cuenta);
            await _context.SaveChangesAsync();

            return Json(new { success = true, tempPass = "Upscale2026" });
        }

        // 1. Listado de Usuarios
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            // Solo el administrador real puede ver esto
            if (User.Identity?.Name != "admin") return RedirectToAction("Profile", "Account");

            var usuarios = await _context.Cuentas
                .Include(c => c.UsuarioDetalle)
                .Select(c => new UserAdminViewModel {
                    UsuarioId = c.UsuarioDetalleId,
                    Username = c.Username,
                    NombreCompleto = $"{c.UsuarioDetalle.Nombres} {c.UsuarioDetalle.PrimerApellido} {c.UsuarioDetalle.SegundoApellido}".Trim(),
                    Email = c.UsuarioDetalle.EmailPrincipal,
                    Rol = c.UsuarioDetalle.Rol,
                    RequiereCambio = c.RequiereCambioPassword,
                    // Campos para edición
                    Nombres = c.UsuarioDetalle.Nombres,
                    PrimerApellido = c.UsuarioDetalle.PrimerApellido,
                    SegundoApellido = c.UsuarioDetalle.SegundoApellido,
                    Entidad = c.UsuarioDetalle.Entidad,
                    TipoDocumento = c.UsuarioDetalle.TipoDocumento,
                    NumeroDocumento = c.UsuarioDetalle.NumeroDocumento,
                    FechaNacimientoStr = c.UsuarioDetalle.FechaNacimiento != null ? c.UsuarioDetalle.FechaNacimiento.Value.ToString("yyyy-MM-dd") : "",
                    Nacionalidad = c.UsuarioDetalle.Nacionalidad,
                    Sexo = c.UsuarioDetalle.Sexo,
                    EmailSecundario = c.UsuarioDetalle.EmailSecundario,
                    TelefonoMovil = c.UsuarioDetalle.TelefonoMovil,
                    TelefonoSecundario = c.UsuarioDetalle.TelefonoSecundario,
                    TipoContratacion = c.UsuarioDetalle.TipoContratacion,
                    FechaContratacionStr = c.UsuarioDetalle.FechaContratacion != null ? c.UsuarioDetalle.FechaContratacion.Value.ToString("yyyy-MM-dd") : ""
                })
                .ToListAsync();

            // Detalles del admin para el header
            ViewBag.AdminProfile = await _context.UsuariosDetalles.FirstOrDefaultAsync(u => u.Rol == "Administrador");

            return View(usuarios);
        }

        // --- ACTUALIZAR USUARIO ---
        [HttpPost]
        public async Task<IActionResult> UpdateUser(
            int usuarioId, string nombres, string primerApellido, string segundoApellido, 
            string rol, string entidad, string numeroDocumento, string tipoDocumento,
            string fechaNacimiento, string nacionalidad, string sexo, string emailPrincipal,
            string telefonoMovil, string tipoContratacion, string fechaContratacion,
            string? emailSecundario = null, string? telefonoSecundario = null)
        {
            var detalle = await _context.UsuariosDetalles.FindAsync(usuarioId);
            if (detalle == null) return Json(new { success = false, message = "Usuario no encontrado." });

            // Parsear fechas
            DateOnly? fNacVal = null; DateOnly? fConVal = null;
            if (DateOnly.TryParse(fechaNacimiento, out var fn)) fNacVal = fn;
            if (DateOnly.TryParse(fechaContratacion, out var fc)) fConVal = fc;

            // Actualizar Detalle
            detalle.Nombres = nombres;
            detalle.PrimerApellido = primerApellido;
            detalle.SegundoApellido = segundoApellido;
            detalle.NombresCompletoCabecera = $"{nombres} {primerApellido} {segundoApellido}".Trim();
            detalle.Rol = rol;
            detalle.Entidad = entidad;
            detalle.NumeroDocumento = numeroDocumento;
            detalle.TipoDocumento = tipoDocumento;
            detalle.FechaNacimiento = fNacVal;
            detalle.Nacionalidad = nacionalidad;
            detalle.Sexo = sexo;
            detalle.EmailPrincipal = emailPrincipal;
            detalle.EmailSecundario = emailSecundario;
            detalle.TelefonoMovil = telefonoMovil;
            detalle.TelefonoSecundario = telefonoSecundario;
            detalle.TipoContratacion = tipoContratacion;
            detalle.FechaContratacion = fConVal;

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // 2. Acción para Restablecer Password
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string username)
        {
            if (User.Identity?.Name != "admin") return Unauthorized();

            var cuenta = await _context.Cuentas.FirstOrDefaultAsync(c => c.Username == username);
            if (cuenta == null) return Json(new { success = false, message = "Usuario no encontrado" });

            // Password temporal: Upscale2026
            cuenta.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Upscale2026");
            cuenta.RequiereCambioPassword = true; // FORZAR CAMBIO
            cuenta.IntentosFallidos = 0;
            cuenta.BloqueadoHasta = null;

            await _context.SaveChangesAsync();
            return Json(new { success = true, tempPass = "Upscale2026" });
        }
    }

    public class UserAdminViewModel {
        public int UsuarioId { get; set; }
        public string Username { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public bool RequiereCambio { get; set; }

        // Campos Adicionales para carga en Modal
        public string Nombres { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Entidad { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string FechaNacimientoStr { get; set; }
        public string Nacionalidad { get; set; }
        public string Sexo { get; set; }
        public string EmailSecundario { get; set; }
        public string TelefonoMovil { get; set; }
        public string TelefonoSecundario { get; set; }
        public string TipoContratacion { get; set; }
        public string FechaContratacionStr { get; set; }
    }
}
