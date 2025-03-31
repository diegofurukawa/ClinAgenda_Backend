using System;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Application.SpecialtyUseCase;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Specialty")]
    public class SpecialtyController : ControllerBase
    {
        private readonly SpecialtyUseCase _specialtyUseCase;

        public SpecialtyController(SpecialtyUseCase service)
        {
            _specialtyUseCase = service;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetSpecialtyAsync(
                [FromQuery] string? name,
                [FromQuery] bool? lActive,
                [FromQuery] int itemsPerPage = 10, 
                [FromQuery] int page = 1
            )
        {
            try
            {
                var specialty = await _specialtyUseCase.GetSpecialtyAsync(name, lActive, itemsPerPage, page);
                return Ok(specialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar especialidades: {ex.Message}");
            }
        }

        [HttpGet("listById/{id}")]
        public async Task<IActionResult> GetSpecialtyByIdAsync(int id)
        {
            try
            {
                var specialty = await _specialtyUseCase.GetSpecialtyByIdAsync(id);

                if (specialty == null)
                {
                    return NotFound($"Especialidade com ID {id} não encontrada.");
                }

                return Ok(specialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar especialidade por ID: {ex.Message}");
            }
        }

        [HttpPost("insert")]
        public async Task<IActionResult> CreateSpecialtyAsync([FromBody] SpecialtyInsertDTO specialty)
        {
            try
            {
                if (specialty == null || string.IsNullOrWhiteSpace(specialty.SpecialtyName))
                {
                    return BadRequest("Os dados da especialidade são inválidos.");
                }

                var createdSpecialtyId = await _specialtyUseCase.CreateSpecialtyAsync(specialty);
                var createdSpecialty = await _specialtyUseCase.GetSpecialtyByIdAsync(createdSpecialtyId);

                return CreatedAtAction(nameof(GetSpecialtyByIdAsync), new { id = createdSpecialtyId }, createdSpecialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar especialidade: {ex.Message}");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateSpecialtyAsync(int id, [FromBody] SpecialtyInsertDTO specialty)
        {
            try
            {
                if (specialty == null || string.IsNullOrWhiteSpace(specialty.SpecialtyName))
                {
                    return BadRequest("Os dados da especialidade são inválidos.");
                }

                var updated = await _specialtyUseCase.UpdateSpecialtyAsync(id, specialty);

                if (!updated)
                {
                    return NotFound($"Especialidade com ID {id} não encontrada.");
                }

                var updatedSpecialty = await _specialtyUseCase.GetSpecialtyByIdAsync(id);
                return Ok(updatedSpecialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar especialidade: {ex.Message}");
            }
        }

        [HttpPatch("toggle-active/{id}")]
        public async Task<IActionResult> ToggleSpecialtyActiveAsync(int id, [FromQuery] bool active)
        {
            try
            {
                var toggled = await _specialtyUseCase.ToggleSpecialtyActiveAsync(id, active);

                if (!toggled)
                {
                    return NotFound($"Especialidade com ID {id} não encontrada.");
                }

                var updatedSpecialty = await _specialtyUseCase.GetSpecialtyByIdAsync(id);
                return Ok(updatedSpecialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao alterar estado ativo da especialidade: {ex.Message}");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSpecialtyAsync(int id)
        {
            try
            {
                var deleted = await _specialtyUseCase.DeleteSpecialtyAsync(id);

                if (!deleted)
                {
                    return NotFound($"Especialidade com ID {id} não encontrada.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir especialidade: {ex.Message}");
            }
        }
    }
}