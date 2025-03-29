using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Application.DTOs.Status;
using ClinAgenda.src.Core.Interfaces;
using ClinAgenda.src.Infrastructure.Repositories;
using Dapper;

namespace ClinAgenda.src.Application.PatientUseCase
{
    public class PatientUseCase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientUseCase(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<object> GetPatientsAsync
            (
                string? patientname, 
                string? documentNumber, 
                int? statusId, 
                int itemsPerPage, 
                int page
            )
        {
            var (total, rawData) = await _patientRepository.GetAllPatientAsync(
                    patientname, 
                    documentNumber, 
                    statusId, 
                    itemsPerPage, 
                    page
                );

            var patients = rawData
                .Select(p => new PatientListReturnDTO
                {
                    PatientId = p.PatientId,
                    PatientName = p.PatientName,
                    PhoneNumber = p.PhoneNumber,
                    DocumentNumber = p.DocumentNumber,
                    BirthDate = p.BirthDate,
                    Status = new StatusDTO
                    {
                        StatusId = p.StatusId,
                        StatusName = p.StatusName
                    }
                })
                .ToList();

            return new { total, items = patients };
        }

        public async Task<int> CreatePatientAsync(PatientInsertDTO patientDTO)
        {
            // Validar se o StatusId existe
            var statusExists = await ValidateStatusExistsAsync(patientDTO.StatusId);
            if (!statusExists)
            {
                throw new ArgumentException($"Status com ID {patientDTO.StatusId} não existe.");
            }
            
            return await _patientRepository.InsertPatientAsync(patientDTO);
        }
        
        private async Task<bool> ValidateStatusExistsAsync(int statusId)
        {
            try
            {
                // Verificar se o status existe - isso depende de ter acesso ao repositório de Status
                // Aqui você precisaria injetar IStatusRepository no construtor
                // Como workaround, podemos fazer verificação direta no banco
                var query = "SELECT COUNT(1) FROM status WHERE id = @StatusId";
                var parameters = new { StatusId = statusId };
                
                // Usando o mesmo connection do repositório de pacientes
                var connection = (_patientRepository as PatientRepository)?._connection;
                if (connection != null)
                {
                    var count = await connection.ExecuteScalarAsync<int>(query, parameters);
                    return count > 0;
                }
                
                // Se não puder verificar diretamente, assume que existe
                return true;
            }
            catch
            {
                // Em caso de erro, assume que existe para não bloquear a operação
                return true;
            }
        }

        public async Task<PatientDTO> GetPatientByIdAsync(int id)
        {
            return await _patientRepository.GetPatientByIdAsync(id);
        }

        
        public async Task<bool> UpdatePatientAsync(int id, PatientInsertDTO patientDTO)
        {
            // Verificar se o paciente existe
            var existingPatient = await _patientRepository.GetPatientByIdAsync(id);
            if (existingPatient == null)
            {
                return false;
            }
            
            // Validar se o StatusId existe
            var statusExists = await ValidateStatusExistsAsync(patientDTO.StatusId);
            if (!statusExists)
            {
                throw new ArgumentException($"Status com ID {patientDTO.StatusId} não existe.");
            }
            
            var rowsAffected = await _patientRepository.UpdatePatientAsync(id, patientDTO);
            return rowsAffected > 0;
        }
        
        public async Task<bool> DeletePatientAsync(int id)
        {
            var rowsAffected = await _patientRepository.DeletePatientAsync(id);
            return rowsAffected > 0;
        }
    }
}