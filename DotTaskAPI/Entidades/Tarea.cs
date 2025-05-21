using System;
using System.Collections.Generic;

namespace DotTaskAPI.Entidades;

public partial class Tarea
{
    public int Id { get; set; }

    public int? IdProyecto { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public string? Estado { get; set; }

    public virtual Proyecto? IdProyectoNavigation { get; set; }
}
