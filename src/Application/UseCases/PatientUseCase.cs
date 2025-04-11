using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Application.DTOs.Status;
using ClinAgenda.src.Core.Interfaces;

namespace ClinAgenda.src.Application.PatientUseCase
{
    public class PatientUseCase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IStatusRepository _statusRepository;

        public PatientUseCase(IPatientRepository patientRepository, IStatusRepository statusRepository)
        {
            _patientRepository = patientRepository;
            _statusRepository = statusRepository;
        }

        public async Task<object> GetPatientsAsync(
            string? name, 
            string? documentNumber, 
            int? statusId,
            bool? lActive,
            int itemsPerPage, 
            int page
        )
        {
            var (total, rawData) = await _patientRepository.GetAllPatientAsync(
                name, 
                documentNumber, 
                statusId,
                lActive,
                itemsPerPage, 
                page
            );

            // Obter informações de status para todos os pacientes
            var statusIds = rawData.Select(p => p.StatusId).Distinct().ToList();
            var statusList = new Dictionary<int, StatusDTO>();
            
            foreach (var id in statusIds)
            {
                var status = await _statusRepository.GetStatusByIdAsync(id);
                if (status != null)
                {
                    statusList[id] = status;
                }
            }

            var patients = rawData
                .Select(p => new PatientListReturnDTO
                {
                    PatientId = p.PatientId,
                    PatientName = p.PatientName,
                    PhoneNumber = p.PhoneNumber,
                    DocumentNumber = p.DocumentNumber,
                    dBirthDate = p.dBirthDate,
                    Status = statusList.ContainsKey(p.StatusId) 
                        ? statusList[p.StatusId]
                        : new StatusDTO
                        {
                            StatusId = p.StatusId,
                            StatusName = p.StatusName,
                            StatusType = "patient",
                            DCreated = p.DCreated,
                            DLastUpdated = p.DLastUpdated,
                            lActive = p.lActive
                        },
                    DCreated = p.DCreated,
                    DLastUpdated = p.DLastUpdated,
                    lActive = p.lActive
                })
                .ToList();

            return new { total, items = patients };
        }

        public async Task<int> CreatePatientAsync(PatientInsertDTO patientDTO)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(patientDTO.PatientName))
            {
                throw new ArgumentException("O nome do paciente é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(patientDTO.PhoneNumber))
            {
                throw new ArgumentException("O telefone do paciente é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(patientDTO.DocumentNumber))
            {
                throw new ArgumentException("O documento do paciente é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(patientDTO.dBirthdate))
            {
                throw new ArgumentException("A data de nascimento do paciente é obrigatória");
            }

            // Validar se a data é válida
            if (!DateTime.TryParse(patientDTO.dBirthdate, out _))
            {
                throw new ArgumentException("Data de nascimento inválida. Use o formato YYYY-MM-DD");
            }

            // Validar se o StatusId existe
            var status = await _statusRepository.GetStatusByIdAsync(patientDTO.StatusId);
            if (status == null)
            {
                throw new ArgumentException($"Status com ID {patientDTO.StatusId} não existe.");
            }
            
            return await _patientRepository.InsertPatientAsync(patientDTO);
        }

        public async Task<PatientDTO> GetPatientByIdAsync(int id)
        {
            return await _patientRepository.GetPatientByIdAsync(id);
        }
        
        public async Task<bool> UpdatePatientAsync(int id, PatientInsertDTO patientDTO)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(patientDTO.PatientName))
            {
                throw new ArgumentException("O nome do paciente é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(patientDTO.PhoneNumber))
            {
                throw new ArgumentException("O telefone do paciente é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(patientDTO.DocumentNumber))
            {
                throw new ArgumentException("O documento do paciente é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(patientDTO.dBirthdate))
            {
                throw new ArgumentException("A data de nascimento do paciente é obrigatória");
            }

            // Validar se a data é válida
            if (!DateTime.TryParse(patientDTO.dBirthdate, out _))
            {
                throw new ArgumentException("Data de nascimento inválida. Use o formato YYYY-MM-DD");
            }

            // Verificar se o paciente existe
            var existingPatient = await _patientRepository.GetPatientByIdAsync(id);
            if (existingPatient == null)
            {
                return false;
            }
            
            // Validar se o StatusId existe
            var status = await _statusRepository.GetStatusByIdAsync(patientDTO.StatusId);
            if (status == null)
            {
                throw new ArgumentException($"Status com ID {patientDTO.StatusId} não existe.");
            }
            
            var rowsAffected = await _patientRepository.UpdatePatientAsync(id, patientDTO);
            return rowsAffected > 0;
        }
        
        public async Task<bool> TogglePatientActiveAsync(int id, bool active)
        {
            // Verificar se o paciente existe
            var existingPatient = await _patientRepository.GetPatientByIdAsync(id);
            if (existingPatient == null)
            {
                return false;
            }
            
            var rowsAffected = await _patientRepository.TogglePatientActiveAsync(id, active);
            return rowsAffected > 0;
        }
        
        public async Task<bool> DeletePatientAsync(int id)
        {
            // Verificar se o paciente existe
            var existingPatient = await _patientRepository.GetPatientByIdAsync(id);
            if (existingPatient == null)
            {
                return false;
            }
            
            var rowsAffected = await _patientRepository.DeletePatientAsync(id);
            return rowsAffected > 0;
        }
    }
}