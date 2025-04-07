using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Application.DoctorUseCase;
using ClinAgenda.src.Application.SpecialtyUseCase;
using ClinAgenda.src.Application.StatusUseCase;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorController : ControllerBase
    {
        private readonly DoctorUseCase _doctorUseCase;
        private readonly StatusUseCase _statusUseCase;
        private readonly SpecialtyUseCase _specialtyUseCase;

        public DoctorController(
            DoctorUseCase doctorUseCase, 
            StatusUseCase statusUseCase, 
            SpecialtyUseCase specialtyUseCase)
        {
            _doctorUseCase = doctorUseCase;
            _statusUseCase = statusUseCase;
            _specialtyUseCase = specialtyUseCase;
        }
        
        /// <summary>
        /// Lista médicos com filtros opcionais
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(DoctorPagedResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDoctors(
            [FromQuery] string? name, 
            [FromQuery] int? specialtyId, 
            [FromQuery] int? statusId,
            [FromQuery] bool? isActive,
            [FromQuery] int pageSize = 10, 
            [FromQuery] int page = 1)
        {
            try
            {
                var result = await _doctorUseCase.GetDoctorsAsync(name, specialtyId, statusId, isActive, pageSize, page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Obtém um médico por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DoctorDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            try
            {
                var doctor = await _doctorUseCase.GetDoctorByIdAsync(id);
                if (doctor == null) return NotFound($"Médico com ID {id} não encontrado.");
                
                return Ok(doctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria um novo médico
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(DoctorDetailDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorCreateDTO doctor)
        {
            try
            {
                // Validações já são feitas pelos atributos no DTO, 
                // mas podemos adicionar validações adicionais aqui
                
                // Verificar se o status existe
                var hasStatus = await _statusUseCase.GetStatusByIdAsync(doctor.StatusId);
                if (hasStatus == null)
                    return BadRequest($"O status com ID {doctor.StatusId} não existe.");

                // Verificar se todas as especialidades existem
                var specialties = await _specialtyUseCase.GetSpecialtiesByIdsAsync(doctor.SpecialtyIds);
                var notFoundSpecialties = doctor.SpecialtyIds.Except(specialties.Select(s => s.SpecialtyId)).ToList();

                if (notFoundSpecialties.Any())
                {
                    return BadRequest(notFoundSpecialties.Count > 1 
                        ? $"As especialidades com os IDs {string.Join(", ", notFoundSpecialties)} não existem." 
                        : $"A especialidade com o ID {notFoundSpecialties.First()} não existe.");
                }

                var createdDoctorId = await _doctorUseCase.CreateDoctorAsync(doctor);
                var doctorInfo = await _doctorUseCase.GetDoctorByIdAsync(createdDoctorId);

                return CreatedAtAction(nameof(GetDoctorById), new { id = createdDoctorId }, doctorInfo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um médico existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DoctorDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorUpdateDTO doctor)
        {
            try
            {
                // Verificar se o status existe
                var hasStatus = await _statusUseCase.GetStatusByIdAsync(doctor.StatusId);
                if (hasStatus == null)
                    return BadRequest($"O status com ID {doctor.StatusId} não existe.");

                // Verificar se todas as especialidades existem
                var specialties = await _specialtyUseCase.GetSpecialtiesByIdsAsync(doctor.SpecialtyIds);
                var notFoundSpecialties = doctor.SpecialtyIds.Except(specialties.Select(s => s.SpecialtyId)).ToList();

                if (notFoundSpecialties.Any())
                {
                    return BadRequest(notFoundSpecialties.Count > 1 
                        ? $"As especialidades com os IDs {string.Join(", ", notFoundSpecialties)} não existem." 
                        : $"A especialidade com o ID {notFoundSpecialties.First()} não existe.");
                }

                bool updated = await _doctorUseCase.UpdateDoctorAsync(id, doctor);
                if (!updated) return NotFound($"Médico com ID {id} não encontrado.");

                var updatedDoctor = await _doctorUseCase.GetDoctorByIdAsync(id);
                return Ok(updatedDoctor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Ativa ou desativa um médico
        /// </summary>
        [HttpPatch("{id}/toggle-active")]
        [ProducesResponseType(typeof(DoctorDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ToggleDoctorActive(int id, [FromBody] DoctorToggleActiveDTO request)
        {
            try
            {
                var toggled = await _doctorUseCase.ToggleDoctorActiveAsync(id, request.lActive);

                if (!toggled)
                {
                    return NotFound($"Médico com ID {id} não encontrado.");
                }

                var updatedDoctor = await _doctorUseCase.GetDoctorByIdAsync(id);
                return Ok(updatedDoctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao alterar estado ativo do médico: {ex.Message}");
            }
        }

        /// <summary>
        /// Ativa ou desativa uma especialidade de um médico
        /// </summary>
        [HttpPatch("{doctorId}/specialties/{specialtyId}/toggle-active")]
        [ProducesResponseType(typeof(DoctorDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ToggleDoctorSpecialtyActive(
            int doctorId,
            int specialtyId,
            [FromBody] DoctorToggleActiveDTO request)
        {
            try
            {
                var toggled = await _doctorUseCase.ToggleDoctorSpecialtyActiveAsync(doctorId, specialtyId, request.lActive);

                if (!toggled)
                {
                    return NotFound($"Relacionamento entre médico ID {doctorId} e especialidade ID {specialtyId} não encontrado.");
                }

                var updatedDoctor = await _doctorUseCase.GetDoctorByIdAsync(doctorId);
                return Ok(updatedDoctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao alterar estado ativo da especialidade do médico: {ex.Message}");
            }
        }

        /// <summary>
        /// Exclui um médico
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                var deleted = await _doctorUseCase.DeleteDoctorAsync(id);

                if (!deleted)
                {
                    return NotFound($"Médico com ID {id} não encontrado.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir médico: {ex.Message}");
            }
        }
    }
}