using Microsoft.Extensions.Logging;

namespace ServiPuntos.Mobile.Services
{
    public static class AppLogger
    {
        private static ILogger? _logger;
        
        public static void Initialize(ILogger logger)
        {
            _logger = logger;
        }
        
        public static void LogInfo(string message)
        {
            var fullMessage = $"[ServiPuntos-INFO] {message}";
            
            // Múltiples formas de logging para asegurar que se vea
            System.Diagnostics.Debug.WriteLine(fullMessage);
            Console.WriteLine(fullMessage);
            _logger?.LogInformation(fullMessage);
            
            // Logging a Android específico con prefijo
#if ANDROID
            try {
                Android.Util.Log.Info("ServiPuntos", message);
            } catch {
                // Fallback si Android.Util.Log no funciona
                System.Diagnostics.Debug.WriteLine($"[ANDROID-INFO] {message}");
            }
#endif
        }
        
        public static void LogError(string message)
        {
            var fullMessage = $"[ServiPuntos-ERROR] {message}";
            
            System.Diagnostics.Debug.WriteLine(fullMessage);
            Console.WriteLine(fullMessage);
            _logger?.LogError(fullMessage);
            
#if ANDROID
            try {
                Android.Util.Log.Error("ServiPuntos", message);
            } catch {
                System.Diagnostics.Debug.WriteLine($"[ANDROID-ERROR] {message}");
            }
#endif
        }
        
        public static void LogDebug(string message)
        {
            var fullMessage = $"[ServiPuntos-DEBUG] {message}";
            
            System.Diagnostics.Debug.WriteLine(fullMessage);
            Console.WriteLine(fullMessage);
            _logger?.LogDebug(fullMessage);
            
#if ANDROID
            try {
                Android.Util.Log.Debug("ServiPuntos", message);
            } catch {
                System.Diagnostics.Debug.WriteLine($"[ANDROID-DEBUG] {message}");
            }
#endif
        }
    }
}

