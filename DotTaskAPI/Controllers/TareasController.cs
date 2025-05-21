using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using DotTaskAPI.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace DotTaskAPI.Controllers
{
    [ApiController]
    [Route("api/proyectos/{proyectoId:int}/tareas")]
    public class TareasController : ControllerBase
    {
        private readonly IRepositorioTareas repositorioTareas;

        public TareasController(IRepositorioTareas repositorioTareas)
        {
            this.repositorioTareas = repositorioTareas;
        }


        //obtener lista de tareas por el id del proyecto
        [HttpGet]
        public async Task<ActionResult<List<TareaDTO>>> get(int proyectoId)
        {


            var entidad = await repositorioTareas.existeProyecto(proyectoId);

            if (!entidad)
            {
                return NotFound();
            }

            var tareasDTO = await repositorioTareas.obtienerTareas();

            return tareasDTO;
        }

        //obtiene una tarea por su id

        [HttpGet("{id:int}", Name = "ObtenerTarea")]
        public async Task<ActionResult<TareaDTO>> Get(int id, int proyectoId)
        {

            if (id == 0)
            {
                return BadRequest($"El id: {id} no es valido");
            }

            var tarea_resultado = await repositorioTareas.obtieneTareaPorId(id);

            if (proyectoId != tarea_resultado.IdProyecto)
            {
                return BadRequest();
            }

            var tarea = await repositorioTareas.obtieneTareaPorId(id);

            if (tarea is null)
            {
                return NotFound();
            }

            return tarea;
        }

        [HttpPost]
        public async Task<ActionResult> post(int proyectoId, TareaCreacionDTO tareaCreacionDTO)
        {
            var entidad = await repositorioTareas.existeProyecto(proyectoId);

            if (!entidad)
            {
                return NotFound();
            }

            var tarea = new Tarea()
            {
                IdProyecto = proyectoId,
                Nombre = tareaCreacionDTO.Nombre,
                Descripcion = tareaCreacionDTO.Descripcion,
                Estado = "pending"
            };

            await repositorioTareas.guardarTarea(tarea);

            var tareaDTO = new TareaDTO()
            {
                Id = tarea.Id,
                IdProyecto = (int)tarea.IdProyecto,
                Nombre = tarea.Nombre,
                Descripcion = tarea.Descripcion,
                Estado = tarea.Estado
            };

            return CreatedAtRoute("ObtenerTarea", new { id = tarea.Id, proyectoId }, tareaDTO);
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> put(int id, int proyectoId, TareaCreacionDTO tareaCreacionDTO)
        {
            if (id <= 0)
            {
                return BadRequest($"El id: {id} no valido");
            }

            var resultado = await repositorioTareas.existeTarea(id);

            if (!resultado)
            {
                return NotFound();
            }

            var tarea_resultado = await repositorioTareas.obtieneTareaPorId(id);

            if (proyectoId != tarea_resultado.IdProyecto)
            {
                return BadRequest();
            }

            var tarea = new Tarea()
            {
                Id = id,
                IdProyecto = proyectoId,
                Nombre = tareaCreacionDTO.Nombre,
                Descripcion = tareaCreacionDTO.Descripcion,
                Estado = tarea_resultado.Estado
            };

            await repositorioTareas.actualizarTarea(tarea);

            return NoContent();
        }


        [HttpPut("{id:int}/status")]
        public async Task<ActionResult> status(int id, int proyectoId, [FromBody] StatusDTO statusDTO)
        {
            if (id <= 0)
            {
                return BadRequest($"El id: {id} no valido");
            }

            var resultado = await repositorioTareas.existeTarea(id);

            if (!resultado)
            {
                return NotFound();
            }

            var tarea_resultado = await repositorioTareas.obtieneTareaPorId(id);

            if (proyectoId != tarea_resultado.IdProyecto)
            {
                return BadRequest();
            }

            var tarea = new Tarea()
            {
                Id = id,
                IdProyecto = proyectoId,
                Nombre = tarea_resultado.Nombre,
                Descripcion = tarea_resultado.Descripcion,
                Estado = statusDTO.Status
            };

            await repositorioTareas.actualizarTarea(tarea);

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> delete(int id, int proyectoId)
        {
            var entidad = await repositorioTareas.existeProyecto(proyectoId);

            if (!entidad)
            {
                return NotFound();
            }
            var tarea_resultado = await repositorioTareas.obtieneTareaPorId(id);

            if (proyectoId != tarea_resultado.IdProyecto)
            {
                return BadRequest();
            }


            var resultado = await repositorioTareas.eliminarTarea(id);

            if (resultado == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
