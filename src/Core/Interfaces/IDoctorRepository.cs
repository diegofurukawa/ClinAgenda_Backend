using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IDoctorRepository
    {
        Task<(int total, IEnumerable<DoctorListDTO> doctors)> GetDoctorsAsync(
            string? doctorName, 
            int? specialtyId, 
            int? statusId, 
            bool? lActive, 
            int offset, 
            int itemsPerPage
            ); Task<IEnumerable<SpecialtyDoctorDTO>> GetDoctorSpecialtiesAsync(int[] doctorIds);
        Task<int> InsertDoctorAsync(DoctorInsertDTO doctor);
        Task<IEnumerable<DoctorListDTO>> GetDoctorByIdAsync(int doctorId);
        Task<bool> UpdateDoctorByIdAsync(DoctorDTO doctor);
        Task<int> DeleteDoctorByIdAsync(int doctorId);
        Task<bool> ToggleDoctorActiveAsync(int doctorId, bool active);
    }
}