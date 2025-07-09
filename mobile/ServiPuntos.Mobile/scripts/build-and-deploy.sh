#!/bin/bash

# Script para construir y desplegar la app MAUI en el emulador
# Configurar variables de entorno
export ANDROID_HOME=/opt/homebrew/share/android-commandlinetools
export PATH=$PATH:$ANDROID_HOME/cmdline-tools/latest/bin:$ANDROID_HOME/platform-tools:$ANDROID_HOME/emulator

echo "🔨 Construyendo aplicación MAUI para Android..."

# Verificar si hay un emulador ejecutándose
if ! adb devices | grep -q "device$"; then
    echo "⚠️  No se detectó ningún emulador. Ejecuta './scripts/start-emulator.sh' primero."
    exit 1
fi

echo "⚙️  Construyendo para Android..."
cd ..
dotnet clean
sleep 5
dotnet build -f net9.0-android

if [ $? -eq 0 ]; then
    echo "✅ Construcción exitosa!"
    echo "🚀 Publicando aplicación..."
    dotnet publish -f net9.0-android -c Debug
    
    if [ $? -eq 0 ]; then
        echo "✅ Publicación exitosa!"
        
        # Buscar el APK generado
        APK_PATH=$(find ./bin/Debug/net9.0-android -name "*-Signed.apk" | head -1)
        
        if [ -z "$APK_PATH" ]; then
            APK_PATH=$(find ./bin/Debug/net9.0-android -name "*.apk" | head -1)
        fi
        
        if [ -n "$APK_PATH" ]; then
            echo "📦 Instalando APK: $APK_PATH"
            adb install -r "$APK_PATH"
            
            if [ $? -eq 0 ]; then
                echo "✅ ¡Aplicación instalada exitosamente!"
                echo "🚀 Intentando abrir la aplicación..."
                
                # Intentar abrir la aplicación usando diferentes métodos
                adb shell am start -a android.intent.action.MAIN -c android.intent.category.LAUNCHER com.companyname.servipuntos.mobile || \
                adb shell monkey -p com.companyname.servipuntos.mobile -c android.intent.category.LAUNCHER 1 || \
                echo "⚠️  La aplicación está instalada pero no se pudo abrir automáticamente. Ábrela manualmente desde el menú del emulador."
                
                echo "📱 ¡Listo! La aplicación debería estar ejecutándose en el emulador."
            else
                echo "❌ Error al instalar la aplicación."
                exit 1
            fi
        else
            echo "❌ No se encontró el archivo APK."
            exit 1
        fi
    else
        echo "❌ Error al publicar la aplicación."
        exit 1
    fi
else
    echo "❌ Error en la construcción."
    exit 1
fi

