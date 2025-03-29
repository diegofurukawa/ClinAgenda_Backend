using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<DoctorListDTO>> GetDoctorsAsync(string? name, int? specialtyId, int? statusId, int offset, int pageSize);
        Task<IEnumerable<SpecialtyDoctorDTO>> GetDoctorSpecialtyAsync(int[] doctorIds);
        Task<int> InsertDoctorAsync(DoctorInsertDTO doctor);
        Task<IEnumerable<DoctorListDTO>> GetDoctorByIdAsync(int id);
        Task<bool> UpdateDoctorAsync(DoctorDTO doctor);
         Task<int> DeleteDoctorByIdAsync(int id);
    }
}