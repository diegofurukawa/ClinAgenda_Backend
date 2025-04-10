using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IDoctorSpecialtyRepository
    {
        /// <summary>
        /// Inserts relationships between a doctor and multiple specialties
        /// </summary>
        Task InsertAsync(DoctorSpecialtyDTO doctorSpecialtyDTO);
        
        /// <summary>
        /// Deletes all specialty relationships for a doctor
        /// </summary>
        Task DeleteByDoctorIdAsync(int doctorId);
        
        /// <summary>
        /// Activates or deactivates a specialty for a doctor
        /// </summary>
        Task<bool> ToggleActiveAsync(int doctorId, int specialtyId, bool active);
        
        /// <summary>
        /// Checks if a relationship exists between a doctor and specialty
        /// </summary>
        Task<bool> ExistsAsync(int doctorId, int specialtyId);
    }
}