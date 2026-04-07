using Microsoft.AspNetCore.Mvc;
using ColegioPrivado.Models;
using ColegioPrivado.Data;
using ColegioPrivado.Security;
using Microsoft.EntityFrameworkCore;
using ColegioPrivado.Helpers;
using System.Linq;

namespace ColegioPrivado.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ----------------------------
        // LOGIN
        // ----------------------------
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Mensaje = "Debe ingresar usuario y contraseña";
                    return View(model);
                }

                var usuario = _context.Usuarios
                    .FirstOrDefault(u => u.UsuarioNombre == model.Usuario);

                if (usuario == null)
                {
                    model.Mensaje = "Usuario o contraseña incorrectos";
                    return View(model);
                }

                if (!usuario.Estado)
                {
                    model.Mensaje = "Usuario inactivo";
                    return View(model);
                }

                bool passwordCorrect = PasswordHelper.VerifyPassword(model.Password, usuario.PasswordHash, usuario.Salt);

                if (!passwordCorrect)
                {
                    usuario.IntentosFallidos++;

                    if (usuario.IntentosFallidos >= 3)
                    {
                        usuario.Estado = false;

                        string codigo = CodigoHelper.GenerarCodigo();

                        usuario.CodigoRecuperacion = codigo;
                        usuario.FechaExpiracionCodigo = DateTime.Now.AddMinutes(10);

                        _context.SaveChanges();

                        if (!string.IsNullOrEmpty(usuario.Email))
                        {
                            EmailHelper.EnviarCodigo(usuario.Email, codigo);
                        }

                        return RedirectToAction("IngresarCodigo");
                    }
                    else
                    {
                        model.Mensaje = "Contraseña incorrecta";
                    }

                    _context.SaveChanges();
                    return View(model);
                }

                // 🔹 PASSWORD CORRECTO

                usuario.IntentosFallidos = 0;

                // 🔹 Guardar acceso anterior
                var ultimoAccesoAnterior = usuario.FechaUltimoAcceso;

                HttpContext.Session.SetString(
                    "UltimoAcceso",
                    ultimoAccesoAnterior?.ToString("dd/MM/yyyy HH:mm") ?? "Primer acceso"
                );

                // 🔹 Actualizar nuevo acceso
                usuario.FechaUltimoAcceso = DateTime.Now;

                _context.SaveChanges();

                // 🔹 Guardar datos en sesión
                HttpContext.Session.SetString("Usuario", usuario.UsuarioNombre);
                HttpContext.Session.SetString("Rol", usuario.Rol ?? "");

                // 🔹 Primer inicio
                if (usuario.EsPrimerInicio)
                {
                    return RedirectToAction("CambiarPassword");
                }

                // 🔹 Redirección por rol
                if (usuario.Rol == "Administrador")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (usuario.Rol == "Usuario Final")
                {
                    return RedirectToAction("Dashboard", "Usuario");
                }
                else if (usuario.Rol == "Maestro")
                {
                    return RedirectToAction("Panel", "Maestro");
                }
                else if (usuario.Rol == "Secretaria")
                {
                    return RedirectToAction("Panel", "Secretaria");
                }

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                model.Mensaje = "Ocurrió un error, intente de nuevo más tarde";
                return View(model);
            }
        }

        // ----------------------------
        // INGRESAR CODIGO
        // ----------------------------
        public IActionResult IngresarCodigo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IngresarCodigo(string codigo)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.CodigoRecuperacion == codigo);

            if (usuario == null)
            {
                ViewBag.Mensaje = "Código incorrecto";
                return View();
            }

            if (usuario.FechaExpiracionCodigo < DateTime.Now)
            {
                ViewBag.Mensaje = "El código ha expirado";
                return View();
            }

            HttpContext.Session.SetString("Usuario", usuario.UsuarioNombre);

            return RedirectToAction("CambiarPassword");
        }

        // ----------------------------
        // CAMBIAR PASSWORD
        // ----------------------------
        public IActionResult CambiarPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CambiarPassword(CambiarPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Mensaje = "Debe completar todos los campos";
                    return View(model);
                }

                if (model.NuevaPassword != model.ConfirmarPassword)
                {
                    model.Mensaje = "Las contraseñas no coinciden";
                    return View(model);
                }

                if (model.NuevaPassword.Length < 8 ||
                    !model.NuevaPassword.Any(char.IsUpper) ||
                    !model.NuevaPassword.Any(char.IsLower) ||
                    !model.NuevaPassword.Any(char.IsDigit) ||
                    model.NuevaPassword.Contains(" "))
                {
                    model.Mensaje = "La contraseña debe tener mínimo 8 caracteres, mayúscula, minúscula, número y sin espacios.";
                    return View(model);
                }

                if (!model.NuevaPassword.Any(ch => "!@#$%^&*()_-+=<>?/{}[]".Contains(ch)))
                {
                    model.Mensaje = "La contraseña debe incluir al menos un carácter especial.";
                    return View(model);
                }

                var nombreUsuario = HttpContext.Session.GetString("Usuario");

                var usuario = _context.Usuarios
                    .FirstOrDefault(u => u.UsuarioNombre == nombreUsuario);

                if (usuario == null)
                {
                    model.Mensaje = "Usuario no encontrado";
                    return View(model);
                }

                // 🚫 VALIDAR QUE NO SEA LA MISMA CONTRASEÑA ACTUAL
                bool esMismaPassword = PasswordHelper.VerifyPassword(
                     model.NuevaPassword,
                     usuario.PasswordHash,
                     usuario.Salt
                );

                if (esMismaPassword)
                {
                    model.Mensaje = "No puede usar la misma contraseña actual.";
                    return View(model);
                }


                var historialPasswords = _context.HistorialPassword
                    .Where(h => h.IdUsuario == usuario.IdUsuario)
                    .ToList();

                var soundexNuevo = SoundexHelper.Generar(model.NuevaPassword);

                foreach (var passAnterior in historialPasswords)
                {
                    bool coincide = PasswordHelper.VerifyPassword(
                        model.NuevaPassword,
                        passAnterior.PasswordHash,
                        passAnterior.Salt
                    );

                    if (coincide)
                    {
                        model.Mensaje = "No puede usar una contraseña que ya utilizó anteriormente.";
                        return View(model);
                    }

                    if (passAnterior.PasswordSoundex == soundexNuevo)
                    {
                        model.Mensaje = "La contraseña es demasiado similar a una utilizada anteriormente.";
                        return View(model);
                    }
                }

                var historial = new HistorialPassword
                {
                    IdUsuario = usuario.IdUsuario,
                    PasswordHash = usuario.PasswordHash,
                    Salt = usuario.Salt,
                    PasswordSoundex = SoundexHelper.Generar(usuario.PasswordHash),
                    FechaCambio = DateTime.Now
                };

                _context.HistorialPassword.Add(historial);

                string salt = PasswordHelper.GenerateSalt();
                string hash = PasswordHelper.HashPassword(model.NuevaPassword, salt);

                usuario.PasswordHash = hash;
                usuario.Salt = salt;
                usuario.EsPrimerInicio = false;
                usuario.Estado = true;

                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                model.Mensaje = ex.Message;
                return View(model);
            }
        }

        // ----------------------------
        // CERRAR SESION
        // ----------------------------
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        // ----------------------------
        // REGISTRO
        // ----------------------------
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(string Email, string NivelSolicitado)
        {
            try
            {
                string codigo = CodigoHelper.GenerarCodigo();

                var solicitud = new SolicitudesAcceso
                {
                    Email = Email,
                    NivelSolicitado = NivelSolicitado,
                    CodigoAcceso = codigo,
                    Estado = "Pendiente",
                    FechaSolicitud = DateTime.Now
                };

                _context.SolicitudesAccesos.Add(solicitud);
                _context.SaveChanges();

                ViewBag.Mensaje = "Solicitud enviada correctamente";

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
                return View();
            }
        }

        // ----------------------------
        // ACTIVAR CUENTA
        // ----------------------------
        public IActionResult ActivarCuenta()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ActivarCuenta(string Email, string Codigo)
        {
            try
            {
                var solicitud = _context.SolicitudesAccesos
                    .FirstOrDefault(s => s.Email == Email && s.CodigoAcceso == Codigo);

                if (solicitud == null)
                {
                    ViewBag.Mensaje = "Código o correo incorrecto";
                    return View();
                }

                if (solicitud.Estado == "CuentaCreada")
                {
                    ViewBag.Mensaje = "La cuenta ya fue creada";
                    return View();
                }

                string passwordTemporal = PasswordGenerator.GenerarPasswordTemporal();

                string salt = PasswordHelper.GenerateSalt();
                string hash = PasswordHelper.HashPassword(passwordTemporal, salt);

                int rol = 3;

                if (solicitud.NivelSolicitado == "Administración")
                    rol = 2;

                var usuario = new Usuario
                {
                    UsuarioNombre = Email,
                    Email = Email,
                    PasswordHash = hash,
                    Salt = salt,
                    IntentosFallidos = 0,
                    Estado = true,
                    IdRol = rol,
                    Rol = solicitud.NivelSolicitado,
                    EsPrimerInicio = true,
                    FechaUltimoAcceso = DateTime.Now
                };

                _context.Usuarios.Add(usuario);

                solicitud.Estado = "CuentaCreada";

                _context.SaveChanges();

                ViewBag.Mensaje = "Cuenta creada correctamente.";
                ViewBag.PasswordTemporal = passwordTemporal;

                return View();
            }
            catch
            {
                ViewBag.Mensaje = "Error al activar cuenta";
                return View();
            }
        }
    }
}
