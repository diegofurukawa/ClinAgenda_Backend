using ClinAgenda.src.Application.DTOs.Status;
using ClinAgenda.src.Application.StatusUseCase;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/status")]
    public class StatusController : ControllerBase
    {
        private readonly StatusUseCase _statusUseCase;

        public StatusController(StatusUseCase service)
        {
            _statusUseCase = service;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetStatusAsync(
                [FromQuery] int itemsPerPage = 10, 
                [FromQuery] int page = 1
            )
        {
            try
            {
                var specialty = await _statusUseCase.GetStatusAsync(itemsPerPage, page);
                return Ok(specialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar status: {ex.Message}");
            }
        }

        [HttpGet("listById/{id}")]
        public async Task<IActionResult> GetStatusByIdAsync(int id)
        {
            try
            {
                var specialty = await _statusUseCase.GetStatusByIdAsync(id);

                if (specialty == null)
                {
                    return NotFound($"Status com ID {id} não encontrado.");
                }

                return Ok(specialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar status por ID: {ex.Message}");
            }
        }

        [HttpPost("insert")]
        public async Task<IActionResult> CreateStatusAsync([FromBody] StatusInsertDTO status)
        {
            try
            {
                if (status == null || string.IsNullOrWhiteSpace(status.Name))
                {
                    return BadRequest("Os dados do status são inválidos.");
                }

                var createdStatus = await _statusUseCase.CreateStatusAsync(status);
                var infosStatusCreated = await _statusUseCase.GetStatusByIdAsync(createdStatus);

                return CreatedAtAction(nameof(GetStatusByIdAsync), new { id = createdStatus }, infosStatusCreated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar status: {ex.Message}");
            }
        }
    }

}