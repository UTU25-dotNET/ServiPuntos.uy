using Microsoft.Extensions.Configuration;
using ServiPuntos.Core.DTOs;
using ServiPuntos.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiPuntos.Application.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _baseUrl;
        private string _accessToken;
        private DateTime _tokenExpiry;

        public PayPalService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _clientId = _configuration["PayPal:ClientId"];
            _clientSecret = _configuration["PayPal:ClientSecret"];
            _baseUrl = _configuration["PayPal:BaseUrl"];
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
            {
                return _accessToken;
            }

            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v1/oauth2/token");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error obteniendo token de PayPal: {content}");
            }

            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(content);
            _accessToken = tokenResponse.GetProperty("access_token").GetString();
            var expiresIn = tokenResponse.GetProperty("expires_in").GetInt32();
            _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60); // 1 minuto de margen

            return _accessToken;
        }

        public async Task<TransaccionPayPalDto> CreatePaymentAsync(decimal amount, string currency = "USD", string description = "ServiPuntos Transaction")
        {
            var token = await GetAccessTokenAsync();

            var paymentData = new
            {
                intent = "sale",
                payer = new { payment_method = "paypal" },
                transactions = new[]
                {
                    new
                    {
                        amount = new
                        {
                            total = amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
                            currency = currency
                        },
                        description = description
                    }
                },
                redirect_urls = new
                {
                    return_url = _configuration["PayPal:ReturnUrl"],
                    cancel_url = _configuration["PayPal:CancelUrl"]
                }
            };

            var json = JsonSerializer.Serialize(paymentData);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v1/payments/payment");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error creando pago en PayPal: {content}");
            }

            var paymentResponse = JsonSerializer.Deserialize<JsonElement>(content);

            return new TransaccionPayPalDto
            {
                PaymentId = paymentResponse.GetProperty("id").GetString(),
                Status = paymentResponse.GetProperty("state").GetString(),
                Amount = amount,
                Currency = currency,
                CreatedTime = DateTime.Parse(paymentResponse.GetProperty("create_time").GetString()),
                AdditionalData = new Dictionary<string, object>
                {
                    { "approval_url", GetApprovalUrl(paymentResponse) },
                    { "full_response", content }
                }
            };
        }

        public async Task<TransaccionPayPalDto> ExecutePaymentAsync(string paymentId, string payerId)
        {
            var token = await GetAccessTokenAsync();

            var executeData = new
            {
                payer_id = payerId
            };

            var json = JsonSerializer.Serialize(executeData);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v1/payments/payment/{paymentId}/execute");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error ejecutando pago en PayPal: {content}");
            }

            var paymentResponse = JsonSerializer.Deserialize<JsonElement>(content);

            return new TransaccionPayPalDto
            {
                PaymentId = paymentId,
                PayerId = payerId,
                Status = paymentResponse.GetProperty("state").GetString(),
                AdditionalData = new Dictionary<string, object>
                {
                    { "full_response", content }
                }
            };
        }

        public async Task<TransaccionPayPalDto> GetPaymentDetailsAsync(string paymentId)
        {
            var token = await GetAccessTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/v1/payments/payment/{paymentId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error consultando pago en PayPal: {content}");
            }

            var paymentResponse = JsonSerializer.Deserialize<JsonElement>(content);

             var amountElement = paymentResponse.GetProperty("transactions")[0].GetProperty("amount");
            var amount = decimal.Parse(amountElement.GetProperty("total").GetString(), System.Globalization.CultureInfo.InvariantCulture);
            var currency = amountElement.GetProperty("currency").GetString();

            return new TransaccionPayPalDto
            {
                PaymentId = paymentId,
                Status = paymentResponse.GetProperty("state").GetString(),
                Amount = amount,
                Currency = currency,
                CreatedTime = DateTime.Parse(paymentResponse.GetProperty("create_time").GetString()),
                AdditionalData = new Dictionary<string, object>
                {
                    { "full_response", content }
                }
            };
        }

        public async Task<bool> ValidatePaymentAsync(string paymentId, decimal expectedAmount)
        {
            try
            {
                var paymentDetails = await GetPaymentDetailsAsync(paymentId);

                // Validar que el pago esté completado y el monto sea correcto
                  var statusOk = paymentDetails.Status.Equals("approved", StringComparison.OrdinalIgnoreCase) ||
                               paymentDetails.Status.Equals("completed", StringComparison.OrdinalIgnoreCase) ||
                               paymentDetails.Status.Equals("captured", StringComparison.OrdinalIgnoreCase);

                return statusOk && Math.Abs(paymentDetails.Amount - expectedAmount) < 0.01m;
            }
            catch
            {
                return false;
            }
        }

        private string GetApprovalUrl(JsonElement paymentResponse)
        {
            var links = paymentResponse.GetProperty("links");

            foreach (var link in links.EnumerateArray())
            {
                if (link.GetProperty("rel").GetString() == "approval_url")
                {
                    return link.GetProperty("href").GetString();
                }
            }

            return string.Empty;
        }

         private string GetCallbackUrl(string configKey, string path)
        {
            var url = _configuration[configKey];
            var baseEnv = Environment.GetEnvironmentVariable("API_BASE_URL") ?? _configuration["API_BASE_URL"];

            if (!string.IsNullOrEmpty(baseEnv))
            {
                url = baseEnv.TrimEnd('/') + path;
            }

            return url;
        }
    }
}