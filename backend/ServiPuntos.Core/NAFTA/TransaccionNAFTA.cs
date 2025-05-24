using ServiPuntos.Core.Enums;
using System;
using System.Collections.Generic;

namespace ServiPuntos.Core.NAFTA
{
    public class TransaccionNAFTA
    {
        public Guid IdTransaccion { get; set; } = Guid.NewGuid();
        public Guid IdentificadorUsuario { get; set; }
        public DateTime FechaTransaccion { get; set; } = DateTime.UtcNow;
        public TipoTransaccion TipoTransaccion { get; set; }
        public int Monto { get; set; }
        public MetodoPago MetodoPago { get; set; } 

        public decimal MontoPayPal { get; set; }
        public string PayPalPaymentId { get; set; }
        public string PayPalPayerId { get; set; }
        public string PayPalToken { get; set; }

        // Puntos utilizados como descuento (solo para transacciones mixtas)
        public int PuntosUtilizados { get; set; } = 0;
        public List<LineaTransaccionNAFTA> Productos { get; set; } = new List<LineaTransaccionNAFTA>();
        public Dictionary<string, object> DatosAdicionales { get; set; } = new Dictionary<string, object>();

        // Propiedades de conveniencia
        public bool EsTransaccionMixta => PuntosUtilizados > 0;
        public bool EsSoloPayPal => PuntosUtilizados == 0;

        public bool EsValida => MontoPayPal > 0 && (PuntosUtilizados == 0 || MontoPayPal < Monto) && (MontoPayPal + (PuntosUtilizados * ValorPunto)) >= Monto;

        // Valor del punto en dinero (esto debería venir de la configuración del tenant)
        private const decimal ValorPunto = 10m; // Ejemplo: 1 punto = $10 UYU
    }
}