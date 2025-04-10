using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Appointment;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<AppointmentDTO> GetAppointmentByIdAsync(int id);
        
        Task<(int total, IEnumerable<AppointmentListDTO> appointments)> GetAllAppointmentsAsync(
            string? doctorName = null,
            int? patientId = null,
            int? doctorId = null, 
            int? specialtyId = null,
            int? statusId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? lActive = null,
            int itemsPerPage = 10, 
            int page = 1);
            
        Task<int> InsertAppointmentAsync(AppointmentInsertDTO appointmentInsertDTO);
        
        Task<int> UpdateAppointmentAsync(int id, AppointmentInsertDTO appointmentInsertDTO);
        
        Task<int> ToggleAppointmentActiveAsync(int id, bool active);
        
        Task<int> DeleteAppointmentAsync(int id);
        
        Task<bool> CheckConflictingAppointmentsAsync(
            int doctorId, 
            int? appointmentId, 
            DateTime startTime, 
            DateTime endTime);
    }
}