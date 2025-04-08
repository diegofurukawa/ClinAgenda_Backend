using System;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Application.DoctorUseCase;
using ClinAgenda.src.Application.SpecialtyUseCase;
using ClinAgenda.src.Application.StatusUseCase;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Doctor")]
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
        
        [HttpGet("list")]
        public async Task<IActionResult> GetDoctorsAsync(
            [FromQuery] string? name, 
            [FromQuery] int? specialtyId, 
            [FromQuery] int? statusId,
            [FromQuery] bool? lActive,
            [FromQuery] int itemsPerPage = 10, 
            [FromQuery] int page = 1)
        {
            try
            {
                var result = await _doctorUseCase.GetDoctorsAsync(name, specialtyId, statusId, lActive, itemsPerPage, page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        
        [HttpGet("listById/{doctorId}")]
        public async Task<IActionResult> GetDoctorByIdAsync(int doctorId)
        {
            try
            {
                var doctor = await _doctorUseCase.GetDoctorByIdAsync(doctorId);
                if (doctor == null) return NotFound($"Médico com ID {doctorId} não encontrado.");
                
                return Ok(doctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPost("insert")]
        public async Task<IActionResult> CreateDoctorAsync([FromBody] DoctorCreateDTO doctor)
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

                var createdDoctorId = await _doctorUseCase.CreateDoctorAsync(doctor);
                if (createdDoctorId <= 0)
                {
                    return StatusCode(500, "Erro ao criar o médico.");
                }
                
                var createdDoctor = await _doctorUseCase.GetDoctorByIdAsync(createdDoctorId);
                
                // return CreatedAtAction(nameof(GetDoctorByIdAsync), new { doctorId = createdDoctorId }, createdDoctor);
                return Ok(createdDoctor);
            }
            // catch (ArgumentException ex)
            // {
            //     return BadRequest(ex.Message);
            // }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPut("update/{doctorId}")]
        public async Task<IActionResult> UpdateDoctorAsync(int doctorId, [FromBody] DoctorUpdateDTO doctor)
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

                bool updated = await _doctorUseCase.UpdateDoctorAsync(doctorId, doctor);
                if (!updated)
                {
                    return NotFound($"Médico com ID {doctorId} não encontrado ou não foi possível atualizar.");
                }

                var updatedDoctor = await _doctorUseCase.GetDoctorByIdAsync(doctorId);
                return Ok(updatedDoctor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar médico: {ex.Message}");
            }
        }

        [HttpPatch("toggle-active/{doctorId}")]
        public async Task<IActionResult> ToggleDoctorActiveAsync(int doctorId, [FromQuery] bool active)
        {
            try
            {
                var toggled = await _doctorUseCase.ToggleDoctorActiveAsync(doctorId, active);

                if (!toggled)
                {
                    return NotFound($"Médico com ID {doctorId} não encontrado.");
                }

                var updatedDoctor = await _doctorUseCase.GetDoctorByIdAsync(doctorId);
                return Ok(updatedDoctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao alterar estado ativo do médico: {ex.Message}");
            }
        }

        [HttpPatch("specialties/{doctorId}/{specialtyId}/toggle-active")]
        public async Task<IActionResult> ToggleDoctorSpecialtyActiveAsync(
            int doctorId,
            int specialtyId,
            [FromQuery] bool active)
        {
            try
            {
                var toggled = await _doctorUseCase.ToggleDoctorSpecialtyActiveAsync(doctorId, specialtyId, active);

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

        [HttpDelete("delete/{doctorId}")]
        public async Task<IActionResult> DeleteDoctorAsync(int doctorId)
        {
            try
            {
                var deleted = await _doctorUseCase.DeleteDoctorAsync(doctorId);
                
                if (!deleted)
                {
                    return NotFound($"Médico com ID {doctorId} não encontrado ou não foi possível excluir.");
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