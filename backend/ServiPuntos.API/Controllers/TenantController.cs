using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Infrastructure.Data;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly ServiPuntosDbContext _ctx;

        public TenantController(ServiPuntosDbContext ctx)
        {
            _ctx = ctx;
        }

        // GET api/tenant
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tenants = await _ctx.Tenants
                                    .Include(t => t.Ubicaciones)
                                    .ToListAsync();
            return Ok(tenants);
        }

        // GET api/tenant/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tenant = await _ctx.Tenants
                                   .Include(t => t.Ubicaciones)
                                   .FirstOrDefaultAsync(t => t.Id == id);

            if (tenant is null)
                return NotFound();

            return Ok(tenant);
        }

        // POST api/tenant
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Tenant tenant)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            tenant.Id = Guid.NewGuid();
            tenant.FechaCreacion = DateTime.UtcNow;
            tenant.FechaModificacion = DateTime.UtcNow;

            _ctx.Tenants.Add(tenant);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                                   new { id = tenant.Id },
                                   tenant);
        }

        // PUT api/tenant/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Tenant updated)
        {
            var existing = await _ctx.Tenants.FindAsync(id);
            if (existing is null)
                return NotFound();

            existing.Nombre = updated.Nombre;
            existing.LogoUrl = updated.LogoUrl;
            existing.Color = updated.Color;
            existing.ValorPunto = updated.ValorPunto;
            existing.FechaModificacion = DateTime.UtcNow;

            _ctx.Tenants.Update(existing);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/tenant/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _ctx.Tenants.FindAsync(id);
            if (existing is null)
                return NotFound();

            _ctx.Tenants.Remove(existing);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }
    }
}
