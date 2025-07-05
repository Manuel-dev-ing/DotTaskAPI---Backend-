using System;
using System.Collections.Generic;

namespace DotTaskAPI.Entidades;

public partial class Usuario
{
    public int Id { get; set; }

    public int? IdRol { get; set; }

    public string? Nombre { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool? Confirmado { get; set; }

    public virtual Rol? IdRolNavigation { get; set; }

    public virtual ICollection<Nota> Nota { get; set; } = new List<Nota>();

    public virtual ICollection<ProyectosUsuario> ProyectosUsuarios { get; set; } = new List<ProyectosUsuario>();

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
}
