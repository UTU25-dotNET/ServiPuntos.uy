#!/bin/bash

# Script para ver logs de la aplicación MAUI
# Configurar variables de entorno
export ANDROID_HOME=/opt/homebrew/share/android-commandlinetools
export PATH=$PATH:$ANDROID_HOME/platform-tools

echo "📱 Mostrando logs de ServiPuntos Mobile..."
echo "💡 Presiona Ctrl+C para salir"
echo "" 

# Verificar si hay un emulador ejecutándose
if ! adb devices | grep -q "device$"; then
    echo "⚠️  No se detectó ningún emulador ejecutándose."
    exit 1
fi

echo "🔍 Filtrando logs por:"
echo "   - Console.WriteLine (mono-stdout)"
echo "   - Errores de la app"
echo "   - Logs de .NET/Mono"
echo "   - Tu aplicación (servipuntos)"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Filtrar logs relevantes para MAUI/Xamarin
adb logcat -v time | grep -E "(mono-stdout|MonoDroid|servipuntos|Console|System\.Console|Xamarin|Microsoft\.Maui)"

