# Configuración del entorno de desarrollo .NET MAUI en macOS (Android)

Este entorno está preparado exclusivamente para compilar y probar aplicaciones móviles Android usando .NET MAUI en macOS.

## Requisitos previos

- macOS 12 o superior  
- .NET 9 SDK  
- Visual Studio Code o Visual Studio para Mac  
- Android Studio  

---

## Instalación paso a paso

### 1. Instalar .NET 9 SDK

```bash
brew install --cask dotnet-sdk
```

O descarga desde: <https://dotnet.microsoft.com/download/dotnet/9.0>

### 2. Instalar Visual Studio Code (opcional)

Descarga desde: <https://code.visualstudio.com/>

Instala extensiones:

- C# (OmniSharp)
- .NET MAUI

### 3. Instalar Android Studio

Descarga desde: <https://developer.android.com/studio>

Durante la instalación, asegúrate de incluir:

- Android SDK  
- Android SDK Platform 31 o superior  
- Android Emulator  
- Android Virtual Device (AVD) Manager  

### 4. Configurar variable de entorno del SDK de Android

```bash
echo 'export ANDROID_SDK_ROOT=$HOME/Library/Android/sdk' >> ~/.zshrc
source ~/.zshrc
```

Verifica:

```bash
echo $ANDROID_SDK_ROOT
```

### 5. Crear y lanzar el emulador Android

Crear AVD en Android Studio → Tools > Device Manager:

1. Click en **Create Device**  
2. Elige **Pixel 5** (u otro modelo)  
3. Selecciona una imagen de sistema:
   - API Level 31+ (recomendado API 33)
   - ABI = ARM64-v8a  
4. Finaliza y presiona ▶️ para arrancar el emulador  

Verifica con:

```bash
adb devices
```

---

## Integración MAUI

### 6. Instalar workload de MAUI

```bash
dotnet workload install maui
```

### 7. Restaurar y compilar el proyecto

```bash
cd ServiPuntos.Mobile
dotnet restore
dotnet build
```

### 8. Ejecutar en Android (emulador conectado)

```bash
dotnet run -f net9.0-android
```

Este comando:

1. Construye la APK  
2. La despliega e instala en el emulador  
3. Lanza la app (MainPage) automáticamente  

### (Opcional) Publicar y desplegar manualmente

```bash
dotnet publish -c Debug -o apk_out -f net9.0-android
adb install -r apk_out/ServiPuntos.Mobile.apk
```

### Logs en tiempo real

```bash
adb logcat --regex "ServiPuntos.Mobile" --info
```

---

## Buenas prácticas

- **Build vs Run**: `dotnet run` internamente hace `build`. Para verificar únicamente compilación:

  ```bash
  dotnet build -c Debug -f net9.0-android
  ```

- **Iteraciones rápidas**: cuando sólo cambias UI o recursos, `dotnet run` reutiliza artefactos previos para acelerar despliegues.
- **Depuración de fallos**: si un `run` arroja error críptico, separa:

  ```bash
  dotnet build
  dotnet run -f net9.0-android
  ```
