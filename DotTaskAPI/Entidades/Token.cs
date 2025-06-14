using System;
using System.Collections.Generic;

namespace DotTaskAPI.Entidades;

public partial class Token
{
    public int Id { get; set; }

    public int? UsuarioId { get; set; }

    public string? Token1 { get; set; }

    public DateTime? FechaExpiracion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
