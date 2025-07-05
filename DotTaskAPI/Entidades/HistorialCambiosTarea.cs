using System;
using System.Collections.Generic;

namespace DotTaskAPI.Entidades;

public partial class HistorialCambiosTarea
{
    public int Id { get; set; }

    public int? IdTarea { get; set; }

    public string? Status { get; set; }

    public string? NombreUsuario { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual Tarea? IdTareaNavigation { get; set; }
}
