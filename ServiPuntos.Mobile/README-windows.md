# ServiPuntos.Mobile – Guía de Proyecto (Windows)

## Estructura actual

- **Views/**: Pantallas XAML + code-behind.
- **ViewModels/**: Lógica MVVM.
- **Models/**: Entidades como TenantConfig.
- **App.xaml.cs**: Arranque de la app.
- **Resources/**: Estilos e imágenes.

## ¿Dónde colocar este archivo?

Coloca este archivo en la raíz de tu proyecto móvil:

```
ServiPuntos.Mobile/
    README-windows.md
    ...
```

## Estado actual

- Selección visual de tenant con logos y nombre.
- Arquitectura MVVM lista para escalar.

## Requisitos para Windows

- Windows 10 u 11 actualizado.
- .NET 9 SDK instalado.
- Visual Studio 2022 (con soporte MAUI y Android).
- Android Studio y/o Emulador configurado.

## Pasos para ejecutar

```cmd
:: 1. Ubicarse en la carpeta del proyecto
cd ServiPuntos.Mobile

:: 2. Limpiar y compilar en modo Release
dotnet clean
dotnet build -f net9.0-android -c Release

:: 3. Abrir el emulador Android (Pixel 5 API 33 recomendado)
:: Abre Android Studio → Tools → Device Manager → Selecciona Pixel 5 API 33 → Start
:: O desde terminal:
emulator -avd "Pixel_5_API_33"

:: 4. Verificar que el emulador está conectado
adb devices
:: Deberías ver: emulator-5554   device

:: 5. (Opcional) Desinstalar versiones anteriores
adb uninstall com.companyname.servipuntos.mobile

:: 6. Instalar la APK generada
adb install -r bin\Release
et9.0-android\com.companyname.servipuntos.mobile-Signed.apk

:: 7. Abrir la app desde el emulador Android
```

## Flujo típico de desarrollo y despliegue

Si cambias el código fuente, repite:

```cmd
dotnet clean
rd /s /q bin obj
dotnet build -f net9.0-android -c Release
adb uninstall com.companyname.servipuntos.mobile
adb install -r bin\Release
et9.0-android\com.companyname.servipuntos.mobile-Signed.apk
```

## Notas adicionales

- El proyecto ignora binarios en `/bin` y `/obj`.
- No se debe commitear la APK ni la carpeta `bin`.
- El flujo actual es solo selección de tenant; para avances y detalles, ver el PDF de documentación.

---
