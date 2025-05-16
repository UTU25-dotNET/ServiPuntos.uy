# ServiPuntos.Mobile

Proyecto .NET MAUI Android-only para la app móvil de ServiPuntos.

---

## 🗂 Estructura de carpetas

```
mobile/
└── ServiPuntos.Mobile/
    ├── Platforms/
    │   └── Android/          ← Solo plataforma Android
    ├── Resources/
    │   ├── Fonts/
    │   ├── Images/
    │   ├── Raw/
    │   ├── Splash/
    │   └── Styles/
    ├── App.xaml
    ├── MauiProgram.cs
    └── ServiPuntos.Mobile.csproj  ← TargetFramework net9.0-android
```

> **Nota**: El proyecto se creó en `mobile/ServiPuntos.Mobile`, no en `backend/`.

---

## ✅ Lo realizado hasta ahora

1. **Instalación del workload MAUI**  

   ```bash
   dotnet workload install maui
   ```

2. **Creación del proyecto MAUI Android-only**  

   ```bash
   cd <raíz-del-repo>
   dotnet new maui -n ServiPuntos.Mobile -o mobile/ServiPuntos.Mobile --force
   ```

3. **Limitado a Android**  
   - En `ServiPuntos.Mobile.csproj` reemplazamos:

     ```xml
     <TargetFrameworks>…</TargetFrameworks>
     ```

     por

     ```xml
     <TargetFramework>net9.0-android</TargetFramework>
     ```

   - Eliminadas carpetas `Platforms/iOS`, `Platforms/Windows`, `Platforms/MacCatalyst`, etc.

4. **Integración a la solución**  

   ```bash
   dotnet sln backend/ServiPuntosUY.sln add mobile/ServiPuntos.Mobile/ServiPuntos.Mobile.csproj
   ```

5. **Limpieza y restore**  

   ```bash
   dotnet clean
   dotnet restore
   ```

---

## 🛠️ Levantar emulador y desplegar

1. **Arranca tu emulador Android** (por ejemplo en Android Studio, Pixel 5 API 33).

2. Verifica que `adb` lo detecte:

   ```bash
   adb devices
   # List of devices attached
   # emulator-5554   device
   ```

3. **Build & Run** desde la raíz de `ServiPuntos.Mobile`:

   ```bash
   cd mobile/ServiPuntos.Mobile
   dotnet run -f net9.0-android
   ```

   > Esto compila la APK y la instala directamente en el emulador.

4. **(Opcional) Instalar manualmente**:

   ```bash
   dotnet publish -c Debug -o apk_out -f net9.0-android
   adb install -r apk_out/ServiPuntos.Mobile.apk
   ```

   - `-r` fuerza la reinstalación si ya existe la app.

5. **Verifica en el emulador**  
   Deberías ver tu app arrancar en `MainPage`.

6. **Logs en tiempo real**:

   ```bash
   adb logcat --regex "ServiPuntos.Mobile" --info
   ```

---

## ⚠️ Warning conocido

Al compilar puede aparecer:

```
warning : An exception occurred while reading the output of '/usr/libexec/java_home -X'. Exception: Root element is missing.
```

No afecta el despliegue ni el runtime en Android.

---

## 🚀 Próximos pasos

- Validar la navegación y UI base.
- Integrar el `HttpClient` apuntando al Web API (`ServiPuntos.WebApp`).
- Empezar a consumir DTOs y servicios ya implementados.
