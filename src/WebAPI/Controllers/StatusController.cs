using System;
using System.Threading.Tasks;
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
                [FromQuery] string? statusType,
                [FromQuery] bool? lActive,
                [FromQuery] int itemsPerPage = 10, 
                [FromQuery] int page = 1
            )
        {
            try
            {
                var result = await _statusUseCase.GetStatusAsync(statusType, lActive, itemsPerPage, page);
                return Ok(result);
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
                var status = await _statusUseCase.GetStatusByIdAsync(id);

                if (status == null)
                {
                    return NotFound($"Status com ID {id} não encontrado.");
                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar status por ID: {ex.Message}");
            }
        }

        [HttpGet("types")]
        public IActionResult GetStatusTypes()
        {
            try
            {
                var types = _statusUseCase.GetValidStatusTypes();
                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter tipos de status: {ex.Message}");
            }
        }

        [HttpPost("insert")]
        public async Task<IActionResult> CreateStatusAsync([FromBody] StatusInsertDTO status)
        {
            try
            {
                if (status == null || string.IsNullOrWhiteSpace(status.StatusName) || string.IsNullOrWhiteSpace(status.StatusType))
                {
                    return BadRequest("Os dados do status são inválidos.");
                }

                // Valida o tipo de status
                if (!IsValidStatusType(status.StatusType))
                {
                    return BadRequest("O tipo de status deve ser patient, specialty, doctor ou appointment.");
                }

                var createdStatusId = await _statusUseCase.CreateStatusAsync(status);
                var infosStatusCreated = await _statusUseCase.GetStatusByIdAsync(createdStatusId);

                return CreatedAtAction(nameof(GetStatusByIdAsync), new { id = createdStatusId }, infosStatusCreated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar status: {ex.Message}");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromBody] StatusInsertDTO status)
        {
            try
            {
                if (status == null || string.IsNullOrWhiteSpace(status.StatusName) || string.IsNullOrWhiteSpace(status.StatusType))
                {
                    return BadRequest("Os dados do status são inválidos.");
                }

                // Valida o tipo de status
                if (!IsValidStatusType(status.StatusType))
                {
                    return BadRequest("O tipo de status deve ser patient, specialty, doctor ou appointment.");
                }

                var updated = await _statusUseCase.UpdateStatusAsync(id, status);

                if (!updated)
                {
                    return NotFound($"Status com ID {id} não encontrado.");
                }

                var updatedStatus = await _statusUseCase.GetStatusByIdAsync(id);
                return Ok(updatedStatus);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar status: {ex.Message}");
            }
        }

        [HttpPatch("toggle-active/{id}")]
        public async Task<IActionResult> ToggleStatusActiveAsync(int id, [FromQuery] bool active)
        {
            try
            {
                var toggled = await _statusUseCase.ToggleStatusActiveAsync(id, active);

                if (!toggled)
                {
                    return NotFound($"Status com ID {id} não encontrado.");
                }

                var updatedStatus = await _statusUseCase.GetStatusByIdAsync(id);
                return Ok(updatedStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao alterar estado ativo do status: {ex.Message}");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStatusAsync(int id)
        {
            try
            {
                var deleted = await _statusUseCase.DeleteStatusAsync(id);

                if (!deleted)
                {
                    return NotFound($"Status com ID {id} não encontrado.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir status: {ex.Message}");
            }
        }

        // Método auxiliar para validar o tipo de status
        private bool IsValidStatusType(string statusType)
        {
            return statusType == "patient" ||
                   statusType == "specialty" ||
                   statusType == "doctor" ||
                   statusType == "appointment";
        }
    }
}