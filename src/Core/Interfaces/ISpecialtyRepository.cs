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
        Task<int> InsertSpecialtyAsync(SpecialtyInsertDTO SpecialtyInsertDTO);
        Task<(int total, IEnumerable<SpecialtyDTO> specialtys)> GetAllSpecialtyAsync(int? itemsPerPage, int? page);
        Task<int> InsertSpecialtyAsync(Specialty specialty);
    }
}