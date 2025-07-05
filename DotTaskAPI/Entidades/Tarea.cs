using System;
using System.Collections.Generic;

namespace DotTaskAPI.Entidades;

public partial class Tarea
{
    public int Id { get; set; }

    public int? IdProyecto { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public int? CompletadoPor { get; set; }

    public string? Estado { get; set; }

    public virtual Usuario? CompletadoPorNavigation { get; set; }

    public virtual ICollection<HistorialCambiosTarea> HistorialCambiosTareas { get; set; } = new List<HistorialCambiosTarea>();

    public virtual Proyecto? IdProyectoNavigation { get; set; }

    public virtual ICollection<Nota> Nota { get; set; } = new List<Nota>();
}
