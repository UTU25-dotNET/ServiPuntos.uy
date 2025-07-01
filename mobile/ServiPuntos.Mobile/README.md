# ServiPuntos.Mobile

App móvil .NET MAUI para **ServiPuntos.uy**. El backend está desplegado en la nube, así que sólo necesitas:

## Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- MAUI workload:

  ```bash
  dotnet workload install maui-android
  ```

- Android SDK (con `adb` en tu PATH)

## Script de despliegue

Crea un fichero `run.sh` dentro de ./ServiPuntos.Mobile con este contenido:

```bash
#!/usr/bin/env bash
set -euo pipefail

# Identificador de la app Android
APP_ID="com.companyname.servipuntos.mobile"
# Carpeta donde se publicará el APK
OUTPUT_DIR="./publish"
APK_FILE="$OUTPUT_DIR/ServiPuntos.Mobile.apk"

echo "🧹 Limpiando bin/obj…"
rm -rf bin obj

echo "📦 Publicando APK (Debug, single-apk)…"
dotnet publish \
  -c Debug \
  -f net9.0-android \
  -o "$OUTPUT_DIR" \
  /p:AndroidUseSharedRuntime=false \
  /p:AndroidPackageFormat=apk \
  /p:PublishAndroidApk=true

echo "🔍 Verificando que $APK_FILE exista…"
if [ ! -f "$APK_FILE" ]; then
  echo "❌ APK no encontrado en $APK_FILE"
  echo "Archivos en $OUTPUT_DIR:"
  find "$OUTPUT_DIR" -maxdepth 1 -type f -name "*.apk"
  exit 1
fi

echo "📱 Desinstalando versión anterior…"
adb uninstall "$APP_ID" || true

echo "📥 Instalando APK…"
adb install -r "$APK_FILE"

echo "🚀 Lanzando la app…"
adb shell monkey -p "$APP_ID" -c android.intent.category.LAUNCHER 1
```

## Paso a paso

1. **Clona el repositorio**  

   ```bash
   git clone
   cd ServiPuntos.uy/mobile/ServiPuntos.Mobile
   ```

2. **Prepara tu entorno**  
   Asegúrate de tener `adb` y un emulador (o dispositivo) corriendo:

   ```bash
   adb devices
   ```

3. **Haz ejecutable el script**  

   ```bash
   chmod +x run.sh
   ```

4. **Ejecuta**  

   ```bash
   ./run.sh
   ```

Con eso tu app móvil quedará instalada y ejecutada en el emulador, conectándose al backend en la nube.
