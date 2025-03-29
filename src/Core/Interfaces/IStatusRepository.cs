using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Status;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IStatusRepository
    {
        Task<StatusDTO> GetStatusByIdAsync(int id);
        Task<int> DeleteStatusAsync(int id);
        Task<int> InsertStatusAsync(StatusInsertDTO statusInsertDTO);
        Task<(int total, IEnumerable<StatusDTO> specialtys)> GetAllStatusAsync(int? itemsPerPage, int? page);
    }
}