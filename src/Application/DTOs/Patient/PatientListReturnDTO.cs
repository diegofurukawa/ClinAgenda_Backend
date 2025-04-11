using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Status;

namespace ClinAgenda.src.Application.DTOs.Patient
{
    public class PatientListReturnDTO
    {
        public required int PatientId { get; set; }
        public required string PatientName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string DocumentNumber { get; set; }
        public required string dBirthDate { get; set; }
        public required StatusDTO Status { get; set; }
        public DateTime DCreated { get; set; }
        public DateTime? DLastUpdated { get; set; }
        public bool lActive { get; set; }
    }
}