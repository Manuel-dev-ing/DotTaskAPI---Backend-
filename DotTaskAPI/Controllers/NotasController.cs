using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using DotTaskAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace DotTaskAPI.Controllers
{

    [ApiController]
    [Route("api/{proyectoId:int}/tareas/{tareaId:int}/notas")]
    [Authorize]
    public class NotasController: ControllerBase
    {
        private readonly IRepositorioNotas repositorioNotas;
        private readonly IRepositorioUsuarios repositorioUsuarios;
        private readonly IRepositorioTareas repositorioTareas;

        public NotasController(IRepositorioNotas repositorioNotas, IRepositorioUsuarios repositorioUsuarios, IRepositorioTareas repositorioTareas)
        {
            this.repositorioNotas = repositorioNotas;
            this.repositorioUsuarios = repositorioUsuarios;
            this.repositorioTareas = repositorioTareas;
        }

        //obtiene las notas que pertenecen a una tarea
        [HttpGet("obtenerNotasTareas")]
        public async Task<ActionResult<List<NotaDTO>>> obtenerNotas(int tareaId)
        {
            if (tareaId <= 0)
            {
                return BadRequest($"El id: {tareaId} no es valido");
            }

            var tarea_resultado = await repositorioTareas.obtieneTareaPorId(tareaId);
            if (tarea_resultado == null)
            {
                return NotFound();
            }

            var notasDTO = await repositorioNotas.obtenerNotasTareas(tareaId);

            return notasDTO;

        }


        [HttpGet("{id:int}", Name = "ObtenerNota")]
        public async Task<ActionResult<NotaDTO>> get(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"El id: {id} no es valido");
            }

            var nota = await repositorioNotas.obtenerNotaPorId(id);

            if (nota == null)
            {
                return NotFound();
            }

            var notaDTO = new NotaDTO()
            {
                Id = nota.Id,
                IdTarea = (int)nota.IdTarea,
                Contenido = nota.Contenido,
                CreadoPor = (int)nota.CreadoPor
            };

            return notaDTO;

        }


        [HttpPost]
        public async Task<ActionResult> post(int tareaId, int proyectoId, NotasCreacionDTO notasCreacion)
        {
            var manager = await repositorioUsuarios.obtenerInformacionJWT();
            var manager_id = int.Parse(manager!);

            var nota = new Nota()
            {
                IdTarea = tareaId,
                Contenido = notasCreacion.Contenido,
                CreadoPor = manager_id
            };

            await repositorioNotas.guardar(nota);

            var notaDTO = new NotaDTO()
            {
                Id = nota.Id,
                IdTarea = (int)nota.IdTarea,
                Contenido = nota.Contenido,
                CreadoPor = (int)nota.CreadoPor
            };

            return CreatedAtRoute("ObtenerNota", new { id = nota.Id, tareaId, proyectoId }, notaDTO);

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> delete(int id)
        {

            var manager = await repositorioUsuarios.obtenerInformacionJWT();
            var manager_id = int.Parse(manager!);

            var nota = await repositorioNotas.obtenerNotaPorId(id);

            if (nota == null)
            {
                return NotFound("Nota no encontrada");    
            }

            var creadoPor = (int)nota.CreadoPor;

            if (creadoPor != manager_id)
            {
                return NotFound("Accion no valida");
            }

            repositorioNotas.eliminar(nota);

            await repositorioNotas.guardarCambios();

            return NoContent();
        }
        

    }
}
