public class CanjeSummaryDto
{
    public Guid Id { get; set; }
    public string Producto { get; set; }
    public string Ubicacion { get; set; }
    public DateTime FechaGeneracion { get; set; }
    public DateTime? FechaCanje { get; set; }
    public string Estado { get; set; }
    public int Puntos { get; set; }
    public string CodigoQR { get; set; }
}
public class CanjeListResponseDto
{
    public List<CanjeSummaryDto> Items { get; set; }
    public Guid? NextCursor { get; set; }
}
