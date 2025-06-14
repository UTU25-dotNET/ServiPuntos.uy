using ServiPuntos.Core.Enums;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.NAFTA;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiPuntos.Application.Services
{
    public class PointsRuleEngine : IPointsRuleEngine
    {
        private readonly ITenantService _tenantService;

        public PointsRuleEngine(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public async Task<int> CalcularPuntosAsync(TransaccionNAFTA transaccion, Guid tenantId)
        {
            // Obtener configuración del tenant para las reglas de puntos
            var tenant = await _tenantService.GetByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new Exception($"Tenant con ID {tenantId} no encontrado");
            }

            // Estos valores deberían venir de la configuración del tenant
            // Aquí simplificamos con valores predeterminados
            decimal tasaConversionCombustible = tenant.TasaCombustible;
            decimal tasaConversionMinimercado = tenant.TasaMinimercado;
            decimal tasaConversionServicios = tenant.TasaServicios;   // 3 puntos por cada 1 pesos

            decimal puntosCalculados = 0m;

            // Aplicar reglas según el tipo de transacción
            switch (transaccion.TipoTransaccion)
            {
                case TipoTransaccion.CompraCombustible:
                    puntosCalculados = transaccion.Monto * tasaConversionCombustible;
                    break;

                case TipoTransaccion.CompraMinimercado:
                    puntosCalculados = transaccion.Monto * tasaConversionMinimercado;
                    break;

                case TipoTransaccion.UsoServicio:
                    puntosCalculados = transaccion.Monto * tasaConversionServicios;
                    break;

                default:
                    // Por defecto aplicar tasa mínima
                    puntosCalculados = transaccion.Monto;
                    break;
            }

            // Aplicar reglas adicionales por categoría de productos
            foreach (var producto in transaccion.Productos)
            {
                // Aquí podrían aplicarse multiplicadores o bonificaciones específicas
                // basadas en categorías de productos, por ejemplo:
                if (producto.Categoria?.ToLower() == "promocion")
                {
                    puntosCalculados += producto.SubTotal; // Puntos extra para productos en promoción
                }
            }

            // Redondear puntos a 2 decimales
            return (int)Math.Round(puntosCalculados);
        }
    }
}