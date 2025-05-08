/*Esta interfaz se usa cuando querés resolver el TenantId solo una vez al comienzo de la request (por ejemplo en un middleware), y luego accederlo desde cualquier parte sin tener que volver a buscarlo.

👉 Cuándo usarla:
Si querés resolver el tenant una vez por request (mejor performance).

Si vas a usarlo desde muchas clases sin repetir lógica.

Si querés simularlo en tests o inyectarlo desde otro lugar.*/

public interface ITenantContext
{
    Guid TenantId { get; set; }
}

