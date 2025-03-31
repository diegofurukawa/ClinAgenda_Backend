using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Core.Interfaces;

namespace ClinAgenda.src.Application.SpecialtyUseCase
{
    public class SpecialtyUseCase
    {
        private readonly ISpecialtyRepository _specialtyRepository;

        public SpecialtyUseCase(ISpecialtyRepository specialtyRepository)
        {
            _specialtyRepository = specialtyRepository;
        }

        public async Task<object> GetSpecialtyAsync(
            string? name = null,
            bool? lActive = null,
            int itemsPerPage = 10, 
            int page = 1)
        {
            var (total, rawData) = await _specialtyRepository.GetAllSpecialtyAsync(
                name,
                lActive,
                itemsPerPage, 
                page);

            return new
            {
                total,
                items = rawData.ToList()
            };
        }

        public async Task<int> CreateSpecialtyAsync(SpecialtyInsertDTO specialtyDTO)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(specialtyDTO.SpecialtyName))
            {
                throw new ArgumentException("O nome da especialidade é obrigatório");
            }

            if (specialtyDTO.NScheduleDuration <= 0 || specialtyDTO.NScheduleDuration > 480)
            {
                throw new ArgumentException("A duração do agendamento deve estar entre 1 e 480 minutos");
            }

            var newSpecialtyId = await _specialtyRepository.InsertSpecialtyAsync(specialtyDTO);

            return newSpecialtyId;
        }
        
        public async Task<SpecialtyDTO?> GetSpecialtyByIdAsync(int id)
        {
            return await _specialtyRepository.GetSpecialtyByIdAsync(id);
        }

        public async Task<bool> UpdateSpecialtyAsync(int id, SpecialtyInsertDTO specialtyDTO)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(specialtyDTO.SpecialtyName))
            {
                throw new ArgumentException("O nome da especialidade é obrigatório");
            }

            if (specialtyDTO.NScheduleDuration <= 0 || specialtyDTO.NScheduleDuration > 480)
            {
                throw new ArgumentException("A duração do agendamento deve estar entre 1 e 480 minutos");
            }

            // Verifica se a especialidade existe
            var existingSpecialty = await _specialtyRepository.GetSpecialtyByIdAsync(id);
            if (existingSpecialty == null)
            {
                return false;
            }

            // Atualiza a especialidade
            var rowsAffected = await _specialtyRepository.UpdateSpecialtyAsync(id, specialtyDTO);
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleSpecialtyActiveAsync(int id, bool active)
        {
            // Verifica se a especialidade existe
            var existingSpecialty = await _specialtyRepository.GetSpecialtyByIdAsync(id);
            if (existingSpecialty == null)
            {
                return false;
            }

            // Ativa/desativa a especialidade
            var rowsAffected = await _specialtyRepository.ToggleSpecialtyActiveAsync(id, active);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteSpecialtyAsync(int id)
        {
            // Verifica se a especialidade existe
            var existingSpecialty = await _specialtyRepository.GetSpecialtyByIdAsync(id);
            if (existingSpecialty == null)
            {
                return false;
            }

            var rowsAffected = await _specialtyRepository.DeleteSpecialtyAsync(id);
            return rowsAffected > 0;
        }
        
        // Método para obter várias especialidades por ID
        public async Task<List<SpecialtyDTO>> GetSpecialtiesByIdsAsync(List<int> specialtyIds)
        {
            var specialties = new List<SpecialtyDTO>();
            
            foreach (var id in specialtyIds)
            {
                var specialty = await _specialtyRepository.GetSpecialtyByIdAsync(id);
                if (specialty != null)
                {
                    specialties.Add(specialty);
                }
            }
            
            return specialties;
        }
    }
}