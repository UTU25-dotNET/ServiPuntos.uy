# Configuración del entorno de desarrollo .NET MAUI en Windows (Android)

Este entorno permite compilar aplicaciones móviles Android con .NET MAUI en Windows.

## Requisitos previos

- Windows 10/11 (64 bits)  
- .NET 9 SDK  
- Visual Studio 2022 (17.6 o superior)  
- Android Studio (opcional pero recomendado)  

---

## Instalación paso a paso

### 1. Instalar .NET 9 SDK

Descarga desde: <https://dotnet.microsoft.com/download/dotnet/9.0>

### 2. Instalar Visual Studio 2022

Descarga desde: <https://visualstudio.microsoft.com/>

Durante la instalación, marca los workloads:

- **.NET Multi-platform App UI development**  
- **Mobile development with .NET** (Android)  

### 3. Configurar emulador Android

#### Opción A: Android Device Manager en Visual Studio

1. Tools > Android > Android Device Manager  
2. Create > New Device  
3. Modelo: Pixel 5  
4. Imagen: API Level 31+ (ABI: x86_64 o ARM64)  
5. Finish > Play para arrancar  

#### Opción B: Android Studio Device Manager

Permite más opciones y mejor rendimiento en M1. Similar a macOS.

### 4. Verificar entorno

Abre **Developer Command Prompt for VS 2022** y ejecuta:

```powershell
dotnet workload install maui
dotnet workload restore
dotnet build
```

---

## Integración MAUI

### 5. Restaurar y construir proyecto

```powershell
cd ServiPuntos.Mobile
dotnet restore
dotnet build
```

### 6. Ejecutar en Android (emulador conectado)

```powershell
dotnet run -f net9.0-android
```

Alternativamente, en Visual Studio:

- Abre la solución  
- Selecciona el AVD en la lista de dispositivos  
- Presiona **Run**  

### (Opcional) Publicar y desplegar manualmente

```powershell
dotnet publish -c Debug -o apk_out -f net9.0-android
adb install -r apk_out\ServiPuntos.Mobile.apk
```

### Logs en tiempo real

```powershell
adb logcat --regex "ServiPuntos.Mobile" --info
```

---

## Buenas prácticas

- **Build vs Run**: a veces conviene separar:
  - `dotnet build -c Debug -f net9.0-android`  
  - `dotnet run -f net9.0-android`  
- **Iteraciones rápidas**: `run` reutiliza artefactos y acelera despliegues.
- **Depuración**: aislar build y deploy ayuda a entender errores.

---

✔️ **Recomendación**: Instala Android Studio aunque trabajes en VS. Facilita:

- Gestión de SDK y licencias  
- Creación de AVD  
- Comprobación de herramientas nativas  
