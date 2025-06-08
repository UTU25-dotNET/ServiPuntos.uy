using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.WebApp.Models;

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
                );

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
                CostoEnPuntos = producto.CostoEnPuntos
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
    }
}