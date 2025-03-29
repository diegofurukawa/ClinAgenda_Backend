using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//Importacoes de DTOs
using ClinAgenda.src.Application.DTOs.Doctor;
//Importacoes de UseCase
using ClinAgenda.src.Application.DoctorUseCase;
using ClinAgenda.src.Application.SpecialtyUseCase;
using ClinAgenda.src.Application.StatusUseCase;
//Importacoes de Arquitetura
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

        public DoctorController(DoctorUseCase doctorUseCase, StatusUseCase statusUseCase, SpecialtyUseCase specialtyUseCase)
        {
            _doctorUseCase = doctorUseCase;
            _statusUseCase = statusUseCase;
            _specialtyUseCase = specialtyUseCase;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetDoctors(
            [FromQuery] string? name, 
            [FromQuery] int? specialtyId, 
            [FromQuery] int? statusId, 
            [FromQuery] int itemsPerPage = 10, 
            [FromQuery] int page = 1
            )
        {
            try
            {
                var result = await _doctorUseCase.GetDoctorsAsync(name, specialtyId, statusId, itemsPerPage, page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        
        [HttpGet("listById/{id}")]
        public async Task<IActionResult> GetDoctorByIdAsync(int id)
        {
            var doctor = await _doctorUseCase.GetDoctorByIdAsync(id);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }


        [HttpPost("insert")]
        public async Task<IActionResult> CreateDoctorAsync([FromBody] DoctorInsertDTO doctor)
        {
            try
            {
                var hasStatus = await _statusUseCase.GetStatusByIdAsync(doctor.StatusId);
                if (hasStatus == null)
                    return BadRequest($"O status com ID {doctor.StatusId} não existe.");

                var specialties = await _specialtyUseCase.GetSpecialtiesByIdsAsync(doctor.Specialty);

                var notFoundSpecialties = doctor.Specialty.Except(specialties.Select(s => s.Id)).ToList();

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


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateDoctorAsync(int id, [FromBody] DoctorInsertDTO doctor)
        {
            if (doctor == null) return BadRequest();

            var hasStatus = await _statusUseCase.GetStatusByIdAsync(doctor.StatusId);
            if (hasStatus == null)
                return BadRequest($"O status com ID {doctor.StatusId} não existe.");

            var specialties = await _specialtyUseCase.GetSpecialtiesByIdsAsync(doctor.Specialty);

            var notFoundSpecialties = doctor.Specialty.Except(specialties.Select(s => s.Id)).ToList();

            if (notFoundSpecialties.Any())
            {
                return BadRequest(notFoundSpecialties.Count > 1 ? $"As especialidades com os IDs {string.Join(", ", notFoundSpecialties)} não existem." : $"A especialidade com o ID {notFoundSpecialties.First().ToString()} não existe.");
            }

            bool updated = await _doctorUseCase.UpdateDoctorAsync(id, doctor);

            if (!updated) return NotFound("Doutor não encontrado.");

            var infosDoctorUpdate = await _doctorUseCase.GetDoctorByIdAsync(id);
            return Ok(infosDoctorUpdate);

        }

    }
}