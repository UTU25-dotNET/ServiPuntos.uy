#!/bin/bash

# Script para iniciar el emulador Android desde VS Code
# Configurar variables de entorno
export ANDROID_HOME=/opt/homebrew/share/android-commandlinetools
export PATH=$PATH:$ANDROID_HOME/cmdline-tools/latest/bin:$ANDROID_HOME/platform-tools:$ANDROID_HOME/emulator

echo "🚀 Iniciando emulador Android Pixel 7 API 33..."

# Verificar si el emulador ya está ejecutándose
if adb devices | grep -q "emulator-"; then
    echo "✅ El emulador ya está ejecutándose:"
    adb devices
else
    echo "📱 Iniciando emulador en segundo plano..."
    emulator -avd Pixel_7_API_33_ARM &
    
    echo "⏳ Esperando que el emulador esté listo..."
    # Esperar hasta que el emulador esté disponible
    while ! adb devices | grep -q "device$"; do
        sleep 2
        echo "   Esperando..."
    done
    
    echo "✅ Emulador listo!"
    adb devices
fi

echo ""
echo "📋 Comandos útiles:"
echo "   - Construir app: dotnet build -f net9.0-android"
echo "   - Publicar app: dotnet publish -f net9.0-android -c Debug"
echo "   - Ver dispositivos: adb devices"
echo "   - Logs del emulador: adb logcat"

