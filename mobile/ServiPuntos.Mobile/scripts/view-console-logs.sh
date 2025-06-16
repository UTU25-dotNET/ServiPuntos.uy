#!/bin/bash

# Script para ver SOLO los Console.WriteLine de la aplicación MAUI
# Configurar variables de entorno
export ANDROID_HOME=/opt/homebrew/share/android-commandlinetools
export PATH=$PATH:$ANDROID_HOME/platform-tools

echo "📝 Mostrando logs de ServiPuntos Mobile..."
echo "💡 Presiona Ctrl+C para salir"
echo "" 

# Verificar si hay un emulador ejecutándose
if ! adb devices | grep -q "device$"; then
    echo "⚠️  No se detectó ningún emulador ejecutándose."
    exit 1
fi

echo "🔍 Filtrando logs de ServiPuntos:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Filtrar SOLO los logs de DOTNET (que incluyen nuestros AppLogger logs)
adb logcat -v time | grep "DOTNET" | grep -v "^--------- beginning of"

