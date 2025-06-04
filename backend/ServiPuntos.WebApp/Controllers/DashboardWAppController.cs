using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.Controllers
{

    public class DashboardWAppController : Controller
    {
        private readonly IUsuarioService _iUsuarioService;
        private readonly ITenantService _iTenantService;
        private readonly IUbicacionService _iUbicacionService;

        public DashboardWAppController(IUsuarioService usuarioService, ITenantService iTenantService, IUbicacionService ubicacionService)
        {
            _iUsuarioService = usuarioService;
            _iTenantService = iTenantService;
            _iUbicacionService = ubicacionService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
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

        [HttpGet]
        public async Task<IActionResult> CambiarConfiguracion()
        {
            //implementar
            return View();
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
}