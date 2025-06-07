using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ServiPuntos.WebApp.Controllers
{
    //[Authorize(Roles = "AdminUbicacion")]
    public class ProductoUbicacionWAppController : Controller
    {
        private readonly IProductoUbicacionService _productoUbicacionService;
        private readonly IProductoCanjeableService _productoCanjeableService;
        private readonly IUbicacionService _ubicacionService;
        private readonly ITenantContext _tenantContext;

        public ProductoUbicacionWAppController(
            IProductoUbicacionService productoUbicacionService,
            IProductoCanjeableService productoCanjeableService,
            IUbicacionService ubicacionService,
            ITenantContext tenantContext)
        {
            _productoUbicacionService = productoUbicacionService;
            _productoCanjeableService = productoCanjeableService;
            _ubicacionService = ubicacionService;
            _tenantContext = tenantContext;
        }

        // GET: ProductoUbicacion
        public async Task<IActionResult> Index()
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == null)
            {
                return Unauthorized();
            }

            var ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
            var productosUbicacion = new List<ProductoUbicacion>();

            foreach (var ubicacion in ubicaciones)
            {
                var productos = await _productoUbicacionService.GetAllAsync(ubicacion.Id);
                productosUbicacion.AddRange(productos);
            }

            var viewModel = new ProductoUbicacionIndexViewModel
            {
                ProductosUbicacion = productosUbicacion,
                Ubicaciones = ubicaciones.ToList()
            };

            return View(viewModel);
        }

        // GET: ProductoUbicacion/AsignarProducto
        public async Task<IActionResult> AsignarProducto()
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == null)
            {
                return Unauthorized();
            }

            var productos = await _productoCanjeableService.GetAllProductosAsync();
            var ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);

            var viewModel = new AsignarProductoUbicacionViewModel
            {
                Productos = productos.Select(p => new ProductoSelectionViewModel
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    CostoEnPuntos = p.CostoEnPuntos,
                    Selected = false
                }).ToList(),
                Ubicaciones = ubicaciones.Select(u => new UbicacionSelectionViewModel
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Selected = false
                }).ToList(),
                StockInicial = 10
            };

            return View(viewModel);
        }

        // POST: ProductoUbicacion/AsignarProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarProducto(AsignarProductoUbicacionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var productosSeleccionados = model.Productos?.Where(p => p.Selected).ToList() ?? new List<ProductoSelectionViewModel>();
                var ubicacionesSeleccionadas = model.Ubicaciones?.Where(u => u.Selected).ToList() ?? new List<UbicacionSelectionViewModel>();

                if (!productosSeleccionados.Any() || !ubicacionesSeleccionadas.Any())
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos un producto y una ubicación.");
                    return await RecargarDatosAsignacion(model);
                }

                int asignaciones = 0;
                foreach (var producto in productosSeleccionados)
                {
                    foreach (var ubicacion in ubicacionesSeleccionadas)
                    {
                        var existente = await _productoUbicacionService.GetAllAsync(ubicacion.Id);
                        if (!existente.Any(pu => pu.ProductoCanjeableId == producto.Id))
                        {
                            var productoUbicacion = new ProductoUbicacion(
                                ubicacion.Id,
                                producto.Id,
                                model.StockInicial
                            )
                            {
                                Activo = true
                            };

                            await _productoUbicacionService.AddAsync(productoUbicacion);
                            asignaciones++;
                        }
                    }
                }

                TempData["Success"] = $"Se realizaron {asignaciones} asignaciones exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return await RecargarDatosAsignacion(model);
        }

        [HttpPost]
        public async Task<IActionResult> AsignarProductosUbicaciones(AsignarProductoUbicacionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var productosSeleccionados = model.Productos.Where(p => p.Selected).ToList();
            var ubicacionesSeleccionadas = model.Ubicaciones.Where(u => u.Selected).ToList();

            foreach (var producto in productosSeleccionados)
            {
                foreach (var ubicacion in ubicacionesSeleccionadas)
                {
                    // Crear ProductoUbicacion para cada combinación
                    var productoUbicacion = new ProductoUbicacion
                    {
                        ProductoCanjeableId = producto.Id,
                        UbicacionId = ubicacion.Id,
                        StockDisponible = model.StockInicial,
                        Activo = true
                    };

                    // Guardar en la base de datos
                    await _productoUbicacionService.AddAsync(productoUbicacion);
                }
            }

            return RedirectToAction("Index");
        }

        // GET: ProductoUbicacion/GestionarStock/{ubicacionId}
        public async Task<IActionResult> GestionarStock(Guid ubicacionId)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == null)
            {
                return Unauthorized();
            }

            var ubicacion = await _ubicacionService.GetUbicacionAsync(ubicacionId);
            if (ubicacion == null || ubicacion.TenantId != tenantId)
            {
                return NotFound();
            }

            var productosUbicacion = await _productoUbicacionService.GetAllAsync(ubicacionId);

            var viewModel = new GestionarStockViewModel
            {
                UbicacionId = ubicacionId,
                UbicacionNombre = ubicacion.Nombre,
                ProductosUbicacion = productosUbicacion.ToList()
            };

            return View(viewModel);
        }

        // POST: ProductoUbicacion/ActualizarStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarStock(Guid productoUbicacionId, int nuevoStock, bool activo)
        {
            var productoUbicacion = await _productoUbicacionService.GetAsync(productoUbicacionId);
            if (productoUbicacion == null)
            {
                return NotFound();
            }

            var tenantId = _tenantContext.TenantId;
            if (tenantId != null)
            {
                var ubicacion = await _ubicacionService.GetUbicacionAsync(productoUbicacion.UbicacionId);
                if (ubicacion?.TenantId != tenantId)
                {
                    return Unauthorized();
                }
            }

            if (nuevoStock < 0)
            {
                TempData["Error"] = "El stock no puede ser negativo.";
                return RedirectToAction(nameof(GestionarStock), new { ubicacionId = productoUbicacion.UbicacionId });
            }

            productoUbicacion.StockDisponible = nuevoStock;
            productoUbicacion.Activo = activo;

            await _productoUbicacionService.UpdateAsync(productoUbicacion);

            TempData["Success"] = "Stock actualizado exitosamente.";
            return RedirectToAction(nameof(GestionarStock), new { ubicacionId = productoUbicacion.UbicacionId });
        }

        // GET: ProductoUbicacion/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var productoUbicacion = await _productoUbicacionService.GetAsync(id);
            if (productoUbicacion == null)
            {
                return NotFound();
            }

            var tenantId = _tenantContext.TenantId;
            if (tenantId != null)
            {
                var ubicacion = await _ubicacionService.GetUbicacionAsync(productoUbicacion.UbicacionId);
                if (ubicacion?.TenantId != tenantId)
                {
                    return Unauthorized();
                }
            }

            var viewModel = new EditProductoUbicacionViewModel
            {
                Id = productoUbicacion.Id,
                UbicacionId = productoUbicacion.UbicacionId,
                ProductoCanjeableId = productoUbicacion.ProductoCanjeableId,
                StockDisponible = productoUbicacion.StockDisponible,
                Activo = productoUbicacion.Activo,
                UbicacionNombre = productoUbicacion.Ubicacion?.Nombre ?? "Cargando...",
                ProductoNombre = productoUbicacion.ProductoCanjeable?.Nombre ?? "Cargando..."
            };

            return View(viewModel);
        }

        // POST: ProductoUbicacion/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditProductoUbicacionViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var productoUbicacion = await _productoUbicacionService.GetAsync(id);
                if (productoUbicacion == null)
                {
                    return NotFound();
                }

                var tenantId = _tenantContext.TenantId;
                if (tenantId != null)
                {
                    var ubicacion = await _ubicacionService.GetUbicacionAsync(productoUbicacion.UbicacionId);
                    if (ubicacion?.TenantId != tenantId)
                    {
                        return Unauthorized();
                    }
                }

                if (model.StockDisponible < 0)
                {
                    ModelState.AddModelError("StockDisponible", "El stock no puede ser negativo.");
                    return View(model);
                }

                productoUbicacion.StockDisponible = model.StockDisponible;
                productoUbicacion.Activo = model.Activo;

                await _productoUbicacionService.UpdateAsync(productoUbicacion);

                TempData["Success"] = "Producto ubicación actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // POST: ProductoUbicacion/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var productoUbicacion = await _productoUbicacionService.GetAsync(id);
            if (productoUbicacion == null)
            {
                return NotFound();
            }

            var tenantId = _tenantContext.TenantId;
            if (tenantId != null)
            {
                var ubicacion = await _ubicacionService.GetUbicacionAsync(productoUbicacion.UbicacionId);
                if (ubicacion?.TenantId != tenantId)
                {
                    return Unauthorized();
                }
            }

            productoUbicacion.Activo = false;
            await _productoUbicacionService.UpdateAsync(productoUbicacion);

            TempData["Success"] = "Producto removido de la ubicación exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: ProductoUbicacion/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var productoUbicacion = await _productoUbicacionService.GetAsync(id);
            if (productoUbicacion == null)
            {
                return NotFound();
            }

            var tenantId = _tenantContext.TenantId;
            if (tenantId != null)
            {
                var ubicacion = await _ubicacionService.GetUbicacionAsync(productoUbicacion.UbicacionId);
                if (ubicacion?.TenantId != tenantId)
                {
                    return Unauthorized();
                }
            }

            return View(productoUbicacion);
        }

        private async Task<IActionResult> RecargarDatosAsignacion(AsignarProductoUbicacionViewModel model)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId != null)
            {
                var productos = await _productoCanjeableService.GetAllProductosAsync();
                var ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);

                model.Productos = productos.Select(p => new ProductoSelectionViewModel
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    CostoEnPuntos = p.CostoEnPuntos,
                    Selected = model.Productos?.Any(mp => mp.Id == p.Id && mp.Selected) ?? false
                }).ToList();

                model.Ubicaciones = ubicaciones.Select(u => new UbicacionSelectionViewModel
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Selected = model.Ubicaciones?.Any(mu => mu.Id == u.Id && mu.Selected) ?? false
                }).ToList();
            }

            return View(model);
        }
    }
}