# ServiPuntos Backend 🛠️

API y backend del sistema ServiPuntos.uy, desarrollado en .NET 9.

## 🚀 Cómo levantar el backend

Desde la raíz del proyecto backend:

```bash
cd backend
dotnet build ServiPuntosUY.sln
dotnet run --project ServiPuntos.API
```

La API estará disponible en [http://localhost:5000](http://localhost:5000) por defecto.

## ⚙️ Variables de entorno

Crea un archivo `.env` en `backend/` basado en `.env.example`:

```env
ASPNETCORE_ENVIRONMENT=Development
JWT_SECRET=tu_clave_super_secreta
SQLSERVER_HOST=localhost
SQLSERVER_PORT=1433
SA_PASSWORD=TuPasswordSegura123
ACCEPT_EULA=Y
MSSQL_PID=Express
API_BASE_URL=http://localhost:5019
FRONTEND_URL=http://localhost:3000
MOCK_VEAI_URL=http://localhost:5050
NAFTA_API_URL=http://localhost:5051
FIREBASE_CREDENTIALS_PATH=/path/al/archivo-service-account.json
```

## 🛠️ Funcionalidades principales

- Gestión de usuarios y programas de fidelización
- Administración multi-tenant
- Integración con SQL Server
- Seguridad JWT
- Comunicación con servicios externos (Mock VEAI, NAFTA API)
- Envío de notificaciones push mediante Firebase Cloud Messaging

## 📢 Notas importantes

- Se utiliza JWT para la autenticación.
- La comunicación entre el frontend, mobile y backend se realiza mediante API REST.
- Toda comunicación debe ser por HTTPS en entornos productivos.

---

_Proyecto académico para Taller de Sistemas de Información .NET – Edición 2025_
