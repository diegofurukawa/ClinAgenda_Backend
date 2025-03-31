using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Application.DTOs.Status;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class DoctorDTO
    {
        public int DoctorId { get; set; }
        public required string DoctorName { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime DCreated { get; set; }
        public DateTime? DLastUpdated { get; set; }
        public bool LActive { get; set; }
    }

}