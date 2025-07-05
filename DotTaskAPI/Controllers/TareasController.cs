using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using DotTaskAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotTaskAPI.Controllers
{
    [ApiController]
    [Route("api/proyectos/{proyectoId:int}/tareas")]
    public class TareasController : ControllerBase
    {
        private readonly IRepositorioTareas repositorioTareas;
        private readonly IRepositorioUsuarios repositorioUsuarios;
        private readonly IRepositorioHistorialCambios repositorioHistorialCambios;
        private readonly IRepositorioNotas repositorioNotas;

        public TareasController(IRepositorioTareas repositorioTareas, IRepositorioUsuarios repositorioUsuarios, IRepositorioHistorialCambios repositorioHistorialCambios, IRepositorioNotas repositorioNotas)
        {
            this.repositorioTareas = repositorioTareas;
            this.repositorioUsuarios = repositorioUsuarios;
            this.repositorioHistorialCambios = repositorioHistorialCambios;
            this.repositorioNotas = repositorioNotas;
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

            if (tarea_resultado is null)
            {
                return NotFound();
            }

            if (proyectoId != tarea_resultado.IdProyecto)
            {
                return BadRequest();
            }

            //var tarea = await repositorioTareas.obtieneTareaPorId(id);

           

            return tarea_resultado;
        }

        [HttpPost]
        [Authorize]
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
        [Authorize(Roles = "manager")]

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
        [Authorize]    
        public async Task<ActionResult> status(int id, int proyectoId, [FromBody] StatusDTO statusDTO)
        {
            var manager = await repositorioUsuarios.obtenerInformacionJWT();
            var manager_id = int.Parse(manager!);

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

            if (statusDTO.Status == "pending")
            {
                tarea_resultado.CompletadoPor = null;
            }
            else
            {
                tarea_resultado.CompletadoPor = manager_id;

            }

            var tarea = new Tarea()
            {
                Id = id,
                IdProyecto = proyectoId,
                Nombre = tarea_resultado.Nombre,
                Descripcion = tarea_resultado.Descripcion,
                Estado = statusDTO.Status,
                CompletadoPor = tarea_resultado.CompletadoPor
            };

            await repositorioTareas.actualizarTarea(tarea);

            var historial = new HistorialCambiosTarea()
            {
                IdTarea = tarea.Id,
                Status = tarea.Estado,
                NombreUsuario = tarea_resultado.NombreUsuario,
                Fecha = DateTime.Now
            };

            await repositorioHistorialCambios.guardar(historial);

            await repositorioTareas.guardarCambios();
            
            return NoContent();
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = "manager")]

        public async Task<ActionResult> delete(int id, int proyectoId)
        {
            var entidad = await repositorioTareas.existeProyecto(proyectoId);

            if (!entidad)
            {
                return NotFound();
            }

            var tarea = await repositorioTareas.obtenerTarea(id);
            if (tarea == null)
            {
                return NotFound();
            }

            var tarea_resultado = await repositorioTareas.obtieneTareaPorId(id);

            if (proyectoId != tarea_resultado.IdProyecto)
            {
                return BadRequest();
            }

            var nota = await repositorioNotas.obtenerNotaPorIdTarea(tarea.Id);
            if (nota != null)
            {
                await repositorioNotas.eliminar(nota);
            }

            repositorioTareas.eliminarTarea(tarea);

            await repositorioTareas.guardarCambios();

            return NoContent();
        }

    }
}
