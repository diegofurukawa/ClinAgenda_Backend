using System.Collections.Generic;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Patient;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IPatientRepository
    {
        Task<PatientDTO> GetPatientByIdAsync(int id);
        
        Task<(int total, IEnumerable<PatientListDTO> patients)> GetAllPatientAsync(
            string? name, 
            string? documentNumber, 
            int? statusId,
            bool? lActive,
            int itemsPerPage, 
            int page);
            
        Task<int> DeletePatientAsync(int id);
        
        Task<int> InsertPatientAsync(PatientInsertDTO patientInsertDTO);
        
        Task<int> UpdatePatientAsync(int id, PatientInsertDTO patientInsertDTO);
        
        Task<int> TogglePatientActiveAsync(int id, bool active);
    }
}