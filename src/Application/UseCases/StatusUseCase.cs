using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Status;
using ClinAgenda.src.Core.Interfaces;
using ClinAgenda.src.Core.Enums;

namespace ClinAgenda.src.Application.StatusUseCase
{
    public class StatusUseCase
    {
        private readonly IStatusRepository _statusRepository;

        public StatusUseCase(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }

        public async Task<object> GetStatusAsync(
            string? statusType = null,
            bool? lActive = null,
            int itemsPerPage = 10,
            int page = 1)
        {
            var (total, rawData) = await _statusRepository.GetAllStatusAsync(
                statusType,
                lActive,
                itemsPerPage,
                page);

            return new
            {
                total,
                items = rawData
            };
        }

        public async Task<StatusDTO?> GetStatusByIdAsync(int id)
        {
            return await _statusRepository.GetStatusByIdAsync(id);
        }

        public async Task<int> CreateStatusAsync(StatusInsertDTO statusDTO)
        {
            // Validação de campos obrigatórios
            if (string.IsNullOrWhiteSpace(statusDTO.StatusName))
            {
                throw new ArgumentException("O nome do status é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(statusDTO.StatusType))
            {
                throw new ArgumentException("O tipo do status é obrigatório");
            }

            // Validação do tipo de status
            if (!IsValidStatusType(statusDTO.StatusType))
            {
                throw new ArgumentException("O tipo de status deve ser patient, specialty, doctor ou appointment");
            }

            var newStatusId = await _statusRepository.InsertStatusAsync(statusDTO);

            return newStatusId;
        }

        public async Task<bool> UpdateStatusAsync(int id, StatusInsertDTO statusDTO)
        {
            // Validação de campos obrigatórios
            if (string.IsNullOrWhiteSpace(statusDTO.StatusName))
            {
                throw new ArgumentException("O nome do status é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(statusDTO.StatusType))
            {
                throw new ArgumentException("O tipo do status é obrigatório");
            }

            // Validação do tipo de status
            if (!IsValidStatusType(statusDTO.StatusType))
            {
                throw new ArgumentException("O tipo de status deve ser patient, specialty, doctor ou appointment");
            }

            var existingStatus = await _statusRepository.GetStatusByIdAsync(id);
            if (existingStatus == null)
            {
                return false;
            }

            var rowsAffected = await _statusRepository.UpdateStatusAsync(id, statusDTO);
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleStatusActiveAsync(int id, bool active)
        {
            var existingStatus = await _statusRepository.GetStatusByIdAsync(id);
            if (existingStatus == null)
            {
                return false;
            }

            var rowsAffected = await _statusRepository.ToggleStatusActiveAsync(id, active);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteStatusAsync(int id)
        {
            var existingStatus = await _statusRepository.GetStatusByIdAsync(id);
            if (existingStatus == null)
            {
                return false;
            }

            var rowsAffected = await _statusRepository.DeleteStatusAsync(id);
            return rowsAffected > 0;
        }
        
        // Método para criar um objeto StatusDTO temporário (compatibilidade)
        public StatusDTO CreateDefaultStatusDTO(int statusId, string statusName)
        {
            return new StatusDTO
            {
                StatusId = statusId,
                StatusName = statusName,
                StatusType = "default",
                DCreated = DateTime.Now,
                lActive = true
            };
        }

        // Método para validar o tipo de status
        private bool IsValidStatusType(string statusType)
        {
            return statusType == "patient" ||
                   statusType == "specialty" ||
                   statusType == "doctor" ||
                   statusType == "appointment";
        }
        
        // Método para obter todos os tipos de status válidos
        public object GetValidStatusTypes()
        {
            // Criamos um dicionário que mapeia tipo de status para seu nome em português
            var statusTypeNames = new Dictionary<string, string>
            {
                { "patient", "PACIENTE" },
                { "specialty", "ESPECIALIDADE" },
                { "doctor", "MEDICO" },
                { "appointment", "AGENDAMENTO" }
            };
            
            // Criamos uma lista de objetos anônimos com as propriedades desejadas
            var items = statusTypeNames.Select(kvp => new 
            { 
                statusType = kvp.Key, 
                statusTypeName = kvp.Value 
            }).ToList();
            
            // Retornamos o objeto com a estrutura desejada
            return new
            {
                total = items.Count,
                items = items
            };
        }
    }
}