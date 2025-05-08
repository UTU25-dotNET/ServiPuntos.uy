/*Esta interfaz se usa para obtener el TenantId directamente desde el contexto HTTP o el token JWT, cada vez que lo necesitás.

👉 Cuándo usarla:
Si querés acceso directo al TenantId en tiempo real.

Si no necesitás guardar el tenant en memoria por request.*/

public interface ITenantResolver
{
    Guid GetCurrentTenantId();
}

