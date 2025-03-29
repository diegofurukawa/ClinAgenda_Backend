using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Application.SpecialtyUseCase;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Specialty")]
    public class SpecialtyController : ControllerBase
    {
        private readonly SpecialtyUseCase _SpecialtyUseCase;

        public SpecialtyController(SpecialtyUseCase service)
        {
            _SpecialtyUseCase = service;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetSpecialtyAsync(
                [FromQuery] int itemsPerPage = 10, 
                [FromQuery] int page = 1
            )
        {
            try
            {
                var specialty = await _SpecialtyUseCase.GetSpecialtyAsync(itemsPerPage, page);
                return Ok(specialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar Specialty: {ex.Message}");
            }
        }

        [HttpGet("listById/{id}")]
        public async Task<IActionResult> GetSpecialtyByIdAsync(int specialtyid)
        {
            try
            {
                var specialty = await _SpecialtyUseCase.GetSpecialtyByIdAsync(specialtyid);

                if (specialty == null)
                {
                    return NotFound($"Specialty com ID {specialtyid} não encontrado.");
                }

                return Ok(specialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar Specialty por ID: {ex.Message}");
            }
        }

        [HttpPost("insert")]
        public async Task<IActionResult> CreateSpecialtyAsync([FromBody] SpecialtyInsertDTO Specialty)
        {
            try
            {
                if (Specialty == null || string.IsNullOrWhiteSpace(Specialty.SpecialtyName))
                {
                    return BadRequest("Os dados do Specialty são inválidos.");
                }

                var createdSpecialty = await _SpecialtyUseCase.CreateSpecialtyAsync(Specialty);
                var infosSpecialtyCreated = await _SpecialtyUseCase.GetSpecialtyByIdAsync(createdSpecialty);

                // Correção do roteamento incluindo o nome do controller e a rota correta
                return Created($"/api/Specialty/listById/{createdSpecialty}", infosSpecialtyCreated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar Specialty: {ex.Message}");
            }
        }
    }
}