using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using DotTaskAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DotTaskAPI.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController: ControllerBase
    {
        private readonly IRepositorioUsuarios repositorioUsuarios;
        private readonly IServicioEmail servicioEmail;
        private readonly IRepositorioToken repositorioToken;
        private readonly IConfiguration config;

        public UsuariosController(IRepositorioUsuarios repositorioUsuarios, IServicioEmail servicioEmail, IRepositorioToken repositorioToken, IConfiguration config)
        {
            this.repositorioUsuarios = repositorioUsuarios;
            this.servicioEmail = servicioEmail;
            this.repositorioToken = repositorioToken;
            this.config = config;
        }

        
        [HttpGet("usuario")]
        [Authorize]
        public async Task<ActionResult<UsuarioAutenticadoDTO>> usuario()
        {
            var usuario = await repositorioUsuarios.obtenerUsuarioAutenticado();
           
            if (usuario == null)
            {
                return NotFound("El usuario no Autenticado");
            }

            return usuario;
        }

        [HttpPost("registro")]
        public async Task<ActionResult> registro(registroCreacioDTO registroCreacioDTO)
        {

            var usuario = await repositorioUsuarios.existeUsuarioPorCorreo(registroCreacioDTO.Email);

            if (usuario)
            {
                return BadRequest("El usuario ya existe");
            }

            var entidad = new Usuario()
            {
                Nombre = registroCreacioDTO.Nombre,
                Email = registroCreacioDTO.Email,
                Password = hashPassword(registroCreacioDTO.Password),
                Confirmado = false
            };

            var token = generarTokenConfirmacion();

            enviarEmail(token, entidad);

            await repositorioUsuarios.guardar(entidad);

            var Token = new Token()
            {
                UsuarioId = entidad.Id,
                Token1 = token.ToString(),
                FechaExpiracion = DateTime.UtcNow.AddHours(24),
                FechaCreacion = DateTime.Now
            };

            await repositorioToken.guardarToken(Token);


            return Ok("Cuenta creada, revisa tu email para confirmarla");
        }

        //Confirmar token
        [HttpGet("confirmar")]
        public async Task<ActionResult> confirmar([FromQuery] string token)
        {
            var Token = await repositorioToken.obtenerToken(token);
            
            if (Token == null)
            {
                return NotFound();     
            }

            if (Token.FechaExpiracion < DateTime.UtcNow)
            {
                return BadRequest("El token ha expirado. Por favor solicita uno nuevo.");
            }

            var usuario = await repositorioUsuarios.obtenerUsuarioId((int)Token.UsuarioId);
            usuario.Confirmado = true;
            await repositorioUsuarios.actualizar(usuario);


            Token.Token1 = null;
            Token.FechaExpiracion = null;
            
            await repositorioToken.actualizar(Token);

            return Ok("Cuenta confirmada correctamente");
        }

        //login
        [HttpPost("login")]
        public async Task<ActionResult> login(loginDTO loginDTO)
        {
            var usuario = await repositorioUsuarios.obtenerUsuarioPorCorreo(loginDTO.Email);
            var rol_usuario = usuario.IdRolNavigation.Nombre; 

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            bool usuario_confirmado = (bool)usuario.Confirmado;

            var token_confirmacion = generarTokenConfirmacion();

            if (!usuario_confirmado)
            {
                var token = new Token();
                token.UsuarioId = usuario.Id;
                token.Token1 = token_confirmacion.ToString();
                token.FechaExpiracion = DateTime.UtcNow.AddHours(24);
                token.FechaCreacion = DateTime.Now;

                enviarEmail(token_confirmacion, usuario);

                await repositorioToken.guardarToken(token);

                return Unauthorized("La cuenta no ha sido confirmada, hemos enviado un email de confirmacion.");
            }

            if (!VerifyPassword(loginDTO.Password, usuario.Password))
            {
                return Unauthorized("Credenciales incorrectas");
            }

            var Token = generarToken(usuario, rol_usuario);

            return Ok(Token);

        }

        //solicitar nuevo codigo
        [HttpPost("solicitar-codigo")]
        public async Task<ActionResult> solicitarCodigo(emailDTO emailDTO)
        {
            var usuario = await repositorioUsuarios.obtenerUsuarioPorCorreo(emailDTO.Email);

            if (usuario == null)
            {
                return BadRequest("El usuario no esta registrado");
            }

            if ((bool)usuario.Confirmado)
            {
                return BadRequest("El usuario ya esta confirmado");

            }

            var token_cofirmacion = generarTokenConfirmacion();

            var token = new Token();
            token.UsuarioId = usuario.Id;
            token.Token1 = token_cofirmacion.ToString();
            token.FechaExpiracion = DateTime.UtcNow.AddHours(24);
            token.FechaCreacion = DateTime.Now;

            await repositorioToken.guardarToken(token);
            enviarEmail(token_cofirmacion, usuario);

            return Ok("Se envio un nuevo token a tu e-mail");
        }


        //olvide la contraseña
        [HttpPost("olvide-password")]
        public async Task<ActionResult> olvidePassword(emailDTO emailDTO)
        {
            var usuario = await repositorioUsuarios.obtenerUsuarioPorCorreo(emailDTO.Email);

            if (usuario == null)
            {
                return BadRequest("El usuario no esta registrado");
            }

            var token_cofirmacion = generarTokenConfirmacion();

            var token = new Token();
            token.UsuarioId = usuario.Id;
            token.Token1 = token_cofirmacion.ToString();
            token.FechaExpiracion = DateTime.UtcNow.AddHours(24);
            token.FechaCreacion = DateTime.Now;

            await repositorioToken.guardarToken(token);
            enviarEmailRestablecerPassword(token_cofirmacion, usuario);

            return Ok("Revisa tu email para instrucciones");
        }

        [HttpPost("validar-token")]
        public async Task<ActionResult> validarToken(tokenDTO tokenDTO)
        {
            var Token = await repositorioToken.obtenerToken(tokenDTO.Token.ToString());

            if (Token == null)
            {
                return NotFound("Token no valido");
            }


            return Ok("Token valido, Define tu nuevo password.");

        }
        // Actualizar contraseña con token
        [HttpPost("actualizar-password")]
        public async Task<ActionResult> actualizarPassword([FromQuery] string token, [FromBody] PasswordDTO passwordDTO)
        {
            var Token = await repositorioToken.obtenerToken(token);

            if (Token == null)
            {
                return NotFound("Token no valido");
            }

            // obtener usuario por id
            var usuario_id = Token.UsuarioId!.Value;
            var usuario = await repositorioUsuarios.obtenerUsuarioId(usuario_id);
            usuario.Password = hashPassword(passwordDTO.Password);
            
            //actualizar password
            await repositorioUsuarios.actualizar(usuario);


            //actualizar en tabla token
            Token.Token1 = null;
            Token.FechaExpiracion = null;

            await repositorioToken.actualizar(Token);

            return Ok("El password se modifico correctamente.");
        }


        //Profile
        [HttpPut("actualizar-perfil")]
        [Authorize]
        public async Task<ActionResult> actualizarPerfil(PerfilDTO perfilDTO)
        {
            var user_auth = await repositorioUsuarios.obtenerInformacionJWT();

            var id_user_auth = int.Parse(user_auth!);

            var existe_usuario = await repositorioUsuarios.existeUsuarioPorCorreo(perfilDTO.Email);

            var usuario = await repositorioUsuarios.obtenerUsuarioPorCorreo(perfilDTO.Email);
            if (existe_usuario && usuario.Id != id_user_auth)
            {
                return BadRequest("El email ya esta registrado");
            }

            usuario.Nombre = perfilDTO.Nombre;
            usuario.Email = perfilDTO.Email;

            await repositorioUsuarios.actualizar(usuario);

            return NoContent();

        }

        [HttpPost("actualizar-password-actual")]
        [Authorize]
        public async Task<ActionResult> actualizarPasswordActual(ActualizarPasswordDTO passwordDTO)
        {

            var user_auth = await repositorioUsuarios.obtenerInformacionJWT();

            var id_user_auth = int.Parse(user_auth!);

            var usuario = await repositorioUsuarios.obtenerUsuarioId(id_user_auth);

            if (!VerifyPassword(passwordDTO.Current_Password, usuario.Password))
            {
                return Unauthorized("El Password actual es incorrecto");
            }

            usuario.Password = hashPassword(passwordDTO.Password);

            await repositorioUsuarios.actualizar(usuario);

            return NoContent();
        }

        //verificar password
        [HttpPost("verificar-password")]
        public async Task<ActionResult> verificarPassword(VerificarPasswordDTO passwordDTO)
        {

            var user_auth = await repositorioUsuarios.obtenerInformacionJWT();

            var id_user_auth = int.Parse(user_auth!);

            var usuario = await repositorioUsuarios.obtenerUsuarioId(id_user_auth);

            if (!VerifyPassword(passwordDTO.Password, usuario.Password))
            {
                return Unauthorized("El Password actual es incorrecto");
            }


            return Ok("Password Correcto");
        }


        //utilidades
        private async void enviarEmail(int token, Usuario usuario)
        {
            var urlConfirmacion = $"http://localhost:5173/auth/confirm-account";
            var cuerpoEmail = $"<p>Hola: {usuario.Nombre}, haz creado una cuenta en DotTask, ya casi esta todo listo, solo debes confirmar tu cuenta</p>" +
                $"<p>Visita el siguiente enlace:</p><a href='{urlConfirmacion}'>Confirmar cuenta</a>" +
                $"<p>E ingresa el codigo: <b>{token}<b></p>" +
                $"<p>Este token expira en 24 horas</p>";


            await servicioEmail.enviarEmailGmail("al15020032@itsa.edu.mx", "Confirma tu Cuenta", cuerpoEmail);

        }

        private async void enviarEmailRestablecerPassword(int token, Usuario usuario)
        {
            var urlConfirmacion = $"http://localhost:5173/auth/new-password";
            var cuerpoEmail = $"<p>Hola: {usuario.Nombre}, has solicitado reestablecer tu contraseña.</p>" +
                $"<p>Visita el siguiente enlace:</p><a href='{urlConfirmacion}'>Restablecer contraseña</a>" +
                $"<p>E ingresa el codigo: <b>{token}<b></p>" +
                $"<p>Este token expira en 24 horas</p>";


            await servicioEmail.enviarEmailGmail("al15020032@itsa.edu.mx", "Restablece tu contraseña", cuerpoEmail);

        }

        //generar json web token
        private RespuestaAutenticacionDTO generarToken(Usuario usuario, string rol_usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, rol_usuario),
            };

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["llavejwt"]));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var tokenSeguridad = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: credenciales);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenSeguridad);

            return new RespuestaAutenticacionDTO
            {
                Token = token,
                Expiracion = expiracion
            };

        }


        private int generarTokenConfirmacion()
        {
            var random = new Random();
            int codigo = random.Next(100000, 1000000);

            return codigo;
        }


        private string hashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string entered, string storedHash)
        {
            return hashPassword(entered) == storedHash;
        }
    }
}
