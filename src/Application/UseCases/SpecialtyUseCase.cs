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

        public async Task<object> GetSpecialtyAsync(int itemsPerPage, int page)
        {
            var (total, rawData) = await _specialtyRepository.GetAllSpecialtyAsync(itemsPerPage, page);

            return new
            {
                total,
                items = rawData.ToList()
            };
        }

        public async Task<int> CreateSpecialtyAsync(SpecialtyInsertDTO specialtyDTO)
        {
            var newSpecialtyId = await _specialtyRepository.InsertSpecialtyAsync(specialtyDTO);

            return newSpecialtyId;
        }
        
        public async Task<SpecialtyDTO?> GetSpecialtyByIdAsync(int id)
        {
            return await _specialtyRepository.GetSpecialtyByIdAsync(id);
        }
        
        // New method to handle a list of specialty IDs
        public async Task<List<SpecialtyDTO>> GetSpecialtiesByIdsAsync(List<int> specialtyIds)
        {
            // If repository doesn't have a method to get multiple specialties by ids,
            // we can retrieve them one by one
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