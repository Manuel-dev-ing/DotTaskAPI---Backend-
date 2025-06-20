using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using DotTaskAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DotTaskAPI.Controllers
{
    [ApiController]
    [Route("api/proyectos")]
    public class ProyectosController : ControllerBase
    {
        private readonly IRepositorioProyectos repositorioProyectos;
        private readonly IRepositorioUsuarios repositorioUsuarios;
        private readonly IRepositorioTeam repositorioTeam;

        public ProyectosController(IRepositorioProyectos repositorioProyectos, IRepositorioUsuarios repositorioUsuarios, IRepositorioTeam repositorioTeam)
        {
            this.repositorioProyectos = repositorioProyectos;
            this.repositorioUsuarios = repositorioUsuarios;
            this.repositorioTeam = repositorioTeam;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<ProyectoDTO>> get()
        {
            var manager = await repositorioUsuarios.obtenerInformacionJWT();

            var manager_id = int.Parse(manager!);

            //obtiene los proyectos que le pertenecen al manager 
            var proyectosDTO = await repositorioProyectos.obtenerProyectos(manager_id);

            return proyectosDTO;
        }

        [HttpGet("{id:int}", Name = "ObtenerProyecto")]
        //[Authorize(Roles = "manager")]
        public async Task<ActionResult<ProyectoDTO>> get(int id)
        {

            if (id <= 0)
            {
                return BadRequest($"El id: {id} no valido");
            }

            var manager = await repositorioUsuarios.obtenerInformacionJWT();

            var manager_id = int.Parse(manager!);

            var proyectoDTO = await repositorioProyectos.ObtenerProyectoIdManager(id, manager_id);

            if (proyectoDTO is null)
            {
                return BadRequest($"El proyecto con el id: {id} no existe");
            }


            return proyectoDTO;

        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult> post([FromBody] ProyectosCreacionDTO proyectosCreacionDTO)
        {
            var manager = await repositorioUsuarios.obtenerInformacionJWT();
            var manager_id = int.Parse(manager!);

            var proyecto = new Proyecto()
            {
                NombreProyecto = proyectosCreacionDTO.NombreProyecto,
                NombreCliente = proyectosCreacionDTO.NombreCliente,
                Descripcion = proyectosCreacionDTO.Descripcion
            };
            var proyectoDTO = await repositorioProyectos.guardarProyecto(proyecto);

            var proyecto_usuario = new ProyectosUsuario()
            {
                IdProyecto = proyectoDTO.Id,
                IdUsuario = manager_id,
                IsManager = true,
                FechaAsignacion = DateTime.Now
            };

            await repositorioTeam.guardar(proyecto_usuario);


            return CreatedAtRoute("ObtenerProyecto", new { id = proyectoDTO.Id }, proyectoDTO);
        }

        [HttpPut("{id:int}")]
        [Authorize]
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

            var manager = await repositorioUsuarios.obtenerInformacionJWT();

            var manager_id = int.Parse(manager!);

            var proyectoUsuario = await repositorioTeam.obtenerProyectosUsuarioPorUsuario(manager_id);

            if (proyectoUsuario.IdUsuario != manager_id)
            {
                return NotFound("Solo el manager puede actualizar un proyecto");
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
        [Authorize]
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
            var manager = await repositorioUsuarios.obtenerInformacionJWT();

            var manager_id = int.Parse(manager!);

            var proyectoUsuario = await repositorioTeam.obtenerProyectosUsuarioPorUsuario(manager_id);

            if (proyectoUsuario.IdUsuario != manager_id)
            {
                return NotFound("Solo el manager puede eliminar un proyecto");
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
