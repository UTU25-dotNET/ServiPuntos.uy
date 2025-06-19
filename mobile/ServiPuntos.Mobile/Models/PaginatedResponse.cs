using System;
using System.Collections.Generic;

namespace ServiPuntos.Mobile.Models;

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public Guid? NextCursor { get; set; }
}
