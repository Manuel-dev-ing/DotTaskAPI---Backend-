using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using DotTaskAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotTaskAPI.Controllers
{

    [ApiController]
    [Route("api/proyectos/{proyectoId:int}/team")]
    public class TeamController: ControllerBase
    {
        private readonly IRepositorioTeam repositorioTeam;

        public TeamController(IRepositorioTeam repositorioTeam)
        {
            this.repositorioTeam = repositorioTeam;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<UsuarioDTO>> get(int proyectoId)
        {
            var usuarios = await repositorioTeam.obtenerMiembrosProyecto(proyectoId);

            return usuarios;
        }

        [HttpPost("buscar")]
        [Authorize]
        public async Task<ActionResult<UsuarioDTO>> buscar(emailDTO emailDTO)
        {
            var usuarioDTO = await repositorioTeam.buscar(emailDTO.Email);
            if (usuarioDTO == null)
            {
                return NotFound("usuario no encontrado");
            }

            return usuarioDTO;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> agregarMiembroPorId(int proyectoId, [FromBody] idDTO id)
        {

            if (id.Id <= 0)
            {
                return BadRequest($"El id: {id} no valido");
            }

            var existe = await repositorioTeam.existeUsuarioProyecto(proyectoId, id.Id);

            if (existe)
            {
                return NotFound("El usuario ya existe en el proyecto");

            }

            var proyecto_usuario = new ProyectosUsuario()
            {
                IdProyecto = proyectoId,
                IdUsuario = id.Id,
                IsManager = false,
                FechaAsignacion = DateTime.Now
            };

            await repositorioTeam.guardar(proyecto_usuario);

            return Ok("Usuario agregado correctamente");
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult> delete(int proyectoId, int id)
        {

            var existe = await repositorioTeam.existeUsuarioProyecto(proyectoId, id);

            if (!existe)
            {
                return NotFound("El usuario no existe en el proyecto");
            }

            var usuario = await repositorioTeam.buscarUsuarioPorProyecto(proyectoId, id);
            //usuario.IdProyecto = null;

            await repositorioTeam.eliminar(usuario);

            return NoContent();
        }

    }
}
