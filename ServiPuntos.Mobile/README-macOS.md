# ServiPuntos.Mobile – Guía de Proyecto (macOS)

## Estructura actual

- **Views/**: Vistas (pantallas) en XAML + code-behind.
- **ViewModels/**: Lógica de presentación (MVVM), ViewModels asociados a cada vista.
- **Models/**: Entidades simples (como TenantConfig).
- **App.xaml.cs**: EntryPoint de la app, define la pantalla principal.
- **Resources/**: Imágenes, fuentes y estilos.

## ¿Qué se ha implementado?

- **Pantalla de selección de Tenant** (multi-cadena):
  - Permite elegir entre las distintas cadenas (Ancap, DUCSA, Petrobras, etc.).
  - Cada tenant muestra su nombre y logo.
- **Arquitectura MVVM básica** para expansión futura.

## ¿Dónde colocar este archivo?

Coloca este archivo en la raíz de tu proyecto móvil:

```
ServiPuntos.Mobile/
    README-macOS.md
    ...
```

## Cómo ejecutar en macOS

### 1. Requisitos previos

- macOS 12 o superior.
- .NET 9 SDK instalado (`brew install --cask dotnet-sdk`).
- Android Studio instalado con emulador configurado (AVD Manager, Pixel 5 API 33 recomendado).
- Herramientas de línea de comandos: `adb`.

### 2. Instalación y ejecución

```bash
# 1. Ir al directorio del proyecto móvil
cd ServiPuntos.Mobile

# 2. Limpiar y compilar en modo Release
dotnet clean
dotnet build -f net9.0-android -c Release

# 3. Iniciar el emulador Android (Pixel 5 API 33)
# Abre Android Studio → Tools → Device Manager → Selecciona Pixel 5 API 33 → Start
# O desde terminal:
emulator -avd "Pixel_5_API_33"

# 4. Verificar que el emulador está conectado
adb devices
# Deberías ver: emulator-5554   device

# 5. Desinstalar versión anterior (opcional, recomendable)
adb uninstall com.companyname.servipuntos.mobile

# 6. Instalar APK generado
adb install -r bin/Release/net9.0-android/com.companyname.servipuntos.mobile-Signed.apk

# 7. Abrir la app manualmente en el emulador
```

### 3. Notas de desarrollo

- Los binarios y carpetas `/bin`, `/obj` están ignorados por git.
- Cualquier cambio de vistas o ViewModels requiere volver a compilar y reinstalar la app.
- El backend aún no está conectado, pero la estructura permite agregar fácilmente llamadas HTTP por tenant.

---
