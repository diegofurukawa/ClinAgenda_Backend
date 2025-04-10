using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ClinAgenda.src.Application.DTOs.Doctor;

using ClinAgenda.src.Application.AppointmentUseCase;
using ClinAgenda.src.Application.SpecialtyUseCase;
using ClinAgenda.src.Application.StatusUseCase;
using ClinAgenda.src.Application.DoctorUseCase;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly DoctorUseCase _doctorUseCase;
        private readonly StatusUseCase _statusUseCase;
        private readonly SpecialtyUseCase _specialtyUseCase;
        private readonly AppointmentUseCase _appointmentUseCase;

        public DoctorController(DoctorUseCase doctorUseCase, StatusUseCase statusUseCase, SpecialtyUseCase specialtyUseCase, AppointmentUseCase appointmentUseCase)
        {
            _doctorUseCase = doctorUseCase;
            _statusUseCase = statusUseCase;
            _specialtyUseCase = specialtyUseCase;
            _appointmentUseCase = appointmentUseCase;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetDoctors(
            [FromQuery] string? doctorName, 
            [FromQuery] int? specialtyId, 
            [FromQuery] int? statusId, 
            [FromQuery] bool? lActive, 
            [FromQuery] int itemsPerPage = 10, 
            [FromQuery] int page = 1
            )
        {
            try
            {
                var result = await _doctorUseCase.GetDoctorsAsync(doctorName, specialtyId, statusId, lActive, itemsPerPage, page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        [HttpPost("insert")]
        public async Task<IActionResult> CreateDoctorAsync([FromBody] DoctorInsertDTO doctor)
        {
            try
            {
                var hasStatus = await _statusUseCase.GetStatusByIdAsync(doctor.StatusId);
                if (hasStatus == null)
                    return BadRequest($"O status com ID {doctor.StatusId} não existe.");

                var specialties = await _specialtyUseCase.GetSpecialtiesByIds(doctor.Specialty);

                var notFoundSpecialties = doctor.Specialty.Except(specialties.Select(s => s.SpecialtyId)).ToList();

                if (notFoundSpecialties.Any())
                {
                    return BadRequest(notFoundSpecialties.Count > 1 ? $"As especialidades com os IDs {string.Join(", ", notFoundSpecialties)} não existem." : $"A especialidade com o ID {notFoundSpecialties.First().ToString()} não existe.");
                }

                var createdDoctorId = await _doctorUseCase.CreateDoctorAsync(doctor);

                var ifosDoctor = await _doctorUseCase.GetDoctorByIdAsync(createdDoctorId);

                return Ok(ifosDoctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        [HttpPut("update/{doctorId}")]
        public async Task<IActionResult> UpdateDoctorAsync(int doctorId, [FromBody] DoctorInsertDTO doctor)
        {
            if (doctor == null) return BadRequest();

            var hasStatus = await _statusUseCase.GetStatusByIdAsync(doctor.StatusId);
            if (hasStatus == null)
                return BadRequest($"O status com ID {doctor.StatusId} não existe.");

            var specialties = await _specialtyUseCase.GetSpecialtiesByIds(doctor.Specialty);

            var notFoundSpecialties = doctor.Specialty.Except(specialties.Select(s => s.SpecialtyId)).ToList();

            if (notFoundSpecialties.Any())
            {
                return BadRequest(notFoundSpecialties.Count > 1 ? $"As especialidades com os IDs {string.Join(", ", notFoundSpecialties)} não existem." : $"A especialidade com o ID {notFoundSpecialties.First().ToString()} não existe.");
            }

            bool updated = await _doctorUseCase.UpdateDoctorAsync(doctorId, doctor);

            if (!updated) return NotFound("Doutor não encontrado.");

            var infosDoctorUpdate = await _doctorUseCase.GetDoctorByIdAsync(doctorId);
            return Ok(infosDoctorUpdate);

        }
        [HttpGet("listById/{doctorId}")]
        public async Task<IActionResult> GetDoctorByIdAsync(int doctorId)
        {
            var doctor = await _doctorUseCase.GetDoctorByIdAsync(doctorId);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }
        [HttpDelete("delete/{doctorId}")]
        public async Task<IActionResult> DeleteDoctorAsync(int doctorId)
        {
            try
            {
                var doctorInfo = await _doctorUseCase.GetDoctorByIdAsync(doctorId);

                var appointment = await _appointmentUseCase.GetAppointmentsAsync(doctorName: doctorInfo.DoctorName, null, null, null, null, null,null, null, 1, 1);

                if (((dynamic)appointment).Total > 0)
                    return NotFound($"Erro ao deletar, Doutor com agendamento marcado");

                var success = await _doctorUseCase.DeleteDoctorByIdAsync(doctorId);

                if (!success)
                {
                    return NotFound($"Doutor com ID {doctorId} não encontrado.");
                }

                return Ok("Doutor deletado com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{doctorId}/toggle-active")]
        public async Task<IActionResult> ToggleDoctorActive(int doctorId, [FromBody] DoctorToggleActiveDTO toggleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Status = 400,
                        Title = "Validation Failed",
                        Detail = "Invalid active status data."
                    });
                }

                var toggled = await _doctorUseCase.ToggleDoctorActiveAsync(doctorId, toggleDto.IsActive);
                
                if (!toggled)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = 404,
                        Title = "Not Found",
                        Detail = $"Doctor with ID {doctorId} was not found."
                    });
                }

                var updatedDoctor = await _doctorUseCase.GetDoctorByIdAsync(doctorId);
                return Ok(updatedDoctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        
        [HttpPatch("{doctorId}/specialties/{specialtyId}/toggle-active")]
        public async Task<IActionResult> ToggleDoctorSpecialtyActive(
            int doctorId, 
            int specialtyId, 
            [FromBody] DoctorToggleActiveDTO toggleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Status = 400,
                        Title = "Validation Failed",
                        Detail = "Invalid active status data."
                    });
                }

                var toggled = await _doctorUseCase.ToggleDoctorSpecialtyActiveAsync(doctorId, specialtyId, toggleDto.IsActive);
                
                if (!toggled)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = 404,
                        Title = "Not Found",
                        Detail = $"Doctor with ID {doctorId} or specialty with ID {specialtyId} was not found."
                    });
                }

                var updatedDoctor = await _doctorUseCase.GetDoctorByIdAsync(doctorId);
                return Ok(updatedDoctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }        
    }
}