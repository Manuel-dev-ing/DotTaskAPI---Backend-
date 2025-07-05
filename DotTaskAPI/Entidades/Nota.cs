using System;
using System.Collections.Generic;

namespace DotTaskAPI.Entidades;

public partial class Nota
{
    public int Id { get; set; }

    public int? IdTarea { get; set; }

    public string? Contenido { get; set; }

    public int? CreadoPor { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Tarea? IdTareaNavigation { get; set; }
}
