using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda
{
    public class Patient
    {
        public int PatientId { get; set; }
        public required string PatientName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string DocumentNumber { get; set; }
        public required int StatusId { get; set; }
        public required DateTime DBirthDate { get; set; }
        public DateTime DCreated { get; set; }
        public DateTime? DLastUpdated { get; set; }
        public bool LActive { get; set; } = true;
    }
}