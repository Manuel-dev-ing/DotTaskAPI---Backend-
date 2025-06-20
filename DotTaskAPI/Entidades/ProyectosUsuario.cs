using System;
using System.Collections.Generic;

namespace DotTaskAPI.Entidades;

public partial class ProyectosUsuario
{
    public int Id { get; set; }

    public int? IdProyecto { get; set; }

    public int? IdUsuario { get; set; }

    public bool? IsManager { get; set; }

    public DateTime? FechaAsignacion { get; set; }

    public virtual Proyecto? IdProyectoNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
