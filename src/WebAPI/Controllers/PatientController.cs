using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Application.PatientUseCase;
using ClinAgenda.src.Application.StatusUseCase;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
            [FromQuery] int? patientId,
            [FromQuery] int itemsPerPage = 10, 
            [FromQuery] int page = 1
            )
        {
            try
            {
                var result = await _patientUseCase.GetPatientsAsync(name, documentNumber, patientId, itemsPerPage, page);
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
                var doctor = await _patientUseCase.GetPatientByIdAsync(id);
                if (doctor == null) return NotFound();
                return Ok(doctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do Servidor: {ex.Message}");
            }
        }

        [HttpPost("insert")]
        // public async Task<IActionResult> CreatePatientAsync([FromBody] PatientInsertDTO patient)
        public async Task<IActionResult> CreatePatientAsync([FromBody] PatientInsertDTO patient)
        {
            try
            {
                var hasStatus = await _statusUseCase.GetStatusByIdAsync(patient.StatusId);
                if (hasStatus == null)
                    return BadRequest($"O status ID {patient.StatusId} não existe");

                var createdPatientId = await _patientUseCase.CreatePatientAsync(patient);

                if (!(createdPatientId > 0))
                {
                    return StatusCode(500, "Erro ao criar a Paciente.");
                }
                var infosPatientCreated = await _patientUseCase.GetPatientByIdAsync(createdPatientId);

                return Ok(infosPatientCreated);
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
                if (patient == null || string.IsNullOrWhiteSpace(patient.Name))
                {
                    return BadRequest("Os dados do paciente são inválidos.");
                }
                
                // Validar formato da data
                if (!DateTime.TryParse(patient.BirthDate, out DateTime birthDate))
                {
                    return BadRequest("A data de nascimento deve estar no formato YYYY-MM-DD.");
                }
                
                // Formatar a data no formato aceito pelo MySQL
                patient.BirthDate = birthDate.ToString("yyyy-MM-dd");
                
                var success = await _patientUseCase.UpdatePatientAsync(id, patient);
                
                if (!success)
                {
                    return NotFound($"Paciente com ID {id} não encontrado ou não foi possível atualizar.");
                }
                
                var updatedPatient = await _patientUseCase.GetPatientByIdAsync(id);
                return Ok(updatedPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar paciente: {ex.Message}");
            }
        }
        
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePatientAsync(int id)
        {
            try
            {
                var success = await _patientUseCase.DeletePatientAsync(id);
                
                if (!success)
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