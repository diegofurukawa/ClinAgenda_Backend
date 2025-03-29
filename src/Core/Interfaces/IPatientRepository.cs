using System.Collections.Generic;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Patient;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IPatientRepository
    {
        Task<PatientDTO> GetPatientByIdAsync(int id);
        // Task<PatientListDTO> GetPatientDetailsAsync(int id);
        Task<(int total, IEnumerable<PatientListDTO> patient)> GetAllPatientAsync(string? name, string? documentNumber, int? statusId, int itemsPerPage, int page);
        Task<int> DeletePatientAsync(int id);
        Task<int> InsertPatientAsync(PatientInsertDTO patientInsertDTO);
        Task<int> UpdatePatientAsync(int id, PatientInsertDTO patientInsertDTO);
    }
}