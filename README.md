# ServiPuntos.uy 🛠️

Plataforma tecnológica para programas de fidelización personalizados para cadenas de estaciones de servicio en Uruguay.

## 🧱 Estructura del proyecto

```
ServiPuntos.uy/
├── backend/               # API y backend en .NET
├── frontend-web/          # Aplicación web en React + Vite + Bootstrap
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
| /frontend-web/ | Frontend en React + Vite | ✅ |
| /mobile/ | App Mobile en .NET MAUI | ✅ |

> 📢 Importante: Antes de iniciar cualquier componente, revisá el README correspondiente para asegurarte de seguir los pasos específicos.

## 🚀 Cómo comenzar

### 1. Requisitos generales

- [.NET SDK 9.0.202](https://dotnet.microsoft.com/en-us/download)
- Node.js 18+ y npm
- Docker (dependiendo de tu SO, si tenés Windows de momento no hace falta.)
- Visual Studio 2022+ o VS Code
- (Opcional) MAUI workload para trabajar en la app mobile

### 2. Configurar la base de datos

Dependiendo de tu sistema operativo:

- [docs/dev/setup/macos/docker-sqlserver.md](docs/dev/setup/macos/docker-sqlserver.md): Para MacOS usando Docker
- docs/dev/setup/windows/sqlserver.md: Para Windows usando SSMS o similar

Esto permitirá conectar a SQL Server en localhost:1433.

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
npm run dev
```

### 5. Levantar la app móvil

```bash
cd mobile/ServiPuntos.Mobile
dotnet build
dotnet run
```

> ⚠️ Requiere tener instalado `dotnet workload install maui` si vas a trabajar en la app mobile.

## 🔌 Conexión a SQL Server

```
Server name: localhost,1433 (Así nadie tiene que preguntar "¿en qué puerto está expuesto?" porque ya queda explícito.)
Authentication Type: SQL Login
User name: sa
Password: TuPasswordSegura123 (o la de tu .env)
Database: master (o la que crees)
```

## ⚙️ Comandos utilizados para generar la estructura inicial

```bash
# Backend
dotnet new sln --name ServiPuntosUY
dotnet new classlib -n ServiPuntos.Core
dotnet new classlib -n ServiPuntos.Infrastructure
dotnet new webapi -n ServiPuntos.API
dotnet new xunit -n ServiPuntos.Tests

## Agregar proyectos a la Solution
dotnet sln backend/ServiPuntosUY.sln add backend/ServiPuntos.Core/ServiPuntos.Core.csproj
dotnet sln backend/ServiPuntosUY.sln add backend/ServiPuntos.Infrastructure/ServiPuntos.Infrastructure.csproj
dotnet sln backend/ServiPuntosUY.sln add backend/ServiPuntos.API/ServiPuntos.API.csproj
dotnet sln backend/ServiPuntosUY.sln add backend/ServiPuntos.Tests/ServiPuntos.Tests.csproj

## Agregar referencias entre proyectos
dotnet add backend/ServiPuntos.API/ServiPuntos.API.csproj reference backend/ServiPuntos.Core/ServiPuntos.Core.csproj
dotnet add backend/ServiPuntos.Infrastructure/ServiPuntos.Infrastructure.csproj reference backend/ServiPuntos.Core/ServiPuntos.Core.csproj
dotnet add backend/ServiPuntos.Tests/ServiPuntos.Tests.csproj reference backend/ServiPuntos.Core/ServiPuntos.Core.csproj

# Mobile
dotnet workload install maui
dotnet new maui -n ServiPuntos.Mobile
dotnet sln backend/ServiPuntosUY.sln add mobile/ServiPuntos.Mobile/ServiPuntos.Mobile.csproj

# Frontend
npm create vite@latest frontend-web -- --template react-ts
cd frontend-web
npm install
npm install bootstrap
```

---

_Proyecto académico para Taller de Sistemas de Información .NET – Edición 2025_
