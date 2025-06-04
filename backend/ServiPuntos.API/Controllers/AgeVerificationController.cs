using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/verify")]
    public class AgeVerificationController : ControllerBase     
    {
        private readonly IWebHostEnvironment _environment;
        private readonly List<UserAgeData> _userAgeData;

        public AgeVerificationController(IWebHostEnvironment environment)
        {
            _environment = environment;

            // Cargar datos del archivo JSON
            var filePath = Path.Combine(_environment.ContentRootPath, "Data", "MockData", "age-verification.json");
            if (System.IO.File.Exists(filePath))
            {
                var jsonData = System.IO.File.ReadAllText(filePath);
                _userAgeData = JsonSerializer.Deserialize<List<UserAgeData>>(jsonData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<UserAgeData>();
            }
            else
            {
                Console.WriteLine($"[AgeVerification] Error: Archivo no encontrado en {filePath}");
                _userAgeData = new List<UserAgeData>();
            }
        }



        /// GET /api/verify/age_verify?cedula=1234567890
        /// Respuesta:
        /// {
        ///   "isAllowed": true,
        ///   "edad": 25
        /// }
        [HttpGet("age_verify")]
        [AllowAnonymous]
        public IActionResult VerifyAge([FromQuery] string cedula)
        {
            Console.WriteLine($"[AgeVerification] Verificando edad para cédula: {cedula}");
            bool isAllowed = false;
            if (string.IsNullOrWhiteSpace(cedula))
            {
                return BadRequest(new { message = "La cédula es requerida", isAllowed = false });
            }

            try
            {
                // Normalizar el formato de la cédula para la búsqueda (eliminar espacios adicionales)
                cedula = cedula.Trim();

                // Buscar la cédula en nuestros datos mock
                var userData = _userAgeData.FirstOrDefault(u => u.Cedula == cedula);

                if (userData == null)
                {
                    // Extraer el último carácter de la cédula (después de eliminar posibles guiones y puntos)
                    string normalizedCedula = new string(cedula.Where(c => char.IsDigit(c)).ToArray());

                    if (normalizedCedula.Length == 0)
                    {
                        Console.WriteLine($"[AgeVerification] Cédula no válida: {cedula}");
                        return BadRequest(new { message = "Formato de cédula no válido", isAllowed = false });
                    }

                    // Obtener el último dígito
                    char lastDigit = normalizedCedula[normalizedCedula.Length - 1];

                    // Verificar si es par (0,2,4,6,8) o impar (1,3,5,7,9)
                    bool isEven = (lastDigit - '0') % 2 == 0;
                    isAllowed = isEven; // Permitido si es par

                    int edad = isAllowed ? 25 : 16; // Edad ficticia basada en si es permitido o no

                    Console.WriteLine($"[AgeVerification] Cédula no encontrada: {cedula}, retornando resultado ficticio. Último dígito: {lastDigit}, Es par: {isEven}, Permitido: {isAllowed}");

                    // Cambiamos NotFound por Ok para evitar errores HTTP 404
                    return Ok(new { message = "Cédula procesada con regla de paridad", isAllowed, edad });
                }

                // Verificar si la persona tiene 18 años o más
                isAllowed = userData.Edad >= 18;
                Console.WriteLine($"[AgeVerification] Resultado para {cedula}: {(isAllowed ? "Permitido" : "No permitido")}. Edad: {userData.Edad}");

                return Ok(new { isAllowed, edad = userData.Edad });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AgeVerification] Error al verificar edad: {ex.Message}");
                return StatusCode(500, new { message = "Error al verificar la edad", isAllowed = false });
            }
        }

        // Clase para deserializar los datos del JSON
        public class UserAgeData
        {
            public required string Cedula { get; set; }
            public int Edad { get; set; }
        }
    }
}
