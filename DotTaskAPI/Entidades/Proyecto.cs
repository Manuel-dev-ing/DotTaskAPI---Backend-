using System;
using System.Collections.Generic;

namespace DotTaskAPI.Entidades;

public partial class Proyecto
{
    public int Id { get; set; }

    public int? Manager { get; set; }

    public string? NombreProyecto { get; set; }

    public string? NombreCliente { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

    public virtual ICollection<Usuario> Team { get; set; } = new List<Usuario>();
}
