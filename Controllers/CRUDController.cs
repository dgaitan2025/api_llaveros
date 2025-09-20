using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using parcial2.Models;

namespace parcial2.Controllers
{
    public class CRUDController : Controller
    {

        private readonly dbparcialContext _context;
        public CRUDController(dbparcialContext context)
        {
            _context = context;
        }

        [HttpPost("CrearVehiculo")]
        public async Task<IActionResult> InsertarVehiculo(vehiculoInsert dto)
        {
            try
            {
                
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_crud_vehiculos ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7},{8})",
                    null,
                    dto.idcolor,
                    dto.idmarca,
                    dto.modelo,
                    dto.chasis,
                    dto.motor,
                    dto.nombre,
                    dto.activo,
                    "C"                  
                );

                return Ok(new { mensaje = "Vehículo insertado correctamente" });
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
