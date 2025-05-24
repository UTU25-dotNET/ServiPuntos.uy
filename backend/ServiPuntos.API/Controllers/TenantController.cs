using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.Entities;

[ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _iTenantService;

        public TenantController(ITenantService tenantService)
        {
            _iTenantService = tenantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tenants = await _iTenantService.GetAllAsync();
            return Ok(tenants);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tenant = await _iTenantService.GetByIdAsync(id);
            return tenant is null ? NotFound() : Ok(tenant);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Tenant tenant)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            tenant.Id = Guid.NewGuid();
            tenant.FechaCreacion = DateTime.UtcNow;
            tenant.FechaModificacion = DateTime.UtcNow;

            await _iTenantService.AddAsync(tenant);
            return CreatedAtAction(nameof(GetById), new { id = tenant.Id }, tenant);
        }

    [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Tenant updated)
        {
            var existing = await _iTenantService.GetByIdAsync(id);
            if (existing is null) return NotFound();

            existing.Nombre = updated.Nombre;
            existing.LogoUrl = updated.LogoUrl;
            existing.Color = updated.Color;
            existing.ValorPunto = updated.ValorPunto;

            await _iTenantService.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _iTenantService.DeleteAsync(id);
            return NoContent();
        }
    }

