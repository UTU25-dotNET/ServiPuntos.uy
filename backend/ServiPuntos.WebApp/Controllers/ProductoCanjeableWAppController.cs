using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.WebApp.Models;
using System.Linq;

namespace ServiPuntos.WebApp.Controllers
{
    //[Authorize(Roles = "AdminUbicacion")]
    public class ProductoCanjeableWAppController : Controller
    {
        private readonly IProductoCanjeableService _productoCanjeableService;
        private readonly IProductoUbicacionService _productoUbicacionService;
        private readonly IUbicacionService _ubicacionService;
        private readonly ITenantContext _tenantContext;

        public ProductoCanjeableWAppController(
            IProductoCanjeableService productoCanjeableService,
            IProductoUbicacionService productoUbicacionService,
            IUbicacionService ubicacionService,
            ITenantContext tenantContext)
        {
            _productoCanjeableService = productoCanjeableService;
            _productoUbicacionService = productoUbicacionService;
            _ubicacionService = ubicacionService;
            _tenantContext = tenantContext;
        }

        // GET: ProductosCanjeables
        public async Task<IActionResult> Index()
        {
            var tenantId = _tenantContext.TenantId;
            //var tenantId  = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
            if (tenantId == null)
            {
                return Unauthorized();
            }

            // Obtener ubicaciones del tenant actual
            var ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);

            // Obtener todos los productos canjeables disponibles
            var productos = await _productoCanjeableService.GetAllProductosAsync();

            var viewModel = new ProductosCanjeablesIndexViewModel
            {
                Productos = productos.ToList(),
                Ubicaciones = ubicaciones.ToList()
            };

            return View(viewModel);
        }

        // GET: ProductosCanjeables/Create
        public IActionResult Crear()
        {
            return View();
        }

        // POST: ProductosCanjeables/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CreateProductoCanjeableViewModel model)
        {
            if (ModelState.IsValid)
            {
                var producto = new ProductoCanjeable(
                    model.Nombre,
                    model.Descripcion,
                    model.CostoEnPuntos
                )
                {
                    FotoUrl = model.FotoUrl
                };

                await _productoCanjeableService.AddProductoAsync(producto);

                TempData["Success"] = "Producto canjeable creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: ProductosCanjeables/Edit/5
        public async Task<IActionResult> Editar(Guid id)
        {
            var producto = await _productoCanjeableService.GetProductoAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            var viewModel = new EditProductoCanjeableViewModel
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                CostoEnPuntos = producto.CostoEnPuntos,
                FotoUrl = producto.FotoUrl
            };

            return View(viewModel);
        }

        // POST: ProductosCanjeables/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Guid id, EditProductoCanjeableViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var producto = await _productoCanjeableService.GetProductoAsync(id);
                if (producto == null)
                {
                    return NotFound();
                }

                producto.Nombre = model.Nombre;
                producto.Descripcion = model.Descripcion;
                producto.CostoEnPuntos = model.CostoEnPuntos;

                producto.FotoUrl = model.FotoUrl;
                await _productoCanjeableService.UpdateProductoAsync(producto);

                TempData["Success"] = "Producto canjeable actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: ProductosCanjeables/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var producto = await _productoCanjeableService.GetProductoAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: ProductosCanjeables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _productoCanjeableService.DeleteProductoAsync(id);
            TempData["Success"] = "Producto canjeable eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: ProductoCanjeable/AsignarUbicacion/{id}
        public async Task<IActionResult> AsignarUbicacion(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == null)
            {
                return Unauthorized();
            }

            var producto = await _productoCanjeableService.GetProductoAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            var ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);

            var viewModel = new AsignarUbicacionesProductoViewModel
            {
                ProductoId = producto.Id,
                ProductoNombre = producto.Nombre,
                CostoEnPuntos = producto.CostoEnPuntos,
                Ubicaciones = ubicaciones.Select(u => new UbicacionSelectionViewModel
                {
                    Id = u.Id,
                    Nombre = u.Nombre ?? string.Empty,
                    Selected = false
                }).ToList(),
                StockInicial = 10
            };

            return View(viewModel);
        }

        // POST: ProductoCanjeable/AsignarUbicacion/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarUbicacion(Guid id, AsignarUbicacionesProductoViewModel model)
        {
            if (id != model.ProductoId)
            {
                return NotFound();
            }

            var tenantId = _tenantContext.TenantId;
            if (tenantId == null)
            {
                return Unauthorized();
            }

            var producto = await _productoCanjeableService.GetProductoAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            var ubicacionesSeleccionadas = model.Ubicaciones.Where(u => u.Selected).ToList();
            if (!ubicacionesSeleccionadas.Any())
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar al menos una ubicación.");
            }

            if (!ModelState.IsValid)
            {
                var ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
                model.Ubicaciones = ubicaciones.Select(u => new UbicacionSelectionViewModel
                {
                    Id = u.Id,
                    Nombre = u.Nombre ?? string.Empty,
                    Selected = model.Ubicaciones.Any(s => s.Id == u.Id && s.Selected)
                }).ToList();
                model.ProductoNombre = producto.Nombre;
                model.CostoEnPuntos = producto.CostoEnPuntos;
                return View(model);
            }

            int asignadas = 0;
            foreach (var ubicacion in ubicacionesSeleccionadas)
            {
                var existentes = await _productoUbicacionService.GetAllAsync(ubicacion.Id);
                if (!existentes.Any(pu => pu.ProductoCanjeableId == id))
                {
                    var productoUbicacion = new ProductoUbicacion(ubicacion.Id, id, model.StockInicial)
                    {
                        Activo = true
                    };
                    await _productoUbicacionService.AddAsync(productoUbicacion);
                    asignadas++;
                }
            }

            TempData["Success"] = $"Se asignó el producto a {asignadas} ubicaciones.";
            return RedirectToAction(nameof(Index));
        }
    }
}