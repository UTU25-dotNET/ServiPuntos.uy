public class ProductoUbicacionDto
{
    public Guid UbicacionId { get; set; }
    public Guid ProductoCanjeableId { get; set; }
    public int StockDisponible { get; set; }
    public bool Activo { get; set; } = true;
}
