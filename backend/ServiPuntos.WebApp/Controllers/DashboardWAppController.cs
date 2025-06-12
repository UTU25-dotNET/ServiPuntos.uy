using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using ServiPuntos.Core.Interfaces;
<<<<<<< HEAD
=======
using ServiPuntos.WebApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
>>>>>>> origin/dev

namespace ServiPuntos.Controllers
{

    public class DashboardWAppController : Controller
    {
        private readonly IUsuarioService _iUsuarioService;
        private readonly ITenantService _iTenantService;
        private readonly IUbicacionService _iUbicacionService;
<<<<<<< HEAD

        public DashboardWAppController(IUsuarioService usuarioService, ITenantService iTenantService, IUbicacionService ubicacionService)
=======
        private readonly IConfigPlataformaService _iConfigPlataformaService;
        private readonly ITransaccionService _iTransaccionService;
        private readonly ICanjeService _iCanjeService;

        public DashboardWAppController(IUsuarioService usuarioService, ITenantService iTenantService, IUbicacionService ubicacionService, IConfigPlataformaService configPlataformaService, ITransaccionService transaccionService, ICanjeService canjeService)
>>>>>>> origin/dev
        {
            _iUsuarioService = usuarioService;
            _iTenantService = iTenantService;
            _iUbicacionService = ubicacionService;
<<<<<<< HEAD
=======
            _iConfigPlataformaService = configPlataformaService;
            _iTransaccionService = transaccionService;
            _iCanjeService = canjeService;
>>>>>>> origin/dev
        }

        public async Task<IActionResult> Index()
        {
            try
            {
<<<<<<< HEAD
=======
                var config = await _iConfigPlataformaService.ObtenerConfiguracionAsync();
                ViewBag.ConfigPlataforma = config;
>>>>>>> origin/dev
                if (User.IsInRole("AdminTenant"))
                {
                    var tenantIdDeCookie = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
                    
                    if (!string.IsNullOrEmpty(tenantIdDeCookie) && Guid.TryParse(tenantIdDeCookie, out Guid tenantId))
                    {
                        var tenantActual = await _iTenantService.GetByIdAsync(tenantId);
                        
                        if (tenantActual != null)
                        {
                            // Obtener ubicaciones del tenant actual de forma segura
                            try
                            {
                                var ubicaciones = await _iUbicacionService.GetAllUbicacionesAsync(tenantActual.Id);
                                ViewBag.Ubicaciones = ubicaciones;
                            }
                            catch (Exception ex)
                            {
                                // Si hay error con ubicaciones, continuar sin ellas
                                Console.WriteLine($"Error al obtener ubicaciones: {ex.Message}");
                                ViewBag.Ubicaciones = new List<ServiPuntos.Core.Entities.Ubicacion>();
                            }

                            ViewBag.MiTenant = tenantActual;
                        }
                        else
                        {
                            ViewBag.MiTenant = null;
                            ViewBag.Ubicaciones = new List<ServiPuntos.Core.Entities.Ubicacion>();
                        }
                    }
                    else
                    {
                        ViewBag.MiTenant = null;
                        ViewBag.Ubicaciones = new List<ServiPuntos.Core.Entities.Ubicacion>();
                    }
                }

<<<<<<< HEAD
=======
                if (User.IsInRole("AdminUbicacion"))
                {
                    var ubicacionIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ubicacionId")?.Value;
                    if (!string.IsNullOrEmpty(ubicacionIdClaim) && Guid.TryParse(ubicacionIdClaim, out Guid ubicacionId))
                    {
                        var ubicacionActual = await _iUbicacionService.GetUbicacionAsync(ubicacionId);
                        if (ubicacionActual != null)
                        {
                            ViewBag.MiUbicacion = ubicacionActual;

                            var transacciones = await _iTransaccionService.GetTransaccionesByUbicacionIdAsync(ubicacionId);
                            var canjes = await _iCanjeService.GetCanjesByUbicacionIdAsync(ubicacionId);
                            var pendientes = await _iCanjeService.GetCanjesPendientesByUbicacionIdAsync(ubicacionId);

                            ViewBag.ReporteUbicacion = new ServiPuntos.WebApp.Models.ReporteUbicacionViewModel
                            {
                                TotalTransacciones = transacciones.Count(),
                                MontoTotalTransacciones = transacciones.Sum(t => t.Monto),
                                TotalCanjes = canjes.Count(),
                                CanjesCompletados = canjes.Count(c => c.Estado == ServiPuntos.Core.Enums.EstadoCanje.Canjeado)
                            };
                            ViewBag.CanjesPendientes = pendientes;
                        }
                    }
                }

>>>>>>> origin/dev
                if (User.IsInRole("AdminPlataforma")|| User.IsInRole("AdminTenant"))
                { //esto solo los carga si sos admin mi pc esta sufriendo y el chat dice q mejora el rendimiento
                    var datosUsuarios = await GetCantidadUsuariosPorTipoTodosInternal();

                    //Grafica por rol
                    var resumenGeneral = datosUsuarios
                        .SelectMany(t => t.PorTipo)
                        .GroupBy(u => u.Tipo)
                        .Select(g => new
                        {
                            Rol = g.Key,
                            Cantidad = g.Sum(x => x.Cantidad)
                        })
                        .OrderBy(x => x.Rol)
                        .ToArray();

                    var roles = resumenGeneral.Select(r => r.Rol).ToArray();
                    var cantidades = resumenGeneral.Select(r => r.Cantidad).ToArray();

                    //Grafica por tenant
                    var resumenTenants = datosUsuarios
                        .Select(t => new
                        {
                            Tenant = t.TenantNombre,
                            Cantidad = t.TotalUsuarios
                        })
                        .OrderBy(x => x.Tenant)
                        .ToArray();

                    var tenants = resumenTenants.Select(t => t.Tenant).ToArray();
                    var cantidadesTenants = resumenTenants.Select(t => t.Cantidad).ToArray();

                    // ViewBag para gráfica por rol
                    ViewBag.Roles = roles;
                    ViewBag.Cantidades = cantidades;

                    // ViewBag para gráfica por tenant
                    ViewBag.Tenants = tenants;
                    ViewBag.CantidadesTenants = cantidadesTenants;
                }
                return View();
            }
            catch (Exception ex)
            {
                // En caso de error, usar datos por defecto
                var roles = new[] { "AdminTenant", "AdminPlataforma", "AdminUbicacion", "UsuarioFinal" };
                var cantidades = new[] { 0, 0, 0, 0 };
                var tenants = new[] { "Sin datos" };
                var cantidadesTenants = new[] { 0 };

                ViewBag.Roles = roles;
                ViewBag.Cantidades = cantidades;
                ViewBag.Tenants = tenants;
                ViewBag.CantidadesTenants = cantidadesTenants;
                ViewBag.Error = $"Error al cargar datos: {ex.Message}";

                return View();
            }
        }

<<<<<<< HEAD
        [HttpGet]
        public async Task<IActionResult> CambiarConfiguracion()
        {
            //implementar
            return View();
=======
        [HttpPost]
        [Authorize(Roles = "AdminPlataforma")]
        public async Task<IActionResult> CambiarConfiguracion(int expiracionSesion, int maxIntentos, int largoMinimo)
        {
            try
            {
                var config = new ServiPuntos.Core.Entities.ConfigPlataforma(maxIntentos, expiracionSesion, largoMinimo);
                await _iConfigPlataformaService.ActualizarConfiguracionAsync(config);
                return Json(new { success = true, message = "Configuración actualizada correctamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al actualizar configuración de plataforma: {ex.Message}");
                return Json(new { success = false, message = $"Error interno: {ex.Message}" });
            }
        }

        // Agregar este método al final de la clase DashboardWAppController, antes de la clase TenantUsuarioInfo

[HttpPost]
[Authorize(Roles = "AdminTenant")]
public async Task<IActionResult> ActualizarValorPuntos(decimal valorPuntos)
{
    try
    {
        var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;

        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out Guid tenantId))
        {
            return Json(new { success = false, message = "No se pudo identificar su tenant." });
        }

        var tenant = await _iTenantService.GetByIdAsync(tenantId);
        if (tenant == null)
        {
            return Json(new { success = false, message = "Tenant no encontrado." });
        }

        // Validaciones
        if (valorPuntos <= 0)
        {
            return Json(new { success = false, message = "El valor de puntos debe ser mayor a 0." });
        }

        if (valorPuntos > 1000)
        {
            return Json(new { success = false, message = "El valor de puntos no puede ser mayor a 1000." });
        }

        // Actualizar el tenant
        tenant.ValorPunto = valorPuntos;
        tenant.FechaModificacion = DateTime.UtcNow;

        await _iTenantService.UpdateAsync(tenant);

        Console.WriteLine($"✅ Valor de puntos actualizado para tenant {tenant.Nombre}: ${valorPuntos}");

        return Json(new
        {
            success = true,
            message = "Valor de puntos actualizado correctamente",
            nuevoValor = valorPuntos.ToString("F2")
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al actualizar valor de puntos: {ex.Message}");
        return Json(new { success = false, message = $"Error interno: {ex.Message}" });
    }
}

[HttpPost]
[Authorize(Roles = "AdminTenant")]
public async Task<IActionResult> ActualizarNombrePuntos(string nombrePuntos)
{
    try
    {
        var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;

        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out Guid tenantId))
        {
            return Json(new { success = false, message = "No se pudo identificar su tenant." });
        }

        var tenant = await _iTenantService.GetByIdAsync(tenantId);
        if (tenant == null)
        {
            return Json(new { success = false, message = "Tenant no encontrado." });
        }

        if (string.IsNullOrWhiteSpace(nombrePuntos))
        {
            return Json(new { success = false, message = "El nombre de los puntos es obligatorio." });
        }

        if (nombrePuntos.Length > 50)
        {
            return Json(new { success = false, message = "El nombre de los puntos es demasiado largo." });
        }

        tenant.NombrePuntos = nombrePuntos.Trim();
        tenant.FechaModificacion = DateTime.UtcNow;

        await _iTenantService.UpdateAsync(tenant);

        Console.WriteLine($"✅ Nombre de puntos actualizado para tenant {tenant.Nombre}: {tenant.NombrePuntos}");

        return Json(new
        {
            success = true,
            message = "Nombre de puntos actualizado correctamente",
            nuevoNombre = tenant.NombrePuntos
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al actualizar nombre de puntos: {ex.Message}");
        return Json(new { success = false, message = $"Error interno: {ex.Message}" });
    }
}

[HttpPost]
[Authorize(Roles = "AdminTenant")]
        public async Task<IActionResult> ActualizarPoliticas(decimal tasaCombustible, decimal tasaMinimercado, decimal tasaServicios, int diasCaducidad)
        {
            try
            {
        var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;

        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out Guid tenantId))
        {
            return Json(new { success = false, message = "No se pudo identificar su tenant." });
        }

        var tenant = await _iTenantService.GetByIdAsync(tenantId);
        if (tenant == null)
        {
            return Json(new { success = false, message = "Tenant no encontrado." });
        }

        if (tasaCombustible < 0 || tasaMinimercado < 0 || tasaServicios < 0 || diasCaducidad < 0)
        {
            return Json(new { success = false, message = "Valores no válidos." });
        }

        tenant.TasaCombustible = tasaCombustible;
        tenant.TasaMinimercado = tasaMinimercado;
        tenant.TasaServicios = tasaServicios;
        tenant.DiasCaducidadPuntos = diasCaducidad;
        tenant.FechaModificacion = DateTime.UtcNow;

        await _iTenantService.UpdateAsync(tenant);

        return Json(new
        {
            success = true,
            message = "Políticas de acumulación actualizadas correctamente",
            tasaCombustible = tenant.TasaCombustible.ToString("F2"),
            tasaMinimercado = tenant.TasaMinimercado.ToString("F2"),
            tasaServicios = tenant.TasaServicios.ToString("F2"),
            diasCaducidad = tenant.DiasCaducidadPuntos
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al actualizar políticas de acumulación: {ex.Message}");
        return Json(new { success = false, message = $"Error interno: {ex.Message}" });
    }
        }

        [HttpPost]
        [Authorize(Roles = "AdminUbicacion")]
        public async Task<IActionResult> ActualizarPreciosCombustible(decimal precioSuper, decimal precioPremium, decimal precioDiesel)
        {
            try
            {
                var ubicacionIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ubicacionId")?.Value;
                if (string.IsNullOrEmpty(ubicacionIdClaim) || !Guid.TryParse(ubicacionIdClaim, out Guid ubicacionId))
                {
                    return Json(new { success = false, message = "No se pudo identificar su ubicación." });
                }

                var ubicacion = await _iUbicacionService.GetUbicacionAsync(ubicacionId);
                if (ubicacion == null)
                {
                    return Json(new { success = false, message = "Ubicación no encontrada." });
                }

                if (precioSuper < 0 || precioPremium < 0 || precioDiesel < 0)
                {
                    return Json(new { success = false, message = "Los precios deben ser valores positivos." });
                }

                ubicacion.PrecioNaftaSuper = precioSuper;
                ubicacion.PrecioNaftaPremium = precioPremium;
                ubicacion.PrecioDiesel = precioDiesel;
                ubicacion.FechaModificacion = DateTime.UtcNow;

                await _iUbicacionService.UpdateUbicacionByTenantAsync(ubicacion.TenantId, ubicacion);

                return Json(new
                {
                    success = true,
                    message = "Precios actualizados correctamente",
                    precioSuper = precioSuper.ToString("F2"),
                    precioPremium = precioPremium.ToString("F2"),
                    precioDiesel = precioDiesel.ToString("F2")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al actualizar precios de combustible: {ex.Message}");
                return Json(new { success = false, message = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "AdminUbicacion")]
        public async Task<IActionResult> ConfirmarCanje(Guid canjeId)
        {
            try
            {
                await _iCanjeService.ConfirmarCanjeAsync(canjeId);
                return Json(new { success = true, message = "Canje confirmado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Authorize(Roles = "AdminTenant")]
        public async Task<IActionResult> Reportes()
        {
            var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
            if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out Guid tenantId))
            {
                TempData["Error"] = "No se pudo identificar su tenant.";
                return RedirectToAction("Index");
            }

            var transacciones = await _iTransaccionService.GetTransaccionesByTenantIdAsync(tenantId);
            var canjes = await _iCanjeService.GetCanjesByTenantIdAsync(tenantId);

            var modelo = new ReporteTenantViewModel
            {
                TotalTransacciones = transacciones.Count(),
                MontoTotalTransacciones = transacciones.Sum(t => t.Monto),
                TotalCanjes = canjes.Count(),
                CanjesCompletados = canjes.Count(c => c.Estado == ServiPuntos.Core.Enums.EstadoCanje.Canjeado)
            };

            return View(modelo);
        }

        [Authorize(Roles = "AdminTenant")]
        public async Task<IActionResult> DescargarReportePdf()
        {
            var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
            if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out Guid tenantId))
            {
                return RedirectToAction("Reportes");
            }

            var transacciones = await _iTransaccionService.GetTransaccionesByTenantIdAsync(tenantId);
            var canjes = await _iCanjeService.GetCanjesByTenantIdAsync(tenantId);

            var modelo = new ReporteTenantViewModel
            {
                TotalTransacciones = transacciones.Count(),
                MontoTotalTransacciones = transacciones.Sum(t => t.Monto),
                TotalCanjes = canjes.Count(),
                CanjesCompletados = canjes.Count(c => c.Estado == ServiPuntos.Core.Enums.EstadoCanje.Canjeado)
            };

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(QuestPDF.Helpers.PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(16));

                    page.Content().Column(col =>
                    {
                        col.Item().Text("Reporte del Tenant").FontSize(20).Bold();
                        col.Item().Text($"Transacciones: {modelo.TotalTransacciones}");
                        col.Item().Text($"Monto Total: ${modelo.MontoTotalTransacciones:F2}");
                        col.Item().Text($"Canjes Generados: {modelo.TotalCanjes}");
                        col.Item().Text($"Canjes Completados: {modelo.CanjesCompletados}");
                    });
                });
            });

            var pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", "reporte.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> GetCantidadUsuariosPorTipoTodos()
        {
            try
            {
                var resultado = await GetCantidadUsuariosPorTipoTodosInternal();
                return Json(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener datos de todos los tenants: {ex.Message}");
            }
        }

        private async Task<List<TenantUsuarioInfo>> GetCantidadUsuariosPorTipoTodosInternal()
        {
            var tenants = await _iTenantService.GetAllAsync();
            var resultado = new List<TenantUsuarioInfo>();

            foreach (var tenant in tenants)
            {
                var usuarios = await _iUsuarioService.GetAllUsuariosAsync(tenant.Id);

                var cantidadPorTipo = usuarios
                    .GroupBy(u => u.Rol)
                    .Select(g => new UsuarioTipoInfo
                    {
                        Tipo = g.Key.ToString(),
                        Cantidad = g.Count()
                    })
                    .ToList();

                resultado.Add(new TenantUsuarioInfo
                {
                    TenantId = tenant.Id,
                    TenantNombre = tenant.Nombre,
                    TotalUsuarios = usuarios.Count(),
                    PorTipo = cantidadPorTipo
                });
            }

            return resultado;
>>>>>>> origin/dev
        }

        // Agregar este método al final de la clase DashboardWAppController, antes de la clase TenantUsuarioInfo

[HttpPost]
[Authorize(Roles = "AdminTenant")]
public async Task<IActionResult> ActualizarValorPuntos(decimal valorPuntos)
{
    try
    {
        var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
        
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out Guid tenantId))
        {
            return Json(new { success = false, message = "No se pudo identificar su tenant." });
        }

        var tenant = await _iTenantService.GetByIdAsync(tenantId);
        if (tenant == null)
        {
            return Json(new { success = false, message = "Tenant no encontrado." });
        }

        // Validaciones
        if (valorPuntos <= 0)
        {
            return Json(new { success = false, message = "El valor de puntos debe ser mayor a 0." });
        }

        if (valorPuntos > 1000)
        {
            return Json(new { success = false, message = "El valor de puntos no puede ser mayor a 1000." });
        }

        // Actualizar el tenant
        tenant.ValorPunto = valorPuntos;
        tenant.FechaModificacion = DateTime.UtcNow;

        await _iTenantService.UpdateAsync(tenant);

        Console.WriteLine($"✅ Valor de puntos actualizado para tenant {tenant.Nombre}: ${valorPuntos}");

        return Json(new { 
            success = true, 
            message = "Valor de puntos actualizado correctamente",
            nuevoValor = valorPuntos.ToString("F2")
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al actualizar valor de puntos: {ex.Message}");
        return Json(new { success = false, message = $"Error interno: {ex.Message}" });
    }
}

        [HttpGet]
        public async Task<IActionResult> GetCantidadUsuariosPorTipoTodos()
        {
            try
            {
                var resultado = await GetCantidadUsuariosPorTipoTodosInternal();
                return Json(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener datos de todos los tenants: {ex.Message}");
            }
        }

        private async Task<List<TenantUsuarioInfo>> GetCantidadUsuariosPorTipoTodosInternal()
        {
            var tenants = await _iTenantService.GetAllAsync();
            var resultado = new List<TenantUsuarioInfo>();

            foreach (var tenant in tenants)
            {
                var usuarios = await _iUsuarioService.GetAllUsuariosAsync(tenant.Id);

                var cantidadPorTipo = usuarios
                    .GroupBy(u => u.Rol)
                    .Select(g => new UsuarioTipoInfo
                    {
                        Tipo = g.Key.ToString(),
                        Cantidad = g.Count()
                    })
                    .ToList();

                resultado.Add(new TenantUsuarioInfo
                {
                    TenantId = tenant.Id,
                    TenantNombre = tenant.Nombre,
                    TotalUsuarios = usuarios.Count(),
                    PorTipo = cantidadPorTipo
                });
            }

            return resultado;
        }
    }


    public class TenantUsuarioInfo
    {
        public Guid TenantId { get; set; }
        public string TenantNombre { get; set; }
        public int TotalUsuarios { get; set; }
        public List<UsuarioTipoInfo> PorTipo { get; set; }
    }

    public class UsuarioTipoInfo
    {
        public string Tipo { get; set; }
        public int Cantidad { get; set; }
    }


    public class TenantUsuarioInfo
    {
        public Guid TenantId { get; set; }
        public string TenantNombre { get; set; }
        public int TotalUsuarios { get; set; }
        public List<UsuarioTipoInfo> PorTipo { get; set; }
    }

    public class UsuarioTipoInfo
    {
        public string Tipo { get; set; }
        public int Cantidad { get; set; }
    }
}