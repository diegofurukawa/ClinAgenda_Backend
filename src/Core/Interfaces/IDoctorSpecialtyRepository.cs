using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IDoctorSpecialtyRepository
    {
        Task InsertAsync(DoctorSpecialtyDTO doctor);
        Task DeleteByDoctorIdAsync(int doctorId);
    }
}