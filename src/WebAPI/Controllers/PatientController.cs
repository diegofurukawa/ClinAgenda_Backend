using System;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Application.PatientUseCase;
using ClinAgenda.src.Application.StatusUseCase;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Patient")]
    public class PatientController : ControllerBase
    {
        private readonly PatientUseCase _patientUseCase;
        private readonly StatusUseCase _statusUseCase;

        public PatientController(PatientUseCase patientService, StatusUseCase statusUseCase)
        {
            _patientUseCase = patientService;
            _statusUseCase = statusUseCase;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetPatientsAsync(
            [FromQuery] string? patientName, 
            [FromQuery] string? documentNumber, 
            [FromQuery] int? statusId,
            [FromQuery] bool? lActive,
            [FromQuery] int itemsPerPage = 10, 
            [FromQuery] int page = 1
            )
        {
            try
            {
                var result = await _patientUseCase.GetPatientsAsync(
                    patientName, 
                    documentNumber, 
                    statusId,
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

        [HttpGet("listById/{patientId}")]
        public async Task<IActionResult> GetPatientByIdAsync(int patientId)        
        {
            try
            {
                var patient = await _patientUseCase.GetPatientByIdAsync(patientId);
                if (patient == null) return NotFound();
                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }

        [HttpPost("insert")]
        public async Task<IActionResult> CreatePatientAsync([FromBody] PatientInsertDTO patient)
        {
            try
            {
                if (patient == null || string.IsNullOrWhiteSpace(patient.PatientName))
                {
                    return BadRequest("Os dados do paciente são inválidos.");
                }
                
                var hasStatus = await _statusUseCase.GetStatusByIdAsync(patient.StatusId);
                if (hasStatus == null)
                    return BadRequest($"O status ID {patient.StatusId} não existe");

                var createdPatientId = await _patientUseCase.CreatePatientAsync(patient);

                if (!(createdPatientId > 0))
                {
                    return StatusCode(500, "Erro ao criar o paciente.");
                }
                
                var createdPatient = await _patientUseCase.GetPatientByIdAsync(createdPatientId);
                
                // return CreatedAtAction(nameof(GetPatientByIdAsync), new { patientid = createdPatientId }, createdPatient);
                return Ok(createdPatient);
            }
            // catch (ArgumentException ex)
            // {
            //     return BadRequest(ex.Message);
            // }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do Servidor: {ex.Message}");
            }
        }
        
        [HttpPut("update/{patientId}")]
        public async Task<IActionResult> UpdatePatientAsync(int patientId, [FromBody] PatientInsertDTO patient)
        {
            try
            {
                if (patient == null || string.IsNullOrWhiteSpace(patient.PatientName))
                {
                    return BadRequest("Os dados do paciente são inválidos.");
                }
                
                var hasStatus = await _statusUseCase.GetStatusByIdAsync(patient.StatusId);
                if (hasStatus == null)
                    return BadRequest($"O status ID {patient.StatusId} não existe");

                var updated = await _patientUseCase.UpdatePatientAsync(patientId, patient);
                
                if (!updated)
                {
                    return NotFound($"Paciente com ID {patientId} não encontrado ou não foi possível atualizar.");
                }
                
                var updatedPatient = await _patientUseCase.GetPatientByIdAsync(patientId);
                return Ok(updatedPatient);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar paciente: {ex.Message}");
            }
        }
        
        [HttpPatch("toggle-active/{patientId}")]
        public async Task<IActionResult> TogglePatientActiveAsync(int patientId, [FromQuery] bool active)
        {
            try
            {
                var toggled = await _patientUseCase.TogglePatientActiveAsync(patientId, active);

                if (!toggled)
                {
                    return NotFound($"Paciente com ID {patientId} não encontrado.");
                }

                var updatedPatient = await _patientUseCase.GetPatientByIdAsync(patientId);
                return Ok(updatedPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao alterar estado ativo do paciente: {ex.Message}");
            }
        }
        
        [HttpDelete("delete/{patientId}")]
        public async Task<IActionResult> DeletePatientAsync(int patientId)
        {
            try
            {
                var deleted = await _patientUseCase.DeletePatientAsync(patientId);
                
                if (!deleted)
                {
                    return NotFound($"Paciente com ID {patientId} não encontrado ou não foi possível excluir.");
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir paciente: {ex.Message}");
            }
        }
    }
}