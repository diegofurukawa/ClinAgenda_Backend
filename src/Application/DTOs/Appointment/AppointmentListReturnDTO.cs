using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Application.DTOs.Status;

namespace ClinAgenda.src.Application.DTOs.Appointment
{
    public class AppointmentListReturnDTO
    {
        public int AppointmentId { get; set; }
        public required PatientDTO Patient { get; set; }
        public required DoctorDTO Doctor { get; set; }
        public required SpecialtyDTO Specialty { get; set; }
        public required StatusDTO Status { get; set; }
        public DateTime DAppointmentDate { get; set; }
        public string Observation { get; set; } = "";
        public DateTime DCreated { get; set; }
        public DateTime? DLastUpdated { get; set; }
        public bool lActive { get; set; }
    }
}