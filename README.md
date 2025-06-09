# ServiPuntos.uy 🛠️

Plataforma tecnológica para programas de fidelización personalizados para cadenas de estaciones de servicio en Uruguay.

## 🧱 Estructura del proyecto

```
ServiPuntos.uy/
├── backend/               # API y backend en .NET
├── frontend-web/          # Aplicación web en React (Create React App) + Bootstrap
├── mobile/                # Aplicación móvil en .NET MAUI
├── docs/                  # Documentación técnica
├── README.md              # Este archivo
└── .gitignore
```

## 📚 Estructura de subproyectos

Este repositorio contiene varios subproyectos. Cada subproyecto tiene su propio README detallado con instrucciones específicas:

| Carpeta | Proyecto | Leer README local |
|:--|:--|:--|
| /backend/ | API en .NET 9 | ✅ |
| /frontend-web/ | Frontend en React (Create React App) | ✅ |
| /mobile/ | App Mobile en .NET MAUI | ✅ |



## 🚀 Cómo comenzar

### 1. Requisitos generales

- [.NET SDK 9.0.202](https://dotnet.microsoft.com/en-us/download)
- Node.js 18+ y npm
- Docker (dependiendo de tu SO, si tenés Windows de momento no hace falta.)
- Visual Studio 2022+ o VS Code
- (Opcional) MAUI workload para trabajar en la app mobile

### 2. Configurar la base de datos

El proyecto utiliza **PostgreSQL** y actualmente la base de datos está desplegada en [Railway](https://railway.app/).
La cadena de conexión por defecto se encuentra en `backend/ServiPuntos.API/appsettings.json`.
Podés sobreescribirla mediante la variable de entorno `ConnectionStrings__DefaultConnection` si necesitás usar una instancia local.
Si preferís levantar PostgreSQL localmente podés utilizar Docker o tu gestor favorito.

### 3. Levantar el backend

```bash
cd backend
dotnet build ServiPuntosUY.sln
dotnet run --project ServiPuntos.API
```

### 4. Levantar el frontend

```bash
cd frontend-web
npm install
npm start
```

### 5. Levantar la app móvil

```bash
cd mobile/ServiPuntos.Mobile
dotnet build
dotnet run
```

> ⚠️ Requiere tener instalado `dotnet workload install maui` si vas a trabajar en la app mobile.

## 🔌 Conexión a PostgreSQL

Ejemplo de cadena de conexión utilizada para desarrollo:

```
Host=shuttle.proxy.rlwy.net;Port=19577;Username=postgres;Password=********;Database=railway;SSL Mode=Require;Trust Server Certificate=true;
```

Podés ajustar los valores según tu entorno local en `appsettings.json` o mediante la variable de entorno `ConnectionStrings__DefaultConnection`.

## ⚙️ Comandos utilizados para generar la estructura inicial

```bash
# Backend
dotnet new sln --name ServiPuntosUY
dotnet new classlib -n ServiPuntos.Core
dotnet new classlib -n ServiPuntos.Infrastructure
dotnet new webapi -n ServiPuntos.API
dotnet new xunit -n ServiPuntos.Tests

dotnet new classlib -n ServiPuntos.Application

## Agregar proyectos a la Solution
dotnet sln backend/ServiPuntosUY.sln add backend/ServiPuntos.Core/ServiPuntos.Core.csproj
dotnet sln backend/ServiPuntosUY.sln add backend/ServiPuntos.Infrastructure/ServiPuntos.Infrastructure.csproj
dotnet sln backend/ServiPuntosUY.sln add backend/ServiPuntos.API/ServiPuntos.API.csproj
dotnet sln backend/ServiPuntosUY.sln add backend/ServiPuntos.Tests/ServiPuntos.Tests.csproj

dotnet sln backend/ServiPuntosUY.sln add backend/ServiPuntos.Application/ServiPuntos.Application.csproj


## Agregar referencias entre proyectos
dotnet add backend/ServiPuntos.API/ServiPuntos.API.csproj reference backend/ServiPuntos.Core/ServiPuntos.Core.csproj
dotnet add backend/ServiPuntos.Infrastructure/ServiPuntos.Infrastructure.csproj reference backend/ServiPuntos.Core/ServiPuntos.Core.csproj
dotnet add backend/ServiPuntos.Tests/ServiPuntos.Tests.csproj reference backend/ServiPuntos.Core/ServiPuntos.Core.csproj

dotnet add backend/ServiPuntos.API/ServiPuntos.API.csproj reference backend/ServiPuntos.Application/ServiPuntos.Application.csproj
dotnet add backend/ServiPuntos.Application/ServiPuntos.Application.csproj reference backend/ServiPuntos.Core/ServiPuntos.Core.csproj

# Mobile
dotnet workload install maui
dotnet new maui -n ServiPuntos.Mobile
dotnet sln backend/ServiPuntosUY.sln add mobile/ServiPuntos.Mobile/ServiPuntos.Mobile.csproj

# Frontend
npx create-react-app frontend-web
cd frontend-web
npm install
npm install bootstrap
```

---

_Proyecto académico para Taller de Sistemas de Información .NET – Edición 2025_
