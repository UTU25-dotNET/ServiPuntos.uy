public class ProductoCanjeable
{
    public string Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int CostoEnPuntos { get; set; }
    public List<DisponibilidadPorUbicacion> DisponibilidadesPorUbicacion { get; set; }
}

public class DisponibilidadPorUbicacion
{
    public string UbicacionId { get; set; }
    public int StockDisponible { get; set; }
    public bool Activo { get; set; }
}
