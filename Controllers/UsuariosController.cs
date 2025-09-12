using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using ValidaNicknameApi.Models;
using ValidaNickname.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity.Data;

namespace ValidaNicknameApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UsuariosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/usuarios/validar?nombre=Juan
        [HttpGet("validar")]
        public async Task<IActionResult> ValidarNombre([FromQuery] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return BadRequest("El nombre es requerido.");

            int existe = 0;

            try
            {


                using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                using (var command = new MySqlCommand("sp_validar_Nickname", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_nombre", nombre);

                    await connection.OpenAsync();
                    var result = await command.ExecuteScalarAsync();
                    existe = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en la base de datos: {ex.Message}");
            }

            return Ok(new { existe = existe == 1 });
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario usuario)
        {
            if (usuario == null)
                return BadRequest("Datos inválidos");

            try
            {
                string hashPHC = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash, workFactor: 12);

                using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using var command = new MySqlCommand("sp_usuarios_crear", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetros IN
                command.Parameters.AddWithValue("p_Email", usuario.Email);
                command.Parameters.AddWithValue("p_Telefono", (object)usuario.Telefono ?? DBNull.Value);
                command.Parameters.AddWithValue("p_FechaNacimiento", (object)usuario.FechaNacimiento ?? DBNull.Value);
                command.Parameters.AddWithValue("p_Nickname", usuario.Nickname);
                command.Parameters.AddWithValue("p_PasswordPHC", hashPHC);
                command.Parameters.AddWithValue("p_Fotografia", (object)usuario.Fotografia ?? DBNull.Value);
                command.Parameters.AddWithValue("p_FotografiaMime", (object)usuario.FotografiaMime ?? DBNull.Value);
                command.Parameters.AddWithValue("p_Fotografia2", (object)usuario.Fotografia2 ?? DBNull.Value);
                command.Parameters.AddWithValue("p_Fotografia2Mime", (object)usuario.Fotografia2Mime ?? DBNull.Value);
                command.Parameters.AddWithValue("p_RolId", usuario.RolId);

                // Parámetros OUT
                var pUsuarioId = new MySqlParameter("p_UsuarioId", MySqlDbType.UInt64) { Direction = ParameterDirection.Output };
                var pCodigo = new MySqlParameter("p_Codigo", MySqlDbType.Int32) { Direction = ParameterDirection.Output };
                var pMensaje = new MySqlParameter("p_Mensaje", MySqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };

                command.Parameters.Add(pUsuarioId);
                command.Parameters.Add(pCodigo);
                command.Parameters.Add(pMensaje);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return Ok(new
                {
                    success = true,
                    usuarioId = pUsuarioId.Value,
                    codigo = pCodigo.Value,
                    mensaje = pMensaje.Value
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ValidaNickname.Models.LoginRequest login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Usuario) || string.IsNullOrWhiteSpace(login.Password))
                return BadRequest(new { success = false, message = "Usuario y contraseña son obligatorios" });

            try
            {
                using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                string query = @"SELECT UsuarioId, Email, Nickname, PasswordHash, Fotografia2 
                         FROM usuarios 
                         WHERE Email = @usuario OR Nickname = @usuario
                         LIMIT 1";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@usuario", login.Usuario);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return Unauthorized(new { success = false, message = "Usuario o contraseña incorrectos" });
                }

                var usuarioId = reader.GetInt64("UsuarioId");
                var email = reader.GetString("Email");
                var nickname = reader.GetString("Nickname");
                var hash = reader.GetString("PasswordHash");

                // 📸 Obtener foto en bytes
                string fotoBase64 = null;
                if (!(reader["Fotografia2"] is DBNull))
                {
                    var fotoBytes = (byte[])reader["Fotografia2"];
                    fotoBase64 = Convert.ToBase64String(fotoBytes);
                }

                // 🔑 Verificar password
                bool valido = BCrypt.Net.BCrypt.Verify(login.Password, hash);
                if (!valido)
                {
                    return Unauthorized(new { success = false, message = "Usuario o contraseña incorrectos" });
                }

                // ✅ Login exitoso
                return Ok(new
                {
                    success = true,
                    message = "Login exitoso",
                    usuario = new
                    {
                        id = usuarioId,
                        email,
                        nickname,
                        photo = fotoBase64 != null ? $"data:image/png;base64,{fotoBase64}" : null
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error en servidor", error = ex.Message });
            }
        }




    }
}

