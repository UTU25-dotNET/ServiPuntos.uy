# ServiPuntos.Mobile 📱

Aplicación móvil multiplataforma para usuarios de ServiPuntos.uy, desarrollada con .NET MAUI.

## 📋 Requisitos

- .NET SDK 9.0.202 o superior
- Workload de MAUI instalado:

  ```bash
  dotnet workload install maui
  ```

- Entorno de desarrollo recomendado:
  - Visual Studio 2022+ (Windows)
  - Visual Studio 2022 for Mac (MacOS)

## 🚀 Cómo ejecutar la app

Desde la carpeta raíz del proyecto:

```bash
cd mobile/ServiPuntos.Mobile
dotnet build
dotnet run
```

También podés abrir la solución `ServiPuntosUY.sln` en Visual Studio y ejecutar el proyecto `ServiPuntos.Mobile`.

## ⚙️ Configuración de la API Base URL

La URL base de la API de ServiPuntos.uy está definida actualmente en:

```
/mobile/ServiPuntos.Mobile/Configuration.cs
```

```csharp
namespace ServiPuntos.Mobile
{
    public static class Configuration
    {
        public const string ApiBaseUrl = "http://localhost:5000";
    }
}
```

> 🔥 Nota: si vas a correr la app en un dispositivo móvil físico, probablemente debas cambiar `localhost` por la IP local de tu máquina de desarrollo.

## 📢 Notas

- Actualmente no se utiliza un `.env` en Mobile.
- La configuración de la API está en una constante para simplificar el desarrollo inicial.
- En etapas avanzadas se puede migrar a configuraciones dinámicas.

⚠️ Nota: Para compilar y ejecutar la app Mobile, se requiere tener instalado un entorno de desarrollo completo para Android y/o iOS. En caso de no contar con los SDKs necesarios, puede que la compilación falle, pero no afecta al desarrollo de backend ni frontend.

---

_Proyecto académico para Taller de Sistemas de Información .NET – Edición 2025_
