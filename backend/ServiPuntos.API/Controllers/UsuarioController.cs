using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Core.DTOs;
using System.Linq;
using System.Security.Claims;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _iUsuarioService;
        private readonly ITransaccionService _transaccionService;
        private readonly ICanjeService _canjeService;

        public UsuarioController(IUsuarioService usuarioService, ITransaccionService transaccionService, ICanjeService canjeService)
        {
            _iUsuarioService = usuarioService;
            _transaccionService = transaccionService;
            _canjeService = canjeService;
        }

        // Obtener todos los usuarios.

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _iUsuarioService.GetAllUsuariosAsync();
            return Ok(usuarios);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUsuarioByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "El email es requerido" });
            }

            try
            {
                // Decodificar el email por si viene URL-encoded
                email = Uri.UnescapeDataString(email).ToLower().Trim();

                Console.WriteLine($"[GetUsuarioByEmail] Buscando usuario con email: {email}");

                // Buscar el usuario por email
                var usuarios = await _iUsuarioService.GetAllUsuariosAsync();
                var usuario = usuarios.FirstOrDefault(u => u.Email.ToLower() == email);

                if (usuario == null)
                {
                    Console.WriteLine($"[GetUsuarioByEmail] Usuario no encontrado: {email}");
                    return NotFound(new { message = "Usuario no encontrado" });
                }
                
                var transacciones = await _transaccionService.GetTransaccionesByUsuarioIdAsync(usuario.Id);
                var canjes = await _canjeService.GetCanjesByUsuarioIdAsync(usuario.Id);

                var totalCompras = transacciones.Count();
                var totalVisitas = totalCompras + canjes.Count();

                usuario.TotalCompras = totalCompras;
                usuario.TotalVisitas = totalVisitas;

                usuario.GastoTotal = transacciones.Sum(t => t.Monto);
                usuario.GastoPromedio = totalCompras > 0 ? usuario.GastoTotal / totalCompras : 0;

                var ultimaTransaccion = transacciones.OrderByDescending(t => t.FechaTransaccion).FirstOrDefault()?.FechaTransaccion;
                var ultimaFechaCanje = canjes.OrderByDescending(c => c.FechaCanje ?? c.FechaGeneracion).FirstOrDefault()?.FechaCanje ??
                                        canjes.OrderByDescending(c => c.FechaCanje ?? c.FechaGeneracion).FirstOrDefault()?.FechaGeneracion;

                DateTime? ultimaVisita = usuario.UltimaVisita;
                if (ultimaTransaccion.HasValue && (!ultimaVisita.HasValue || ultimaTransaccion > ultimaVisita))
                {
                    ultimaVisita = ultimaTransaccion;
                }
                if (ultimaFechaCanje.HasValue && (!ultimaVisita.HasValue || ultimaFechaCanje > ultimaVisita))
                {
                    ultimaVisita = ultimaFechaCanje;
                }
                usuario.UltimaVisita = ultimaVisita;

                var mesesDesdeCreacion = (DateTime.UtcNow - usuario.FechaCreacion).Days / 30.0;
                if (mesesDesdeCreacion > 0)
                {
                    usuario.VisitasPorMes = (decimal)(totalVisitas / mesesDesdeCreacion);
                }
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUsuarioByEmail] Error: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("transaccion/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetTransaccion(Guid id)
        {
            try
            {
                var emailClaim = User.FindFirst(ClaimTypes.Email) ?? User.FindFirst("email");
                if (emailClaim == null)
                {
                    return Unauthorized(new { message = "Email no disponible" });
                }

                var usuario = await _iUsuarioService.GetUsuarioAsync(emailClaim.Value);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                var transaccion = await _transaccionService.GetTransaccionByIdAsync(id);
                if (transaccion == null || transaccion.UsuarioId != usuario.Id)
                {
                    return NotFound(new { message = "Transacción no encontrada" });
                }

                var response = new
                {
                    id = transaccion.Id,
                    fecha = transaccion.FechaTransaccion,
                    monto = transaccion.Monto,
                    tipo = transaccion.TipoTransaccion.ToString(),
                    ubicacion = transaccion.Ubicacion?.Nombre,
                    puntosOtorgados = transaccion.PuntosOtorgados,
                    puntosUtilizados = transaccion.PuntosUtilizados,
                    detalles = transaccion.Detalles
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener transacción", error = ex.Message });
            }
        }

        // Obtener un usuario por ID.

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(Guid id)
        {
            var usuario = await _iUsuarioService.GetUsuarioAsync(id);
            return usuario == null ? NotFound() : Ok(usuario);
        }

        // Crear usuario

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            usuario.Id = Guid.NewGuid();
            usuario.FechaCreacion = DateTime.UtcNow;
            usuario.FechaModificacion = DateTime.UtcNow;

            // Encriptar contraseña si viene en texto plano
            if (!string.IsNullOrEmpty(usuario.Password))
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

            await _iUsuarioService.AddUsuarioAsync(usuario);
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }


        // Actualizar un usuario existente.

        // Reemplaza el método Update en UsuarioController.cs para permitir más campos

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UsuarioDto usuarioDto)
        {
            try
            {
                // Validar entrada
                if (usuarioDto == null)
                {
                    return BadRequest(new { message = "Los datos del usuario son requeridos" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Console.WriteLine($"[Update] Actualizando usuario ID: {id}");
                Console.WriteLine($"[Update] Datos recibidos: {System.Text.Json.JsonSerializer.Serialize(usuarioDto)}");

                // Buscar usuario existente
                var existente = await _iUsuarioService.GetUsuarioAsync(id);
                if (existente == null)
                {
                    Console.WriteLine($"[Update] Usuario no encontrado: {id}");
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                Console.WriteLine($"[Update] Usuario encontrado: {existente.Email}");

                // Actualizar campos básicos (siempre requeridos)
                if (!string.IsNullOrWhiteSpace(usuarioDto.Nombre))
                {
                    existente.Nombre = usuarioDto.Nombre.Trim();
                }

                if (!string.IsNullOrWhiteSpace(usuarioDto.Email))
                {
                    existente.Email = usuarioDto.Email.Trim();
                }

                // Actualizar campos opcionales solo si vienen en el DTO
                if (!string.IsNullOrEmpty(usuarioDto.Apellido))
                {
                    existente.Apellido = usuarioDto.Apellido.Trim();
                }

                if (usuarioDto.Telefono.HasValue && usuarioDto.Telefono.Value > 0)
                {
                    existente.Telefono = usuarioDto.Telefono.Value;
                }

                if (!string.IsNullOrEmpty(usuarioDto.CiudadResidencia))
                {
                    existente.CiudadResidencia = usuarioDto.CiudadResidencia.Trim();
                }

                if (!string.IsNullOrEmpty(usuarioDto.CombustiblePreferido))
                {
                    existente.CombustiblePreferido = usuarioDto.CombustiblePreferido.Trim();
                }

                if (usuarioDto.FechaNacimiento.HasValue)
                {
                    existente.FechaNacimiento = usuarioDto.FechaNacimiento;
                }

                if (!string.IsNullOrEmpty(usuarioDto.Intereses))
                {
                    // Convertir string separado por comas a lista
                    existente.Intereses = usuarioDto.Intereses
                        .Split(',')
                        .Select(i => i.Trim())
                        .Where(i => !string.IsNullOrEmpty(i))
                        .ToList();
                }

                // Actualizar puntos si viene en el DTO (mantener existente si no viene)
                if (usuarioDto.Puntos.HasValue)
                {
                    existente.Puntos = usuarioDto.Puntos.Value;
                }

                // Actualizar contraseña solo si se proporciona
                if (!string.IsNullOrWhiteSpace(usuarioDto.Password))
                {
                    Console.WriteLine("[Update] Actualizando contraseña");
                    existente.Password = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Password);
                }

                // Actualizar fecha de modificación
                existente.FechaModificacion = DateTime.UtcNow;

                Console.WriteLine($"[Update] Guardando cambios para usuario: {existente.Email}");

                // Guardar cambios
                await _iUsuarioService.UpdateUsuarioAsync(existente);

                Console.WriteLine($"[Update] Usuario actualizado exitosamente: {existente.Email}");

                // Retornar respuesta exitosa sin datos sensibles
                return Ok(new
                {
                    message = "Perfil actualizado exitosamente",
                    usuario = new
                    {
                        id = existente.Id,
                        nombre = existente.Nombre,
                        apellido = existente.Apellido,
                        email = existente.Email,
                        telefono = existente.Telefono,
                        ciudadResidencia = existente.CiudadResidencia,
                        combustiblePreferido = existente.CombustiblePreferido,
                        puntos = existente.Puntos,
                        fechaModificacion = existente.FechaModificacion
                    }
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Update] Error: {ex.Message}");
                Console.WriteLine($"[Update] StackTrace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    message = "Error interno del servidor al actualizar el perfil",
                    error = ex.Message
                });
            }
        }
        [HttpGet("historial-transacciones")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetHistorialTransacciones()
        {
            try
            {
                var emailClaim = User.FindFirst(ClaimTypes.Email) ?? User.FindFirst("email");
                if (emailClaim == null)
                {
                    return Unauthorized(new { message = "Email no disponible" });
                }

                var usuario = await _iUsuarioService.GetUsuarioAsync(emailClaim.Value);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                var transacciones = await _transaccionService.GetTransaccionesByUsuarioIdAsync(usuario.Id);

                var response = transacciones.Select(t => new
                {
                    id = t.Id,
                    fecha = t.FechaTransaccion,
                    monto = t.Monto,
                    tipo = t.TipoTransaccion.ToString(),
                    ubicacion = t.Ubicacion?.Nombre,
                    puntosOtorgados = t.PuntosOtorgados,
                    puntosUtilizados = t.PuntosUtilizados,
                    detalles = t.Detalles
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener historial", error = ex.Message });
            }
        }
        // Eliminar un usuario por ID

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var usuario = await _iUsuarioService.GetUsuarioAsync(id);
            if (usuario == null)
                return NotFound();

            await _iUsuarioService.DeleteUsuarioAsync(id);
            return NoContent();
        }
    }
}
