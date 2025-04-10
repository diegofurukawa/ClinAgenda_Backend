using System;
using System.Threading.Tasks;
using ClinAgenda.src.Application.AppointmentUseCase;
using ClinAgenda.src.Application.DTOs.Appointment;
using ClinAgenda.src.Application.PatientUseCase;
using ClinAgenda.src.Application.DoctorUseCase;
using ClinAgenda.src.Application.SpecialtyUseCase;
using ClinAgenda.src.Application.StatusUseCase;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Appointment")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentUseCase _appointmentUseCase;
        private readonly PatientUseCase _patientUseCase;
        private readonly DoctorUseCase _doctorUseCase;
        private readonly SpecialtyUseCase _specialtyUseCase;
        private readonly StatusUseCase _statusUseCase;

        public AppointmentController(
            AppointmentUseCase appointmentUseCase,
            PatientUseCase patientUseCase,
            DoctorUseCase doctorUseCase,
            SpecialtyUseCase specialtyUseCase,
            StatusUseCase statusUseCase)
        {
            _appointmentUseCase = appointmentUseCase;
            _patientUseCase = patientUseCase;
            _doctorUseCase = doctorUseCase;
            _specialtyUseCase = specialtyUseCase;
            _statusUseCase = statusUseCase;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAppointmentsAsync(
            [FromQuery] string? doctorName,
            [FromQuery] int? patientId, 
            [FromQuery] int? doctorId, 
            [FromQuery] int? specialtyId,
            [FromQuery] int? statusId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] bool? lActive,
            [FromQuery] int itemsPerPage = 10, 
            [FromQuery] int page = 1)
        {
            try
            {
                var result = await _appointmentUseCase.GetAppointmentsAsync(
                    doctorName,
                    patientId,
                    doctorId,
                    specialtyId,
                    statusId,
                    startDate,
                    endDate,
                    lActive,
                    itemsPerPage,
                    page);
                    
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("listById/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentByIdAsync(int appointmentId)
        {
            try
            {
                var appointment = await _appointmentUseCase.GetAppointmentByIdAsync(appointmentId);
                if (appointment == null) return NotFound($"Agendamento com ID {appointmentId} não encontrado.");
                
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPost("insert")]
        public async Task<IActionResult> CreateAppointmentAsync([FromBody] AppointmentInsertDTO appointment)
        {
            try
            {
                if (appointment == null)
                {
                    return BadRequest("Os dados do agendamento são inválidos.");
                }
                
                // Verificações adicionais
                var hasPatient = await _patientUseCase.GetPatientByIdAsync(appointment.PatientId);
                if (hasPatient == null)
                    return BadRequest($"O paciente com ID {appointment.PatientId} não existe.");
                
                var hasDoctor = await _doctorUseCase.GetDoctorByIdAsync(appointment.DoctorId);
                if (hasDoctor == null)
                    return BadRequest($"O médico com ID {appointment.DoctorId} não existe.");
                
                var hasSpecialty = await _specialtyUseCase.GetSpecialtyByIdAsync(appointment.SpecialtyId);
                if (hasSpecialty == null)
                    return BadRequest($"A especialidade com ID {appointment.SpecialtyId} não existe.");
                
                var hasStatus = await _statusUseCase.GetStatusByIdAsync(appointment.StatusId);
                if (hasStatus == null)
                    return BadRequest($"O status com ID {appointment.StatusId} não existe.");

                var createdAppointmentId = await _appointmentUseCase.CreateAppointmentAsync(appointment);

                if (createdAppointmentId <= 0)
                {
                    return StatusCode(500, "Erro ao criar o agendamento.");
                }
                
                var createdAppointment = await _appointmentUseCase.GetAppointmentByIdAsync(createdAppointmentId);
                
                // return CreatedAtAction(nameof(GetAppointmentByIdAsync), new { appointmentId = createdAppointmentId }, createdAppointment);
                return Ok(createdAppointment);
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
        
        [HttpPut("update/{appointmentId}")]
        public async Task<IActionResult> UpdateAppointmentAsync(int appointmentId, [FromBody] AppointmentInsertDTO appointment)
        {
            try
            {
                if (appointment == null)
                {
                    return BadRequest("Os dados do agendamento são inválidos.");
                }
                
                // Verificações adicionais
                var hasPatient = await _patientUseCase.GetPatientByIdAsync(appointment.PatientId);
                if (hasPatient == null)
                    return BadRequest($"O paciente com ID {appointment.PatientId} não existe.");
                
                var hasDoctor = await _doctorUseCase.GetDoctorByIdAsync(appointment.DoctorId);
                if (hasDoctor == null)
                    return BadRequest($"O médico com ID {appointment.DoctorId} não existe.");
                
                var hasSpecialty = await _specialtyUseCase.GetSpecialtyByIdAsync(appointment.SpecialtyId);
                if (hasSpecialty == null)
                    return BadRequest($"A especialidade com ID {appointment.SpecialtyId} não existe.");
                
                var hasStatus = await _statusUseCase.GetStatusByIdAsync(appointment.StatusId);
                if (hasStatus == null)
                    return BadRequest($"O status com ID {appointment.StatusId} não existe.");

                var updated = await _appointmentUseCase.UpdateAppointmentAsync(appointmentId, appointment);
                
                if (!updated)
                {
                    return NotFound($"Agendamento com ID {appointmentId} não encontrado ou não foi possível atualizar.");
                }
                
                var updatedAppointment = await _appointmentUseCase.GetAppointmentByIdAsync(appointmentId);
                return Ok(updatedAppointment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar agendamento: {ex.Message}");
            }
        }
        
        [HttpPatch("toggle-active/{appointmentId}")]
        public async Task<IActionResult> ToggleAppointmentActiveAsync(int appointmentId, [FromQuery] bool active)
        {
            try
            {
                var toggled = await _appointmentUseCase.ToggleAppointmentActiveAsync(appointmentId, active);

                if (!toggled)
                {
                    return NotFound($"Agendamento com ID {appointmentId} não encontrado.");
                }

                var updatedAppointment = await _appointmentUseCase.GetAppointmentByIdAsync(appointmentId);
                return Ok(updatedAppointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao alterar estado ativo do agendamento: {ex.Message}");
            }
        }
        
        [HttpDelete("delete/{appointmentId}")]
        public async Task<IActionResult> DeleteAppointmentAsync(int appointmentId)
        {
            try
            {
                var deleted = await _appointmentUseCase.DeleteAppointmentAsync(appointmentId);
                
                if (!deleted)
                {
                    return NotFound($"Agendamento com ID {appointmentId} não encontrado ou não foi possível excluir.");
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir agendamento: {ex.Message}");
            }
        }
    }
}