
# 🚀 Configuración de SQL Server en MacOS usando Docker

Este documento explica cómo levantar un contenedor de **SQL Server 2022** en **Docker** sobre **MacOS**, y cómo conectarse usando **Visual Studio Code** mediante la extensión oficial **SQL Server (mssql)**.

---

## 1. Requisitos previos

- Tener [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y corriendo.
- Tener [Visual Studio Code](https://code.visualstudio.com/) instalado.
- Instalar en VS Code la extensión "**SQL Server (mssql)**" (publicada por Microsoft).

---

## 2. Crear y levantar el contenedor de SQL Server (solo la primera vez)

En una terminal, ejecutar:

```bash
docker run --platform linux/amd64 -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=TuPasswordSegura123' -p 1433:1433 --name sqlserver2022 -d mcr.microsoft.com/mssql/server:2022-latest
```

**Explicación rápida:**

- `--platform linux/amd64`: fuerza la emulación correcta en Mac con procesadores Apple Silicon (M1/M2/M3).
- `-e 'ACCEPT_EULA=Y'`: acepta el acuerdo de licencia.
- `-e 'SA_PASSWORD=TuPasswordSegura123'`: define la contraseña del usuario `sa` (mínimo 8 caracteres, una mayúscula, una minúscula, un número).
- `-p 1433:1433`: expone el puerto 1433 en `localhost`.
- `--name sqlserver2022`: nombre del contenedor.
- `-d`: corre el contenedor en segundo plano (background).

---

## 3. Flujo de trabajo diario

### 3.1 Iniciar el contenedor

Cada vez que quieras trabajar:

```bash
docker start sqlserver2022
```

Verificar que esté corriendo:

```bash
docker ps
```

Deberías ver algo similar a:

```bash
CONTAINER ID   IMAGE                                    STATUS          PORTS                    NAMES
xxxxxx         mcr.microsoft.com/mssql/server:2022-latest   Up X minutes   0.0.0.0:1433->1433/tcp   sqlserver2022
```

---

### 3.2 Conectarte a SQL Server desde VS Code

En Visual Studio Code:

1. Abrir la paleta de comandos (`Ctrl+Shift+P` o `⇧⌘P`).
2. Buscar "**SQL: Connect**".
3. Completar los datos de conexión:
   - **Server name**: `localhost,1433`
   - **Authentication Type**: `SQL Login`
   - **User name**: `sa`
   - **Password**: `TuPasswordSegura123`
   - **Database**: (opcional) `master` o la base que desees usar.
4. Guardar la conexión si querés usarla fácilmente en el futuro.

---

### 3.3 Trabajar con archivos `.sql`

- Crear o abrir archivos `.sql` en VS Code.
- Seleccionar la conexión activa.
- Escribir y ejecutar consultas (`SELECT`, `INSERT`, `UPDATE`, `DELETE`, `CREATE TABLE`, etc.).
- Ver resultados directamente en VS Code.

---

### 3.4 Detener el contenedor

Cuando termines de trabajar:

```bash
docker stop sqlserver2022
```

---

## 4. Comandos útiles de Docker

| Acción                   | Comando                            |
|---------------------------|------------------------------------|
| Ver contenedores activos  | `docker ps`                        |
| Iniciar contenedor        | `docker start sqlserver2022`        |
| Detener contenedor        | `docker stop sqlserver2022`         |
| Eliminar contenedor       | `docker rm -f sqlserver2022`        |

---

## 5. Notas importantes

- **Persistencia de datos**:  
  Este contenedor **no** guarda los datos si se elimina.  
  Para persistencia real, se recomienda montar un volumen de Docker.

- **Seguridad**:  
  Para entornos de producción, es obligatorio cambiar `SA_PASSWORD` por una contraseña fuerte.

---

## 6. Opcional: Automatizar con Docker Compose

Crear un archivo `docker-compose.yml`:

```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: sqlserver2022
    ports:
      - "1433:1433"
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=TuPasswordSegura123
    platform: linux/amd64
```

Después, levantar todo con:

```bash
docker-compose up -d
```

Y apagarlo con:

```bash
docker-compose down
```

---

## 7. Comandos básicos para usar la extensión "SQL Server (mssql)" en VS Code

| Acción                          | Comando en VS Code                | Descripción                                     |
|----------------------------------|-----------------------------------|-------------------------------------------------|
| Ejecutar consulta actual        | `Ctrl+Shift+E` / `⇧⌘E`             | Ejecuta solo la consulta donde está el cursor.  |
| Ejecutar todas las consultas del archivo | `Ctrl+Alt+E` / `⌥⌘E`      | Ejecuta todo el contenido del archivo `.sql`.   |
| Conectarse a un servidor        | `Ctrl+Shift+P` → `SQL: Connect`   | Iniciar o cambiar conexión a SQL Server.        |
| Cambiar base de datos activa    | `Ctrl+Shift+P` → `SQL: Use Database` | Cambiar la base de datos en una conexión existente. |
| Desconectarse del servidor      | `Ctrl+Shift+P` → `SQL: Disconnect` | Cerrar conexión activa con el servidor.         |

---

## 📋 Resumen rápido

1. `docker start sqlserver2022`
2. Conectarse desde VS Code con la extensión "SQL Server (mssql)".
3. Trabajar en archivos `.sql`.
4. `docker stop sqlserver2022` (opcional al terminar).

---

# 🎯 Notas finales

- El contenedor usa plataforma `linux/amd64`, necesario en Macs Apple Silicon (M1/M2/M3).
- La conexión se hace por `localhost,1433` usando el usuario `sa`.
- Este setup es para entornos de **desarrollo local**. No recomendado para producción tal cual está.
