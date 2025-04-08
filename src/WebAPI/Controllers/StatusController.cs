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

        [HttpGet("listById/{statusId}")]
        public async Task<IActionResult> GetStatusByIdAsync(int statusId)
        {
            try
            {
                var status = await _statusUseCase.GetStatusByIdAsync(statusId);

                if (status == null)
                {
                    return NotFound($"Status com ID {statusId} não encontrado.");
                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar status por ID: {ex.Message}");
            }
        }

        [HttpPost("insert")]
        public async Task<IActionResult> InsertStatusAsync([FromBody] StatusInsertDTO status)
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

                return CreatedAtAction(nameof(GetStatusByIdAsync), new { statusId = createdStatusId }, infosStatusCreated);
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

        [HttpPut("update/{statusId}")]
        public async Task<IActionResult> UpdateStatusAsync(int statusId, [FromBody] StatusInsertDTO status)
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

                var updated = await _statusUseCase.UpdateStatusAsync(statusId, status);

                if (!updated)
                {
                    return NotFound($"Status com ID {statusId} não encontrado.");
                }

                var updatedStatus = await _statusUseCase.GetStatusByIdAsync(statusId);
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

        [HttpPatch("toggle-active/{statusId}")]
        public async Task<IActionResult> ToggleStatusActiveAsync(int statusId, [FromQuery] bool active)
        {
            try
            {
                var toggled = await _statusUseCase.ToggleStatusActiveAsync(statusId, active);

                if (!toggled)
                {
                    return NotFound($"Status com ID {statusId} não encontrado.");
                }

                var updatedStatus = await _statusUseCase.GetStatusByIdAsync(statusId);
                return Ok(updatedStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao alterar estado ativo do status: {ex.Message}");
            }
        }

        [HttpDelete("delete/{statusId}")]
        public async Task<IActionResult> DeleteStatusAsync(int statusId)
        {
            try
            {
                var deleted = await _statusUseCase.DeleteStatusAsync(statusId);

                if (!deleted)
                {
                    return NotFound($"Status com ID {statusId} não encontrado.");
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

        [HttpGet("types")]
        public IActionResult GetStatusTypes()
        {
            try
            {
                var result = _statusUseCase.GetValidStatusTypes();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter tipos de status: {ex.Message}");
            }
        }
    }
}