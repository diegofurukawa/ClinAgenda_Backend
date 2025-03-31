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
            [FromQuery] string? name, 
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
                    name, 
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

        [HttpGet("listById/{id}")]
        public async Task<IActionResult> GetPatientByIdAsync(int id)        
        {
            try
            {
                var patient = await _patientUseCase.GetPatientByIdAsync(id);
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
                
                return CreatedAtAction(nameof(GetPatientByIdAsync), new { id = createdPatientId }, createdPatient);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do Servidor: {ex.Message}");
            }
        }
        
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePatientAsync(int id, [FromBody] PatientInsertDTO patient)
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

                var updated = await _patientUseCase.UpdatePatientAsync(id, patient);
                
                if (!updated)
                {
                    return NotFound($"Paciente com ID {id} não encontrado ou não foi possível atualizar.");
                }
                
                var updatedPatient = await _patientUseCase.GetPatientByIdAsync(id);
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
        
        [HttpPatch("toggle-active/{id}")]
        public async Task<IActionResult> TogglePatientActiveAsync(int id, [FromQuery] bool active)
        {
            try
            {
                var toggled = await _patientUseCase.TogglePatientActiveAsync(id, active);

                if (!toggled)
                {
                    return NotFound($"Paciente com ID {id} não encontrado.");
                }

                var updatedPatient = await _patientUseCase.GetPatientByIdAsync(id);
                return Ok(updatedPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao alterar estado ativo do paciente: {ex.Message}");
            }
        }
        
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePatientAsync(int id)
        {
            try
            {
                var deleted = await _patientUseCase.DeletePatientAsync(id);
                
                if (!deleted)
                {
                    return NotFound($"Paciente com ID {id} não encontrado ou não foi possível excluir.");
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