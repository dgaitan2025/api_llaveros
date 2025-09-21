using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using parcial2.Models;

namespace parcial2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CRUDController : Controller
    {
        

        private readonly dbparcialContext _context;
        public CRUDController(dbparcialContext context)
        {
            _context = context;
        }

        [HttpPost("CrearVehiculo")]
        public async Task<ActionResult <vehiculoInsert>> InsertarVehiculo(vehiculoInsert dto)
            
        {
            try
            {
               await using var conn = _context.Database.GetDbConnection();
await conn.OpenAsync();

// Paso 1: Ejecutar el SP
await using (var cmd = conn.CreateCommand())
{
    cmd.CommandText = "CALL sp_crud_vehiculos(@idvehiculo, @idcolor, @idmarca, @modelo, @chasis, @motor, @nombre, @activo, @opcion)";
    cmd.Parameters.Add(new MySqlParameter("@idvehiculo", DBNull.Value));
    cmd.Parameters.Add(new MySqlParameter("@idcolor", dto.idcolor));
    cmd.Parameters.Add(new MySqlParameter("@idmarca", dto.idmarca));
    cmd.Parameters.Add(new MySqlParameter("@modelo", dto.modelo));
    cmd.Parameters.Add(new MySqlParameter("@chasis", dto.chasis ?? string.Empty));
    cmd.Parameters.Add(new MySqlParameter("@motor", dto.motor ?? string.Empty));
    cmd.Parameters.Add(new MySqlParameter("@nombre", dto.nombre ?? string.Empty));
    cmd.Parameters.Add(new MySqlParameter("@activo", dto.activo));
    cmd.Parameters.Add(new MySqlParameter("@opcion", "C"));
    await cmd.ExecuteNonQueryAsync();
}

// Paso 2: Leer el LAST_INSERT_ID()
await using (var cmd2 = conn.CreateCommand())
{
    cmd2.CommandText = "SELECT LAST_INSERT_ID()";
    var result = await cmd2.ExecuteScalarAsync();
    int idInsertado = Convert.ToInt32(result);

    return Ok(new { mensaje = "Vehículo insertado correctamente", id = idInsertado });
}
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al insertar el vehículo", detalle = ex.Message });
            }
        }


        [HttpPost("ActualizarVehiculo")]
        public async Task<IActionResult> ActualizarVehiculo(vehiculoActualizar dto)
        {
            try
            {

                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_crud_vehiculos ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7},{8})",
                    dto.idvehiculo,
                    dto.idcolor,
                    dto.idmarca,
                    dto.modelo,
                    dto.chasis,
                    dto.motor,
                    dto.nombre,
                    dto.activo,
                    "U"
                );

                return Ok(new { mensaje = "Vehículo Actulaizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al insertar el vehículo", detalle = ex.Message });
            }
        }

        [HttpPost("EliminarVehiculo")]
        public async Task<IActionResult> EliminarVehiculo(vehiculoEliminar dto)
        {
            try
            {

                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_crud_vehiculos ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7},{8})",
                    dto.idvehiculo,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    "D"
                );

                return Ok(new { mensaje = "Vehículo eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al insertar el vehículo", detalle = ex.Message });
            }
        }

        [HttpGet("ObtenerVehiculo/{id}")]
        public async Task<IActionResult> ObtenerVehiculo(int id)
        {
            try
            {
                // Ejecutar el SP que devuelve datos
                var vehiculos = await _context.vehiculos
                    .FromSqlRaw("CALL sp_crud_vehiculos({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        id,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        "R"
                    )
                    .ToListAsync();

                if (!vehiculos.Any())
                {
                    return NotFound(new { mensaje = "No se encontró el vehículo" });
                }

                return Ok(vehiculos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al obtener el vehículo", detalle = ex.Message });
            }
        }


    }
}
