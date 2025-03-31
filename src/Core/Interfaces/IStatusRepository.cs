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
        Task<(int total, IEnumerable<StatusDTO> statuses)> GetAllStatusAsync(
            string? statusType = null, 
            bool? lActive = null, 
            int? itemsPerPage = 10, 
            int? page = 1);
        Task<int> UpdateStatusAsync(int id, StatusInsertDTO statusInsertDTO);
        
        // Método corrigido
        Task<int> ToggleStatusActiveAsync(int id, bool active);
    }
}