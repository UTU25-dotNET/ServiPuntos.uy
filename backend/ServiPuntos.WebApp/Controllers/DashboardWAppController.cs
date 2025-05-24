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

        public DashboardWAppController(IUsuarioService usuarioService, ITenantService iTenantService)
        {
            _iUsuarioService = usuarioService;
            _iTenantService = iTenantService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
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