# 🛠️ Configuración y herramientas útiles para .NET en MacOS con VS Code

Complementos y comandos recomendados para un flujo de trabajo ágil al desarrollar en .NET desde macOS.

---

## ✅ Testing rápido

- Instalar la extensión **.NET Core Test Explorer**: permite visualizar y ejecutar pruebas xUnit, MSTest y NUnit desde la barra lateral.
- Usar `dotnet test --watch` en terminal para ejecutar pruebas automáticamente al guardar cambios.

---

## 🧰 CLI de .NET como eje central

Aprovechá la terminal integrada (`⌃ + \``) para ejecutar comandos clave:

```bash
dotnet new mvc
dotnet ef migrations add Initial
dotnet watch run
```

Además, podés crear tareas personalizadas en `.vscode/tasks.json` para:

| Acción         | Comando          | Atajo recomendado |
|----------------|------------------|-------------------|
| Compilar       | `dotnet build`   | `⇧⌘B`             |
| Ejecutar       | `dotnet run`     | (con botón o terminal) |
| Ejecutar tests | `dotnet test`    | (o desde Test Explorer) |

---

## 🐞 Depuración y Hot Reload

- Configurá un archivo `launch.json` para usar `"type": "coreclr"` y la opción de **.NET Core Launch (web)**.
- Agregá breakpoints en tu código C#.
- Al iniciar desde el botón de depuración (`F5`), VS Code lanzará **Kestrel** y atará el depurador automáticamente.
- Si usás ASP.NET Core 6+, podés activar **Hot Reload**: los cambios en archivos `.cs` se reflejan instantáneamente en el navegador o Swagger UI.

---

> Esta guía mejora tu productividad diaria sin necesidad de Visual Studio completo, especialmente útil en MacOS.
