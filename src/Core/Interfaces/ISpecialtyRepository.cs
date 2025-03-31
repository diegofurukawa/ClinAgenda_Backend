using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Core.Entities;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface ISpecialtyRepository
    {
        Task<SpecialtyDTO> GetSpecialtyByIdAsync(int id);
        Task<int> DeleteSpecialtyAsync(int id);
        Task<int> InsertSpecialtyAsync(SpecialtyInsertDTO specialtyInsertDTO);
        Task<(int total, IEnumerable<SpecialtyDTO> specialties)> GetAllSpecialtyAsync(
            string? name = null, 
            bool? lActive = null, 
            int? itemsPerPage = 10, 
            int? page = 1);
        Task<int> UpdateSpecialtyAsync(int id, SpecialtyInsertDTO specialtyInsertDTO);
        Task<int> ToggleSpecialtyActiveAsync(int id, bool active);
    }
}