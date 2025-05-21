using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using DotTaskAPI.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace DotTaskAPI.Controllers
{
    [ApiController]
    [Route("api/proyectos")]
    public class ProyectosController : ControllerBase
    {
        private readonly IRepositorioProyectos repositorioProyectos;

        public ProyectosController(IRepositorioProyectos repositorioProyectos)
        {
            this.repositorioProyectos = repositorioProyectos;
        }

        [HttpGet]
        public async Task<IEnumerable<ProyectoDTO>> get()
        {

            var proyectosDTO = await repositorioProyectos.obtenerProyectos();

            return proyectosDTO;
        }

        [HttpGet("{id:int}", Name = "ObtenerProyecto")]
        public async Task<ActionResult<ProyectoDTO>> get(int id)
        {

            if (id == 0)
            {
                return BadRequest($"El id: {id} no valido");
            }

            var proyectoDTO = await repositorioProyectos.ObtenerProyectoPorId(id);

            if (proyectoDTO is null)
            {
                return BadRequest($"El proyecto con el id: {id} no existe");
            }

            return proyectoDTO;

        }


        [HttpPost]
        public async Task<ActionResult> post([FromBody] ProyectosCreacionDTO proyectosCreacionDTO)
        {

            var proyecto = new Proyecto()
            {
                NombreProyecto = proyectosCreacionDTO.NombreProyecto,
                NombreCliente = proyectosCreacionDTO.NombreCliente,
                Descripcion = proyectosCreacionDTO.Descripcion
            };

            var proyectoDTO = await repositorioProyectos.guardarProyecto(proyecto);

            return CreatedAtRoute("ObtenerProyecto", new { id = proyectoDTO.Id }, proyectoDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> put(int id, ProyectosCreacionDTO proyectosCreacionDTO)
        {
            if (id == 0)
            {
                return BadRequest($"El id: {id} no valido");
            }

            var proyectoDTO = await repositorioProyectos.ObtenerProyectoPorId(id);

            if (proyectoDTO is null)
            {
                return BadRequest($"El proyecto con el id: {id} no existe");
            }

            var proyecto = new Proyecto()
            {
                Id = id,
                NombreProyecto = proyectosCreacionDTO.NombreProyecto,
                NombreCliente = proyectosCreacionDTO.NombreCliente,
                Descripcion = proyectosCreacionDTO.Descripcion
            };

            await repositorioProyectos.actualizarProyecto(proyecto);

            return NoContent();

        }

      


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> delete(int id)
        {
            if (id == 0)
            {
                return BadRequest($"El id: {id} no valido");
            }

            var proyectoDTO = await repositorioProyectos.ObtenerProyectoPorId(id);

            if (proyectoDTO is null)
            {
                return BadRequest($"El proyecto con el id: {id} no existe");
            }

            var resultado = await repositorioProyectos.eliminarProyecto(id);


            if (resultado > 0)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }

        }


    }
}
