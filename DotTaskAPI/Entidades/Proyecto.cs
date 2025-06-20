using System;
using System.Collections.Generic;

namespace DotTaskAPI.Entidades;

public partial class Proyecto
{
    public int Id { get; set; }

    public string? NombreProyecto { get; set; }

    public string? NombreCliente { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<ProyectosUsuario> ProyectosUsuarios { get; set; } = new List<ProyectosUsuario>();

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
