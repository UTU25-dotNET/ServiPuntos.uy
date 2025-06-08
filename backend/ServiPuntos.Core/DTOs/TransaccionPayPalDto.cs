using System;
using System.Collections.Generic;

namespace ServiPuntos.Core.DTOs
{
    public class TransaccionPayPalDto
    {
        public string PaymentId { get; set; }
        public string PayerId { get; set; }
        public string Token { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD"; // PayPal sandbox usa USD
        public string Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }
}