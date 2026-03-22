using Microsoft.AspNetCore.Mvc;
using UpscaleWeb.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace UpscaleWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UpscalePruebaContext _context;

        public AccountController(UpscalePruebaContext context)
        {
            _context = context;
            
            // --- AUTO-FIX: Asegurar que la columna FotoUrl exista en SQL Server ---
            try {
                _context.Database.ExecuteSqlRaw("ALTER TABLE UsuariosDetalle ADD FotoUrl NVARCHAR(500) NULL");
            } catch { 
                /* Si ya existe o hay error de permisos, ignoramos para no detener la app */ 
            }
        }

        [HttpGet]
        public IActionResult Welcome() => View();

        [HttpGet]
        public IActionResult Login(string reason) 
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Profile");
            
            var model = new LoginViewModel { ErrorMessage = TempData["ErrorMessage"] as string };
            ViewBag.Reason = reason;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
        
            // Permitir Login por Username O por Numero de Documento (DNI, C.E, etc)
            var cuenta = await _context.Cuentas
                .Include(c => c.UsuarioDetalle)
                .FirstOrDefaultAsync(c => c.Username == model.Username || 
                                          (c.UsuarioDetalle != null && c.UsuarioDetalle.NumeroDocumento == model.Username));
        
            if (cuenta == null)
            {
                TempData["ErrorMessage"] = "Identificación o usuario no encontrado.";
                return RedirectToAction("Login");
            }
        
            if (cuenta.BloqueadoHasta.HasValue && cuenta.BloqueadoHasta > DateTime.Now)
            {
                TempData["ErrorMessage"] = $"Cuenta bloqueada hasta {cuenta.BloqueadoHasta:HH:mm:ss}.";
                return RedirectToAction("Login");
            }
        
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.Password, cuenta.PasswordHash);

            if (isPasswordCorrect)
            {
                cuenta.IntentosFallidos = 0;
                cuenta.BloqueadoHasta = null;
                await _context.SaveChangesAsync();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, cuenta.Username),
                    new Claim("UsuarioId", cuenta.UsuarioDetalleId.ToString()),
                    new Claim(ClaimTypes.Role, cuenta.UsuarioDetalle?.Rol ?? "Usuario")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);
                
                // --- REDIRECCION INTELIGENTE: Si es admin, va directo al listado ---
                if (cuenta.Username == "admin") return RedirectToAction("Users", "Admin");

                // --- DETECCION DE CAMBIO FORZOSO ---
                if (cuenta.RequiereCambioPassword) return RedirectToAction("ChangePassword");
                
                return RedirectToAction("Profile");
            }
            else
            {
                cuenta.IntentosFallidos++;
                if (cuenta.IntentosFallidos >= 5)
                {
                    cuenta.BloqueadoHasta = DateTime.Now.AddMinutes(2);
                    TempData["ToastMessage"] = $"La cuenta del usuario '{model.Username}' ha sido bloqueada hasta {cuenta.BloqueadoHasta:HH:mm:ss} por seguridad.";
                    TempData["ToastType"] = "error";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Contraseña incorrecta ({cuenta.IntentosFallidos}/5).";
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
        }

        [Authorize(AuthenticationSchemes = "CookieAuth")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirst("UsuarioId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login");

            int userId = int.Parse(userIdClaim);
            var usuario = await _context.UsuariosDetalles.FirstOrDefaultAsync(u => u.UsuarioId == userId);
            
            if (usuario == null) return RedirectToAction("Login");

            return View(usuario);
        }

        [Authorize(AuthenticationSchemes = "CookieAuth")]
        [HttpPost]
        public async Task<IActionResult> UpdateContact(string field, string value)
        {
            var userIdClaim = User.FindFirst("UsuarioId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Json(new { success = false, message = "Sesión no válida" });
            int userId = int.Parse(userIdClaim);
            var usuario = await _context.UsuariosDetalles.FirstOrDefaultAsync(u => u.UsuarioId == userId);
            if (usuario == null) return Json(new { success = false, message = "Usuario no encontrado" });

            if (field == "email")
            {
                var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(value)) return Json(new { success = false, message = "Formato de correo no válido" });
                usuario.EmailPrincipal = value;
            }
            else if (field == "emailSec")
            {
                if (!string.IsNullOrEmpty(value)) {
                    var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                    if (!emailRegex.IsMatch(value)) return Json(new { success = false, message = "Formato de correo secundario no válido" });
                }
                usuario.EmailSecundario = value;
            }
            else if (field == "telefono")
            {
                var phoneRegex = new System.Text.RegularExpressions.Regex(@"^\+?[0-9\s-]{7,15}$");
                if (!phoneRegex.IsMatch(value)) return Json(new { success = false, message = "Teléfono inválido" });
                usuario.TelefonoMovil = value;
            }
            else if (field == "telefonoSec")
            {
                if (!string.IsNullOrEmpty(value)) {
                    var phoneRegex = new System.Text.RegularExpressions.Regex(@"^\+?[0-9\s-]{7,15}$");
                    if (!phoneRegex.IsMatch(value)) return Json(new { success = false, message = "Teléfono secundario inválido" });
                }
                usuario.TelefonoSecundario = value;
            }
            else return Json(new { success = false, message = "No permitido" });

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [Authorize(AuthenticationSchemes = "CookieAuth")]
        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile profilePic)
        {
            if (profilePic == null || profilePic.Length == 0) return Json(new { success = false, message = "Archivo vacío" });
            
            var userIdClaim = User.FindFirst("UsuarioId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Json(new { success = false, message = "Error de sesión" });
            int userId = int.Parse(userIdClaim);
            var usuario = await _context.UsuariosDetalles.FirstOrDefaultAsync(u => u.UsuarioId == userId);
            if (usuario == null) return Json(new { success = false, message = "Usuario no hallado" });

            var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "avatars");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var fileName = $"avatar_{userId}{Path.GetExtension(profilePic.FileName)}";
            var filePath = Path.Combine(dir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePic.CopyToAsync(stream);
            }

            usuario.FotoUrl = $"/img/avatars/{fileName}";
            await _context.SaveChangesAsync();

            return Json(new { success = true, url = usuario.FotoUrl });
        }

        [Authorize(AuthenticationSchemes = "CookieAuth")]
        [HttpGet]
        public IActionResult ChangePassword() => View();

        [Authorize(AuthenticationSchemes = "CookieAuth")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string newPass)
        {
            var user = User.Identity?.Name;
            var cuenta = await _context.Cuentas.FirstOrDefaultAsync(c => c.Username == user);
            if (cuenta == null) return Json(new { success = false, message = "Error de sesión" });

            if (string.IsNullOrEmpty(newPass)) 
                return Json(new { success = false, message = "La clave no puede estar vacía" });

            // VALIDACIÓN DE SEGURIDAD EXIGIDA:
            // - Mínimo 3 números, 3 letras, 1 especial, 1 mayúscula
            int numeros = newPass.Count(char.IsDigit);
            int letras = newPass.Count(char.IsLetter);
            bool tieneMayuscula = newPass.Any(char.IsUpper);
            bool tieneEspecial = newPass.Any(c => !char.IsLetterOrDigit(c));

            if (numeros < 3 || letras < 3 || !tieneMayuscula || !tieneEspecial)
            {
                return Json(new { 
                    success = false, 
                    message = "Seguridad insuficiente: Debe tener al menos 3 números, 3 letras, 1 mayúscula y 1 carácter especial." 
                });
            }

            cuenta.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPass);
            cuenta.RequiereCambioPassword = false;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string username, string dni, string nombreCompleto)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(dni) || string.IsNullOrEmpty(nombreCompleto))
                return Json(new { success = false, message = "Por favor, complete todos los campos de validación." });

            var cuenta = await _context.Cuentas
                .Include(c => c.UsuarioDetalle)
                .FirstOrDefaultAsync(c => c.Username == username && 
                                          c.UsuarioDetalle.NumeroDocumento == dni &&
                                          c.UsuarioDetalle.NombresCompletoCabecera == nombreCompleto);

            if (cuenta == null)
            {
                return Json(new { success = false, message = "Los datos ingresados no coinciden con ningún registro activo." });
            }

            // REINICIO AUTOMÁTICO A CLAVE CORPORATIVA
            cuenta.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Upscale2026");
            cuenta.RequiereCambioPassword = true;
            cuenta.IntentosFallidos = 0;
            cuenta.BloqueadoHasta = null;
            
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Contraseña reiniciada con éxito. Su nueva clave temporal es: Upscale2026" });
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout(string reason)
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", new { reason = reason });
        }
    }
}